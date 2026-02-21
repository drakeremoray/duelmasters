using System.Text.Json.Nodes;
using DuelMastersApi.Services;
using DuelMastersApi.Models;
using Xunit;

namespace DuelMastersApi.Tests
{
    public class MatchEngineTests
    {
        [Fact]
        public void EndTurn_IncrementsTurnAndRotatesActivePlayer()
        {
            var state = new JsonObject
            {
                ["turn"] = 1,
                ["players"] = new JsonArray(1, 2),
                ["activePlayer"] = 1,
                ["events"] = new JsonArray()
            };

            var action = new MatchActionDto { ActionType = "endTurn" };

            var result = MatchEngine.ApplyAction(state.ToJsonString(), action, 1);

            var newState = JsonNode.Parse(result.NewState)!;
            Assert.Equal(2, newState["turn"]!.GetValue<int>());
            Assert.Equal(2, newState["activePlayer"]!.GetValue<int>());
            Assert.NotEmpty(result.Events);
        }
    }
}
