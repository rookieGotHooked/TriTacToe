using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
	GameManager _gameManager;
	PlayerActionState _playerActionState;

	[SerializeField] TilePositionIndex _tileIndex;

	[Header("Display")]
	[SerializeField] Sprite _xSprite;
	[SerializeField] Sprite _oSprite;

	[Header("SFX")]
	[SerializeField] AudioSource _scoreSFX;
	[SerializeField] AudioSource _clearSFX;
	[SerializeField] AudioSource _xMarkSFX;
	[SerializeField] AudioSource _oMarkSFX;

	Tweens2D _highlightTween;
	Tweens2D _symbolTween;
	Image _symbolImage;
	Button _buttonComponent;

	private void Awake()
	{
		_gameManager = GameManager.Instance;
		_playerActionState = _gameManager.PlayerActionState;

		GameObject highlightObject = transform.GetChild(0).gameObject;
		GameObject symbolObject = transform.GetChild(1).gameObject;

		if (!highlightObject.TryGetComponent(out _highlightTween))
		{
			throw new System.Exception($"{highlightObject.name} does not contains Tweens2D component");
		}
		if (!symbolObject.TryGetComponent(out _symbolImage))
		{
			throw new System.Exception($"{symbolObject.name} does not contains Image component");
		}
		if (!symbolObject.TryGetComponent(out _symbolTween))
		{
			throw new System.Exception($"{symbolObject.name} does not contains Tweens2D component");
		}
		if (!gameObject.TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		if (_scoreSFX == null) 
		{
			throw new System.Exception("Missing component: AudioSource; variable: _scoreSFX");
		}
		if (_clearSFX == null)
		{
			throw new System.Exception("Missing component: AudioSource; variable: _clearSFX");
		}
		if (_xMarkSFX == null)
		{
			throw new System.Exception("Missing component: AudioSource; variable: _xMarkSFX");
		}
		if (_oMarkSFX == null)
		{
			throw new System.Exception("Missing component: AudioSource; variable: _oMarkSFX");
		}
		_buttonComponent.onClick.AddListener(ClickHandler);
	}

	async private void ClickHandler()
	{
		await _playerActionState.TileClick(_tileIndex);
	}

	async public Task MarkSymbol(Symbol symbol)
	{
		//Debug.Log($"MarkSymbol: {symbol}");

		switch (symbol)
		{
			case Symbol.X:
				_symbolImage.sprite = _xSprite;
				_xMarkSFX.Play();
				break;
			case Symbol.O:
				_symbolImage.sprite = _oSprite;
				_oMarkSFX.Play();
				break;
			default:
				break;
		}

		await _symbolTween.ExecuteTweenOrders("Appear");
	}

	async public Task HighlightAndClear()
	{
		await _highlightTween.ExecuteTweenOrders("Appear");

		await Task.Delay(250);

		List<Task> tweenTasks = new()
		{
			_highlightTween.ExecuteTweenOrders("Disappear"),
			_symbolTween.ExecuteTweenOrders("Disappear")
		};
		_scoreSFX.Play();

		foreach (var task in tweenTasks) 
		{
			await task;
		}
	}

	async public Task SymbolClear()
	{
		_clearSFX.Play();
		await _symbolTween.ExecuteTweenOrders("Disappear");

		//_buttonComponent.interactable = true;
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
	}
}
