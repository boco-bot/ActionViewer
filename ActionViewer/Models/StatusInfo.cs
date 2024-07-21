using Dalamud.Game.ClientState.Objects.SubKinds;

namespace ActionViewer.Models
{
    public class CharRow
    {
        public IPlayerCharacter character { get; set; }
        public uint jobId { get; set; }
        public string playerName { get; set; }
        public StatusInfo statusInfo { get; set; }
    }
    public class StatusInfo
    {
        public Lumina.Excel.GeneratedSheets2.Action? leftLuminaStatusInfo { get; set; }
        public Lumina.Excel.GeneratedSheets2.Action? rightLuminaStatusInfo { get; set; }
		public int essenceId { get; set; }
        public int reraiserStatus { get; set; }
        public uint essenceIconID
        {
            get
            {
                if (essenceId >= 2311 && essenceId <= 2325)
                {
                    return 62386 + (uint)essenceId;
                }
                if (essenceId >= 2434 && essenceId <= 2439)
                {
                    return 62293 + (uint)essenceId;
                }
                return 26;
            }

        }
        public uint rightIconID
        {
            get
            {
                if (rightLuminaStatusInfo != null)
                {
				    return rightLuminaStatusInfo.Icon;
				}
                return 33;
            }
        }
        public uint leftIconID
        {
            get
            {
                if (leftLuminaStatusInfo != null)
                {
                    return leftLuminaStatusInfo.Icon;
				}
                return 33;
            }
        }
        public uint reraiserIconID
        {
            get
            {
                switch(reraiserStatus)
                {
                    case 1:
                        return 19301;
                    case 2:
                        return 19302;
                    default:
                        return 19321;
                }
            }
        }
    }
}
