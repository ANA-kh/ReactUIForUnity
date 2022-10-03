using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReactUI
{
    [Serializable]
    public sealed class UIMapVariable
    {
        [SerializeField]
        private int srcVarIdx = -1;
        
        [SerializeField]
        private UIVariable outVariable;

        [SerializeField] 
        public List<MapKV> map;
        
        [Serializable]
        public class MapKV
        {
            [SerializeField] 
            public bool BoolKey;
            [SerializeField] 
            public bool BoolValue;
            
            [SerializeField] 
            public int IntKey;
            [SerializeField] 
            public int IntValue;
            
            [SerializeField] 
            public float FloatKey;
            [SerializeField] 
            public float FloatValue;
            
            [SerializeField] 
            public string StringKey;
            [SerializeField] 
            public string StringValue;
        }
        
        public UIVariable OutVariable => outVariable;

        private UIVariable srcVariable;

        public bool IsValid => srcVarIdx >= 0 && !string.IsNullOrEmpty(outVariable.Name);
        
        
        public UIMapVariable()
        {
            map = new List<MapKV>();
        }
        
        public void SetSrcVariable(UIVariableTable variableTable, int srcIdx)
        {
            srcVarIdx = srcIdx;
            Init(variableTable);
        } 

        public void Init(UIVariableTable variableTable)
        {
            if (variableTable == null || srcVarIdx < 0) return;
            srcVariable = variableTable.GetVariable(srcVarIdx);
            if (srcVariable == null) return;
            BindEvent();
        }

        public void BindEvent()
        {
            if (srcVariable != null)
            {
                srcVariable.OnValueChanged += UpdateValues;
                srcVariable.OnValueInitialized += UpdateValues;
            }
        }

        public void UnBindEvent()
        {
            if (srcVariable != null)
            {
                srcVariable.OnValueChanged -= UpdateValues;
                srcVariable.OnValueInitialized -= UpdateValues;
            }
        }

        public void UpdateValues()
        {
            if (srcVariable == null)
            {
                outVariable.ResetValue();
                return;
            }

            switch (srcVariable.Type)
            {
                case UIVariableType.Boolean:
                {
                    var srcVal = srcVariable.GetBoolean();
                    if (map != null && map.Count > 0)
                    {
                        foreach (var kv in map)
                        {
                            if (kv.BoolKey == srcVal)
                            {
                                SetOutVariable(kv);
                            }
                        }
                    }
                    else
                    {
                        MapSrcValueBoolean(srcVal);
                    }
                }
                    break;
                case UIVariableType.Integer:
                {
                    var srcVal = srcVariable.GetInteger();
                    if (map != null && map.Count > 0)
                    {
                        foreach (var kv in map)
                        {
                            if (kv.IntKey == srcVal)
                            {
                                SetOutVariable(kv);
                            }
                        }
                    }
                    else
                    {
                        MapSrcValueInt(srcVal);
                    }
                }
                    break;
                case UIVariableType.Float:
                {
                    var srcVal = srcVariable.GetFloat();
                    if (map != null && map.Count > 0)
                    {
                        foreach (var kv in map)
                        {
                            if (Mathf.Approximately(kv.FloatKey, srcVal))
                            {
                                SetOutVariable(kv);
                            }
                        }
                    }
                    else
                    {
                        MapSrcValueFloat(srcVal);
                    }
                }
                    break;
                case UIVariableType.String:
                {
                    var srcVal = srcVariable.GetString();
                    if (map != null && map.Count > 0)
                    {
                        foreach (var kv in map)
                        {
                            if (kv.StringKey.Equals(srcVal))
                            {
                                SetOutVariable(kv);
                            }
                        }
                    }
                    else
                    {                        
                        MapSrcValueString(srcVal);
                    }
                }
                    break;
            }
        }

        private void MapSrcValueBoolean(bool srcVal)
        {
            if (outVariable.Type == UIVariableType.Boolean) 
            {
                outVariable.SetBoolean(srcVal);
            }
            else if (outVariable.Type == UIVariableType.Integer)
            {
                outVariable.SetInteger(srcVal ? 1 : 0);
            }
            else if (outVariable.Type == UIVariableType.Float)
            {
                outVariable.SetFloat(srcVal ? 1.0f : 0f);
            }
            else if (outVariable.Type == UIVariableType.String)
            {
                outVariable.SetString(srcVal.ToString());
            }
        }
        
        private void MapSrcValueInt(int srcVal)
        {
            if (outVariable.Type == UIVariableType.Boolean) 
            {
                outVariable.SetBoolean(srcVal > 0);
            }
            else if (outVariable.Type == UIVariableType.Integer)
            {
                outVariable.SetInteger(srcVal);
            }
            else if (outVariable.Type == UIVariableType.Float)
            {
                outVariable.SetFloat(srcVal);
            }
            else if (outVariable.Type == UIVariableType.String)
            {
                outVariable.SetString(srcVal.ToString());
            }
        }
        
        private void MapSrcValueFloat(float srcVal)
        {
            if (outVariable.Type == UIVariableType.Boolean) 
            {
                outVariable.SetBoolean(srcVal > 0);
            }
            else if (outVariable.Type == UIVariableType.Integer)
            {
                outVariable.SetInteger(Mathf.RoundToInt(srcVal));
            }
            else if (outVariable.Type == UIVariableType.Float)
            {
                outVariable.SetFloat(srcVal);
            }
            else if (outVariable.Type == UIVariableType.String)
            {
                outVariable.SetString(srcVal.ToString("F2"));
            }
        }
        
        private void MapSrcValueString(string srcVal)
        {
            if (outVariable.Type == UIVariableType.Boolean)
            {
                outVariable.SetBoolean(bool.TryParse(srcVal, out var b) && b);
            }
            else if (outVariable.Type == UIVariableType.Integer)
            {
                outVariable.SetInteger(int.TryParse(srcVal, out var i) ? i : 0);
            }
            else if (outVariable.Type == UIVariableType.Float)
            {
                outVariable.SetFloat(float.TryParse(srcVal, out var f) ? f : 0f);
            }
            else if (outVariable.Type == UIVariableType.String)
            {
                outVariable.SetString(srcVal);
            }
        }
        
            
        private void SetOutVariable(MapKV _map)
        {
            switch (outVariable.Type)
            {
                case UIVariableType.Boolean:
                    outVariable.SetBoolean(_map.BoolValue);
                    break;
                case UIVariableType.Integer:
                    outVariable.SetInteger(_map.IntValue);
                    break;
                case UIVariableType.Float:
                    outVariable.SetFloat(_map.FloatValue);
                    break;
                case UIVariableType.String:
                    outVariable.SetString(_map.StringValue);
                    break;
            }
        }

        public UIVariableType SrcType => srcVariable?.Type ?? UIVariableType.Boolean;

        public void AddDefaultMapElement()
        {
            var kv = new MapKV();
            if (map == null)
            {
                map = new List<MapKV> {kv};
            }
            else
            {
                map.Add(kv);
            }
        }
    }
}