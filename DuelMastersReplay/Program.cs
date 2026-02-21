using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DuelMastersApi.Data;
using DuelMastersApi.Services;
using DuelMastersApi.Models;
using System.Text.Json;

if (args.Length == 0)
{
    Console.WriteLine("Usage: DuelMastersReplay <matchId> [connectionString]");
    return 1;
}

var matchId = int.Parse(args[0]);
var conn = args.Length > 1 ? args[1] : Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") ?? "Host=localhost;Port=5432;Database=duelmasters;Username=duelmaster;Password=duelmaster_pwd";

var services = new ServiceCollection();
services.AddDbContext<DuelMastersContext>(opt => opt.UseNpgsql(conn));
var sp = services.BuildServiceProvider();

using var scope = sp.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DuelMastersContext>();

var actions = await db.MatchActions.Where(a => a.MatchId == matchId).OrderBy(a => a.CreatedAt).ToListAsync();
var snapshots = await db.GameStates.Where(g => g.MatchId == matchId).OrderBy(g => g.CreatedAt).ToListAsync();

Console.WriteLine($"Found {actions.Count} actions and {snapshots.Count} snapshots for match {matchId}");

var state = snapshots.FirstOrDefault()?.State ?? "{}";
Console.WriteLine("Initial state:\n" + state);

foreach (var a in actions)
{
    Console.WriteLine($"\nAction {a.Id} by player {a.PlayerId}: {a.ActionType} {a.Payload}");
    MatchActionDto dto;
    try
    {
        var payload = JsonSerializer.Deserialize<object>(a.Payload);
        dto = new MatchActionDto { ActionType = a.ActionType, Payload = payload };
    }
    catch
    {
        dto = new MatchActionDto { ActionType = a.ActionType, Payload = null };
    }

    var result = MatchEngine.ApplyAction(state, dto, a.PlayerId);
    Console.WriteLine("Resulting state:\n" + result.NewState);
    state = result.NewState;
}

Console.WriteLine("Replay finished.");
return 0;
