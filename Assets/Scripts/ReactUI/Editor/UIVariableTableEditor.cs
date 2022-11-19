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

/// <summary>
/// VariableTable外观类
/// </summary>
[CustomEditor(typeof(UIVariableTable))]
internal sealed class UIVariableTableEditor : Editor
{
	private SerializedProperty variables;

	private ReorderableList list;

	private Dictionary<int, SerializedProperty> propMap = new Dictionary<int, SerializedProperty>();

	private HashSet<int> nameSet = new HashSet<int>();

	private Dictionary<string, int> name2idx = new Dictionary<string, int>(StringComparer.Ordinal);

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		list.DoLayoutList();
		if (serializedObject.ApplyModifiedProperties())
		{
			propMap.Clear();
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
		if(GUILayout.Button("Generate C# code to clipboard"))
		{
			var sb = new StringBuilder();
			foreach(UIVariable v in val.Variables)
			{
				sb.AppendLine("[AutoBindVariable]");
				sb.Append("UIVariable var_").Append(v.Name).AppendLine(";");
			}
			GUIUtility.systemCopyBuffer = sb.ToString();
		}
		if(GUILayout.Button("Generate C# code to clipboard(with GameObject Bind)"))
		{
			var sb = new StringBuilder();
			foreach(UIVariable v in val.Variables)
			{
				sb.AppendLine("[AutoBindVariable]");
				sb.Append("UIVariable var_").Append(v.Name).AppendLine(";");
			}
			UIItemVariable[] bindGOList = val.GetComponentsInChildren<UIItemVariable>();
			foreach(UIItemVariable bindGO in bindGOList)
			{
				sb.AppendLine("[AutoBindGameObject]");
				string typeName = bindGO.GetEUIItemExportTypeCorrespondOriginTypeName(bindGO.ExportType,bindGO);
				sb.Append(typeName).Append(" go_").Append(bindGO.GetExportedName()).AppendLine(";");
			}
			GUIUtility.systemCopyBuffer = sb.ToString();
		}

		#endregion
	}

	private void OnEnable()
	{
		if (!(target == null))
		{
			variables = serializedObject.FindProperty("variables");
			list = (ReorderableList)(object)new ReorderableList(serializedObject, variables);
			list.drawHeaderCallback = delegate(Rect P_0)
			{
				GUI.Label(P_0, "Variables:");
			};
			list.elementHeightCallback = (ElementHeightCallbackDelegate)(object)(ElementHeightCallbackDelegate)((int P_0) => GetHeight(variables, P_0));
			list.drawElementCallback = delegate(Rect P_0, int P_1, bool P_2, bool P_3)
			{
				DrawOneVariable(variables, P_0, P_1, P_2, P_3);
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

	private float GetHeight(SerializedProperty P_0, int P_1)
	{
		if (!propMap.TryGetValue(P_1, out SerializedProperty value))
		{
			if (P_0.arraySize == 0)
			{
				return 0;
			}
			value = P_0.GetArrayElementAtIndex(P_1);
			propMap.Add(P_1, value);
		}
		if (!value.isExpanded)
		{
			return 2f * EditorGUIUtility.singleLineHeight;
		}
		UIVariableTable val = (UIVariableTable)(object)(UIVariableTable)target;
		UIVariable variable = val.GetVariable(P_1);
		ICollection<UIVariableBind> binds = variable.Binds;
		return (float)(2 + binds.Count) * EditorGUIUtility.singleLineHeight;
	}

	private void DrawOneVariable(SerializedProperty P_0, Rect P_1, int P_2, bool P_3, bool P_4)
	{
		if (!propMap.TryGetValue(P_2, out SerializedProperty value))
		{
			value = P_0.GetArrayElementAtIndex(P_2);
			propMap.Add(P_2, value);
		}
		bool flag = nameSet.Contains(P_2);
		Color color = GUI.color;
		if (flag)
		{
			GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
		}
		SerializedProperty val = value.FindPropertyRelative("name");
		SerializedProperty val2 = value.FindPropertyRelative("type");
		Rect rect = new Rect(P_1.x + 8f, P_1.y, 16f, EditorGUIUtility.singleLineHeight);
		Rect rect2 = new Rect(P_1.x + 12f, P_1.y, (P_1.width - 12f) / 2f - 5f, EditorGUIUtility.singleLineHeight);
		Rect rect3 = new Rect(P_1.x + P_1.width / 2f + 5f, P_1.y, (P_1.width - 12f) / 2f - 5f, EditorGUIUtility.singleLineHeight);
		value.isExpanded=(EditorGUI.Foldout(rect, value.isExpanded, GUIContent.none));
		EditorGUI.PropertyField(rect2, val, GUIContent.none);
		EditorGUI.PropertyField(rect3, val2, GUIContent.none);
		Rect rect4 = new Rect(P_1.x + 12f, P_1.y + EditorGUIUtility.singleLineHeight, P_1.width - 12f, EditorGUIUtility.singleLineHeight);
		SerializedProperty val3 = null;
		UIVariableType val4 = (UIVariableType)val2.enumValueIndex;
		switch ((int)val4)
		{
		case 0:
			val3 = value.FindPropertyRelative("booleanValue");
			val3.boolValue=(EditorGUI.ToggleLeft(rect4, GUIContent.none, val3.boolValue));
			break;
		case 1:
			val3 = value.FindPropertyRelative("integerValue");
			val3.intValue=(EditorGUI.IntField(rect4, GUIContent.none, val3.intValue));
			break;
		case 2:
			val3 = value.FindPropertyRelative("floatValue");
			val3.floatValue=(EditorGUI.FloatField(rect4, GUIContent.none, val3.floatValue));
			break;
		case 3:
			val3 = value.FindPropertyRelative("stringValue");
			val3.stringValue=(EditorGUI.TextField(rect4, GUIContent.none, val3.stringValue));
			break;
		}
		if (value.isExpanded)
		{
			UIVariableTable val5 = (UIVariableTable)(object)(UIVariableTable)target;
			UIVariable variable = val5.GetVariable(P_2);
			ICollection<UIVariableBind> binds = variable.Binds;
			if (binds.Count > 0)
			{
				GUI.enabled = false;
				Rect rect5 = new Rect(P_1.x + 12f, P_1.y + EditorGUIUtility.singleLineHeight, P_1.width - 12f, EditorGUIUtility.singleLineHeight);
				foreach (UIVariableBind item in binds)
				{
					rect5.y += EditorGUIUtility.singleLineHeight;
					if ((UnityEngine.Object)(object)item != null)
					{
						EditorGUI.ObjectField(rect5, (UnityEngine.Object)(object)item, ((object)item).GetType(), true);
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
		nameSet.Clear();
		name2idx.Clear();
		for (int i = 0; i < variables.arraySize; i++)
		{
			SerializedProperty arrayElementAtIndex = variables.GetArrayElementAtIndex(i);
			SerializedProperty val = arrayElementAtIndex.FindPropertyRelative("name");
			if (name2idx.ContainsKey(val.stringValue))
			{
				nameSet.Add(name2idx[val.stringValue]);
				nameSet.Add(i);
			}
			else
			{
				name2idx.Add(val.stringValue, i);
			}
		}
	}
}