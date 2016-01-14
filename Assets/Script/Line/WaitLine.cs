using System.Collections;
using UnityEngine;

public class WaitLine : Line
{
	public int WaitIndex;
	public int WaitValue;

	public WaitLine(int waitIndex, int waitValue)
	{
		WaitIndex = waitIndex;
		WaitValue = waitValue;
	}

	public override bool React()
	{
		while (!GameManager.FitAsked(WaitIndex, WaitValue))
		{
			Wait();
		}
		return true;
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(1f);
	}
}