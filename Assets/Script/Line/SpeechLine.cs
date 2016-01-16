using System;

public class SpeechLine : Line
{
	public string _content;
	public string _person;

	public SpeechLine(string person = "???", string content = "***")
	{
		_person = person;
		_content = content;
	}

	public override void React()
	{
		GameManager.GetNewSpeech(this);
	}

	public override void Build(string[] lineEle)
	{
		// Sp <timeDelay> <person> <content> [<askVar> <askValue>]
		_person = lineEle[2];
		_content = lineEle[3];
	}
}