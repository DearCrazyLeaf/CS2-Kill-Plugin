using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;

namespace Kill;

public class Kill : BasePlugin, IPluginConfig<KillConfig>
{
    public override string ModuleName => "Kill";
    public override string ModuleVersion => "[hlymcnmodify]0.5.0";
    public override string ModuleAuthor => "Preach";

    required public KillConfig Config { get; set; }

    public override void Load(bool hotReload)
    {
        AddCommand("kill", "Kill yourself", SuicideHandler);
    }

    private void SuicideHandler(CCSPlayerController? ply, CommandInfo? info)
    {
        if (ply == null || !ply.IsValid || !ply.PawnIsAlive || ply.PlayerPawn == null || !ply.PlayerPawn.IsValid)
            return;

        if (!Config.ToggleCommand)
        {
            ply.PrintToChat("[Kill] Command is disabled");
            return;
        }

        var playerPawn = ply.PlayerPawn.Value;
        playerPawn?.CommitSuicide(true, false);
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnChat(EventPlayerChat @event, GameEventInfo info)
    {
        var player = Utilities.GetPlayerFromUserid(@event.Userid);
        if (player is not null)
        {
            var text = @event.Text.Trim();
            if (string.Equals(text, "kill", StringComparison.OrdinalIgnoreCase))
            {
                SuicideHandler(player, null);
            }
        }
        return HookResult.Continue;
    }

    public void OnConfigParsed(KillConfig config)
    {
        Config = config;
    }
}

public class KillConfig : BasePluginConfig
{
    [JsonPropertyName("Toggle Command")]
    public bool ToggleCommand { get; set; } = true;
}