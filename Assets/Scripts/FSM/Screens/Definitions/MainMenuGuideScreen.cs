using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGuideScreen : BaseScreen<ScreensEnum>
{
	public MainMenuGuideScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
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

	public override async void OnEnter()
	{
		if (!IsInit)
		{
			await InstantiateObjects();
			SetInit();
		}

		SetInteractableButtons(true);
	}

	public override void OnUpdate()
	{

	}

	public override void OnExit()
	{
		SetInteractableButtons(false);
	}
}
