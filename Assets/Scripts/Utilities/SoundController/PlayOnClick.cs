using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayOnClick : MonoBehaviour
{
	private Button _button;
	private AudioSource _audioSource;

	private void Awake()
	{
		if (!TryGetComponent(out _audioSource))
		{
			throw new System.Exception($"{gameObject.name} does not contains AudioSource component");
		}

		if (!TryGetComponent(out _button))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		_button.onClick.AddListener(PlaySFXOnClick);
	}

	public void PlaySFXOnClick()
	{
		_audioSource.Play();
	}
}
