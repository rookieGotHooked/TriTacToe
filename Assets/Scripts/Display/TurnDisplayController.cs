using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplayController : MonoBehaviour
{
    [SerializeField] Tweens2D _xSymbolTween;
	[SerializeField] Tweens2D _oSymbolTween;

	Symbol _currentSymbol = Symbol.X;

	Image _imageComponent;
	Image _turnTextImageComponent;
	Image _xSymbolImageComponent;
	Image _oSymbolImageComponent;

	RectTransform _rectTransformComponent;
	Tweens2D _tweenComponent;

	Vector2 _originalPosition;

	private List<Task> _tweenTasks = new();

	private void Awake()
	{
		if (!TryGetComponent(out _tweenComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}

		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}

		if (!gameObject.transform.GetChild(0).gameObject.TryGetComponent(out _turnTextImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(0).gameObject.name} does not contains Image component");
		}

		if (!gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.TryGetComponent(out _xSymbolImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.name} " +
				$"does not contains Image component");
		}

		if (!gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.TryGetComponent(out _oSymbolImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.name} " +
				$"does not contains Image component");
		}

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}
		else
		{
			_originalPosition = _rectTransformComponent.anchoredPosition;
		}
	}

	async public Task ChangeSymbol(Symbol newSymbol)
	{
		if (newSymbol == Symbol.O && _currentSymbol == Symbol.X)
		{
			_tweenTasks.Add(_xSymbolTween.ExecuteTweenOrders("Move Down"));
			_tweenTasks.Add(_oSymbolTween.ExecuteTweenOrders("Move Down"));

			_currentSymbol = Symbol.O;
		}
		else if (newSymbol == Symbol.X && _currentSymbol == Symbol.O)
		{
			_tweenTasks.Add(_xSymbolTween.ExecuteTweenOrders("Move Up"));
			_tweenTasks.Add(_oSymbolTween.ExecuteTweenOrders("Move Up"));

			_currentSymbol = Symbol.X;
		}
		else if (newSymbol != Symbol.X && newSymbol != Symbol.O)
		{
			throw new System.Exception($"Unexpected symbol detected: {newSymbol}");
		}

		foreach (var task in _tweenTasks)
		{
			await task;
		}

		_tweenTasks.Clear();

		//Debug.Log($"TurnDisplayController: {_currentSymbol}");
		//Debug.Log($"GameManager: {GameManager.Instance.CurrentSymbol}");
	}

	async public Task MoveOut()
	{
		await _tweenComponent.ExecuteTweenOrders("Move Out");
	}

	async public Task MoveIn()
	{
		await _tweenComponent.ExecuteTweenOrders("Move In");
	}

	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}

	public void SetTransparencyAll(float value)
	{
		_imageComponent.color = new Color(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			value);

		_turnTextImageComponent.color = new Color(
			_turnTextImageComponent.color.r,
			_turnTextImageComponent.color.g,
			_turnTextImageComponent.color.b,
			value);

		_xSymbolImageComponent.color = new Color(
			_xSymbolImageComponent.color.r,
			_xSymbolImageComponent.color.g,
			_xSymbolImageComponent.color.b,
			value);

		_oSymbolImageComponent.color = new Color(
			_oSymbolImageComponent.color.r,
			_oSymbolImageComponent.color.g,
			_oSymbolImageComponent.color.b,
			value);
	}

	async public Task ResetSymbol()
	{
		if (_currentSymbol == Symbol.O)
		{
			_tweenTasks.Add(_xSymbolTween.ExecuteTweenOrders("Move Up"));
			_tweenTasks.Add(_oSymbolTween.ExecuteTweenOrders("Move Up"));

			_currentSymbol = Symbol.X;

			foreach (var task in _tweenTasks)
			{
				await task;
			}
		}
		else if (_currentSymbol == Symbol.X)
		{
			return;
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {_currentSymbol}");
		}
	}
}
