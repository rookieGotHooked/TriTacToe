public class MainMenuScreen : BaseScreen<ScreensEnum>
{

	public MainMenuScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
	}

	public override void OnEnter()
	{
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			await InstantiateObjects();
			SetInit();
		}
	}

	public override void OnExit()
	{
	}
}
