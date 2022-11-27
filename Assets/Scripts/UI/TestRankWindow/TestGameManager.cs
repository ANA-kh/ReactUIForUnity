using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class TestGameManager : MonoBehaviour
    {
        private void Start()
        {
            //做些数据填充Model
            string[] icoNames =
            {
                "Add Icon","Bell", "Coin", "Facebook", "Gem",
                "Info", "Inventory", "Star","World"
            };
            var rankData = new List<UIRankWindowController.RankItemData>();
            for (int i = 0; i < 15; i++)
            {
                var levelRandom = Random.Range(1, 5);
                var level = (14 - i) * 5 + levelRandom;
                rankData.Add(new UIRankWindowController.RankItemData()
                {
                    RankNum = i + 1,
                    UserIcon = icoNames[i%9],
                    UserName = icoNames[i%9],
                    Level = level,
                    LevelPoints = (int)(level*level*Math.Log(level)),
                    ShowFlag = true
                });
            }
            var model = UIModelManager.Instance.GetModel<UIModelRank>();
            model.LoadData("RANK",rankData);
        }
    }
}