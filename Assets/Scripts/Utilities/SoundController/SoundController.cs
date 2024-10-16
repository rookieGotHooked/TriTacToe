using UnityEngine;

public class SoundController : MonoBehaviour
{
    private float _sfxVolume;

	[SerializeField] OnSFXChangeEvent _SFXChangeEvent;

	private void Awake()
	{
		_sfxVolume = GetAudioVolume();
	}

	private void Start()
	{
		UpdateAudioVolume(_sfxVolume);
	}

	public float GetAudioVolume()
	{
		if (!PlayerPrefs.HasKey("TriTacToe_SFXVolume"))
		{
			PlayerPrefs.SetFloat("TriTacToe_SFXVolume", 1f);

			return 1f;
		}
		else
		{
			return PlayerPrefs.GetFloat("TriTacToe_SFXVolume");
		}
	}

	private void SetAudioVolume()
	{
		PlayerPrefs.SetFloat("TriTacToe_SFXVolume", _sfxVolume);
	}

	public void UpdateAudioVolume(float value)
	{
		_SFXChangeEvent.Raise(new FloatWrapper(value));
		_sfxVolume = value;

		SetAudioVolume();
	}
}
