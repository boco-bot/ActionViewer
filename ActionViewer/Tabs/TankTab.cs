using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace ActionViewer.Tabs;

public class TankTab : MainWindowTab
{
    private string searchText = string.Empty;
    private List<uint> tanks = new List<uint>() { 1, 3, 12, 17 };

    public TankTab(Plugin plugin) : base("Tanks", plugin) { }

    public override void Draw()
    {
        List<IPlayerCharacter> playerCharacters = this.Plugin.ActionViewer.getPlayerCharacters().Where(x => tanks.Contains((uint)x.ClassJob.GameData?.JobIndex)).ToList();

        ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
        ImGui.InputText("", ref searchText, 256);
        StatusInfoFunctions.GenerateStatusTable(playerCharacters, searchText, this.Plugin.Configuration.AnonymousMode, this.Plugin.ActionSheet);
    }
}