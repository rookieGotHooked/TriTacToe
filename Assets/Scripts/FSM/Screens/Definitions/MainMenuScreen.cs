using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class MainMenuScreen : BaseScreen<ScreensEnum>
{
	public MainMenuScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
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
		throw new System.NotImplementedException();
	}

	public override void OnUpdate()
	{
		throw new System.NotImplementedException();
	}

	public override void OnExit()
	{
		throw new System.NotImplementedException();
	}
}
