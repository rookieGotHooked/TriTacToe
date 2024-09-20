using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NextButton : MonoBehaviour
{
	private RectTransform _rectTransform;
    private Tweens2D _tweens2D;
    private Button _buttonComponent;

	private void Awake()
	{
		if (!TryGetComponent(out _rectTransform))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		if (!TryGetComponent(out _tweens2D))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}

		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		_buttonComponent.onClick.AddListener(FSM_GuideManager.Instance.ChangeNextPage);
	}

	async public Task MoveOut()
	{
		await _tweens2D.ExecuteTweenOrders("Move Out");

		_rectTransform.anchoredPosition = new Vector2(-1080f, 208f);
	}

	async public Task MoveIn()
	{
		await _tweens2D.ExecuteTweenOrders("Move In");
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
	}

	//public void RequestChangePage()
	//{

	//}
}
