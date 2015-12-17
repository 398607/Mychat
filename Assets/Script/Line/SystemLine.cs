using UnityEngine;
using System.Collections;

public class SystemLine : Line {

	public string _content;

	public SystemLine(string content) {
		_content = content;
	}

	public override bool React() {
		GameManager.GetNewSystemMessage(this);
		return true;
	}
}
