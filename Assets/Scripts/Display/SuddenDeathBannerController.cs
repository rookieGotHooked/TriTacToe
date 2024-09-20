using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SuddenDeathBannerController : MonoBehaviour
{
    private Tweens2D _tweenComponent;

	private RectTransform _rectTransformComponent;
	Vector2 _originalPosition;

	private SuddenDeathRule _currentRule;
	public SuddenDeathRule CurrentRule => _currentRule;

    private Image _imageComponent;
    private Image _suddenDeathRule1ImageComponent;
	private Image _suddenDeathRule2ImageComponent;
	private Image _suddenDeathRule3ImageComponent;
	private Image _suddenDeathRule4ImageComponent;


	private void Awake()
	{
		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!TryGetComponent(out _tweenComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}
		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}

		GameObject ruleAlignGameObject = transform.GetChild(0).gameObject;

		if (!ruleAlignGameObject.transform.GetChild(0).gameObject.TryGetComponent(out _suddenDeathRule1ImageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}
		if (!ruleAlignGameObject.transform.GetChild(1).gameObject.TryGetComponent(out _suddenDeathRule2ImageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}
		if (!ruleAlignGameObject.transform.GetChild(2).gameObject.TryGetComponent(out _suddenDeathRule3ImageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}
		if (!ruleAlignGameObject.transform.GetChild(3).gameObject.TryGetComponent(out _suddenDeathRule4ImageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}
	}

	async public Task Move()
	{
		await _tweenComponent.ExecuteTweenOrders("Move Right To Left");
	}

	public void ShowRule1()
	{
		SetTransparency(_suddenDeathRule1ImageComponent, 255f);
		SetTransparency(_suddenDeathRule2ImageComponent, 0f);
		SetTransparency(_suddenDeathRule3ImageComponent, 0f);
		SetTransparency(_suddenDeathRule4ImageComponent, 0f);
	}

	public void ShowRule2()
	{
		SetTransparency(_suddenDeathRule1ImageComponent, 0f);
		SetTransparency(_suddenDeathRule2ImageComponent, 255f);
		SetTransparency(_suddenDeathRule3ImageComponent, 0f);
		SetTransparency(_suddenDeathRule4ImageComponent, 0f);

		_currentRule = SuddenDeathRule.Rule1;
	}

	public void ShowRule3()
	{
		SetTransparency(_suddenDeathRule1ImageComponent, 0f);
		SetTransparency(_suddenDeathRule2ImageComponent, 0f);
		SetTransparency(_suddenDeathRule3ImageComponent, 255f);
		SetTransparency(_suddenDeathRule4ImageComponent, 0f);
	}

	public void ShowRule4()
	{
		SetTransparency(_suddenDeathRule1ImageComponent, 0f);
		SetTransparency(_suddenDeathRule2ImageComponent, 0f);
		SetTransparency(_suddenDeathRule3ImageComponent, 0f);
		SetTransparency(_suddenDeathRule4ImageComponent, 255f);
	}


	public void SetRule1()
	{
		_currentRule = SuddenDeathRule.Rule1;
	}
	public void SetRule2()
	{
		_currentRule = SuddenDeathRule.Rule2;
	}
	public void SetRule3()
	{
		_currentRule = SuddenDeathRule.Rule3;
	}

	public void Hide()
	{
		SetTransparency(_imageComponent, 0f);
		SetTransparency(_suddenDeathRule1ImageComponent, 0f);
		SetTransparency(_suddenDeathRule2ImageComponent, 0f);
		SetTransparency(_suddenDeathRule3ImageComponent, 0f);
		SetTransparency(_suddenDeathRule4ImageComponent, 0f);
	}

	public void AssignRule(SuddenDeathRule rule)
	{
		switch (rule)
		{
			case SuddenDeathRule.Rule1:
				SetRule1(); 
				break;
			case SuddenDeathRule.Rule2:
				SetRule2();
				break;
			case SuddenDeathRule.Rule3:
				SetRule3();
				break;
			default:
				throw new System.Exception($"Unexpected rule detected: {rule}");
		}
	}

	public void Show()
	{
		SetTransparency(_imageComponent, 255f);

		switch (_currentRule)
		{
			case SuddenDeathRule.Rule1:
				ShowRule1();
				break;
			case SuddenDeathRule.Rule2:
				ShowRule2();
				break;
			case SuddenDeathRule.Rule3:
				ShowRule3();
				break;
			default:
				throw new System.Exception($"Unexpected rule detected: {_currentRule}");
		}
	}

	public void SetTransparency(Image imageComponent, float transparencyValue)
	{
		imageComponent.color = new Color(
			imageComponent.color.r,
			imageComponent.color.g,
			imageComponent.color.b,
			transparencyValue);
	}

	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}

public enum SuddenDeathRule
{
	Rule1, Rule2, Rule3
}
