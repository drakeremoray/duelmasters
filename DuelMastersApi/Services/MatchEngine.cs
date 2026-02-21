using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Linq;
using DuelMastersApi.Models;

namespace DuelMastersApi.Services
{
    // Small deterministic engine: operates on JSON state and returns new state + events.
    public static class MatchEngine
    {
        public static MatchResultDto ApplyAction(string stateJson, MatchActionDto action, int playerId)
        {
            var stateNode = JsonNode.Parse(stateJson) ?? new JsonObject();

            // Ensure baseline fields
            if (stateNode["turn"] == null) stateNode["turn"] = 1;
            if (stateNode["activePlayer"] == null) stateNode["activePlayer"] = playerId;
            if (stateNode["events"] == null) stateNode["events"] = new JsonArray();

            var events = new List<MatchEventDto>();

            switch ((action?.ActionType ?? "").ToLowerInvariant())
            {
                case "endturn":
                case "end_turn":
                    // increment turn and rotate active player if players list exists
                    int turn = stateNode["turn"]!.GetValue<int>();
                    stateNode["turn"] = turn + 1;

                    // simple rotation: if players array exists, pick next index
                    if (stateNode["players"] is JsonArray playersArr && playersArr.Count > 0)
                    {
                        var active = stateNode["activePlayer"]!.GetValue<int>();
                        int idx = playersArr.ToList().FindIndex(n => n!.GetValue<int>() == active);
                        int nextIdx = (idx + 1) % playersArr.Count;
                        stateNode["activePlayer"] = playersArr[nextIdx]!.GetValue<int>();
                    }

                    events.Add(new MatchEventDto { Type = "TurnEnded", Data = new { by = playerId } });
                    break;
                case "draw":
                    // draw a card from player's deck into hand
                    if (stateNode["players"] is JsonArray parr)
                    {
                        var playerNode = parr.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == playerId);
                        if (playerNode != null)
                        {
                            var deck = playerNode["deck"] as JsonArray ?? new JsonArray();
                            var hand = playerNode["hand"] as JsonArray ?? new JsonArray();
                            if (deck.Count > 0)
                            {
                                var card = deck[0];
                                deck.RemoveAt(0);
                                hand.Add(card);
                                playerNode["deck"] = deck;
                                playerNode["hand"] = hand;
                                events.Add(new MatchEventDto { Type = "CardDrawn", Data = new { player = playerId, card } });
                            }
                            else
                            {
                                events.Add(new MatchEventDto { Type = "DeckEmpty", Data = new { player = playerId } });
                            }
                        }
                    }
                    break;
                case "draw_multiple":
                    // draw N cards (payload: { count: N }), deterministic order from top of deck
                    try
                    {
                        int count = 1;
                        try
                        {
                            var payloadJson = JsonSerializer.Serialize(action?.Payload);
                            using var doc = System.Text.Json.JsonDocument.Parse(payloadJson);
                            if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object && doc.RootElement.TryGetProperty("count", out var prop))
                            {
                                if (prop.ValueKind == System.Text.Json.JsonValueKind.Number && prop.TryGetInt32(out var v)) count = v;
                                else
                                {
                                    var raw = prop.GetRawText().Trim('"');
                                    if (!int.TryParse(raw, out count)) count = 1;
                                }
                            }
                        }
                        catch { count = 1; }
                        if (count < 1) count = 1;

                        if (stateNode["players"] is JsonArray parrDraw)
                        {
                            var playerNode = parrDraw.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == playerId);
                            if (playerNode != null)
                            {
                                var deck = playerNode["deck"] as JsonArray ?? new JsonArray();
                                var hand = playerNode["hand"] as JsonArray ?? new JsonArray();
                                var drawn = new JsonArray();
                                for (int i = 0; i < count && deck.Count > 0; i++)
                                {
                                    var card = deck[0];
                                    deck.RemoveAt(0);
                                    hand.Add(card);
                                    // add a shallow copy for event payload to avoid JsonNode parent conflicts
                                    var cardCopy = JsonNode.Parse(card.ToJsonString());
                                    drawn.Add(cardCopy);
                                }
                                playerNode["deck"] = deck;
                                playerNode["hand"] = hand;
                                events.Add(new MatchEventDto { Type = "CardsDrawn", Data = new { player = playerId, count = drawn.Count, cards = drawn } });
                            }
                        }
                    }
                    catch (System.Exception ex) { events.Add(new MatchEventDto { Type = "DrawFailed", Data = new { reason = "invalid-payload", detail = ex.Message } }); }
                    break;
                case "playcard":
                case "play_card":
                    // Play a card from hand to battlefield, validate cost and deduct resources.
                    try
                    {
                        var payloadJson = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode = JsonNode.Parse(payloadJson);
                        var cidNode = payloadNode? ["cardId"];
                        if (cidNode == null) { events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "missing-cardId" } }); break; }

                        var cidInt = 0;
                        if (!int.TryParse(cidNode.ToString(), out cidInt)) { events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "invalid-cardId" } }); break; }

                        if (stateNode["players"] is JsonArray parr2)
                        {
                            var playerNode = parr2.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == playerId);
                            if (playerNode == null) { events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "player-not-found" } }); break; }

                            var hand = playerNode["hand"] as JsonArray ?? new JsonArray();
                            var battlefield = playerNode["battlefield"] as JsonArray ?? new JsonArray();

                            // find card in hand by numeric id
                            JsonNode? cardNode = null;
                            foreach (var n in hand)
                            {
                                if (n == null) continue;
                                var nid = n["id"];
                                if (nid != null && nid.GetValue<int>() == cidInt) { cardNode = n; break; }
                            }

                            if (cardNode == null)
                            {
                                events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "card-not-in-hand", player = playerId, cardId = cidInt } });
                                break;
                            }

                            // card cost validation
                            var costNode = cardNode["cost"];
                            var cost = costNode != null ? costNode.GetValue<int>() : 0;

                            var resourcesNode = playerNode["resources"] ?? playerNode["mana"];
                            var resources = resourcesNode != null ? resourcesNode.GetValue<int>() : 0;
                            if (resources < cost)
                            {
                                events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "insufficient-resources", player = playerId, cost, resources } });
                                break;
                            }

                            // deduct resources, move card
                            playerNode["resources"] = resources - cost;
                            hand.Remove(cardNode);
                            battlefield.Add(cardNode);
                            playerNode["hand"] = hand;
                            playerNode["battlefield"] = battlefield;

                            events.Add(new MatchEventDto { Type = "CardPlayed", Data = new { player = playerId, card = cardNode, cost = cost } });
                        }
                    }
                    catch
                    {
                        events.Add(new MatchEventDto { Type = "PlayFailed", Data = new { reason = "invalid-payload" } });
                    }
                    break;
                case "attack":
                    // Validate attacker on player's battlefield and target existence.
                    try
                    {
                        var payloadJson2 = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode2 = JsonNode.Parse(payloadJson2);
                        var attackerId = payloadNode2?["attacker"]?.GetValue<int>();
                        var targetId = payloadNode2?["target"]?.GetValue<int>();

                        if (attackerId == null || targetId == null)
                        {
                            events.Add(new MatchEventDto { Type = "AttackFailed", Data = new { reason = "missing-attacker-or-target" } });
                            break;
                        }

                        var attackerExists = false;
                        var targetExists = false;
                        JsonNode? attackerNode = null;

                        if (stateNode["players"] is JsonArray playersArr2)
                        {
                            // check attacker on player's battlefield
                            var playerNode = playersArr2.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == playerId);
                            if (playerNode != null)
                            {
                                var battlefield = playerNode["battlefield"] as JsonArray ?? new JsonArray();
                                foreach (var c in battlefield)
                                {
                                    if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == attackerId)
                                    {
                                        attackerExists = true; attackerNode = c; break;
                                    }
                                }
                            }

                            // check target on any player's battlefield or a player id (direct)
                            foreach (var p in playersArr2)
                            {
                                var bf = p?["battlefield"] as JsonArray;
                                if (bf != null)
                                {
                                    foreach (var c in bf)
                                    {
                                        if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == targetId)
                                        {
                                            targetExists = true; break;
                                        }
                                    }
                                }
                                if (targetExists) break;
                            }

                            // allow direct attack if targetId equals some player id
                            if (!targetExists)
                            {
                                foreach (var p in playersArr2)
                                {
                                    if (p?["id"] != null && p["id"]!.GetValue<int>() == targetId) { targetExists = true; break; }
                                }
                            }
                        }

                        if (!attackerExists) { events.Add(new MatchEventDto { Type = "AttackFailed", Data = new { reason = "attacker-not-found" } }); break; }
                        if (!targetExists) { events.Add(new MatchEventDto { Type = "AttackFailed", Data = new { reason = "target-not-found" } }); break; }

                        // Simplified resolution: emit AttackResolved; damage/resolution handled later
                        events.Add(new MatchEventDto { Type = "AttackResolved", Data = new { player = playerId, attacker = attackerId, target = targetId } });
                    }
                    catch
                    {
                        events.Add(new MatchEventDto { Type = "AttackFailed", Data = new { reason = "invalid-payload" } });
                    }
                    break;
                case "discard":
                case "discard_specific":
                    // discard a specific card from a zone (hand or battlefield)
                    try
                    {
                        var payloadJson3 = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode3 = JsonNode.Parse(payloadJson3);
                        var cid = payloadNode3?["cardId"]?.GetValue<int>();
                        var zone = payloadNode3?["zone"]?.ToString() ?? "hand";
                        if (cid == null) { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "missing-cardId" } }); break; }

                        if (stateNode["players"] is JsonArray par)
                        {
                            // find owner: payload may include ownerId, default to acting player
                            var ownerId = payloadNode3?["ownerId"]?.GetValue<int>() ?? playerId;
                            var ownerNode = par.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == ownerId);
                            if (ownerNode == null) { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "owner-not-found" } }); break; }

                            var zoneArr = ownerNode[zone] as JsonArray ?? new JsonArray();
                            JsonNode? cardNode = null;
                            foreach (var c in zoneArr)
                            {
                                if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == cid) { cardNode = c; break; }
                            }

                            if (cardNode == null) { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "card-not-found", owner = ownerId, cardId = cid } }); break; }

                            zoneArr.Remove(cardNode);
                            ownerNode[zone] = zoneArr;
                            events.Add(new MatchEventDto { Type = "CardDiscarded", Data = new { by = playerId, owner = ownerId, card = cardNode, zone } });
                        }
                    }
                    catch { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "invalid-payload" } }); }
                    break;
                case "discard_random":
                    // deterministically discard the first card in the specified zone
                    try
                    {
                        var payloadJson4 = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode4 = JsonNode.Parse(payloadJson4);
                        var zone = payloadNode4?["zone"]?.ToString() ?? "hand";
                        var ownerId = payloadNode4?["ownerId"]?.GetValue<int>() ?? playerId;

                        if (stateNode["players"] is JsonArray parr3)
                        {
                            var ownerNode = parr3.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == ownerId);
                            if (ownerNode == null) { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "owner-not-found" } }); break; }

                            var zoneArr = ownerNode[zone] as JsonArray ?? new JsonArray();
                            if (zoneArr.Count == 0) { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "zone-empty", owner = ownerId, zone } }); break; }

                            // deterministic "random": pick first element
                            var card = zoneArr[0];
                            zoneArr.RemoveAt(0);
                            ownerNode[zone] = zoneArr;
                            events.Add(new MatchEventDto { Type = "CardDiscarded", Data = new { by = playerId, owner = ownerId, card, zone, method = "random" } });
                        }
                    }
                    catch { events.Add(new MatchEventDto { Type = "DiscardFailed", Data = new { reason = "invalid-payload" } }); }
                    break;
                case "flip":
                case "flip_card":
                    // toggle faceDown/tapped state on a specific card
                    try
                    {
                        var payloadJson5 = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode5 = JsonNode.Parse(payloadJson5);
                        var cid2 = payloadNode5?["cardId"]?.GetValue<int>();
                        var ownerId2 = payloadNode5?["ownerId"]?.GetValue<int>() ?? playerId;
                        if (cid2 == null) { events.Add(new MatchEventDto { Type = "FlipFailed", Data = new { reason = "missing-cardId" } }); break; }

                        if (stateNode["players"] is JsonArray parr4)
                        {
                            var ownerNode = parr4.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == ownerId2);
                            if (ownerNode == null) { events.Add(new MatchEventDto { Type = "FlipFailed", Data = new { reason = "owner-not-found" } }); break; }

                            // search battlefield then hand
                            JsonNode? cardNode = null;
                            var bf = ownerNode["battlefield"] as JsonArray ?? new JsonArray();
                            foreach (var c in bf) if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == cid2) { cardNode = c; break; }
                            if (cardNode == null)
                            {
                                var hand = ownerNode["hand"] as JsonArray ?? new JsonArray();
                                foreach (var c in hand) if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == cid2) { cardNode = c; break; }
                            }

                            if (cardNode == null) { events.Add(new MatchEventDto { Type = "FlipFailed", Data = new { reason = "card-not-found" } }); break; }

                            // toggle faceDown property
                            var face = cardNode["faceDown"]?.GetValue<bool>() ?? false;
                            cardNode["faceDown"] = !face;
                            events.Add(new MatchEventDto { Type = "CardFlipped", Data = new { by = playerId, owner = ownerId2, cardId = cid2, faceDown = !face } });
                        }
                    }
                    catch { events.Add(new MatchEventDto { Type = "FlipFailed", Data = new { reason = "invalid-payload" } }); }
                    break;
                case "highlight":
                case "target":
                    // mark a card as highlighted/targeted by the acting player
                    try
                    {
                        var payloadJson6 = JsonSerializer.Serialize(action?.Payload);
                        var payloadNode6 = JsonNode.Parse(payloadJson6);
                        var cid3 = payloadNode6?["cardId"]?.GetValue<int>();
                        var ownerId3 = payloadNode6?["ownerId"]?.GetValue<int>() ?? playerId;
                        if (cid3 == null) { events.Add(new MatchEventDto { Type = "TargetFailed", Data = new { reason = "missing-cardId" } }); break; }

                        if (stateNode["players"] is JsonArray parr5)
                        {
                            var ownerNode = parr5.ToList().FirstOrDefault(n => n != null && n["id"] != null && n["id"]!.GetValue<int>() == ownerId3);
                            if (ownerNode == null) { events.Add(new MatchEventDto { Type = "TargetFailed", Data = new { reason = "owner-not-found" } }); break; }

                            JsonNode? cardNode = null;
                            var zones = new[] { "hand", "battlefield", "graveyard", "deck" };
                            foreach (var z in zones)
                            {
                                var zarr = ownerNode[z] as JsonArray;
                                if (zarr == null) continue;
                                foreach (var c in zarr)
                                {
                                    if (c != null && c["id"] != null && c["id"]!.GetValue<int>() == cid3) { cardNode = c; break; }
                                }
                                if (cardNode != null) break;
                            }

                            if (cardNode == null) { events.Add(new MatchEventDto { Type = "TargetFailed", Data = new { reason = "card-not-found" } }); break; }

                            // set a targetedBy array or property
                            var tarr = cardNode["targetedBy"] as JsonArray ?? new JsonArray();
                            tarr.Add(playerId);
                            cardNode["targetedBy"] = tarr;
                            events.Add(new MatchEventDto { Type = "CardTargeted", Data = new { by = playerId, owner = ownerId3, cardId = cid3 } });
                        }
                    }
                    catch { events.Add(new MatchEventDto { Type = "TargetFailed", Data = new { reason = "invalid-payload" } }); }
                    break;
                default:
                    // no-op action: record it as an event
                    events.Add(new MatchEventDto { Type = action?.ActionType ?? "noop", Data = action?.Payload });
                    break;
            }

            // append events to state.events
            var arr = (JsonArray)stateNode["events"]!;
            foreach (var e in events)
            {
                arr.Add(JsonSerializer.SerializeToNode(e));
            }

            return new MatchResultDto { NewState = stateNode.ToJsonString(), Events = events };
        }
    }
}
