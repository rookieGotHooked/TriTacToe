using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSymbolController : MonoBehaviour
{
    Image _imageComponent;

	[SerializeField] Sprite _xSymbolSprite;
	[SerializeField] Sprite _oSymbolSprite;

	private void Awake()
	{
		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"GameObject {gameObject.name} does not contains Image component");
		}
		else
		{
			_imageComponent.sprite = _xSymbolSprite;
		}
	}

	public void ChangeSymbol()
	{
		if (_imageComponent.sprite == _xSymbolSprite)
		{
			_imageComponent.sprite = _oSymbolSprite;
		}
		else if (_imageComponent ==  _oSymbolSprite)
		{
			_imageComponent.sprite = _xSymbolSprite;
		}
		else
		{
			throw new System.Exception($"Unexpected Image component sprite: {_imageComponent.sprite}");
		}
	}
}
