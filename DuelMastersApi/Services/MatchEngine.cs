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
