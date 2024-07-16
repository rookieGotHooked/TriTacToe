using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class FSM_ScreensManager<EScreen>: MonoBehaviour where EScreen : Enum
{
	protected BaseScreen<EScreen> _currentScreen;
	protected EScreen _nextScreen;
	protected Dictionary<EScreen, BaseScreen<EScreen>> _screenDict = new Dictionary<EScreen, BaseScreen<EScreen>>();
    protected Dictionary<EScreen, AssetsGroup<EScreen>> _assetsDict = new Dictionary<EScreen, AssetsGroup<EScreen>>();

    [SerializeField] protected RectTransform _canvasGroup;

    [Header("Screen Assets Section")]
    [SerializeField]
    List<ScreenAssetsGroup<EScreen>> _screenAssetsList = new List<ScreenAssetsGroup<EScreen>>();

	protected bool _transitioning = false;

    protected virtual void Start()
    {
        _currentScreen.OnEnter();
    }

    protected virtual void Update()
    {
        if (!_transitioning && _currentScreen.NextScreen.Equals(_currentScreen.ScreenKey))
		{
			_currentScreen.OnUpdate();
		}
        else if (!_transitioning)
        {
            ChangeScreen(_currentScreen.NextScreen);
        }
    }

    protected virtual void ChangeScreen(EScreen screenKey)
    {
        _transitioning = true;
        _currentScreen.OnExit();
        _currentScreen = _screenDict[screenKey];
        _currentScreen.OnEnter();
        _transitioning = false;
    }

	protected virtual bool AddScreen(EScreen eScreen, BaseScreen<EScreen> screen)
	{
		return _screenDict.TryAdd(eScreen, screen);
	}

    protected virtual void PopuplateAssetDict()
    {
        foreach (var item in _screenAssetsList)
        {
            bool result = _assetsDict.TryAdd(item.ScreenEnum, item.AssetsGroup);

            if (!result)
            {
                throw new Exception("Encountered error during populating process of Assets Dict");
            }
        }
	}
}
