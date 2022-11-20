using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ReactUI
{
	[ExecuteInEditMode]
	public abstract class UIEventBind : UIVariableBind
	{
		[Tooltip("The event table for this bind.")]
		[SerializeField]
		private UIEventTable eventTable;

		//事件的标记ID，用于区分是哪个按钮，在事件回调中传递
		[SerializeField]
		[VariableName]
		private string paramEventID;
		private UIVariable _eventIDVariable;

		private Dictionary<string, LinkedListNode<Component>> _name2Node = new Dictionary<string, LinkedListNode<Component>>(StringComparer.Ordinal);


		public UIEventTable EventTable
		{
			get;
			private set;
		}

		internal SignalDelegateList Add(string name)
		{
			if (EventTable != null)
			{
				SignalDelegateList SignalDelegateList = EventTable.GetDelegateByName(name);
				if (SignalDelegateList != null)
				{
					if (_name2Node.TryGetValue(name, out LinkedListNode<Component> value))
					{
						EventTable.Remove(name, value);
						_name2Node.Remove(name);
					}
					value = EventTable.Add(name, this);
					_name2Node.Add(name, value);
				}
				return SignalDelegateList;
			}
			return null;
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			Clear();
			Init();
			RefreshBind();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Clear();
		}

		protected abstract void RefreshBind();

		protected override void Awake()
		{
			base.Awake();
			Init();
		}

		internal override void Init()
		{
			base.Init();
			if (eventTable == null)
			{
				eventTable = this.GetComponentInParent<UIEventTable>();
			}
			EventTable = eventTable;
		}

		private void Clear()
		{
			if (EventTable != null)
			{
				foreach (KeyValuePair<string, LinkedListNode<Component>> item in _name2Node)
				{
					EventTable.Remove(item.Key, item.Value);
				}
			}
			_name2Node.Clear();
		}
		public UIVariable GetEventIDVar()
        {
			if(_eventIDVariable != null)
            {
				return _eventIDVariable;
            }
            if (string.IsNullOrEmpty(paramEventID))
            {
				return null;
            }
			_eventIDVariable = FindVariable(paramEventID);
			return _eventIDVariable;
		}
	}
}
