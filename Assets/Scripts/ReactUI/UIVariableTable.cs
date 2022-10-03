using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ReactUI
{
	/// <summary>
	/// 序列化保存UIVariable，供UIVariableBind查找和使用；并展示在inspector界面
	/// </summary>
	[AddComponentMenu("ReactUI/UI/Bind/UI Variable Table")]
	public sealed class UIVariableTable : MonoBehaviour
	{
		[SerializeField]
		private UIVariable[] variables;

		private Dictionary<string, UIVariable> varMap;

		private UIMapVariableTable mapVarTable;
		public UIMapVariableTable MapVarTable
		{
			get
			{
				if (mapVarTable == null)
					mapVarTable = GetComponent<UIMapVariableTable>();
				return mapVarTable;
			}
		}


		public UIVariable[] Variables => variables;

		private Dictionary<string, UIVariable> GetVariableMap()
		{
			if (varMap == null)
			{
				varMap = new Dictionary<string, UIVariable>(StringComparer.Ordinal);
				if (variables != null)
				{
					UIVariable[] array = variables;
					foreach (UIVariable uIVariable in array)
					{
						varMap.Add(uIVariable.Name, uIVariable);
					}
				}
			}

			if (MapVarTable != null)
			{
				foreach (var mVar in MapVarTable.Variables)
				{
					if (mVar.IsValid && !varMap.ContainsKey(mVar.OutVariable.Name))
					{
						varMap.Add(mVar.OutVariable.Name, mVar.OutVariable);
					}
				}
			}
			return varMap;
		}

		public UIVariable FindVariable(string name)
		{
            if (string.IsNullOrEmpty(name))
            {
				return null;
            }
			if (GetVariableMap().TryGetValue(name, out UIVariable value))
			{
				return value;
			}
			return null;
		}

		public void AddDefaultVariable()
		{
			UIVariable uIVariable = new UIVariable();
			if (variables == null)
			{
				variables = new UIVariable[1];
				variables[0] = uIVariable;
			}
			else
			{
				UIVariable[] var_new = new UIVariable[variables.Length+1];
				for(int i=0;i< variables.Length;i++)
                {
					var_new[i] = variables[i];
				}
				var_new[var_new.Length - 1] = uIVariable;
				variables = var_new;
			}
		}

		public string[] GetVariableNames()
		{
			var keys = GetVariableMap().Keys;
			var array = new string[keys.Count];
			var idx = 0;
			foreach (var key in keys)
			{
				array[idx] = key;
				idx++;
			}
			return array;
		}

		public string[] GetOriginalVariableNames(List<UIVariableType> excludeTypes = null)
		{
			var checkExclude = excludeTypes != null && excludeTypes.Count > 0;
			var names = new List<string>();
			foreach (var var in variables)
			{
				if (checkExclude)
				{
					if (!excludeTypes.Contains(var.Type))
					{
						names.Add(var.Name);
					}
					else
					{
						names.Add($"{var.Name} (X)");
					}
				}
				else
				{
					names.Add(var.Name);
				}
			}

			return names.ToArray();
		}

		public void Sort()
		{
			Array.Sort(variables, (UIVariable P_0, UIVariable P_1) => P_0.Name.CompareTo(P_1.Name));
		}

		public void InitializeBinds()
		{
			InitVariableMap(base.transform);
		}

		public UIVariable GetVariable(int index)
		{
			return variables[index];
		}

		private static void InitVariableMap(Transform trans)
		{
			UIVariableBind[] components = trans.GetComponents<UIVariableBind>();
			UIVariableBind[] array = components;
			foreach (UIVariableBind uIVariableBind in array)
			{
				uIVariableBind.Init();
			}
			foreach (Transform item in trans)
			{
				DeepInitVariableBind(item);
			}
		}

		private static void DeepInitVariableBind(Transform trans)
		{
			if (trans.GetComponent<UIVariableTable>() == null)
			{
				UIVariableBind[] components = trans.GetComponents<UIVariableBind>();
				UIVariableBind[] array = components;
				foreach (UIVariableBind uIVariableBind in array)
				{
					uIVariableBind.Init();
				}
				foreach (Transform item in trans)
				{
					DeepInitVariableBind(item);
				}
			}
		}

		private void OnValidate()
		{
			varMap = null;
			if (variables != null)
			{
				UIVariable[] array = variables;
				foreach (UIVariable uIVariable in array)
				{
					//uIVariable.ResetValue();
					//uIVariable.ClearBinderList();
					uIVariable.InvokeValueChange();
				}
			}
		}

		public void ResetVarMap()
		{
			varMap = null;
		}

		private void Awake()
		{
			if (MapVarTable != null)
				MapVarTable.Init();
			InitVariableMap(base.transform);
			varMap = null;
		}

	}
}
