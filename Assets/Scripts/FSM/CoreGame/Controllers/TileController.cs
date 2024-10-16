using System;
using System.Collections.Generic;
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

	async public Task MarkSymbol(Symbol symbol, bool isWithSFX)
	{
		switch (symbol)
		{
			case Symbol.X:
				_symbolImage.sprite = _xSprite;

				if (isWithSFX)
				{
					_xMarkSFX.Play();
				}
				break;
			case Symbol.O:
				_symbolImage.sprite = _oSprite;

				if (isWithSFX)
				{
					_oMarkSFX.Play();
				}
				break;
			default:
				throw new Exception($"Unexpected symbol: {symbol}, in {gameObject.name}");
		}

		await _symbolTween.ExecuteTweenOrders("Appear");
	}

	async public Task HighlightAndClear()
	{
		await _highlightTween.ExecuteTweenOrders("Appear");

		//await Task.Delay(250);
		await DelayHelper.Delay(0.25f);

		List<Task> tweenTasks = new()
		{
			_highlightTween.ExecuteTweenOrders("Disappear"),
			_symbolTween.ExecuteTweenOrders("Disappear")
		};

		foreach (var task in tweenTasks) 
		{
			await task;
		}
	}

	async public Task SymbolClear()
	{
		await _symbolTween.ExecuteTweenOrders("Disappear");
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
	}
}
