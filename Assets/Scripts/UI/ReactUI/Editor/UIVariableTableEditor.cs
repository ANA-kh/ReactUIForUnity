using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ReactUI;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace ReactUI
{
    /// <summary>
    /// VariableTable外观类
    /// </summary>
    [CustomEditor(typeof(UIVariableTable))]
    internal sealed class UIVariableTableEditor : Editor
    {
        private SerializedProperty _variables;

        private ReorderableList list;

        private Dictionary<int, SerializedProperty> _propMap = new Dictionary<int, SerializedProperty>();

        private HashSet<int> _nameSet = new HashSet<int>();

        private Dictionary<string, int> _name2Idx = new Dictionary<string, int>(StringComparer.Ordinal);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            if (serializedObject.ApplyModifiedProperties())
            {
                _propMap.Clear();
                Init();
                //打印时间
            }

            UIVariableTable val = (UIVariableTable)target;

            #region 生成样板代码
            if (GUILayout.Button("Sort"))
            {
                Undo.RecordObject(val, "Sort Variable Table");
                serializedObject.Update();
                val.Sort();
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate C# code to clipboard"))
            {
                var sb = new StringBuilder();
                foreach (UIVariable v in val.Variables)
                {
                    sb.AppendLine("[AutoBindVariable]");
                    sb.Append("UIVariable var_").Append(v.Name).AppendLine(";");
                }

                GUIUtility.systemCopyBuffer = sb.ToString();
            }

            if (GUILayout.Button("Generate C# code to clipboard(with GameObject Bind)"))
            {
                var sb = new StringBuilder();
                foreach (UIVariable v in val.Variables)
                {
                    sb.AppendLine("[AutoBindVariable]");
                    sb.Append("UIVariable " + UIVariableBindHelper.prefix_var).Append(v.Name).AppendLine(";");
                }

                UIItemVariable[] bindGOList = val.GetComponentsInChildren<UIItemVariable>();
                foreach (UIItemVariable bindGO in bindGOList)
                {
                    sb.AppendLine("[AutoBindGameObject]");
                    string typeName = bindGO.GetEUIItemExportTypeCorrespondOriginTypeName(bindGO.ExportType, bindGO);
                    sb.Append(typeName).Append(" " + UIVariableBindHelper.prefix_gameobject)
                        .Append(bindGO.GetExportedName()).AppendLine(";");
                }

                GUIUtility.systemCopyBuffer = sb.ToString();
            }
            #endregion
        }

        private void OnEnable()
        {
            if (!(target == null))
            {
                _variables = serializedObject.FindProperty("variables");
                list = (ReorderableList)(object)new ReorderableList(serializedObject, _variables);
                list.drawHeaderCallback = delegate(Rect position) { GUI.Label(position, "Variables:"); };
                list.elementHeightCallback =
                    (ElementHeightCallbackDelegate)(object)(ElementHeightCallbackDelegate)((int index) =>
                        GetHeight(_variables, index));
                list.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
                {
                    DrawOneVariable(_variables, rect, index, isActive, isFocused);
                };
                list.onAddCallback = delegate
                {
                    UIVariableTable val = (UIVariableTable)(object)(UIVariableTable)target;
                    val.AddDefaultVariable();
                    EditorUtility.SetDirty(target);
                };
                Init();
            }
        }

        private float GetHeight(SerializedProperty property, int index)
        {
            if (!_propMap.TryGetValue(index, out SerializedProperty value))
            {
                if (property.arraySize == 0)
                {
                    return 0;
                }

                value = property.GetArrayElementAtIndex(index);
                _propMap.Add(index, value);
            }

            if (!value.isExpanded)
            {
                return 2f * EditorGUIUtility.singleLineHeight;
            }

            UIVariableTable val = (UIVariableTable)target;
            UIVariable variable = val.GetVariable(index);
            ICollection<UIVariableBind> binds = variable.Binds;
            return (2 + binds.Count) * EditorGUIUtility.singleLineHeight;
        }

        private void DrawOneVariable(SerializedProperty property, Rect position, int index, bool isActive, bool isFocused)
        {
            if (!_propMap.TryGetValue(index, out SerializedProperty value))
            {
                value = property.GetArrayElementAtIndex(index);
                _propMap.Add(index, value);
            }

            bool flag = _nameSet.Contains(index);
            Color color = GUI.color;
            if (flag)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
            }

            SerializedProperty val = value.FindPropertyRelative("name");
            SerializedProperty val2 = value.FindPropertyRelative("type");
            Rect rect = new Rect(position.x + 8f, position.y, 16f, EditorGUIUtility.singleLineHeight);
            Rect rect2 = new Rect(position.x + 12f, position.y, (position.width - 12f) / 2f - 5f, EditorGUIUtility.singleLineHeight);
            Rect rect3 = new Rect(position.x + position.width / 2f + 5f, position.y, (position.width - 12f) / 2f - 5f,
                EditorGUIUtility.singleLineHeight);
            value.isExpanded = (EditorGUI.Foldout(rect, value.isExpanded, GUIContent.none));
            EditorGUI.PropertyField(rect2, val, GUIContent.none);
            EditorGUI.PropertyField(rect3, val2, GUIContent.none);
            Rect rect4 = new Rect(position.x + 12f, position.y + EditorGUIUtility.singleLineHeight, position.width - 12f,
                EditorGUIUtility.singleLineHeight);
            SerializedProperty val3 = null;
            UIVariableType val4 = (UIVariableType)val2.enumValueIndex;
            switch ((int)val4)
            {
                case 0:
                    val3 = value.FindPropertyRelative("booleanValue");
                    val3.boolValue = (EditorGUI.ToggleLeft(rect4, GUIContent.none, val3.boolValue));
                    break;
                case 1:
                    val3 = value.FindPropertyRelative("integerValue");
                    val3.intValue = (EditorGUI.IntField(rect4, GUIContent.none, val3.intValue));
                    break;
                case 2:
                    val3 = value.FindPropertyRelative("floatValue");
                    val3.floatValue = (EditorGUI.FloatField(rect4, GUIContent.none, val3.floatValue));
                    break;
                case 3:
                    val3 = value.FindPropertyRelative("stringValue");
                    val3.stringValue = (EditorGUI.TextField(rect4, GUIContent.none, val3.stringValue));
                    break;
            }

            if (value.isExpanded)
            {
                UIVariableTable val5 = (UIVariableTable)(object)(UIVariableTable)target;
                UIVariable variable = val5.GetVariable(index);
                ICollection<UIVariableBind> binds = variable.Binds;
                if (binds.Count > 0)
                {
                    GUI.enabled = false;
                    Rect rect5 = new Rect(position.x + 12f, position.y + EditorGUIUtility.singleLineHeight, position.width - 12f,
                        EditorGUIUtility.singleLineHeight);
                    foreach (UIVariableBind item in binds)
                    {
                        rect5.y += EditorGUIUtility.singleLineHeight;
                        if ((UnityEngine.Object)(object)item != null)
                        {
                            EditorGUI.ObjectField(rect5, (UnityEngine.Object)(object)item, ((object)item).GetType(),
                                true);
                        }
                        else
                        {
                            EditorGUI.ObjectField(rect5, (UnityEngine.Object)null, typeof(UnityEngine.Object), true);
                        }
                    }

                    GUI.enabled = true;
                }
            }

            GUI.color = color;
        }

        private void Init()
        {
            _nameSet.Clear();
            _name2Idx.Clear();
            for (int i = 0; i < _variables.arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = _variables.GetArrayElementAtIndex(i);
                SerializedProperty val = arrayElementAtIndex.FindPropertyRelative("name");
                if (_name2Idx.ContainsKey(val.stringValue))
                {
                    _nameSet.Add(_name2Idx[val.stringValue]);
                    _nameSet.Add(i);
                }
                else
                {
                    _name2Idx.Add(val.stringValue, i);
                }
            }
        }
    }
}