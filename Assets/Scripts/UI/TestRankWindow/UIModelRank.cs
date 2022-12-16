using System.Collections.Generic;

namespace UI
{
    public class RankItemData
    {
        public int RankNum;
        public string UserIcon;
        public string UserName;
        public int Level;
        public int LevelPoints;
        public bool ShowFlag;
    }
    public class UIModelRank : UIBaseModel
    {
        public const uint PropID_RankDataChange = 1 << 1;
        public string Title;
        public List<RankItemData> RankData = new List<RankItemData>();
        public override uint GetModelType()
        {
            return (uint)EUIModelType.LootWindow;
        }

        public override void Init()
        {
            
        }

        public void LoadData(string title, List<RankItemData> rankData)
        {
            Title = title;
            RankData = rankData;
            NotifyDataChanged(PropID_RankDataChange);
        }

        protected override void OnCleanup()
        {
            base.OnCleanup();
        }
    }
}