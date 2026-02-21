using System.Collections.Generic;

namespace DuelMastersApi.Models
{
    public record MatchActionDto
    {
        public string ActionType { get; init; } = "noop";
        public object? Payload { get; init; }
    }

    public record MatchEventDto
    {
        public string Type { get; init; } = string.Empty;
        public object? Data { get; init; }
    }

    public record MatchResultDto
    {
        public string NewState { get; init; } = "{}";
        public List<MatchEventDto> Events { get; init; } = new List<MatchEventDto>();
    }
}
