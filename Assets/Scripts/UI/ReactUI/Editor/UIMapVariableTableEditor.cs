using System;
using System.Collections.Generic;
using ReactUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ReactUI
{
    [CustomEditor(typeof(UIMapVariableTable))]
    public class UIMapVariableTableEditor : Editor
    {
        private UIMapVariableTable _targetTable;
        private SerializedProperty _mapVariables;
        private ReorderableList _list;

        private readonly Dictionary<int, SerializedProperty> propMap = new Dictionary<int, SerializedProperty>();

        private readonly HashSet<int> nameSet = new HashSet<int>();
        private readonly Dictionary<string, int> name2Idx = new Dictionary<string, int>(StringComparer.Ordinal);

        private string[] typeNames;
        private readonly string[] boolNames = { "False", "True" };

        private readonly List<UIVariableType> excludeTypes = new List<UIVariableType>()
            { UIVariableType.Array };

        private readonly Dictionary<int, int> choseSrcMap = new Dictionary<int, int>();

        private void OnEnable()
        {
            if (target == null) return;
            _targetTable = (UIMapVariableTable)target;
            _mapVariables = serializedObject.FindProperty("mapVariables");
            _list = new ReorderableList(serializedObject, _mapVariables)
            {
                drawHeaderCallback = rect => GUI.Label(rect, "Variables:"),
                elementHeightCallback = GetHeight,
                drawElementCallback = (rect, index, active, focused) =>
                    DrawOneMapVariable(_mapVariables, rect, index, active, focused),
                onAddCallback = list =>
                {
                    _targetTable.AddDefaultVariable();
                    EditorUtility.SetDirty(target);
                }
            };

            typeNames = new string[(int)UIVariableType.String + 1];
            for (int i = 0; i < typeNames.Length; i++)
            {
                typeNames[i] = ((UIVariableType)i).ToString();
            }

            Init();
        }

        private void Init()
        {
            nameSet.Clear();
            name2Idx.Clear();
            for (int i = 0; i < _mapVariables.arraySize; i++)
            {
                var varProp = _mapVariables.GetArrayElementAtIndex(i);
                var outVarProp = varProp.FindPropertyRelative("outVariable");
                var outNameProp = outVarProp.FindPropertyRelative("name");
                if (name2Idx.ContainsKey(outNameProp.stringValue))
                {
                    nameSet.Add(name2Idx[outNameProp.stringValue]);
                    nameSet.Add(i);
                }
                else
                {
                    name2Idx.Add(outNameProp.stringValue, i);
                }
            }
        }

        private float GetHeight(int idx)
        {
            var map = _targetTable.GetMapVariable(idx).map;
            var mapCount = map?.Count + 2 ?? 2;
            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.singleLineHeight * mapCount + 30;
        }

        private void DrawOneMapVariable(SerializedProperty mapVarsProp, Rect rect, int idx, bool active, bool focused)
        {
            if (!propMap.TryGetValue(idx, out var varProp))
            {
                varProp = mapVarsProp.GetArrayElementAtIndex(idx);
                propMap.Add(idx, varProp);
            }

            var oldWidth = EditorGUIUtility.labelWidth;

            var bgColor = GUI.backgroundColor;
            var color = GUI.color;
            var flag = nameSet.Contains(idx);
            // var mapNull = targetTable.GetMapVariable(idx).map == null || targetTable.GetMapVariable(idx).map.Count == 0;
            if (flag)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
            }

            EditorGUIUtility.labelWidth = 85;

            var srcNameIdxProp = varProp.FindPropertyRelative("srcVarIdx");
            var outVarProp = varProp.FindPropertyRelative("outVariable");
            var outNameProp = outVarProp.FindPropertyRelative("name");
            var outTypeProp = outVarProp.FindPropertyRelative("type");

            // src variable index
            var tmpRect = new Rect(rect.x + 5f, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight);
            var srcVarNameList = _targetTable.VarTable.GetOriginalVariableNames(excludeTypes);
            srcNameIdxProp.intValue = EditorGUI.Popup(tmpRect, "来自Variable:", srcNameIdxProp.intValue, srcVarNameList);
            if (!choseSrcMap.ContainsKey(idx) ||
                (choseSrcMap[idx] != srcNameIdxProp.intValue && srcNameIdxProp.intValue >= 0))
            {
                _targetTable.SetSrcValue(srcNameIdxProp.intValue, idx);
                choseSrcMap[idx] = srcNameIdxProp.intValue;
                if (string.IsNullOrEmpty(outNameProp.stringValue) && srcNameIdxProp.intValue >= 0 &&
                    srcNameIdxProp.intValue < srcVarNameList.Length)
                {
                    outNameProp.stringValue = srcVarNameList[srcNameIdxProp.intValue] + "Map";
                }
            }

            // name
            tmpRect = new Rect(rect.x + 5f, rect.y + EditorGUIUtility.singleLineHeight + 3, rect.width - 5,
                EditorGUIUtility.singleLineHeight);
            GUI.backgroundColor = string.IsNullOrEmpty(outNameProp.stringValue) ? Color.red : bgColor;
            outNameProp.stringValue = EditorGUI.TextField(tmpRect, "Name:", outNameProp.stringValue);
            GUI.backgroundColor = bgColor;

            // type
            tmpRect = new Rect(rect.x + 5f, rect.y + EditorGUIUtility.singleLineHeight * 2 + 5, rect.width - 5,
                EditorGUIUtility.singleLineHeight);
            outTypeProp.enumValueIndex = EditorGUI.Popup(tmpRect, "Type:", outTypeProp.enumValueIndex, typeNames);

            // map
            tmpRect = new Rect(rect.x + 5f, rect.y + EditorGUIUtility.singleLineHeight * 3 + 10, rect.width - 5,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tmpRect, "映射值:");
            DrawMapKV(_targetTable.GetMapVariable(idx), tmpRect);


            GUI.color = color;
            EditorGUIUtility.labelWidth = oldWidth;
        }

        private void DrawMapKV(UIMapVariable mapVar, Rect rect)
        {
            if (mapVar.map == null) mapVar.map = new List<UIMapVariable.MapKV>();

            var srcType = mapVar.SrcType;
            var outType = mapVar.OutVariable.Type;

            for (int i = 0; i < mapVar.map.Count; i++)
            {
                var tRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * i, rect.width,
                    EditorGUIUtility.singleLineHeight);
                DrawOneMapKV(i, mapVar.map, tRect, srcType, outType);
            }

            var tmpRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * (mapVar.map.Count + 1) + 10,
                rect.width / 2f, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(tmpRect, "添加映射"))
            {
                mapVar.map.Add(new UIMapVariable.MapKV());
            }
        }

        private void DrawOneMapKV(int idx, List<UIMapVariable.MapKV> map, Rect rect, UIVariableType srcType,
            UIVariableType outType)
        {
            var kv = map[idx];
            EditorGUIUtility.labelWidth = 40;

            var width = (rect.width - 80) / 2;
            var tmpRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, width,
                EditorGUIUtility.singleLineHeight);
            switch (srcType)
            {
                case UIVariableType.Boolean:
                    DrawBoolenPart("Src:", ref kv.BoolKey, tmpRect);
                    break;
                case UIVariableType.Integer:
                    DrawIntPart("Src:", ref kv.IntKey, tmpRect);
                    break;
                case UIVariableType.Float:
                    DrawFloatPart("Src:", ref kv.FloatKey, tmpRect);
                    break;
                case UIVariableType.String:
                    DrawStringPart("Src:", ref kv.StringKey, tmpRect);
                    break;
            }

            tmpRect = new Rect(rect.x + width + 20, rect.y + EditorGUIUtility.singleLineHeight, width,
                EditorGUIUtility.singleLineHeight);
            switch (outType)
            {
                case UIVariableType.Boolean:
                    DrawBoolenPart("Out:", ref kv.BoolValue, tmpRect);
                    break;
                case UIVariableType.Integer:
                    DrawIntPart("Out:", ref kv.IntValue, tmpRect);
                    break;
                case UIVariableType.Float:
                    DrawFloatPart("Out:", ref kv.FloatValue, tmpRect);
                    break;
                case UIVariableType.String:
                    DrawStringPart("Out:", ref kv.StringValue, tmpRect);
                    break;
            }

            tmpRect = new Rect(rect.x + width * 2 + 40, rect.y + EditorGUIUtility.singleLineHeight, 40,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(tmpRect, "-"))
            {
                map.RemoveAt(idx);
            }
        }

        private void DrawBoolenPart(string label, ref bool obj, Rect rect)
        {
            var var = obj is bool b && b ? 1 : 0;
            var setVal = EditorGUI.Popup(rect, label, var, boolNames);
            obj = setVal > 0;
        }

        private void DrawIntPart(string label, ref int obj, Rect rect)
        {
            var var = obj is int i ? i : 0;
            var setVal = EditorGUI.IntField(rect, label, var);
            obj = setVal;
        }

        private void DrawStringPart(string label, ref string obj, Rect rect)
        {
            var var = obj is string s ? s : "";
            var setVal = EditorGUI.TextField(rect, label, var);
            obj = setVal;
        }

        private void DrawFloatPart(string label, ref float obj, Rect rect)
        {
            var var = obj is float f ? f : 0f;
            var setVal = EditorGUI.FloatField(rect, label, var);
            obj = setVal;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list.DoLayoutList();
            if (serializedObject.ApplyModifiedProperties())
            {
                propMap.Clear();
                Init();
            }

            /*
            if (GUILayout.Button("Generate C# code to clipboard"))
            {
                var sb = new System.Text.StringBuilder();
                foreach (UIMapVariable v in targetTable.Variables)
                {
                    sb.AppendLine("[AutoBindVariable]");
                    sb.Append("UIVariable var_").Append(v.OutVariable.Name).AppendLine(";");
                }
    
                GUIUtility.systemCopyBuffer = sb.ToString();
            }
            */
        }
    }
}