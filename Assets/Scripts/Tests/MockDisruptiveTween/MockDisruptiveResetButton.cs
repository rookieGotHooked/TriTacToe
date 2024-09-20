using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MockDisruptiveResetButton : MonoBehaviour
{
	[SerializeField] GameObject _testObject;
	RectTransform _rectTransformComp;

	DisruptibleTweens _disruptibleTweensComp;

	Vector2 _originalPos;
	Button _buttonComp;

	private void Awake()
	{
		if (!_testObject.TryGetComponent(out _rectTransformComp))
		{
			throw new System.Exception($"_rectTransformComp is null - {gameObject.name} needs RectTransform component.");
		}
		_originalPos = _rectTransformComp.anchoredPosition;

		if (!_testObject.TryGetComponent(out _disruptibleTweensComp))
		{
			throw new System.Exception($"_disruptibleTweensComp is null - {gameObject.name} needs DisruptibleTweens component.");
		}

		if (!TryGetComponent(out _buttonComp))
		{
			throw new System.Exception($"_buttonComp is null - {gameObject.name} needs Button component.");
		}

		_buttonComp.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		_rectTransformComp.anchoredPosition = _originalPos;

		//_disruptibleTweensComp.PopulateRepeatableTweenOrders();
	}
}
