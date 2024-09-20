using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayController : MonoBehaviour
{
	Tweens2D _scoreDisplayTween;
    Tweens2D _highlightTween;

	RectTransform _rectTransformComponent;

	Image _imageComponent;
    Image _highlightImageComponent;
	Image _symbolImageComponent;

	List<Image> _scoreImageComponents = new();

	[SerializeField] List<GameObject> _scoreGameObjects = new();
	List<Tweens2D> _scoreTweenComponents = new();
	List<RectTransform> _scoreDisplayRectTransformComponents = new();

	Dictionary<RectTransform, Vector2> _originalAnchoredRectTransformDict = new();

	Vector2 _originalPosition;

	private void Awake()
	{
		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		GameObject highlightGameObject = gameObject.transform.GetChild(0).gameObject;

		if (!TryGetComponent(out _scoreDisplayTween))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}

		if (!highlightGameObject.TryGetComponent(out _highlightTween))
		{
			throw new System.Exception($"{highlightGameObject.name} does not contains Tweens2D component");
		}

		if (!transform.GetChild(1).gameObject.TryGetComponent(out _symbolImageComponent))
		{
			throw new System.Exception($"{transform.GetChild(1).gameObject.name} does not contains Image component");
		}

		if (!highlightGameObject.TryGetComponent(out _highlightImageComponent))
		{
			throw new System.Exception($"{highlightGameObject.name} does not contains Image component");
		}

		foreach (var gameObj in _scoreGameObjects)
		{
			Tweens2D tweenComponent;

			if (!gameObj.TryGetComponent(out tweenComponent))
			{
				throw new System.Exception($"{gameObj.name} does not contains Tweens2D component");
			}
			else
			{
				_scoreTweenComponents.Add(tweenComponent);
			}

			RectTransform rectTransform;

			if (!gameObj.TryGetComponent(out rectTransform))
			{
				throw new System.Exception($"{gameObj.name} does not contains RectTransform component");
			}
			else
			{
				_scoreDisplayRectTransformComponents.Add(rectTransform);

				if (!_originalAnchoredRectTransformDict.TryAdd(rectTransform, rectTransform.anchoredPosition)) 
				{
					throw new System.Exception($"Error occured when trying to add value to _originalAnchoredRectTransformDict; " +
						$"values are: {rectTransform}, {rectTransform.anchoredPosition}");
				}
			}

			Image tempImageComponent;

			if (!gameObj.TryGetComponent(out tempImageComponent))
			{
				throw new System.Exception($"{gameObj.name} does not contains Image component");
			}
			else
			{
				_scoreImageComponents.Add(tempImageComponent);
			}
		}
	}

	async public Task HighlighAppear()
	{
		await _highlightTween.ExecuteTweenOrders("Appear");
	}

	async public Task MoveTextUp(int amount)
	{
		//Debug.Log($"Called with amount = {amount}");

		List<Task> tasks = new();

		string order = amount switch
		{
			1 => "Move Up +1",
			2 => "Move Up +2",
			3 => "Move Up +3",
			_ => throw new System.Exception($"Unexpected move up amount received: {amount}")
		};

		//Debug.Log(_scoreTweenComponents.Count);

		foreach (var tween in _scoreTweenComponents)
		{
			tasks.Add(tween.ExecuteTweenOrders(order));
		}

		foreach (var task in tasks)
		{
			await task;
		}
	}

	public void ResetAll()
	{
		_highlightImageComponent.color = new(_highlightImageComponent.color.r, _highlightImageComponent.color.g, _highlightImageComponent.color.b, 0f);

		foreach (var rectTransform in _scoreDisplayRectTransformComponents)
		{
			rectTransform.anchoredPosition = _originalAnchoredRectTransformDict[rectTransform];
		}
	}

	async public Task MoveIn()
	{
		await _scoreDisplayTween.ExecuteTweenOrders("Move In");
	}

	async public Task MoveOut()
	{
		await _scoreDisplayTween.ExecuteTweenOrders("Move Out");
	}
	
	public void SetTransparencyAll(float highlightValue, float allValue)
	{
		_imageComponent.color = new Color(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			allValue);

		_highlightImageComponent.color = new Color(
			_highlightImageComponent.color.r,
			_highlightImageComponent.color.g,
			_highlightImageComponent.color.b,
			highlightValue);

		_symbolImageComponent.color = new Color(
			_symbolImageComponent.color.r,
			_symbolImageComponent.color.g,
			_symbolImageComponent.color.b,
			allValue);

		foreach (var component in _scoreImageComponents)
		{
			component.color = new Color(
				component.color.r,
				component.color.g,
				component.color.b,
				allValue);
		}
	}

	public void ResetPosition()
	{
		SetTransparencyAll(0f, 0f);
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}
