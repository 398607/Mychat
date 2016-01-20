using System;
using System.Collections.Generic;

public class Choice
{
	public string Content;

	public Choice(string content = "Default Choice")
	{
		Content = content;
	}
}

public class ChoiceList : List<Choice>
{
}

public class ChoiceLine : Line
{
	public int _affindex;
	public ChoiceList _list;

	public ChoiceLine(int affindex = 0)
	{
		_affindex = affindex;
		_list = new ChoiceList();
	}

	public ChoiceLine(int affindex, ChoiceList list)
	{
		_affindex = affindex;
		_list = list;
	}

	public override void React(string plotName)
	{
		GameManager.GetNewChoice(this);
	}

	public override void Build(string[] lineEle)
	{
		// Ch <timeDelay> <aff_index> <choice_0> [<choice_1> ... <choice_n>] [<askVar> <askValue>]
		_affindex = Convert.ToInt32(lineEle[2]);
		_list = new ChoiceList();
		var i = 3;
		for (; i < lineEle.Length; i++)
		{
			if ("0123456789".Contains(lineEle[i].Substring(0, 1)))
				break;
			_list.Add(new Choice(lineEle[i]));
		}
	}
}