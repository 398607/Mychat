using System;
using UnityEngine;
using System.Collections;

public class SetLine : Line
{
	private int _index;
	private int _value;

	public SetLine(int index = 0, int value = 0)
	{
		_index = index;
		_value = value;
	}

	public override void React()
	{
		GameManager.SetGlobalVarible(_index, _value);
	}

	public override void Build(string[] lineEle)
	{
		// Set < set_index > < set_value > [< askVar > < askValue >]
		_index = Convert.ToInt32(lineEle[1]);
		_value = Convert.ToInt32(lineEle[2]);
	}
}
