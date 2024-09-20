using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SymbolSelectController : MonoBehaviour
{
	Button _backgroundButtonComponent;
	Button _highlightedButtonComponent;

	RectTransform _rectTransformComponent;

	Vector2 _originalPosition;

	Image _backgroundImageComponent;
	Image _highlightedButtonImageComponent;
	Image _symbolXImageComponent;
	Image _symbolOImageComponent;

	Tweens2D _highlightButtonTweenComponent;
	Tweens2D _maskedSymbolXHighlightTweenComponent;
	Tweens2D _maskedSymbolOHighlightTweenComponent;

	Symbol _selectedSymbol = Symbol.X;
	public Symbol SelectedSymbol => _selectedSymbol;

	private void Awake()
	{
		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!transform.GetChild(0).gameObject.TryGetComponent(out _backgroundButtonComponent))
		{
			throw new System.Exception($"{transform.GetChild(0).gameObject.name} does not contains Button component");
		}

		_backgroundButtonComponent.onClick.AddListener(OnSelectionClick);

		if (!transform.GetChild(0).gameObject.TryGetComponent(out _backgroundImageComponent))
		{
			throw new System.Exception($"{transform.GetChild(0).gameObject.name} does not contains Image component");
		}


		if (!transform.GetChild(1).gameObject.TryGetComponent(out _highlightButtonTweenComponent))
		{
			throw new System.Exception($"{transform.GetChild(1).gameObject.name} does not contains Tweens2D component");
		}

		if (!transform.GetChild(1).gameObject.TryGetComponent(out _highlightedButtonComponent))
		{
			throw new System.Exception($"{transform.GetChild(1).gameObject.name} does not contains Button component");
		}

		if (!transform.GetChild(1).gameObject.TryGetComponent(out _highlightedButtonImageComponent))
		{
			throw new System.Exception($"{transform.GetChild(1).gameObject.name} does not contains Image component");
		}

		_highlightedButtonComponent.onClick.AddListener(OnSelectionClick);


		GameObject symbolMask = transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

		if (!symbolMask.transform.GetChild(0).gameObject.TryGetComponent(out _maskedSymbolXHighlightTweenComponent))
		{
			throw new System.Exception($"{symbolMask.transform.GetChild(0).gameObject.name} does not contains Tweens2D component");
		}

		if (!symbolMask.transform.GetChild(0).gameObject.TryGetComponent(out _symbolXImageComponent))
		{
			throw new System.Exception($"{symbolMask.transform.GetChild(0).gameObject.name} does not contains Image component");
		}


		if (!symbolMask.transform.GetChild(1).gameObject.TryGetComponent(out _maskedSymbolOHighlightTweenComponent))
		{
			throw new System.Exception($"{symbolMask.transform.GetChild(1).gameObject.name} does not contains Tweens2D component");
		}

		if (!symbolMask.transform.GetChild(1).gameObject.TryGetComponent(out _symbolOImageComponent))
		{
			throw new System.Exception($"{symbolMask.transform.GetChild(0).gameObject.name} does not contains Image component");
		}
	}

	async public void OnSelectionClick()
	{
		_backgroundButtonComponent.interactable = false;
		_highlightedButtonComponent.interactable = false;

		string highlightOrder;
		string symbolOrder;

		if (_selectedSymbol == Symbol.X)
		{
			highlightOrder = "Move Left To Right";
			symbolOrder = "Move Right To Left";
			_selectedSymbol = Symbol.O;
		}
		else if (_selectedSymbol == Symbol.O)
		{
			highlightOrder = "Move Right To Left";
			symbolOrder = "Move Left To Right";
			_selectedSymbol = Symbol.X;
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {_selectedSymbol}");
		}

		List<Task> tweenTasks = new()
		{
			_highlightButtonTweenComponent.ExecuteTweenOrders(highlightOrder),
			_maskedSymbolXHighlightTweenComponent.ExecuteTweenOrders(symbolOrder),
			_maskedSymbolOHighlightTweenComponent.ExecuteTweenOrders(symbolOrder)
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}

		Debug.Log($"_selectedSymbol: {_selectedSymbol}");

		_backgroundButtonComponent.interactable = true;
		_highlightedButtonComponent.interactable = true;
	}

	public void SetTransparencyAll(float value)
	{
		_backgroundImageComponent.color = new(
			_backgroundImageComponent.color.r,
			_backgroundImageComponent.color.g,
			_backgroundImageComponent.color.b,
			value);

		_highlightedButtonImageComponent.color = new(
			_highlightedButtonImageComponent.color.r,
			_highlightedButtonImageComponent.color.g,
			_highlightedButtonImageComponent.color.b,
			value);

		_symbolXImageComponent.color = new(
			_symbolXImageComponent.color.r,
			_symbolXImageComponent.color.g,
			_symbolXImageComponent.color.b,
			value);

		_symbolOImageComponent.color = new(
			_symbolOImageComponent.color.r,
			_symbolOImageComponent.color.g,
			_symbolOImageComponent.color.b,
			value);
	}

	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}
