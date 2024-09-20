using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaymodeButton : MonoBehaviour
{
	[SerializeField] GameMode _gameMode;
    Button _buttonComponent;

	private void Awake()
	{
		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}
		_buttonComponent.onClick.AddListener(SetModeOnClick);
	}

	private void SetModeOnClick() 
	{
		ScreenManager.Instance.SetGameMode(_gameMode);

		//if (GameManager.Instance != null)
		//{
		//	await GameManager.Instance.ResetGame(false, false);
		//}
	}
}
