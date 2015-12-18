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
		unitList = new UnitList { new PlotUnit(0.2f, new SpeechLine("npc1", "嘿呀！竟然有人类出现了！")),
								  new PlotUnit(1f, new SpeechLine("You", "这里是哪里……？")),
								  new PlotUnit(1f, new SpeechLine("npc2", "嘿呀！这里是魔林村哦KI★RA！")),
								  new PlotUnit(0.8f, new SystemLine("怪物弯下身拿起了一把斧头")),
								  new PlotUnit(0.8f, new ChoiceLine(1, new ChoiceList {new Choice("∑你要干什么？！"), new Choice("（转身逃跑）")})),
								  new PlotUnit(0.8f, new SpeechLine("npc3", "嘿呀！难得有人类来，我给你杀头猪做饭吧！"), 1, 0),
								  new PlotUnit(0.8f, new SpeechLine("npc3", "喂！你跑什么嘿呀~！"), 1, 1),
								  new PlotUnit(0.8f, new SystemLine("怪物举着菜刀向你追来"), 1, 1)
								};
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
