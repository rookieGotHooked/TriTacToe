using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TextboxButton : MonoBehaviour
{
    //[SerializeField] 
    //private GameObject _linkedTextbox;
    //public GameObject LinkedTextbox { get => _linkedTextbox; set => _linkedTextbox = value; }

    [SerializeField]
    private float _textboxDisplayDuration;

    private Tweens2D _tweens2DComponent;
    private Button _buttonComponent;

	private void Awake()
	{
        if (!TryGetComponent(out _buttonComponent))
        {
            throw new System.Exception($"{gameObject.name} does not contains Button component");
        }

        _buttonComponent.onClick.AddListener(delegate { _ = TextboxHandling(); });
        _buttonComponent.interactable = false;
	}

    public void AssignTextbox(GameObject textbox)
    {
        if (!textbox.TryGetComponent(out _tweens2DComponent))
        {
            throw new System.Exception($"{textbox.name} does not contains Tweens2D component");
        }

        _buttonComponent.interactable = true;
    }

	async private Task TextboxHandling()
    {
        _buttonComponent.interactable = false;

        await _tweens2DComponent.ExecuteTweenOrders("Move In Appear");

        //await Task.Delay((int)(_textboxDisplayDuration * 1000f));
        await DelayHelper.Delay(_textboxDisplayDuration);

		await _tweens2DComponent.ExecuteTweenOrders("Move Out Disappear");

        _buttonComponent.interactable = true;
	}

    public void SetInteractable(bool value)
    {
        _buttonComponent.interactable = value;
    }
}
