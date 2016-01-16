using System;
using System.Collections;
using UnityEngine;

public class WaitLine : Line
{
	public int WaitIndex;
	public int WaitValue;

	public WaitLine(int waitIndex = 0, int waitValue = 0)
	{
		WaitIndex = waitIndex;
		WaitValue = waitValue;
	}

	public override void React()
	{
		while (!GameManager.FitAsked(WaitIndex, WaitValue))
		{
			Wait();
		}
	}

	private static IEnumerator Wait()
	{
		yield return new WaitForSeconds(1f);
	}

	public override void Build(string[] lineEle)
	{
		// Wait <wait_index> <wait_value> [<askVar> <askValue>]
		WaitIndex = Convert.ToInt32(lineEle[1]);
		WaitValue = Convert.ToInt32(lineEle[2]);
	}
}