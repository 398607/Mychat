using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlotChoose : ViewPanel
{
	private readonly Button _plotChooseButton;

	private int _plotNum;

	public PlotChoose()
	{
		_plotChooseButton = GameObject.Find("ChoiceButton").GetComponent<Button>();
		_plotNum = 0;
	}

	public void GetPlot(string plotName)
	{
		var newBtn = Instantiate(_plotChooseButton);
		newBtn.transform.SetParent(ScrollTransorm);
		newBtn.transform.localPosition = new Vector3(0, -20-30*_plotNum, 0);
		newBtn.GetComponentInChildren<Text>().text = plotName;
		newBtn.onClick.AddListener(delegate() { GameManager.SetFocusPlot(plotName);});
		_plotNum++;
	}
}