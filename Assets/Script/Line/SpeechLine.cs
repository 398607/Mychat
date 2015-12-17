using UnityEngine;
using System.Collections;

public class SpeechLine : Line {
	public string _person;
	public string _content;
	public SpeechLine(string person = "???", string content = "***") {
		_person = person;
		_content = content;
	}
	public override bool React() {
		GameManager.GetNewSpeech(this);
		return true;
	}
}
