using UnityEditor;
using UnityEngine;

namespace ReactUI
{//TODO VariableNameAttribute_Drawer  UIVariableBindTextEditor
    /// <summary>
    /// 此基类只持有variableTable并保持更新
    /// variable的绑定根据子类的具体功能在子类中实现
    /// </summary>
    public abstract class UIVariableBind : MonoBehaviour
    {
        public static string _markCustomParentVariableTable="@";//TODO 对应VariableNameAttribute_Drawer DrawTable 里的@  后面统一下
        
        [Tooltip("The variable table for this bind.")]
        [SerializeField]
        private UIVariableTable variableTable;

        private bool isInited;

        public UIVariableTable VariableTable
        {
            get;
            private set;
        }

        internal virtual void Init()
        {
            if (!isInited)
            {
                isInited = true;
                FindVarTable();
                BindVariables();
            }
        }
        
        /// <summary>
        /// 通过variableName在绑定的variableTable中查找变量
        /// </summary>
        /// <param name="name">variableName</param>
        /// <returns></returns>
        public UIVariable FindVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            string realName = name;
            UIVariableTable vt = VariableTable;

            if (name.StartsWith(_markCustomParentVariableTable))
            {
                int pos = name.IndexOf('/');
                if(pos >= 0)
                {
                    string tableName = name.Substring(1, pos - 1);
                    realName = name.Substring(pos+1);
                    vt = FindCustomParentTable(tableName);
                }
				
            }
            if (vt != null)
            {
                return vt.FindVariable(realName);
            }
            return null;
        }
        private UIVariableTable FindCustomParentTable(string name)
        {
            Transform t = this.transform;
            while (t != null)
            {
                if (t.name == name)
                {
                    UIVariableTable table = t.GetComponent<UIVariableTable>();
                    if(table!=null)
                    {
                        return table;
                    }
                }
                t = t.parent;
            }
            return null;
        }
        private void FindVarTable()
        {
            if (variableTable == null)
            {
                variableTable = this.GetComponentInParent<UIVariableTable>();
            }
            VariableTable = variableTable;
        }
        protected virtual void BindVariables()
        {

        }

        protected virtual void UnbindVariables()
        {

        }


        #region UnityLifeCycle

        protected virtual void OnDestroy()
        {
            UnbindVariables();
            isInited = false;
        }

        protected virtual void Awake()
        {
            Init();
        }
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            if ((int)prefabType != 1)
            {
                UnbindVariables();
                isInited = true;
                FindVarTable();
                BindVariables();
            }
            else if (variableTable == null)
            {
                variableTable = this.GetComponentInParent<UIVariableTable>();
            }
#else
			if (variableTable == null)
			{
				variableTable = this.GetComponentInParent<UIVariableTable>();
			}
#endif
        }

        #endregion

        
    }
}