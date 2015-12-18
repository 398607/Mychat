using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlotUnit {
	public float _delay;
	public Line _line;

	public int _asked_var;
	public int _asked_value;

	public PlotUnit(float delay, Line line, int asked_var = 0, int asked_value = 0) {
		_delay = delay;
		_line = line;
		_asked_value = asked_value;
		_asked_var = asked_var;
	}
}

public class UnitList : List<PlotUnit> { }

public class PlotManager : MonoBehaviour {

	private float time;
	private UnitList unitList;
	private int nextLine_index;

	public void LoadLineList() {
		unitList = new UnitList { new PlotUnit(0.2f, new SpeechLine("npc1", "你……就是我的Master么？")),
								  new PlotUnit(1.0f, new ChoiceLine(1, new ChoiceList {new Choice("在下正是你的Master。"), new Choice("你要找的人，并不是我。"), new Choice("和我没关系，我是来买薯片的。")})),
								  new PlotUnit(0.7f, new SpeechLine("npc2", "我的剑即是你的剑，我将为您赢得圣杯。"), 1, 0),
								  new PlotUnit(1.0f, new SpeechLine("npc2", "……也许我错误地降临于此，但即使这样我也不得不战斗。"), 1, 1),
								  new PlotUnit(0.4f, new SpeechLine("npc3", "………………"), 1, 2),
								  new PlotUnit(1.5f, new SystemLine("你被砍死了。"), 1, 2) };
	}

	public GameManager.State GetGMState() {
		return GameManager.GetState();
	}

	public void SendLine() {
		PlotUnit nextUnit = unitList[nextLine_index];
        if (GameManager.FitAsked(nextUnit._asked_var, nextUnit._asked_value)) {
			time = 0f;
			nextUnit._line.React();
			nextLine_index++;
		}
		else {
			nextLine_index++;
		}
	}

	// Use this for initialization
	void Start () {
		time = 0f;
		nextLine_index = 0;
		LoadLineList();
	}
	
	// Update is called once per frame
	void Update () {
		if (GetGMState() == GameManager.State.Flowing && nextLine_index < unitList.Count) {
			time += Time.deltaTime;
			if (time > unitList[nextLine_index]._delay) {
				SendLine();
			}
		}
	}
}
