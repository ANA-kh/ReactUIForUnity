using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ReactUI;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace ReactUI
{
    [CustomPropertyDrawer(typeof(VariableNameAttribute))]
    internal sealed class VariableNameAttribute_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect rc, SerializedProperty prop, GUIContent ctx)
        {
            EditorGUI.BeginProperty(rc, ctx, prop);
            UIVariableBind val = (UIVariableBind)prop.serializedObject.targetObject;
            if (val != null)
            {
                UIVariableTable variableTable = val.VariableTable;
                if (variableTable != null)
                {
                    string[] variableNames = variableTable.GetVariableNames();
                    if (variableNames != null)
                    {
                        DrawTable(rc, prop, ctx, variableTable);
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUI.PropertyField(rc, prop);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(rc, prop);
                    GUI.enabled = true;
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(rc, prop);
                GUI.enabled = true;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent ctx)
        {
            return 1f * EditorGUIUtility.singleLineHeight;
        }

        private void DrawTable(Rect P_0, SerializedProperty propVar, GUIContent ctx, UIVariableTable table)
        {
            string[] varNameList = table.GetVariableNames();
            VariableNameAttribute val = (VariableNameAttribute)this.attribute;
            Assert.IsNotNull<VariableNameAttribute>(val);
            List<string> list = new List<string>();
            foreach (string text in varNameList)
            {
                UIVariable val2 = table.FindVariable(text);
                if (val2 != null && val.IsValid(val2.Type))
                {
                    list.Add(text);
                }
            }

            UIVariableBind obj = (UIVariableBind)propVar.serializedObject.targetObject;
            //search parent variable table
            Transform start = obj.transform.parent;
            string baseNamePath = "";
            while (start != null)
            {
                UIVariableTable parentTable = start.gameObject.GetComponentInParent<UIVariableTable>();
                if (parentTable == null)
                {
                    break;
                }

                if (parentTable == table)
                {
                    baseNamePath = parentTable.name + "/";
                    start = parentTable.transform.parent;
                    continue;
                }

                varNameList = parentTable.GetVariableNames();
                foreach (string text in varNameList)
                {
                    UIVariable val2 = parentTable.FindVariable(text);
                    if (val2 != null && val.IsValid(val2.Type))
                    {
                        //string namePath = baseNamePath + parentTable.name + "/" + text;
                        string namePath = "@" + parentTable.name + "/" + text;
                        list.Add(namePath);
                    }
                }

                baseNamePath = parentTable.name + "/";
                start = parentTable.transform.parent;
            }

            ////////////////////////////////
            Rect rect = new Rect(P_0.x, P_0.y, P_0.width * 0.39f, P_0.height);
            Rect rect2 = new Rect(P_0.x + P_0.width * 0.39f, P_0.y, P_0.width * 0.61f, P_0.height);
            EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), ctx);
            int num = list.IndexOf(propVar.stringValue);
            list.Add("None");
            int num2 = EditorGUI.Popup(rect2, num, list.ToArray());
            if (num2 != num && num2 >= 0)
            {
                if (num2 < list.Count - 1)
                {
                    propVar.stringValue = (list[num2]);
                }
                else
                {
                    propVar.stringValue = (string.Empty);
                }
            }
        }
    }

    [CustomEditor(typeof(UIVariableBindText))]
    internal sealed class UIVariableBindTextEditor : Editor
    {
        private SerializedProperty _variableTable;

        private SerializedProperty _setTextNotKey;
        private SerializedProperty _format;

        private SerializedProperty _paramBinds;

        private ReorderableList _list;

        public override void OnInspectorGUI()
        {
            UIVariableBind val = (UIVariableBind)target;
            UIVariableTable variableTable = val.VariableTable;
            if ((UnityEngine.Object)(object)variableTable == null)
            {
                EditorGUILayout.HelpBox("There is no EventTable in parent.", (MessageType)3);
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(_variableTable, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(_format, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(_setTextNotKey, new GUILayoutOption[0]);
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            SerializedObject serializedObject = this.serializedObject;
            _variableTable = serializedObject.FindProperty("variableTable");
            _setTextNotKey = serializedObject.FindProperty("setTextNotKey");
            _format = serializedObject.FindProperty("format");
            _paramBinds = serializedObject.FindProperty("paramBinds");
            _list = (ReorderableList)(object)new ReorderableList(serializedObject, _paramBinds);
            _list.drawHeaderCallback = (HeaderCallbackDelegate)(object)(HeaderCallbackDelegate)delegate(Rect P_0)
            {
                GUI.Label(P_0, "Param Binds:");
            };
            _list.elementHeight = 1f * EditorGUIUtility.singleLineHeight;
            _list.drawElementCallback =
                (ElementCallbackDelegate)(object)(ElementCallbackDelegate)delegate(Rect P_0, int P_1, bool P_2,
                    bool P_3)
                {
                    SerializedProperty arrayElementAtIndex = _paramBinds.GetArrayElementAtIndex(P_1);
                    EditorGUI.PropertyField(P_0, arrayElementAtIndex);
                };
        }
    }
}