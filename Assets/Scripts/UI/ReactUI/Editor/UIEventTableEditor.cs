using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ReactUI;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using System.IO;

namespace ReactUI
{
    [CustomEditor(typeof(UIEventTable))]
    class UIEventTableEditor : Editor
    {
        private SerializedProperty _eventsProp;

        private ReorderableList _list;

        private HashSet<int> _setIdx = new HashSet<int>();

        private Dictionary<string, int> _name2idx = new Dictionary<string, int>(StringComparer.Ordinal);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list.DoLayoutList();
            if (serializedObject.ApplyModifiedProperties())
            {
                Init();
            }

            UIEventTable val = (UIEventTable)target;
            if (GUILayout.Button("Sort"))
            {
                Undo.RecordObject(val, "Sort Bind Table");
                serializedObject.Update();
                val.Sort();
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Generate C# code to clipboard"))
            {
                var sb = new System.Text.StringBuilder();
                foreach (string v in val.Events)
                {
                    sb.AppendLine("[AutoBindEvent]");
                    sb.Append("void event_").Append(v).AppendLine("(params object[] args)");
                    sb.AppendLine("{");
                    sb.AppendLine("");
                    sb.AppendLine("}");
                    sb.AppendLine();
                }

                GUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        private void OnEnable()
        {
            if (target != null)
            {
                SerializedObject serializedObject = ((Editor)this).serializedObject;
                _eventsProp = serializedObject.FindProperty("events");
                _list = new ReorderableList(serializedObject, _eventsProp);
                _list.drawHeaderCallback = (HeaderCallbackDelegate)(object)(HeaderCallbackDelegate)delegate(Rect P_0)
                {
                    GUI.Label(P_0, "Events:");
                };
                _list.elementHeightCallback =
                    (ElementHeightCallbackDelegate)(object)(ElementHeightCallbackDelegate)((int P_0) =>
                        GetHeight(_eventsProp, P_0));
                _list.drawElementCallback = delegate(Rect P_0, int P_1, bool P_2, bool P_3)
                {
                    DrawOneVariable(_eventsProp, P_0, P_1, P_2, P_3);
                };
                Init();
            }
        }

        private float GetHeight(SerializedProperty P_0, int P_1)
        {
            if (P_0.arraySize == 0)
            {
                return 0;
            }
            SerializedProperty arrayElementAtIndex = P_0.GetArrayElementAtIndex(P_1);
            if (!arrayElementAtIndex.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            UIEventTable val = (UIEventTable)target;
            ICollection<Component> collection = val.FindReferenced(arrayElementAtIndex.stringValue);
            if (collection == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return (float)(1 + collection.Count) * EditorGUIUtility.singleLineHeight;
        }

        private void DrawOneVariable(SerializedProperty P_0, Rect P_1, int P_2, bool P_3, bool P_4)
        {
            SerializedProperty arrayElementAtIndex = P_0.GetArrayElementAtIndex(P_2);
            bool flag = _setIdx.Contains(P_2);
            Color color = GUI.color;
            if (flag)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
            }

            Rect rect = new Rect(P_1.x + 8f, P_1.y, 16f, EditorGUIUtility.singleLineHeight);
            Rect rect2 = new Rect(P_1.x + 12f, P_1.y, P_1.width - 12f, EditorGUIUtility.singleLineHeight);
            arrayElementAtIndex.isExpanded = (EditorGUI.Foldout(rect, arrayElementAtIndex.isExpanded, GUIContent.none));
            EditorGUI.PropertyField(rect2, arrayElementAtIndex, GUIContent.none);
            if (arrayElementAtIndex.isExpanded)
            {
                UIEventTable val = (UIEventTable)target;
                ICollection<Component> collection = val.FindReferenced(arrayElementAtIndex.stringValue);
                if (collection != null)
                {
                    GUI.enabled = false;
                    Rect rect3 = new Rect(P_1.x + 12f, P_1.y, P_1.width - 12f, EditorGUIUtility.singleLineHeight);
                    foreach (Component item in collection)
                    {
                        rect3.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.ObjectField(rect3, (UnityEngine.Object)item, item.GetType(), true);
                    }

                    GUI.enabled = true;
                }
            }

            GUI.color = color;
        }

        private void Init()
        {
            _setIdx.Clear();
            _name2idx.Clear();
            for (int i = 0; i < _eventsProp.arraySize; i++)
            {
                SerializedProperty arrayElementAtIndex = _eventsProp.GetArrayElementAtIndex(i);
                if (_name2idx.ContainsKey(arrayElementAtIndex.stringValue))
                {
                    _setIdx.Add(_name2idx[arrayElementAtIndex.stringValue]);
                    _setIdx.Add(i);
                }
                else
                {
                    _name2idx.Add(arrayElementAtIndex.stringValue, i);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(EventNameAttribute))]
    class EventNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect P_0, SerializedProperty P_1, GUIContent P_2)
        {
            EditorGUI.BeginProperty(P_0, P_2, P_1);
            UIEventBind val = (UIEventBind)P_1.serializedObject.targetObject;
            if (val != null)
            {
                UIEventTable eventTable = val.EventTable;
                if (eventTable != null)
                {
                    Rect rect = new Rect(P_0.x, P_0.y, P_0.width * 0.39f, P_0.height);
                    Rect rect2 = new Rect(P_0.x + P_0.width * 0.39f, P_0.y, P_0.width * 0.61f, P_0.height);
                    EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), P_2);
                    string[] events = eventTable.Events;
                    if (events != null)
                    {
                        string[] array = new string[events.Length + 1];
                        Array.Copy(events, array, events.Length);
                        array[events.Length] = "None";
                        int num = Array.FindIndex(events, (string name) => name == P_1.stringValue);
                        int num2 = EditorGUI.Popup(rect2, num, array);
                        if (num2 != num && num2 >= 0)
                        {
                            if (num2 < events.Length)
                            {
                                P_1.stringValue = (events[num2]);
                            }
                            else
                            {
                                P_1.stringValue = (string.Empty);
                            }
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUI.PropertyField(P_0, P_1);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(P_0, P_1);
                    GUI.enabled = true;
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(P_0, P_1);
                GUI.enabled = true;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty P_0, GUIContent P_1)
        {
            return 1f * EditorGUIUtility.singleLineHeight;
        }
    }
}