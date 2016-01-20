public class SystemLine : Line
{
	public string _content;

	public SystemLine(string content = "***")
	{
		_content = content;
	}

	public override void React(string plotName)
	{
		GameManager.GetNewSystemMessage(this, plotName);
	}

	public override void Build(string[] lineEle)
	{
		// Sys <timeDelay> <content> [<askVar> <askValue>]
		_content = lineEle[2];
	}
}