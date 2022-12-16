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
                SerializedObject serializedObject = this.serializedObject;
                _eventsProp = serializedObject.FindProperty("events");
                _list = new ReorderableList(serializedObject, _eventsProp);
                _list.drawHeaderCallback = delegate(Rect position)
                {
                    GUI.Label(position, "Events:");
                };
                _list.elementHeightCallback =
                    index =>
                        GetHeight(_eventsProp, index);
                _list.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
                {
                    DrawOneVariable(_eventsProp, rect, index, isActive, isFocused);
                };
                Init();
            }
        }

        private float GetHeight(SerializedProperty property, int index)
        {
            if (property.arraySize == 0)
            {
                return 0;
            }
            SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(index);
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

            return (1 + collection.Count) * EditorGUIUtility.singleLineHeight;
        }

        private void DrawOneVariable(SerializedProperty property, Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(index);
            bool flag = _setIdx.Contains(index);
            Color color = GUI.color;
            if (flag)
            {
                GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
            }

            Rect rect1 = new Rect(rect.x + 8f, rect.y, 16f, EditorGUIUtility.singleLineHeight);
            Rect rect2 = new Rect(rect.x + 12f, rect.y, rect.width - 12f, EditorGUIUtility.singleLineHeight);
            arrayElementAtIndex.isExpanded = (EditorGUI.Foldout(rect1, arrayElementAtIndex.isExpanded, GUIContent.none));
            EditorGUI.PropertyField(rect2, arrayElementAtIndex, GUIContent.none);
            if (arrayElementAtIndex.isExpanded)
            {
                UIEventTable val = (UIEventTable)target;
                ICollection<Component> collection = val.FindReferenced(arrayElementAtIndex.stringValue);
                if (collection != null)
                {
                    GUI.enabled = false;
                    Rect rect3 = new Rect(rect.x + 12f, rect.y, rect.width - 12f, EditorGUIUtility.singleLineHeight);
                    foreach (Component item in collection)
                    {
                        rect3.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.ObjectField(rect3, item, item.GetType(), true);
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
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent content)
        {
            EditorGUI.BeginProperty(position, content, property);
            UIEventBind val = (UIEventBind)property.serializedObject.targetObject;
            if (val != null)
            {
                UIEventTable eventTable = val.EventTable;
                if (eventTable != null)
                {
                    Rect rect = new Rect(position.x, position.y, position.width * 0.39f, position.height);
                    Rect rect2 = new Rect(position.x + position.width * 0.39f, position.y, position.width * 0.61f, position.height);
                    EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), content);
                    string[] events = eventTable.Events;
                    if (events != null)
                    {
                        string[] array = new string[events.Length + 1];
                        Array.Copy(events, array, events.Length);
                        array[events.Length] = "None";
                        int num = Array.FindIndex(events, (string name) => name == property.stringValue);
                        int num2 = EditorGUI.Popup(rect2, num, array);
                        if (num2 != num && num2 >= 0)
                        {
                            if (num2 < events.Length)
                            {
                                property.stringValue = (events[num2]);
                            }
                            else
                            {
                                property.stringValue = (string.Empty);
                            }
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUI.PropertyField(position, property);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property);
                    GUI.enabled = true;
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property);
                GUI.enabled = true;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent content)
        {
            return 1f * EditorGUIUtility.singleLineHeight;
        }
    }
}