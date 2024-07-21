using ActionViewer.Models;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Textures;
using ImGuiNET;
using Lumina.Excel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;

namespace ActionViewer.Functions
{
    public static class StatusInfoFunctions
    {
		private static List<ushort> eurekaTerritories = new List<ushort>() { 795, 827 };
		private static List<int> essenceIds = new List<int>() { 2311, 2312, 2313, 2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2434, 2435, 2436, 2437, 2438, 2439, };
		private static StatusInfo GetStatusInfo(StatusList statusList, ExcelSheet<Lumina.Excel.GeneratedSheets2.Action> actionSheet)
        {
            StatusInfo statusInfo = new StatusInfo();

            foreach (Status status in statusList)
            {
                int statusId = (int)status.StatusId;
                if (essenceIds.Contains(statusId))
                {
                    statusInfo.essenceId = statusId;
                }
                if (statusId.Equals(2348))
                {
                    int leftId = status.Param % 256;
                    int rightId = (status.Param - leftId) / 256;

					int leftStartingRow = leftId < 71 ? 20700 : leftId > 83 ? 23823 : 22273;
                    int rightStartingRow = rightId < 71 ? 20700 : rightId > 83 ? 23823 : 22273;

					if (leftId > 0)
						statusInfo.leftLuminaStatusInfo = actionSheet.GetRow((uint)(leftStartingRow + leftId));

					if (rightId > 0)
						statusInfo.rightLuminaStatusInfo = actionSheet.GetRow((uint)(rightStartingRow + rightId));
				}
				if (statusId.Equals(1618))
				{
					int leftId = status.Param % 256;
					int rightId = (status.Param - leftId) / 256;

                    if (leftId > 0)
                        statusInfo.leftLuminaStatusInfo = actionSheet.GetRow((uint)(12957 + leftId));
					
                    if(rightId > 0)
					    statusInfo.rightLuminaStatusInfo = actionSheet.GetRow((uint)(12957 + rightId));
				}
				if (statusId.Equals(2355))
                {
                    statusInfo.reraiserStatus = status.Param == 70 ? 1 : 2;
                }
                if (statusId.Equals(1641))
                {
					statusInfo.reraiserStatus = 1;
				}
            }
            return statusInfo;
        }
        private static List<CharRow> GenerateRows(List<IPlayerCharacter> playerCharacters, ExcelSheet<Lumina.Excel.GeneratedSheets2.Action> actionSheet)
        {
            List<CharRow> charRowList = new List<CharRow>();
            foreach (IPlayerCharacter character in playerCharacters)
            {
                // get player name, job ID, status list
                CharRow row = new CharRow();
                row.character = character;
                row.playerName = character.Name.ToString();
                row.jobId = (uint)character.ClassJob.GameData?.JobIndex;
                row.statusInfo = GetStatusInfo(character.StatusList, actionSheet);
                charRowList.Add(row);
            }
            return charRowList;
        }

