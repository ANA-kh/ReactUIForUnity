using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReactUI
{
	public delegate void SignalDelegate(params object[] args);
	public sealed class SignalHandle
	{
		private LinkedList<SignalDelegate> _ownerList;

		private LinkedListNode<SignalDelegate> _signalNode;

		internal SignalHandle(LinkedList<SignalDelegate> list, LinkedListNode<SignalDelegate> node)
		{
			_ownerList = list;
			_signalNode = node;
		}

		public void Dispose()
		{
			if (_ownerList != null && _signalNode != null)
			{
				_ownerList.Remove(_signalNode);
				_signalNode = null;
				_ownerList = null;
			}
		}
	}
	public class SignalDelegateList
	{
		private LinkedList<SignalDelegate> _delegateList;

		public void Clear()
		{
			if (_delegateList != null)
			{
				_delegateList.Clear();
			}
		}

		public SignalHandle GetHandle(SignalDelegate s)
		{
			if (_delegateList == null)
			{
				_delegateList = new LinkedList<SignalDelegate>();
			}
			LinkedListNode<SignalDelegate> linkedListNode = _delegateList.AddLast(s);
			return new SignalHandle(_delegateList, linkedListNode);
		}

		public void CallHandle(params object[] args)
		{
			if (_delegateList != null)
			{
				LinkedListNode<SignalDelegate> linkedListNode = _delegateList.First;
				while (linkedListNode != null)
				{
					LinkedListNode<SignalDelegate> next = linkedListNode.Next;
					SignalDelegate value = linkedListNode.Value;
					value(args);
					linkedListNode = next;
				}
			}
		}
	}

	[AddComponentMenu("ReactUI/UI/Bind/UI Event Table")]
	public sealed class UIEventTable : MonoBehaviour
	{
		public delegate void EventDelegate(params object[] args);


		[SerializeField]
		[Tooltip("The event list.")]
		private string[] events;

		//UIEventBind通过eventName索引到这里，得到一个SignalDelegateList-对应到controller里绑定到该eventName的函数
		//多->1->多
		private Dictionary<string, SignalDelegateList> _delegateMap;
		
		//每一个eventName对应绑定的Component，用于绘制
		private Dictionary<string, LinkedList<Component>> _componentMap = new Dictionary<string, LinkedList<Component>>(StringComparer.Ordinal);

		public string[] Events => events;

		private Dictionary<string, SignalDelegateList> DelegateMap
		{
			get
			{
				if (_delegateMap == null)
				{
					_delegateMap = new Dictionary<string, SignalDelegateList>(StringComparer.Ordinal);
					string[] array = events;
					foreach (string key in array)
					{
						if (!_delegateMap.ContainsKey(key))
						{
							_delegateMap.Add(key, new SignalDelegateList());
						}
					}
				}

				return _delegateMap;
			}
		}

		public void Sort()
		{
			Array.Sort(events);
		}

		public ICollection<Component> FindReferenced(string eventName)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			if (!_componentMap.TryGetValue(eventName, out LinkedList<Component> value))
			{
				return null;
			}
			return value;
		}

		public SignalHandle ListenEvent(string eventName, SignalDelegate callback)
		{
			if (!DelegateMap.TryGetValue(eventName, out SignalDelegateList value))
			{
				//Debug.LogWarning("{0} is trying to listen event {1}, but it does not existed.", base.name, eventName);
				return new SignalHandle(null, null);
			}
			return value.GetHandle(callback);
		}

		public void ClearEvent(string eventName)
		{
			if (!DelegateMap.TryGetValue(eventName, out SignalDelegateList value))
			{
				//Debug.LogWarning("{0} is trying to clear event {1}, but it does not existed.", base.name, eventName);
			}
			else
			{
				value?.Clear();
			}
		}

		public void ClearAllEvents()
		{
			foreach (KeyValuePair<string, SignalDelegateList> item in DelegateMap)
			{
				item.Value.Clear();
			}
		}

		internal LinkedListNode<Component> Add(string name, Component P_1)
		{
			if (!_componentMap.TryGetValue(name, out LinkedList<Component> value))
			{
				value = new LinkedList<Component>();
				_componentMap.Add(name, value);
			}
			return value.AddLast(P_1);
		}

		internal void Remove(string name, LinkedListNode<Component> P_1)
		{
			if (_componentMap.TryGetValue(name, out LinkedList<Component> value))
			{
				value.Remove(P_1);
			}
		}

		public SignalDelegateList GetDelegateByName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (!DelegateMap.TryGetValue(name, out SignalDelegateList value))
			{
				return null;
			}
			return value;
		}

		private void OnValidate()
		{
			_delegateMap = null;
		}
	}
}
