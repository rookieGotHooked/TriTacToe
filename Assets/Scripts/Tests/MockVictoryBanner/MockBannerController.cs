using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MockBannerController : MonoBehaviour
{
    [SerializeField] GameObject _yellowChevronLeft;
	[SerializeField] GameObject _blackChevronRight;
	[SerializeField] GameObject _victoryText;
	[SerializeField] GameObject _symbolX; 
    [SerializeField] GameObject _symbolO;

    Tweens2D _yellowChevronLeftTween;
    Tweens2D _blackChevronRightTween;
    Tweens2D _victoryTextTween;
    Tweens2D _symbolXTween;
    Tweens2D _symbolOTween;

	private void Awake()
	{
		if (!_yellowChevronLeft.TryGetComponent(out _yellowChevronLeftTween)) 
        {
            throw new System.Exception($"{_yellowChevronLeft.name} does not contains Tweens2D component");
        }

        if (!_blackChevronRight.TryGetComponent(out _blackChevronRightTween))
		{
			throw new System.Exception($"{_blackChevronRight.name} does not contains Tweens2D component");
		}

        if (!_victoryText.TryGetComponent(out _victoryTextTween))
		{
			throw new System.Exception($"{_victoryText.name} does not contains Tweens2D component");
		}

		if (!_symbolX.TryGetComponent(out _symbolXTween))
		{
			throw new System.Exception($"{_symbolX.name} does not contains Tweens2D component");
		}

        if (!_symbolO.TryGetComponent(out _symbolOTween))
		{
			throw new System.Exception($"{_symbolO.name} does not contains Tweens2D component");
		}
	}

	private async void Start()
	{
		await Tween();
	}

	private async Task Tween()
	{
		List<Task> tasks = new();
		List<Tweens2D> tweens = new()
		{
			_yellowChevronLeftTween,
			_blackChevronRightTween,
			_victoryTextTween,
			_symbolXTween,
			_symbolOTween
		};

		foreach (var tween in tweens)
		{
			tasks.Add(AwaitInit(tween));
		}

		foreach (var task in tasks)
		{
			await task;
		}

		tasks.Clear();

		await Task.Delay(2000);

		foreach (var tween in tweens)
		{
			tasks.Add(tween.ExecuteTweenOrders("Move Out"));
		}

		foreach (var task in tasks)
		{
			await task;
		}
	}

	private async Task AwaitInit(Tweens2D component)
	{
		while (component.IsInit == false)
		{
			await Task.Yield();
		}
	}
}
