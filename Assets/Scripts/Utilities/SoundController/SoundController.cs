using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    List<AudioSource> _sfxSources = new();
	List<AudioSource> _bgmSources = new();

    private float _sfxVolume;
	public float SFXVolume => _sfxVolume;

	private float _bgmVolume;
	public float BGMVolume => _bgmVolume;

	private void Awake()
	{
		GetAudioVolume(AudioType.SFX);
		GetAudioVolume(AudioType.BGM);
	}

	public void SetAudioVolume(AudioType type, float value)
	{
		if (type == AudioType.SFX)
		{
			if (value <= 0)
			{
				_sfxVolume = 0f;
			}
			else if (value >= 1)
			{
				_sfxVolume = 1f;
			}
			else
			{
				_sfxVolume = value;
			}

			PlayerPrefs.SetFloat("TriTacToe_SFXVolume", value);
		}
		else
		{
			if (value <= 0)
			{
				_bgmVolume = 0f;
			}
			else if (value >= 1)
			{
				_bgmVolume = 1f;
			}
			else
			{
				_bgmVolume = value;
			}

			PlayerPrefs.SetFloat("TriTacToe_BGMVolume", value);
		}
	}

	public void GetAudioVolume(AudioType type)
	{
		if (type == AudioType.SFX)
		{
			if (!PlayerPrefs.HasKey("TriTacToe_SFXVolume"))
			{
				_sfxVolume = 1f;

				PlayerPrefs.SetFloat("TriTacToe_SFXVolume", _sfxVolume);
			}
			else
			{
				_sfxVolume = PlayerPrefs.GetFloat("TriTacToe_SFXVolume");
			}
		}
		else
		{
			if (!PlayerPrefs.HasKey("TriTacToe_BGMVolume"))
			{
				_bgmVolume = 1f;

				PlayerPrefs.SetFloat("TriTacToe_BGMVolume", _bgmVolume);
			}
			else
			{
				_bgmVolume = PlayerPrefs.GetFloat("TriTacToe_BGMVolume");
			}
		}
	}

	public void AddAudioSource(AudioType type, AudioSource audioSource)
	{
		if (type == AudioType.SFX)
		{
			if (!_sfxSources.Contains(audioSource))
			{
				_sfxSources.Add(audioSource);
			}
		}
		else
		{
			if (!_bgmSources.Contains(audioSource))
			{
				_bgmSources.Add(audioSource);
			}
		}
	}

	public void AddAudioSource(AudioType type, List<AudioSource> audioSources)
	{
		if (type == AudioType.SFX)
		{
			foreach (var source in audioSources)
			{
				if (!_sfxSources.Contains(source))
				{
					_sfxSources.Add(source);
				}
			}
		}
		else
		{
			foreach (var source in audioSources)
			{
				if (!_bgmSources.Contains(source))
				{
					_bgmSources.Add(source);
				}
			}
		}
	}

	public void UpdateAllSourcesVolume(AudioType type)
	{
		if (type == AudioType.SFX)
		{
			foreach (var source in _sfxSources)
			{
				source.volume = _sfxVolume;
			}
		}
		else
		{
			foreach (var source in _bgmSources)
			{
				source.volume = _bgmVolume;
			}
		}
	}
}

public enum AudioType
{
	SFX, BGM
}
