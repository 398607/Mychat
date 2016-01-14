public class SpeechLine : Line
{
	public string _content;
	public string _person;

	public SpeechLine(string person = "???", string content = "***")
	{
		_person = person;
		_content = content;
	}

	public override bool React()
	{
		GameManager.GetNewSpeech(this);
		return true;
	}
}