using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReactUI
{
    public interface IUIVariableBindArrayItem
    {
        bool SkipAutoSetValue { get; }
        void RefreshItem(object obj);
    }

    public class UIVariableAutoBind
    {
        public class TypeFieldInfos
        {
            private static Dictionary<Type, TypeFieldInfos> TypeInfos = new Dictionary<Type, TypeFieldInfos>();
            public static TypeFieldInfos GetTypeFieldInfos(object obj)
            {
                TypeFieldInfos reslult = null;
                var t = obj.GetType();
                if (!TypeInfos.TryGetValue(t, out reslult))
                {
                    var info = new TypeFieldInfos();
                    info.Bind(t);
                    TypeInfos[t] = info;
                    reslult = info;
                }
                return reslult;
            }

            private Dictionary<string, FieldInfo> fieldDic = new Dictionary<string, FieldInfo>();
            private Dictionary<string, PropertyInfo> propertyDic = new Dictionary<string, PropertyInfo>();
            public void Bind(Type type)
            {
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
                foreach (var fieldInfo in fieldInfos)
                {
                    fieldDic[fieldInfo.Name] = fieldInfo;
                }
                foreach (var propInfo in propInfos)
                {
                    propertyDic[propInfo.Name] = propInfo;
                }
            }
            public bool GetValueImpl(object obj, string key, out object value)
            {
                if (fieldDic.ContainsKey(key))
                {
                    value = fieldDic[key].GetValue(obj);
                    return true;
                }
                else
                {
                    if (propertyDic.ContainsKey(key))
                    {
                        value = propertyDic[key].GetValue(obj);
                        return true;
                    }
                }
                value = null;
                return false;
            }
        }

        public static void AutoBind(UIVariableTable variableTable, object obj)
        {
            if (variableTable == null || obj == null)
            {
                return;
            }
            var fieldInfos = TypeFieldInfos.GetTypeFieldInfos(obj);
            foreach (var variable in variableTable.Variables)
            {
                if (variable != null)
                {
                    object value = null;
                    if (fieldInfos.GetValueImpl(obj, variable.Name, out value))
                    {
                        variable.SetValue(value);
                    }
                }
            }
        }
    }

    [AddComponentMenu("ReactUI/UI/Bind/Variable Bind Array")]
    public sealed class UIVariableBindArray : UIVariableBind
    {
        [SerializeField]
        [VariableName(UIVariableType.Array)]
        private string paramBinds;

        [SerializeField]
        GameObject itemTemplate;

        private UIVariable _bindVariable;

        protected override void BindVariables()
        {
            if (Application.isPlaying)
            {
                if (itemTemplate)
                {
                    var tab = itemTemplate.GetComponent<UIVariableTable>();
                    if (!tab)
                    {
                        itemTemplate = null;
                        return;
                    }
                    itemTemplate.SetActive(false);
                }
            }
            Assert.IsNull(_bindVariable);
            if (paramBinds == null || paramBinds.Length <= 0)
            {
                return;
            }
            _bindVariable = FindVariable(paramBinds);
            if (_bindVariable != null)
            {
                _bindVariable.OnValueInitialized += OnVariableInit;
                _bindVariable.OnValueChanged += RefreshList;
                _bindVariable.AddBind(this);
            }
        }

        protected override void UnbindVariables()
        {
            if (_bindVariable == null)
            {
                return;
            }
            if (_bindVariable != null)
            {
                _bindVariable.OnValueInitialized -= OnVariableInit;
                _bindVariable.OnValueChanged -= RefreshList;
                _bindVariable.RemoveBind(this);
            }
            _bindVariable = null;
        }

        private void RefreshList()
        {
            RefreshListNormal();
        }

        private void OnVariableInit()
        {
            RefreshListNormal();
        }

        private List<UIVariableTable> variableTables = new List<UIVariableTable>();
        private List<IUIVariableBindArrayItem> variableBindArrayItems = new List<IUIVariableBindArrayItem>();

        void RefreshListNormal()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (paramBinds == null || itemTemplate == null)
            {
                return;
            }

            var list = _bindVariable.GetArray();
            if (list == null)
            {
                foreach (var item in variableTables)
                {
                    item.gameObject.SetActive(false);
                }
            }
            else
            {
                var totalCount = list.Count;
                var delta = totalCount - variableTables.Count;
                var isPrefab = !gameObject.scene.IsValid();
                var parent = isPrefab ? this.transform : itemTemplate.transform.parent;
                for (int i = 0; i < delta; i++)
                {
                    var go = GameObject.Instantiate(itemTemplate, parent);
                    var tab = go.GetComponent<UIVariableTable>();
                    var bindArrayItem = go.GetComponent<IUIVariableBindArrayItem>();
                    variableTables.Add(tab);
                    variableBindArrayItems.Add(bindArrayItem);
                }

                for (int i = 0; i < totalCount; i++)
                {
                    variableTables[i].gameObject.SetActive(true);
                }
                for (int i = totalCount; i < variableTables.Count; i++)
                {
                    variableTables[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < totalCount; i++)
                {
                    var obj = list[i];
                    var variableTable = variableTables[i];
                    var bindArrayItem = variableBindArrayItems[i];
                    if (bindArrayItem != null)
                    {
                        bindArrayItem.RefreshItem(obj);
                        if (bindArrayItem.SkipAutoSetValue)
                        {
                            continue;
                        }
                    }
                    if (obj is IDictionary<string, object> dic)
                    {
                        foreach (var variable in variableTable.Variables)
                        {
                            if (variable != null)
                            {
                                object value = null;
                                if (dic.TryGetValue(variable.Name, out value))
                                {
                                    variable.SetValue(value);
                                }
                            }
                        }
                    }
                    else
                    {
                        UIVariableAutoBind.AutoBind(variableTable, obj);
                    }
                }
            }
        }
    }
}
