using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SFXVolumeChangeListener : GameEventListener<FloatWrapper>
{
	List<AudioSource> _audioSources = new();

	private void Start()
	{
		_audioSources = GetComponents<AudioSource>().ToList();
	}

	public override void OnEventRaised(FloatWrapper floatWrapper)
	{
		foreach (var audioSource in _audioSources)
		{
			audioSource.volume = floatWrapper.Value;
		}
	}
}
