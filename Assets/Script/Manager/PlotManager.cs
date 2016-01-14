using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlotUnit
{
	public int AskedValue;

	public int AskedVar;
	public float Delay;
	public Line Line;

	public PlotUnit(float delay, Line line, int asked_var = 0, int asked_value = 0)
	{
		Delay = delay;
		Line = line;
		AskedValue = asked_value;
		AskedVar = asked_var;
	}
}

public class UnitList : List<PlotUnit>
{
}

public class PlotManager : MonoBehaviour
{
	// about control
	private int _nextLineIndex;
	private float _time;
	private UnitList _unitList;
	// about itself
	public string PlotFileName;

	public PlotManager(string plotFileName = "test.txt")
	{
		PlotFileName = plotFileName;
		_unitList = new UnitList();
	}

	public void LoadUnitList(string fileName = "test.txt")
	{
		if (_unitList == null)
		{
			_unitList = new UnitList();
		}
		_unitList.Clear();

		if (!File.Exists(fileName))
		{
			return;
		}

		var testFile = File.ReadAllLines(fileName);

		foreach (var line in testFile)
		{
			Debug.Log(line);

			var lineEle = line.Split(' ');
			switch (lineEle[0])
			{
				case "//":
				{
					continue;
				}
				case "Sp":
				{
					var timeDelay = (float) Convert.ToDouble(lineEle[1]);
					var person = lineEle[2];
					var content = lineEle[3];
					if (lineEle.Length > 4)
					{
						var askedVar = Convert.ToInt32(lineEle[4]);
						var askedValue = Convert.ToInt32(lineEle[5]);
						_unitList.Add(new PlotUnit(timeDelay, new SpeechLine(person, content), askedVar, askedValue));
					}
					else
					{
						_unitList.Add(new PlotUnit(timeDelay, new SpeechLine(person, content)));
					}
					break;
				}
				case "Sys":
				{
					var timeDelay = (float) Convert.ToDouble(lineEle[1]);
					var content = lineEle[2];
					if (lineEle.Length > 4)
					{
						var askedVar = Convert.ToInt32(lineEle[4]);
						var askedValue = Convert.ToInt32(lineEle[5]);
						_unitList.Add(new PlotUnit(timeDelay, new SystemLine(content), askedVar, askedValue));
					}
					else
					{
						_unitList.Add(new PlotUnit(timeDelay, new SystemLine(content)));
					}
					break;
				}
				case "Ch":
				{
					var timeDelay = (float) Convert.ToDouble(lineEle[1]);
					var affindex = Convert.ToInt32(lineEle[2]);
					var choicelist = new ChoiceList();
					var i = 3;
					for (; i < lineEle.Length; i++)
					{
						if ("0123456789".Contains(lineEle[i].Substring(0, 1)))
						{
							break;
						}
						choicelist.Add(new Choice(lineEle[i]));
					}
					if (lineEle.Length > i + 1)
					{
						var askedVar = Convert.ToInt32(lineEle[i]);
						var askedValue = Convert.ToInt32(lineEle[i + 1]);
						_unitList.Add(new PlotUnit(timeDelay, new ChoiceLine(affindex, choicelist), askedVar, askedValue));
					}
					else
					{
						_unitList.Add(new PlotUnit(timeDelay, new ChoiceLine(affindex, choicelist)));
					}
					break;
				}
				case "Wait":
				{
					var waitIndex = Convert.ToInt32(lineEle[1]);
					var waitValue = Convert.ToInt32(lineEle[2]);

					if (lineEle.Length > 3)
					{
						var askedVar = Convert.ToInt32(lineEle[3]);
						var askedValue = Convert.ToInt32(lineEle[4]);
						_unitList.Add(new PlotUnit(0f, new WaitLine(waitIndex, waitValue), askedVar, askedValue));
					}
					else
					{
						_unitList.Add(new PlotUnit(0f, new WaitLine(waitIndex, waitValue)));
					}
					break;
				}
				default:
				{
					break;
				}
			}
		}
	}

	public GameManager.State GetGMState()
	{
		return GameManager.GetState();
	}

	// handle next plot unit
	public void SendLine()
	{
		var nextUnit = _unitList[_nextLineIndex];
		if (GameManager.FitAsked(nextUnit.AskedVar, nextUnit.AskedValue))
		{
			_time = 0f;
			nextUnit.Line.React();
			_nextLineIndex++;
		}
		else
		{
			_nextLineIndex++;
		}
	}

	// Use this for initialization
	private void Start()
	{
		_time = 0f;
		_nextLineIndex = 0;

		LoadUnitList();
	}

	// Update is called once per frame
	private void Update()
	{
		if (GetGMState() != GameManager.State.Flowing || _nextLineIndex >= _unitList.Count)
		{
			return;
		}
		_time += Time.deltaTime;

		// handle next unit when time is ready
		if (_time > _unitList[_nextLineIndex].Delay)
		{
			SendLine();
		}
	}
}