using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace ActionViewer.Tabs;

public class MeleeTab : MainWindowTab
{
    private string searchText = string.Empty;
    private List<uint> physDPS = new List<uint>() { 2, 4, 10, 14, 19, 21 };

    public MeleeTab(Plugin plugin) : base("Melee", plugin) { }

    public override void Draw()
    {
        List<IPlayerCharacter> playerCharacters = this.Plugin.ActionViewer.getPlayerCharacters().Where(x => physDPS.Contains((uint)x.ClassJob.GameData?.JobIndex)).ToList();

        ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
        ImGui.InputText("", ref searchText, 256);
        StatusInfoFunctions.GenerateStatusTable(playerCharacters, searchText, this.Plugin.Configuration.AnonymousMode, this.Plugin.ActionSheet);
    }
}