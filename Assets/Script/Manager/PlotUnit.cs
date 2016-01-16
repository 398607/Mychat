using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlotUnit
{
	public int AskedValue;

	public int AskedVar;
	public float Delay;
	public Line Line;

	public PlotUnit()
	{
		Delay = -0f;
		AskedVar = 0;
		AskedValue = 0;
	}

	public PlotUnit(float delay, Line line, int askedVar = 0, int askedValue = 0)
	{
		Delay = delay;
		Line = line;
		AskedValue = askedValue;
		AskedVar = askedVar;
	}

	public void Build(string str)
	{
		var lineEle = str.Split(' ');

		switch (lineEle[0])
		{
			case "Sp":
			{
				// Sp <timeDelay> <person> <content> [<askVar> <askValue>]
				Delay = (float)Convert.ToDouble(lineEle[1]);
				Line = new SpeechLine();
				Line.Build(lineEle);
				if (lineEle.Length > 4)
				{
					AskedVar = Convert.ToInt32(lineEle[4]);
					AskedValue = Convert.ToInt32(lineEle[5]);
				}

				break;
			}
			case "Sys":
			{
				// Sys <timeDelay> <content> [<askVar> <askValue>]

				Delay = (float)Convert.ToDouble(lineEle[1]);
				Line = new SystemLine();
				Line.Build(lineEle);
				if (lineEle.Length > 4)
					SetAskedVarValue(lineEle, 3, 4);
				break;
			}
			case "Ch":
			{
				// Ch <timeDelay> <aff_index> <choice_0> [<choice_1> ... <choice_n>] [<askVar> <askValue>]

				Delay = (float)Convert.ToDouble(lineEle[1]);
				Line = new ChoiceLine();
				Line.Build(lineEle);

				var i = 3;
				while (lineEle.Length > i && !"0123456789".Contains(lineEle[i].Substring(0, 1)))
					i++;

				if (lineEle.Length > i + 1)
					SetAskedVarValue(lineEle, i, i + 1);
				break;
			}
			case "Wait":
			{
				// Wait <wait_index> <wait_value> [<askVar> <askValue>]
				Delay = 0f;
				Line = new WaitLine();
				Line.Build(lineEle);

				if (lineEle.Length > 4)
					SetAskedVarValue(lineEle, 3, 4);
				break;
			}
			case "Set":
			{
				// Set <setindex> <set_value> [<askVar> <askValue>]
				Delay = 0f;
				Line = new SetLine();
				Line.Build(lineEle);

				if (lineEle.Length > 4)
					SetAskedVarValue(lineEle, 3, 4);
				break;
			}
			default:
			{
				Delay = -1f;
				break;
			}
		}
	}

	private void SetAskedVarValue(string[] lineEle, int varIndex, int valueIndex)
	{
		AskedVar = Convert.ToInt32(lineEle[3]);
		AskedValue = Convert.ToInt32(lineEle[4]);
	}
}

public class PlotUnitList : List<PlotUnit>
{
}
