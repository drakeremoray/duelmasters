using System.Text.Json.Nodes;
using DuelMastersApi.Models;
using DuelMastersApi.Services;
using Xunit;

namespace DuelMastersApi.Tests
{
    public class MatchEngineDrawTests
    {
        [Fact]
        public void DrawMultiple_TakesMultipleCardsFromDeck()
        {
            var c1 = new JsonObject { ["id"] = 101 };
            var c2 = new JsonObject { ["id"] = 102 };
            var c3 = new JsonObject { ["id"] = 103 };

            var player = new JsonObject {
                ["id"] = 9,
                ["hand"] = new JsonArray(),
                ["deck"] = new JsonArray(c1, c2, c3),
                ["battlefield"] = new JsonArray(),
                ["resources"] = 3
            };

            var state = new JsonObject { ["players"] = new JsonArray(player), ["events"] = new JsonArray() };

            var action = new MatchActionDto { ActionType = "draw_multiple", Payload = new { count = 2 } };
            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 9);
            // debug output for test investigation
            System.Console.WriteLine("DrawMultiple result events: " + System.Text.Json.JsonSerializer.Serialize(result.Events));

            Assert.Contains(result.Events, e => e.Type == "CardsDrawn");
            var newState = JsonNode.Parse(result.NewState)!;
            var hand = newState["players"]![0]! ["hand"] as JsonArray;
            var deck = newState["players"]![0]! ["deck"] as JsonArray;
            Assert.Equal(2, hand!.Count);
            Assert.Equal(1, deck!.Count);
            Assert.Equal(101, hand[0]! ["id"]!.GetValue<int>());
            Assert.Equal(102, hand[1]! ["id"]!.GetValue<int>());
        }
    }
}
