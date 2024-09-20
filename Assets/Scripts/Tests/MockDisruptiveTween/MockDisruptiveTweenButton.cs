using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MockDisruptiveTweenButton : MonoBehaviour
{
	[SerializeField] MockDirection _direction;
    [SerializeField] DisruptibleTweens _tweenComp;
    Button _buttonComp;

	private void Awake()
	{
		if (!TryGetComponent(out _buttonComp))
		{
			throw new System.Exception($"_buttonComp is null - {gameObject.name} needs Button component.");
		}

		_buttonComp.onClick.AddListener(OnClick);
	}

	async private void OnClick()
	{
		if (_tweenComp.IsTweening)
		{
			//Debug.Log("Skip Tween");

			_tweenComp.SkipTween();
		}
		else
		{
			//Debug.Log("ExecuteRepeatableOrder");

			if (_direction == MockDirection.Down)
			{
				await _tweenComp.ExecuteRepeatableOrder("Move Down");
			}
			else
			{
				await _tweenComp.ExecuteRepeatableOrder("Move Right");
			}
		}
	}
}

public enum MockDirection
{
    Down, Right
}
