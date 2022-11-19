using System;
using UnityEngine;

namespace ReactUI.TestReactUI
{
    public class TestController : MonoBehaviour
    {
        [AutoBindVariable]
        UIVariable var_name;
        [AutoBindVariable]
        UIVariable var_avtive;
        [AutoBindVariable]
        UIVariable var_money;
        [AutoBindVariable]
        UIVariable var_item;

        private void Awake()
        {
            UIVariableBindHelper.AutoBind(this, gameObject);
            var_name.SetString("testName");
            var_avtive.SetBoolean(false);
            var_money.SetInteger(101);
        }
    }
}