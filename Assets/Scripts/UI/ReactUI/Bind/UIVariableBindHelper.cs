using System;
using System.Collections.Generic;
using System.Reflection;
using ReactUI.TestReactUI;
using UnityEngine;

namespace ReactUI
{
    public class UIVariableBindHelper
    {
        public struct VarBindData
        {
            public string VarName;
            public PropertyInfo Prop;
            public FieldInfo Field;
        }

        public struct EventBindData
        {
            public string EventName;
            public MethodInfo Method;
        }

        public struct GameObjectBindData
        {
            public string VarName;
            public PropertyInfo Prop;
            public FieldInfo Field;
        }

        public class AutoBindForClass
        {
            public List<VarBindData> BindVars = new List<VarBindData>();
            public List<EventBindData> BindEvents = new List<EventBindData>();
            public List<GameObjectBindData> BindGameObjects = null;
        }

        private static Dictionary<Type, AutoBindForClass> _cacheBindProp = new Dictionary<Type, AutoBindForClass>();
        
        /*
         TODO 突发奇想 考虑不使用运行时反射绑定，在非运行时通过反射生成绑定的样板代码参考UIHudLootView。
        思路是反射获取名字，生成类似var_item = UIVariableTable.FindVariable("item") 代码;
        或许有性能提升，待实验
        */
        public static void AutoBind(object obj, GameObject view, Type baseClass = null)
        {
            var t = obj.GetType();
            AutoBindForClass autoBind = null;
            if (!_cacheBindProp.TryGetValue(t, out autoBind))
            {
                //反射获取各个绑定了指定属性（[AutoBindVariable] [AutoBindGameObject]）的元素（field prop method）   还没实际绑定到variableTable
                autoBind = GenerateAutoBind(t, baseClass);
                _cacheBindProp.Add(t,autoBind);
            }
            
            //实际的绑定  controller->variableTable
            //bind variable
            var vt = view.GetComponent<UIVariableTable>();
            if (vt != null)
            {
                foreach (var prop in autoBind.BindVars)
                {
                    var n = prop.VarName;
                    var variable = vt.FindVariable(n);
                    if (variable != null)
                    {
                        if (prop.Prop != null)
                        {
                            (prop.Prop.GetSetMethod(true) ?? throw new ArgumentException("Property " + prop.Prop.Name + " has no setter")).Invoke(obj, new object[1]
                            {
                                variable
                            });
                        }
                        if (prop.Field != null)
                        {
                            prop.Field.SetValue(obj, variable);
                        }
                    }
                }
            }
            //bind event
            var et = view.GetComponent<UIEventTable>();
            if (et != null)
            {
                for (int i = 0; i < autoBind.BindEvents.Count; i++)
                {
                    var evdata = autoBind.BindEvents[i];
                    var method = evdata.Method;

                    var del = Delegate.CreateDelegate(typeof(SignalDelegate), obj, method) as SignalDelegate;
                    et.ListenEvent(evdata.EventName, del);
                }
            }
            //bind gameObject
            if (autoBind.BindGameObjects != null)
            {
                AutoBindGameObject(obj, view, autoBind.BindGameObjects);
            }
        }
        
        private static void AutoBindGameObject(object targetObject, GameObject rootObj, List<GameObjectBindData> bind)
        {
            var childItem = rootObj.GetComponentsInChildren<UIItemVariable>(true);

            for (int i = 0; i < bind.Count; i++)
            {
                var bindData = bind[i];
                var n = bindData.VarName;
                UIItemVariable refItem = null;
                for (int k = 0; k < childItem.Length; k++)
                {
                    var item = childItem[k];
                    string itemName = string.IsNullOrEmpty(item.ExportName) ? item.name : item.ExportName;
                    if (itemName == n)
                    {
                        refItem = childItem[k];
                        break;
                    }
                }
                if (refItem == null)
                {
                    Debug.LogError("AutoBindGameObject missed game object item:" + n);
                    continue;
                }
                Type targetType = null;
                if (bindData.Prop != null)
                {
                    targetType = bindData.Prop.PropertyType;
                }
                if (bindData.Field != null)
                {
                    targetType = bindData.Field.FieldType;
                }
                object value = null;
                if (targetType == typeof(GameObject))
                {
                    value = refItem.gameObject;
                }
                else if (targetType == typeof(Transform))
                {
                    value = refItem.transform;
                }
                else
                {
                    value = refItem.GetComponent(targetType);
                }

                if (bindData.Prop != null)
                {
                    (bindData.Prop.GetSetMethod(true) ?? throw new ArgumentException("Property " + bindData.Prop.Name + " has no setter")).Invoke(targetObject, new object[1]
                    {
                        value
                    });
                }
                if (bindData.Field != null)
                {
                    bindData.Field.SetValue(targetObject, value);
                }

            }
        }

