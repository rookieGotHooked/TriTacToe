using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryBannerController : MonoBehaviour
{
	[Header("Game Objects")]
	[SerializeField] GameObject _yellowChevronLeftGameObject;
	[SerializeField] GameObject _blackChevronRightGameObject;
	[SerializeField] GameObject _victoryTextGameObject;
	[SerializeField] GameObject _symbolGameObject;
	[SerializeField] GameObject _blurLayer;

	[Header("Configs")]
	[SerializeField] float _delaySeconds;
	[SerializeField] Sprite _symbolXSprite;
	[SerializeField] Sprite _symbolOSprite;

	Image _yellowChevronLeftImageComponent;
	Image _blackChevronRightImageComponent;
	Image _victoryTextImageComponent;
	Image _symbolImageComponent;
	//Image _blurLayerImageComponent;

	RectTransform _yellowChevronLeftRectTransformComponent;
	RectTransform _blackChevronRightRectTransformComponent;
	RectTransform _victoryTextRectTransformComponent;
	RectTransform _symbolRectTransformComponent;

	Vector2 _yellowChevronLeftOriginalPosition;
	Vector2 _blackChevronRightOriginalPosition;
	Vector2 _victoryTextOriginalPosition;
	Vector2 _symbolOriginalPosition;

	Tweens2D _yellowChevronLeftTween;
	Tweens2D _blackChevronRightTween;
	Tweens2D _victoryTextTween;
	Tweens2D _symbolTween;
	Tweens2D _blurLayerTween;
	//Tweens2D _symbolOTween;

	private void Awake()
	{
		// Yellow Chevron Left
		if (!_yellowChevronLeftGameObject.TryGetComponent(out _yellowChevronLeftTween))
		{
			throw new System.Exception($"{_yellowChevronLeftGameObject.name} does not contains Tweens2D component");
		}

		if (!_yellowChevronLeftGameObject.TryGetComponent(out _yellowChevronLeftImageComponent))
		{
			throw new System.Exception($"{_yellowChevronLeftGameObject.name} does not contains Image component");
		}

		if (!_yellowChevronLeftGameObject.TryGetComponent(out _yellowChevronLeftRectTransformComponent))
		{
			throw new System.Exception($"{_yellowChevronLeftGameObject.name} does not contains RectTransform component");
		}

		_yellowChevronLeftOriginalPosition = _yellowChevronLeftRectTransformComponent.anchoredPosition;


		// Black Chervron Right
		if (!_blackChevronRightGameObject.TryGetComponent(out _blackChevronRightTween))
		{
			throw new System.Exception($"{_blackChevronRightGameObject.name} does not contains Tweens2D component");
		}

		if (!_blackChevronRightGameObject.TryGetComponent(out _blackChevronRightImageComponent))
		{
			throw new System.Exception($"{_yellowChevronLeftGameObject.name} does not contains Image component");
		}

		if (!_blackChevronRightGameObject.TryGetComponent(out _blackChevronRightRectTransformComponent))
		{
			throw new System.Exception($"{_blackChevronRightGameObject.name} does not contains RectTransform component");
		}

		_blackChevronRightOriginalPosition = _blackChevronRightRectTransformComponent.anchoredPosition;


		// Victory Text
		if (!_victoryTextGameObject.TryGetComponent(out _victoryTextTween))
		{
			throw new System.Exception($"{_victoryTextGameObject.name} does not contains Tweens2D component");
		}

		if (!_victoryTextGameObject.TryGetComponent(out _victoryTextImageComponent))
		{
			throw new System.Exception($"{_victoryTextGameObject.name} does not contains Image component");
		}

		if (!_victoryTextGameObject.TryGetComponent(out _victoryTextRectTransformComponent))
		{
			throw new System.Exception($"{_blackChevronRightGameObject.name} does not contains RectTransform component");
		}

		_victoryTextOriginalPosition = _victoryTextRectTransformComponent.anchoredPosition;


		// Symbol
		if (!_symbolGameObject.TryGetComponent(out _symbolTween))
		{
			throw new System.Exception($"{_symbolGameObject.name} does not contains Tweens2D component");
		}

		if (!_symbolGameObject.TryGetComponent(out _symbolImageComponent))
		{
			throw new System.Exception($"{_symbolGameObject.name} does not contains Image component");
		}

		if (!_symbolGameObject.TryGetComponent(out _symbolRectTransformComponent))
		{
			throw new System.Exception($"{_symbolGameObject.name} does not contains RectTransform component");
		}

		_symbolOriginalPosition = _symbolRectTransformComponent.anchoredPosition;


		// Blur Layer
		if (!_blurLayer.TryGetComponent(out _blurLayerTween))
		{
			throw new System.Exception($"{_blurLayer.name} does not contains Tweens2D component");
		}

		//if (!_blurLayer.TryGetComponent(out _blurLayerImageComponent))
		//{
		//	throw new System.Exception($"{_blurLayer.name} does not contains Image component");
		//}

		SetSpritesTransparent(0f);
		_blurLayer.SetActive(false);
	}

	private void SetSpritesTransparent(float value)
	{
		_yellowChevronLeftImageComponent.color = new Color(
			_yellowChevronLeftImageComponent.color.r,
			_yellowChevronLeftImageComponent.color.g,
			_yellowChevronLeftImageComponent.color.b,
			value);

		_blackChevronRightImageComponent.color = new Color(
			_blackChevronRightImageComponent.color.r,
			_blackChevronRightImageComponent.color.g,
			_blackChevronRightImageComponent.color.b,
			value);

		_victoryTextImageComponent.color = new Color(
			_victoryTextImageComponent.color.r,
			_victoryTextImageComponent.color.g,
			_victoryTextImageComponent.color.b,
			value);

		_symbolImageComponent.color = new Color(
			_symbolImageComponent.color.r,
			_symbolImageComponent.color.g,
			_symbolImageComponent.color.b,
			value);
	}

	public void SetSymbol(Symbol symbol)
	{
		if (symbol == Symbol.X)
		{
			_symbolImageComponent.sprite = _symbolXSprite;
		}
		else if (symbol == Symbol.O)
		{
			_symbolImageComponent.sprite = _symbolOSprite;
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {symbol}");
		}
	}

	public async Task StartAnimation()
	{
		_blurLayer.SetActive(true);

		SetSpritesTransparent(255f);

		List<Task> tasks = new()
		{
			_yellowChevronLeftTween.ExecuteTweenOrders("Move In"),
			_blackChevronRightTween.ExecuteTweenOrders("Move In"),
			_victoryTextTween.ExecuteTweenOrders("Move In"),
			_symbolTween.ExecuteTweenOrders("Move In"),
			_blurLayerTween.ExecuteTweenOrders("Appear")
		};

		foreach (var task in tasks)
		{
			await task;
		}

		tasks.Clear();

		await Task.Delay((int)(_delaySeconds * 1000));

		tasks = new()
		{
			_yellowChevronLeftTween.ExecuteTweenOrders("Move Out"),
			_blackChevronRightTween.ExecuteTweenOrders("Move Out"),
			_victoryTextTween.ExecuteTweenOrders("Move Out"),
			_symbolTween.ExecuteTweenOrders("Move Out"),
			_blurLayerTween.ExecuteTweenOrders("Disappear")
		};

		foreach (var task in tasks)
		{
			await task;
		}

		SetSpritesTransparent(0f);
		ResetPosition();
		_blurLayer.SetActive(false);
	}

	public void ResetPosition()
	{
		_yellowChevronLeftRectTransformComponent.anchoredPosition = _yellowChevronLeftOriginalPosition;
		_blackChevronRightRectTransformComponent.anchoredPosition = _blackChevronRightOriginalPosition;
		_victoryTextRectTransformComponent.anchoredPosition = _victoryTextOriginalPosition;
		_symbolRectTransformComponent.anchoredPosition = _symbolOriginalPosition;
	}
}
