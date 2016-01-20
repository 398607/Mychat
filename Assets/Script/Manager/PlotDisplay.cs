using UnityEngine;
using System.Collections;

public class PlotDisplay : ViewPanel
{
	public enum State
	{
		Flowing,
		WaitForChoice,
		WaitForValue
	}

	public PlotManager PlotManager;

	// choice
	public int NowAffIndex;
	public ChoiceLine NowChoiceLine;

	// wait
	private int _waitIndex;
	private int _waitValue;

	// scroll
	public float NextLinePositionY;
	public float TargetPositionY;

	public State PlotState;

	public PlotDisplay(string name = "test")
	{
		PlotState = State.Flowing;

		PlotManager = new PlotManager(name);

		TargetPositionY = -1f;
		NextLinePositionY = 0;
	}

	public void Update()
	{
		switch (PlotState)
		{
			case State.Flowing:
			{
				PlotManager.Update();
				break;
			}
			case State.WaitForValue:
			{
				if (GameManager.FitAsked(_waitIndex, _waitValue))
				{
					PlotState = State.Flowing;
				}
				break;
			}
			default:
			{
				break;
			}
		}

		if (PlotState == State.Flowing)
			PlotManager.Update();
	}

	public void GetWait(int waitIndex, int waitValue)
	{
		_waitIndex = waitIndex;
		_waitValue = waitValue;
		PlotState = State.WaitForValue;
	}
}
