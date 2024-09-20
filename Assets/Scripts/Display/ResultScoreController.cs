using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ResultScoreController : MonoBehaviour
{
	[SerializeField] Sprite _scoreZero;
	[SerializeField] Sprite _scoreOne;
	[SerializeField] Sprite _scoreTwo;
	[SerializeField] Sprite _scoreThree;

	Image _symbolIndicatorImageComponent;
	Image _normalBackgroundImageComponent;
	Image _highlightBackgroundImageComponent;
	Image _scoreImageComponent;

	Tweens2D _objectTween;
	Tweens2D _highlightTween;

	RectTransform _rectTransformComponent;

	Vector2 _originalPosition;

	private void Awake()
	{
		//Debug.Log("Entered ResultScoreController.Awake()");

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!TryGetComponent(out _objectTween))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}
		//else
		//{
		//	Debug.Log($"_objectTween found: {_objectTween}");
		//}

		if (!gameObject.transform.GetChild(0).gameObject.TryGetComponent(out _symbolIndicatorImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(0).gameObject.name} does not contains Image component");
		}

		if (!gameObject.transform.GetChild(1).gameObject.TryGetComponent(out _normalBackgroundImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(1).gameObject.name} does not contains Image component");
		}

		if (!gameObject.transform.GetChild(2).gameObject.TryGetComponent(out _highlightBackgroundImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(2).gameObject.name} does not contains Image component");
		}

		if (!gameObject.transform.GetChild(2).gameObject.TryGetComponent(out _highlightTween))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(2).gameObject.name} does not contains Tweens2D component");
		}

		if (!gameObject.transform.GetChild(3).gameObject.TryGetComponent(out _scoreImageComponent))
		{
			throw new System.Exception($"{gameObject.transform.GetChild(3).gameObject.name} does not contains Image component");
		}
	}

	public void SetTransparencyAll(float highligthValue, float allValue)
	{
		_symbolIndicatorImageComponent.color = new Color(
			_symbolIndicatorImageComponent.color.r,
			_symbolIndicatorImageComponent.color.g,
			_symbolIndicatorImageComponent.color.b,
			allValue);

		_normalBackgroundImageComponent.color = new Color(
			_normalBackgroundImageComponent.color.r,
			_normalBackgroundImageComponent.color.g,
			_normalBackgroundImageComponent.color.b,
			allValue);

		_highlightBackgroundImageComponent.color = new Color(
			_highlightBackgroundImageComponent.color.r,
			_highlightBackgroundImageComponent.color.g,
			_highlightBackgroundImageComponent.color.b,
			highligthValue);

		_scoreImageComponent.color = new Color(
			_highlightBackgroundImageComponent.color.r,
			_highlightBackgroundImageComponent.color.g,
			_highlightBackgroundImageComponent.color.b,
			allValue);
	}

	public void SetScore(int score)
	{
		switch (score)
		{
			case 0:
				_scoreImageComponent.sprite = _scoreZero; 
				break;
			case 1:
				_scoreImageComponent.sprite = _scoreOne;
				break;
			case 2:
				_scoreImageComponent.sprite = _scoreTwo;
				break;
			case 3:
				_scoreImageComponent.sprite = _scoreThree;
				break;
			default:
				throw new System.Exception($"Invalid score detected: {score}");
		}
	}

	async public Task HighlightAppear()
	{
		await _highlightTween.ExecuteTweenOrders("Appear");
	}

	public void HighlightDisappear()
	{
		_highlightBackgroundImageComponent.color = new Color(
			_highlightBackgroundImageComponent.color.r,
			_highlightBackgroundImageComponent.color.g,
			_highlightBackgroundImageComponent.color.b,
			0f);
	}

	async public Task MoveIn()
	{
		await _objectTween.ExecuteTweenOrders("Move In");
	}

	async public Task MoveOut()
	{
		await _objectTween.ExecuteTweenOrders("Move Out");
	}

	public void ResetPosition()
	{
		SetTransparencyAll(0f, 0f);
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}
