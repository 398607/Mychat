using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Choice {
	public string _content;
	public Choice(string content = "Default Choice") {
		_content = content;
	}
}

public class ChoiceList: List<Choice> { }

public class ChoiceLine : Line {

	public int _affindex;
	public ChoiceList _list;

	public ChoiceLine(int affindex, ChoiceList list) {
		_affindex = affindex;
		_list = list;
	}

	public override bool React() {
		GameManager.GetNewChoice(this);
		return true;
	}
}
