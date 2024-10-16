using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class FSM_ScreensManager<EScreen>: MonoBehaviour where EScreen : Enum
{
    #region Protected screen definitions and variables
    //protected SoundController _soundController;
    //public SoundController SoundController => _soundController;
	protected BaseScreen<EScreen> _currentScreen;
	protected EScreen _nextScreen;
    protected EScreen _previousScreen;
    public EScreen PreviousScreen { get => _previousScreen; }
    protected ScreenTransitionCode _currentTransitionCode;

	protected List<BaseScreen<EScreen>> _screensList;
	protected Dictionary<EScreen, BaseScreen<EScreen>> _screenDict = new Dictionary<EScreen, BaseScreen<EScreen>>();
    public Dictionary<EScreen, BaseScreen<EScreen>> ScreenDict => _screenDict;
    protected Dictionary<EScreen, ScreenDefinition<EScreen>> _screenDefinitionDict = new Dictionary<EScreen, ScreenDefinition<EScreen>>();
    protected Dictionary<ScreenTransitionCode, List<ScreenTransitionGroup<EScreen>>> _transitionDict = new Dictionary<ScreenTransitionCode, List<ScreenTransitionGroup<EScreen>>>();
	#endregion

	#region Serialized values
    [Header("Screen Assets Section")]
    [SerializeField]
    List<ScreenDefinition<EScreen>> _screenAssetsList = new List<ScreenDefinition<EScreen>>();

    [Header("Screen Transitions Section")]
    [SerializeField]
    List<ScreenTransitionBase<EScreen>> _screenTransitions = new List<ScreenTransitionBase<EScreen>>();
	#endregion

	protected bool _transitioning = false;

	protected virtual void Awake()
	{
        Application.targetFrameRate = 60;

		//if (!TryGetComponent(out _soundController))
  //      {
  //          throw new Exception($"{gameObject.name} does not contains SoundController component.");
  //      }

        PopuplateAssetDict();
        PopulateTransitionDict();
        AddScreens();
	}

	protected virtual void Start()
    {
		_currentScreen.OnEnter();
    }

    protected virtual void Update()
    {
        if (!_transitioning && _nextScreen.Equals(_currentScreen.ScreenKey))
		{
			_currentScreen.OnUpdate();
		}
        else if (!_transitioning)
        {
            ChangeScreen(_nextScreen);
        }
    }

    async protected virtual void ChangeScreen(EScreen screenKey)
    {
        _transitioning = true;

		_currentScreen.OnExit();

        _currentScreen = _screenDict[screenKey];

        _currentScreen.OnEnter();

		await ExecuteTransition();

		_transitioning = false;
    }

    protected virtual void AddScreens()
    {
        if (_screensList != null)
        {
            foreach (var screen in _screensList)
            {
                if (!_screenDict.TryAdd(screen.ScreenKey, screen))
                {
                    throw new Exception("Unexpected error encountered when adding item to screen dictionary");
                }
            }
        }
        else
        {
            throw new Exception("List of screens is currently null");
        }
    }

    public virtual void SetNextScreen(EScreen eScreen, ScreenTransitionCode transitionCode)
    {
        if (_screenDict.ContainsKey(eScreen))
        {
            _previousScreen = _currentScreen.ScreenKey;
            _nextScreen = eScreen;
            _currentTransitionCode = transitionCode;
        }
        else
        {
            throw new Exception($"Unexpected value: Undefined screen detected - {eScreen}");
        }
    }

    protected virtual void PopuplateAssetDict()
    {
        foreach (var item in _screenAssetsList)
        {
            bool result = _screenDefinitionDict.TryAdd(item.ScreenEnum, item);

            if (!result)
            {
                throw new Exception("Encountered error during populating process of _screenDefinitionDict");
            }
        }
	}

    protected virtual void PopulateTransitionDict()
    {
        foreach (var transition in _screenTransitions)
        {
            bool result = _transitionDict.TryAdd(transition.transitionCode, transition.transitionGroups);

            if (!result)
            {
                throw new Exception("Encountered error during population process of _transitionDict");
            }
        }
    }
	async protected virtual Task ExecuteTransition()
	{
		if (_transitionDict.ContainsKey(_currentTransitionCode))
        {
            foreach (var transitionGroup in _transitionDict[_currentTransitionCode])
            {
                List<Task> transitionTask = new List<Task>();

                foreach (var singleTransition in transitionGroup.transitionList)
                {
                    float duration;

                    if (singleTransition.overridenTransitionTime >= 0)
                    {
                        duration = singleTransition.overridenTransitionTime;
                    }
                    else
                    {
                        duration = singleTransition.directionalMovement.duration;
					}

                    transitionTask.Add(
                        _screenDict[singleTransition.screen].TransitionMapping(
                            singleTransition.directionalMovement.direction,
                            singleTransition.directionalMovement.movementValue, 
                            singleTransition.directionalMovement.tweenFormula,
							duration));
                }

                foreach (var task in transitionTask)
                {
                    await task;
                }
            }
        }
        else
        {
            throw new Exception($"Unexpected value: Unknown key received: {_currentTransitionCode}");
        }
	}
}
