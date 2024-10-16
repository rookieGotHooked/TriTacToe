using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXSliderBehavior : MonoBehaviour, IPointerUpHandler
{
    ScreenManager _managerInstance = ScreenManager.Instance;
    Slider _sfxSlider; 
    [SerializeField] AudioSource _testSound;

	private void Awake()
	{
		if (!TryGetComponent(out _sfxSlider))
        {
            throw new System.Exception($"{gameObject.name} does not contains Slider component");
        }
	}

	private void Start()
	{
		_sfxSlider.value = _managerInstance.SoundController.GetAudioVolume();
	}

	public void OnSFXChange()
    {
        _managerInstance.SoundController.UpdateAudioVolume(_sfxSlider.value);
    }

	public void OnPointerUp(PointerEventData eventData)
	{
        if (_testSound)
        {
            _testSound.Play();
        }
	}
}
