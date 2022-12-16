using System;
using System.Collections.Generic;
using ReactUI;
using UnityEngine;

namespace UI
{
    public class UIRankWindowController : MonoBehaviour,IUIModelDataChangeObserver
    {
        #region AutoBind
        [AutoBindVariable]
        UIVariable var_Title;
        [AutoBindVariable]
        UIVariable var_Users;

        [AutoBindEvent]
        void event_OnUserButtonClicked(params object[] args)
        {
            if (args[0] is UIVariable v) Debug.Log($"clicked {v.GetInteger()}");
        }
        
        #endregion

        private List<RankItemData> _rankDatas = new List<RankItemData>();
        private UIModelRank _model;

        private void Awake()
        {
            UIVariableBindHelper.AutoBind(this,gameObject);
            _model = UIModelManager.Instance.GetModel<UIModelRank>();
            _model.RegisterDataChangedNotification(this);
        }

        private void RefreshRank()
        {
            var_Title.SetString(_model.Title);
            var_Users.SetArray(_model.RankData);
        }
        
        #region IUIModelDataChangeObserver
        public void OnDataChanged(UIBaseModel model, uint propID, params object[] param)
        {
            if (model is UIModelRank)
            {
                RefreshRank();
            }
        }

        public uint GetInterestedPropID(UIBaseModel model)
        {
            return UIModelRank.PropID_RankDataChange;
        }
        #endregion
    }
}