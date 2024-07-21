using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace ActionViewer.Tabs;

public class PhysRangedTab : MainWindowTab
{
    private string searchText = string.Empty;
    private List<uint> physRanged = new List<uint>() { 5, 11, 18 };

    public PhysRangedTab(Plugin plugin) : base("Phys Ranged", plugin) { }

    public override void Draw()
    {
        List<IPlayerCharacter> playerCharacters = this.Plugin.ActionViewer.getPlayerCharacters().Where(x => physRanged.Contains((uint)x.ClassJob.GameData?.JobIndex)).ToList();

        ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
        ImGui.InputText("", ref searchText, 256);
        StatusInfoFunctions.GenerateStatusTable(playerCharacters, searchText, this.Plugin.Configuration.AnonymousMode, this.Plugin.ActionSheet);
    }
}