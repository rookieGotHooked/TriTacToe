using System;

public class GameplayScreen : BaseScreen<ScreensEnum>
{
	public GameplayScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
		if (screenDefinition != null)
		{
			ScreenKey = screenDefinition.ScreenEnum;
		}
		else
		{
			throw new Exception($"{GetType().Name} assets is null.");
		}
	}

	public override void OnEnter()
	{
		if (GameManager.Instance != null && ScreenManager.Instance.PreviousScreen == ScreensEnum.MainMenu)
		{
			GameManager.Instance.SetGameMode(ScreenManager.Instance.CurrentGameMode);
			GameManager.Instance.GameStateDict[GameStates.Initiate].OnEnter();
		}
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			_screenDefinition.ObjectGroupsList[1].Objects[0].Object.gameObject.GetComponent<GameManager>().ScreenObject = _screenObject;

			await InstantiateObjects();
			SetInit();
		}

	}

	public override void OnExit()
	{
	}
}
