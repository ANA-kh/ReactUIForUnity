using System.Collections.Generic;

namespace UI
{
    public class UIModelRank : UIBaseModel
    {
        public const uint PropID_RankDataChange = 1 << 1;
        public string Title;
        public List<UIRankWindowController.RankItemData> RankData = new List<UIRankWindowController.RankItemData>();
        public override uint GetModelType()
        {
            return (uint)EUIModelType.LootWindow;
        }

        public override void Init()
        {
            
        }

        public void LoadData(string title, List<UIRankWindowController.RankItemData> rankData)
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