using System.Linq;
using System.Text.Json.Nodes;
using DuelMastersApi.Models;
using DuelMastersApi.Services;
using Xunit;

namespace DuelMastersApi.Tests
{
    public class MatchEngineActionTests
    {
        [Fact]
        public void PlayCard_Fails_When_InsufficientResources()
        {
            var player = new JsonObject {
                ["id"] = 1,
                ["hand"] = new JsonArray(new JsonObject { ["id"] = 10, ["cost"] = 5 }),
                ["battlefield"] = new JsonArray(),
                ["deck"] = new JsonArray(),
                ["resources"] = 2
            };

            var state = new JsonObject { ["players"] = new JsonArray(player), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "play_card", Payload = new { cardId = 10 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 1);

            Assert.Contains(result.Events, e => e.Type == "PlayFailed");
        }

        [Fact]
        public void PlayCard_Succeeds_And_DeductsResources()
        {
            var card = new JsonObject { ["id"] = 11, ["cost"] = 1 };
            var player = new JsonObject {
                ["id"] = 2,
                ["hand"] = new JsonArray(card),
                ["battlefield"] = new JsonArray(),
                ["deck"] = new JsonArray(),
                ["resources"] = 3
            };

            var state = new JsonObject { ["players"] = new JsonArray(player), ["events"] = new JsonArray() };
            var action = new MatchActionDto { ActionType = "play_card", Payload = new { cardId = 11 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 2);

            Assert.Contains(result.Events, e => e.Type == "CardPlayed");
            var newState = JsonNode.Parse(result.NewState)!;
            var newResources = newState["players"]![0]! ["resources"]!.GetValue<int>();
            Assert.Equal(2, newResources);
        }

        [Fact]
        public void DiscardSpecific_RemovesCardFromZone()
        {
            var card = new JsonObject { ["id"] = 20 };
            var owner = new JsonObject { ["id"] = 3, ["hand"] = new JsonArray(card), ["battlefield"] = new JsonArray() };
            var state = new JsonObject { ["players"] = new JsonArray(owner), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "discard", Payload = new { cardId = 20, zone = "hand", ownerId = 3 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 3);

            Assert.Contains(result.Events, e => e.Type == "CardDiscarded");
            var newState = JsonNode.Parse(result.NewState)!;
            var hand = newState["players"]![0]! ["hand"] as JsonArray;
            Assert.True(hand == null || hand.Count == 0);
        }

        [Fact]
        public void DiscardRandom_RemovesFirstCard()
        {
            var c1 = new JsonObject { ["id"] = 21 };
            var c2 = new JsonObject { ["id"] = 22 };
            var owner = new JsonObject { ["id"] = 4, ["hand"] = new JsonArray(c1, c2) };
            var state = new JsonObject { ["players"] = new JsonArray(owner), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "discard_random", Payload = new { zone = "hand", ownerId = 4 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 4);

            Assert.Contains(result.Events, e => e.Type == "CardDiscarded");
            var newState = JsonNode.Parse(result.NewState)!;
            var hand = newState["players"]![0]! ["hand"] as JsonArray;
            Assert.Equal(1, hand!.Count);
            Assert.Equal(22, hand[0]! ["id"]!.GetValue<int>());
        }

        [Fact]
        public void Flip_TogglesFaceDown()
        {
            var card = new JsonObject { ["id"] = 30, ["faceDown"] = true };
            var owner = new JsonObject { ["id"] = 5, ["battlefield"] = new JsonArray(card) };
            var state = new JsonObject { ["players"] = new JsonArray(owner), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "flip", Payload = new { cardId = 30, ownerId = 5 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 5);

            Assert.Contains(result.Events, e => e.Type == "CardFlipped");
            var newState = JsonNode.Parse(result.NewState)!;
            var face = newState["players"]![0]! ["battlefield"]![0]! ["faceDown"]!.GetValue<bool>();
            Assert.False(face);
        }

        [Fact]
        public void Target_AddsTargetedBy()
        {
            var card = new JsonObject { ["id"] = 40 };
            var owner = new JsonObject { ["id"] = 6, ["battlefield"] = new JsonArray(card) };
            var state = new JsonObject { ["players"] = new JsonArray(owner), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "target", Payload = new { cardId = 40, ownerId = 6 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 7);

            Assert.Contains(result.Events, e => e.Type == "CardTargeted");
            var newState = JsonNode.Parse(result.NewState)!;
            var targeted = newState["players"]![0]! ["battlefield"]![0]! ["targetedBy"] as JsonArray;
            Assert.NotNull(targeted);
            Assert.Contains(7, targeted!.Select(n => n!.GetValue<int>()));
        }
    }
}
