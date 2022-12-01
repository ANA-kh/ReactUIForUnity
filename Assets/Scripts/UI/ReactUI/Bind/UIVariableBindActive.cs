using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReactUI
{
	[AddComponentMenu("ReactUI/UI/Bind/Variable Bind Active")]
	public sealed class UIVariableBindActive : UIVariableBindBool
	{
		public enum TransitionModeEnum
		{
			Instant,
			Fade
		}

		[SerializeField]
		private TransitionModeEnum transitionMode;

		[SerializeField]
		private float transitionTime = 0.1f; 
		
		protected override void OnValueChanged()
		{
			bool result = GetResult();
			if (transitionMode == TransitionModeEnum.Instant)
			{
				gameObject.SetActive(result);
				return;
			}

			// graph
			var graphic = gameObject.GetComponent<Graphic>();
			if (graphic != null)
			{
				if (gameObject.activeInHierarchy)
				{
					graphic.DOKill();
					graphic.DOFade(result ? 1 : 0, transitionTime);
				}
				else
				{
					gameObject.SetActive(result);
				}
				return;
			}
			
			// tmp
			var tmp = gameObject.GetComponent<TextMeshProUGUI>();
			if (tmp != null)
			{
				if (gameObject.activeInHierarchy)
				{
					tmp.DOKill();
					tmp.DOFade(result ? 1 : 0, transitionTime);
				}
				else
				{
					gameObject.SetActive(result);
				}
				return;
			}
			
			// canvas
			var fadeCanvas = gameObject.GetComponent<CanvasGroup>();
			if (fadeCanvas != null)
			{
				gameObject.SetActive(value: true);
				if (gameObject.activeInHierarchy)
				{
					fadeCanvas.DOKill();
					fadeCanvas.DOFade(result ? 1 : 0, transitionTime);
				}
				else
				{
					gameObject.SetActive(result);
				}
			}
			else
			{
				gameObject.SetActive(result);
			}
		}
	}
}
