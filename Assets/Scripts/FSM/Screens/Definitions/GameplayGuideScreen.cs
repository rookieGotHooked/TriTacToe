using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayGuideScreen : BaseScreen<ScreensEnum>
{
	public GameplayGuideScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
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
		if (!_isInit)
		{
			await InstantiateObjects();
			_isInit = true;
		}
	}

	public override void OnUpdate()
	{

	}

	public override void OnExit()
	{

	}
}
