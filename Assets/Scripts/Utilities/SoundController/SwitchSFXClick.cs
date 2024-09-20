using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchSFXClick : MonoBehaviour
{
	private Button _button;

	[SerializeField] private bool _isForwardToBackward = true;

	[SerializeField] private AudioSource _audioForward;
	[SerializeField] private AudioSource _audioBackward;


	private void Awake()
	{
		if (!_audioBackward)
		{
			throw new System.Exception("No component found for _audioBackward");
		}

		if (!_audioForward)
		{
			throw new System.Exception("No component found for _audioForward");
		}

		if (!TryGetComponent(out _button))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		_button.onClick.AddListener(PlaySFXOnClick);
	}



	public void PlaySFXOnClick()
	{
		if (_isForwardToBackward)
		{
			_audioForward.Play();
			_isForwardToBackward = false;
		}
		else
		{
			_audioBackward.Play();
			_isForwardToBackward = true;
		}
	}
}
