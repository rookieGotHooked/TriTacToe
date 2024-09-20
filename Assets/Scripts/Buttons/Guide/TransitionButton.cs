using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionButton : MonoBehaviour
{
	[SerializeField]
	private ScreensEnum _transitionTo;

	[SerializeField]
	private ScreenTransitionCode _transitionCode;

	private Button _buttonComponent;

	private void Awake()
	{
		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		_buttonComponent.onClick.AddListener(RequestChangeScreen);
	}

	private void RequestChangeScreen()
	{
		ScreenManager.Instance.SetNextScreen(_transitionTo, _transitionCode);
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
	}
}