        private static AutoBindForClass GenerateAutoBind(Type t, Type baseClass = null)
        {
            var autoBind = new AutoBindForClass();
            GenerateAutoBind(autoBind, t);
            while (true)
            {
                if (baseClass != null && t != baseClass)
                {
                    if (t.BaseType != null)
                    {
                        t = t.BaseType;
                        GenerateAutoBind(autoBind, t);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return autoBind;
        }
        public static string prefix_var = "var_";
        public static string prefix_event = "event_";
        public static string prefix_gameobject = "go_";
        private static void GenerateAutoBind(AutoBindForClass autoBind, Type t)
        {
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var methodInfo = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldInfo = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            //bind props
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                //GetCustomAttribute 获取标记了[AutoBindVariable]属性的元素
                var bindAttr = prop.GetCustomAttribute<AutoBindVariableAttribute>();
                if (bindAttr != null)
                {
                    VarBindData d = new VarBindData();
                    d.Prop = prop;
                    d.VarName = prop.Name;
                    if (d.VarName.StartsWith(prefix_var))
                    {
                        d.VarName = d.VarName.Substring(prefix_var.Length);
                    }
                    autoBind.BindVars.Add(d);
                }
                var autoGo = prop.GetCustomAttribute<AutoBindGameObjectAttribute>();
                if (autoGo != null)
                {
                    GameObjectBindData d = new GameObjectBindData();
                    d.Prop = prop;
                    d.VarName = prop.Name;

                    if (d.VarName.StartsWith(prefix_gameobject))
                    {
                        d.VarName = d.VarName.Substring(prefix_gameobject.Length);
                    }
                    autoBind.BindGameObjects.Add(d);
                }
            }
            
            //bind fields
            foreach (var prop in fieldInfo)
            {
                var bindAttr = prop.GetCustomAttribute<AutoBindVariableAttribute>();
                if (bindAttr != null)
                {
                    var d = new VarBindData();
                    d.Field = prop;
                    d.VarName = prop.Name;
                    if (d.VarName.StartsWith(prefix_var))
                    {
                        d.VarName = d.VarName.Substring(prefix_var.Length);
                    }
                    autoBind.BindVars.Add(d);
                }

                var autoGo = prop.GetCustomAttribute<AutoBindGameObjectAttribute>();
                if (autoGo != null)
                {
                    var d = new GameObjectBindData();
                    d.Field = prop;
                    d.VarName = prop.Name;

                    if (d.VarName.StartsWith(prefix_gameobject))
                    {
                        d.VarName = d.VarName.Substring(prefix_gameobject.Length);
                    }

                    if (autoBind.BindGameObjects == null)
                    {
                        autoBind.BindGameObjects = new List<GameObjectBindData>();
                    }
                    autoBind.BindGameObjects.Add(d);
                }
            }
            //bind events
            for (int i = 0; i < methodInfo.Length; i++)
            {
                var info = methodInfo[i];
                var attr = info.GetCustomAttribute<AutoBindEventAttribute>(true);
                if (attr != null)
                {
                    var d = new EventBindData();
                    d.Method = info;
                    d.EventName = info.Name;
                    if (d.EventName.StartsWith(prefix_event))
                    {
                        d.EventName = d.EventName.Substring(prefix_event.Length);
                    }
                    autoBind.BindEvents.Add(d);
                }
            }
        }
    }
}