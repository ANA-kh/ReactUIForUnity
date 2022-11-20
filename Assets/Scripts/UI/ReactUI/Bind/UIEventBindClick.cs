using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReactUI
{
	[AddComponentMenu("ReactUI/UI/Bind/Event Bind Click")]
	public sealed class UIEventBindClick : UIEventBind, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		[ReactUI.EventName]
		private string eventName;

		private Selectable _selectable;

		private SignalDelegateList _delegateList;

		private SignalDelegateList GetDelegateList()
		{
			if (_delegateList == null)
			{
				_delegateList = Add(eventName);
			}
			return _delegateList;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
            if (_selectable == null || _selectable.interactable)
            {
                GetDelegateList()?.CallHandle(GetEventIDVar());
            }
		}

		protected override void RefreshBind()
		{
			_delegateList = Add(eventName);
		}

		private new void Awake()
		{
			base.Awake();
			_selectable = GetComponent<Selectable>();
		}
	}
}
