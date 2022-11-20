using UnityEngine;

namespace ReactUI
{
    /// <summary>
    /// 提供UIVariable的映射功能，供UIVariableBind使用
    /// 比如将float类型的UIVariable按某一阈值映射为bool
    /// </summary>
    [RequireComponent(typeof(UIVariableTable))]
    public sealed class UIMapVariableTable : MonoBehaviour
    {
        [SerializeField] private UIMapVariable[] mapVariables;
        public UIMapVariable[] Variables => mapVariables;


        private UIVariableTable varTable;

        public UIVariableTable VarTable
        {
            get
            {
                if (varTable == null)
                    varTable = GetComponent<UIVariableTable>();
                return varTable;
            }
        }

        public void Init()
        {
            if (mapVariables == null)
            {
                mapVariables = new UIMapVariable[0];
            }
            foreach (var mv in mapVariables)
            {
                mv.Init(VarTable);
            }
        }
        
        private void OnEnable()
        {
            if (mapVariables == null)
            {
                mapVariables = new UIMapVariable[0];
            }
            varTable = GetComponent<UIVariableTable>();
        }

        private void OnValidate()
        {
            if (mapVariables == null)
            {
                mapVariables = new UIMapVariable[0];
            }
            foreach (var variable in mapVariables)
            {
                variable.UnBindEvent();
            }
            VarTable.ResetVarMap();
            foreach (var variable in mapVariables)
            {
                variable.BindEvent();
            }
        }

        private void OnDestroy()
        {
            foreach (var variable in mapVariables)
            {
                variable.UnBindEvent();
            }
        }

        public UIMapVariable GetMapVariable(int index)
        {
            return mapVariables[index];
        }

        public void SetSrcValue(int srcIdx, int mapVarIdx)
        {
            if (mapVarIdx >= 0 && mapVarIdx < mapVariables.Length)
            {
                mapVariables[mapVarIdx].SetSrcVariable(VarTable, srcIdx);
            }
        }

        public void AddDefaultVariable()
        {
            var mapVariable = new UIMapVariable();
            if (mapVariables == null)
            {
                mapVariables = new UIMapVariable[1];
                mapVariables[0] = mapVariable;
            }
            else
            {
                var var_new = new UIMapVariable[mapVariables.Length + 1];
                for (int i = 0; i < mapVariables.Length; i++)
                {
                    var_new[i] = mapVariables[i];
                }

                var_new[var_new.Length - 1] = mapVariable;
                mapVariables = var_new;
            }
        }
    }
}