        public static void GenerateStatusTable(List<IPlayerCharacter> playerCharacters, string searchText, bool anonymousMode, ExcelSheet<Lumina.Excel.GeneratedSheets2.Action> actionSheet, string filter = "none")
        {
            ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable;// | ImGuiTableFlags.SizingFixedFit;
            var iconSize = ImGui.GetTextLineHeight() * 2f;
            var iconSizeVec = new Vector2(iconSize, iconSize);
            bool eurekaTerritory = eurekaTerritories.Contains(Services.ClientState.TerritoryType);
            int columnCount = eurekaTerritory ? 5 : 6;


			List<CharRow> charRowList = GenerateRows(playerCharacters, actionSheet);

            if (ImGui.BeginTable("table1", anonymousMode ? columnCount - 1 : columnCount, tableFlags))
            {
                ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Job);
                if(!anonymousMode)
                {
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.PreferSortDescending, 1f, (int)charColumns.Name);
                }
                if (!eurekaTerritory)
                {
					ImGui.TableSetupColumn("RR", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Reraiser);
					ImGui.TableSetupColumn("Ess.", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Essence);
				} else
                {
					ImGui.TableSetupColumn("Remembered", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Reraiser);
				}
                ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Left);
                ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Right);
                ImGui.TableHeadersRow();
                ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                charRowList = SortCharDataWithSortSpecs(sortSpecs, charRowList);

                foreach (CharRow row in charRowList)
                {

                    if ((searchText == string.Empty ||
                            (row.statusInfo.rightIconID != 33 && (row.statusInfo.rightLuminaStatusInfo.Name.ToString().ToLowerInvariant().IndexOf(searchText.ToLowerInvariant()) != -1)) ||
                            (row.statusInfo.leftIconID != 33 && (row.statusInfo.leftLuminaStatusInfo.Name.ToString().ToLowerInvariant().IndexOf(searchText.ToLowerInvariant()) != -1))) && 
                        (filter == "none" || (filter == "noEss" && 
                            row.statusInfo.essenceIconID == 26))
                        )
                    {

                        // player job, name
                        ImGui.TableNextColumn();

                        uint jobIconId = 62118;
                        if(row.jobId >= 10)
                        {
                            jobIconId += 2 + row.jobId;
                        }
                        else if(row.jobId >= 8)
                        {
                            jobIconId += 1 + row.jobId;
                        }
                        else
                        {
                            jobIconId += row.jobId;
                        }

                        ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(jobIconId)).GetWrapOrEmpty().ImGuiHandle,
                            iconSizeVec, Vector2.Zero, Vector2.One);
                        var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
                        var left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
                        if (left)
                        {
                            Plugin.TargetManager.Target = row.character;
                        }
                        if (!anonymousMode)
                        {
                            ImGui.TableNextColumn();
                            ImGui.Selectable(row.playerName, false);
                            hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
                            left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
                            if (left)
                            {
                                Plugin.TargetManager.Target = row.character;
                            }
                        }

						// reraiser
						ImGui.TableNextColumn();
						ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.reraiserIconID)).GetWrapOrEmpty().ImGuiHandle,
							new Vector2(iconSize * (float)0.8, iconSize));

						if (!eurekaTerritory)
                        {
                            // essence
                            ImGui.TableNextColumn();
                            ImGui.Image(
                                Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.essenceIconID)).GetWrapOrEmpty().ImGuiHandle,
                                iconSizeVec, Vector2.Zero, Vector2.One);
                        }

                        // left/right actions
                        ImGui.TableNextColumn();
                        ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.leftIconID)).GetWrapOrEmpty().ImGuiHandle,
                            iconSizeVec, Vector2.Zero, Vector2.One);
                        ImGui.TableNextColumn();
                        ImGui.Image(
                            Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.rightIconID)).GetWrapOrEmpty().ImGuiHandle, 
                            iconSizeVec, Vector2.Zero, Vector2.One);
                    }
                }
                ImGui.EndTable();
            }
        }
        public enum charColumns
        {
            Job,
            Name,
            Reraiser,
            Essence,
            Left,
            Right
        }

        public static List<CharRow> SortCharDataWithSortSpecs(ImGuiTableSortSpecsPtr sortSpecs, List<CharRow> charDataList)
        {

            Dictionary<uint, uint> jobSort = new Dictionary<uint, uint>()
            {
                {1, 1 }, // PLD
                {3, 2 }, // WAR
                {12, 3}, // DRK
                {17, 4 }, // GNB
                {6 , 5 }, // WHM
                {9 , 6 }, // SCH
                {13, 7}, // AST
                {20 , 8 }, // SGE
                {2, 9 }, // MNK
                {4 , 10 }, // DRG
                {10 , 11 }, // NIN
                {14 , 12 }, // SAM
                {19 , 13 }, // RPR
                {5 , 14 }, // BRD
                {11 , 15 }, // MCH
                {18 , 16 }, // DNC
                {7 , 17 }, // BLM
                {8 , 18 }, // SMN
                {15 , 19 }, // RDM

            };

            IEnumerable<CharRow> sortedCharaData = charDataList;

            for (int i = 0; i < sortSpecs.SpecsCount; i++)
            {
                ImGuiTableColumnSortSpecsPtr columnSortSpec = sortSpecs.Specs;

                switch ((charColumns)columnSortSpec.ColumnUserID)
                {
                    case charColumns.Job:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => jobSort.GetValueOrDefault(o.jobId));
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => jobSort.GetValueOrDefault(o.jobId));
                        }
                        break;
                    case charColumns.Name:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => o.playerName);
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => o.playerName);
                        }
                        break;
                    case charColumns.Essence:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.essenceIconID);
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.essenceIconID);
                        }
                        break;
                    case charColumns.Reraiser:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.reraiserStatus);
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.reraiserStatus);
                        }
                        break;
                    case charColumns.Left:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.leftIconID);
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.leftIconID);
                        }
                        break;
                    case charColumns.Right:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.rightIconID);
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.rightIconID);
                        }
                        break;
                    default:
                        break;
                }
            }

            return sortedCharaData.ToList();
        }
    }
}
