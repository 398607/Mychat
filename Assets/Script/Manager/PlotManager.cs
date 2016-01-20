using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class PlotManager
{
	// about control
	private int _nextLineIndex;
	private float _time;
	private PlotUnitList _plotUnitList;

	// about itself
	public string PlotName;

	public PlotManager(string plotName = "test")
	{
		PlotName = plotName;
		_plotUnitList = new PlotUnitList();
		LoadUnitList();
		_time = 0f;
		_nextLineIndex = 0;
	}

	public void ResetPlotName(string newName)
	{
		PlotName = newName;
		LoadUnitList();
	}

	public void LoadUnitList()
	{
		var fileName = @"PlotFile\" + PlotName + ".txt";

		if (_plotUnitList == null)
		{
			_plotUnitList = new PlotUnitList();
		}
		_plotUnitList.Clear();

		if (!File.Exists(fileName))
		{
			return;
		}

		var testFile = File.ReadAllLines(fileName);

		foreach (var line in testFile)
		{
			// Debug.Log(line);

			var tmpPlotUnit = new PlotUnit();
			tmpPlotUnit.Build(line.Replace("\t", ""));

			if (tmpPlotUnit.Delay < 0f)
				continue;
			_plotUnitList.Add(tmpPlotUnit);
		}
	}

	// handle next plot unit
	public void SendLine()
	{
		var nextUnit = _plotUnitList[_nextLineIndex];
		if (GameManager.FitAsked(nextUnit.AskedVar, nextUnit.AskedValue))
		{
			_time = 0f;
			nextUnit.Line.React(PlotName);
			_nextLineIndex++;
		}
		else
		{
			_nextLineIndex++;
		}
	}

	// Update is called by GameManager instance
	public void Update()
	{
		if (_nextLineIndex >= _plotUnitList.Count)
		{
			return;
		}
		_time += Time.deltaTime;

		// handle next unit when time is ready
		if (_time > _plotUnitList[_nextLineIndex].Delay)
		{
			SendLine();
		}
	}
}