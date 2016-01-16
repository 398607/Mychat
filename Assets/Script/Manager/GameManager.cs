using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager
{
	private Dictionary<string, Image> _dict;

	public string[] NameList = {"You", "npc1", "npc2", "npc3"};

	public ImageManager()
	{
		_dict = new Dictionary<string, Image>();
	}

	public void LoadImage()
	{
		foreach (var name in NameList)
		{
			var tempIns = GameObject.Find(name).GetComponent<Image>();
			_dict.Add(name, tempIns);
		}
	}

	public Image GetImageByName(string name)
	{
		if (_dict.ContainsKey(name))
		{
			return _dict[name];
		}
		return null;
	}
}

public class GameManager : MonoBehaviour
{
	public enum State
	{
		Flowing,
		WaitForChoice
	}

	// skeleton
	private static GameManager _instance;

	// plot manager
	private PlotManager _plotManager;

	// choice global
	private readonly int[] _globalVaribles = new int[100];

	// Image usage
	private ImageManager _imageManager;

	// choosing
	private int _nowAffIndex;
	private int _nowChoice;
	private int _nowChoiceMax;
	private ChoiceLine _nowChoiceLine;

	// show area
	private Transform _scrollTransorm;
	private int _speechCount;

	// speech usage
	private Speech _speechIns;

	// state
	public State GameState;
	private SystemMessage _sysMessageIns;
	private int _systemCount;
	private Text _typeBoard;
	private YouLine _youLineIns;

	// get the static typeBoard
	private static Text TypeBoard()
	{
		return _instance._typeBoard;
	}

	// handle new SpeechLine
	public static void GetNewSpeech(SpeechLine speechLine)
	{
		// build line prefab 
		LinePrefab lp;
		if (speechLine._person == "You")
		{
			lp = Instantiate(_instance._youLineIns);
			lp.transform.position = new Vector3(_instance._scrollTransorm.position.x + Screen.width*540f/600,
				_instance._scrollTransorm.position.y - 40 - 60*_instance._speechCount - 40*_instance._systemCount);
		}
		else // npc talking
		{
			lp = Instantiate(_instance._speechIns);
			lp.transform.position = new Vector3(_instance._scrollTransorm.position.x + 25,
				_instance._scrollTransorm.position.y - 40 - 60*_instance._speechCount - 40*_instance._systemCount);
		}

		// set parent to scroll content, set text
		lp.transform.SetParent(_instance._scrollTransorm);
		lp.SetText(speechLine._content);
		
		Debug.Log(speechLine._content.Length.ToString());

		var delta = 1f;
		// vertical expand when content is too long
		if (speechLine._content.Length > 12)
		{
			var vec3 = lp.GetComponentInChildren<Button>().transform.localScale;
			delta = speechLine._content.Length / 12f + 1;
			lp.GetComponentInChildren<Button>().transform.localScale = new Vector3(vec3.x, vec3.y * delta, vec3.z);
			vec3 = lp.GetComponentInChildren<Text>().transform.localScale;
			lp.GetComponentInChildren<Text>().transform.localScale = new Vector3(vec3.x, vec3.y / delta, vec3.z);
		}
		// horizontal shrink when content is too short
		else if (speechLine._content.Length < 12)
		{
			var vec3 = lp.GetComponentInChildren<Button>().transform.localScale;
			delta = (speechLine._content.Length + 0.8f) / 12f;
			lp.GetComponentInChildren<Button>().transform.localScale = new Vector3(vec3.x * delta, vec3.y , vec3.z);
			vec3 = lp.GetComponentInChildren<Text>().transform.localScale;
			lp.GetComponentInChildren<Text>().transform.localScale = new Vector3(vec3.x / delta, vec3.y, vec3.z);
		}

		// show speaker image
		var ins = _instance._imageManager.GetImageByName(speechLine._person);
		if (ins != null)
		{
			var ime = Instantiate(ins);
			ime.transform.SetParent(lp.transform);
			ime.transform.localPosition = new Vector3(0, 0, 0);
		}

		// when hidden, scroll to show
		if (lp.transform.position.y < 160)
		{
			_instance._scrollTransorm.Translate(new Vector3(0, 70 * delta, 0));
		}

		// update count
		_instance._speechCount++;
	}

	// handle new SystemLine
	public static void GetNewSystemMessage(SystemLine sysLine)
	{
		var smsg = Instantiate(_instance._sysMessageIns);
		smsg.transform.SetParent(_instance._scrollTransorm);
		smsg.SetText(sysLine._content);

		smsg.transform.position = new Vector3(_instance._scrollTransorm.position.x + 350f*Screen.width/1024,
			_instance._scrollTransorm.position.y - 40 - 60*_instance._speechCount - 40*_instance._systemCount);

		if (smsg.transform.position.y < 160)
		{
			_instance._scrollTransorm.Translate(new Vector3(0, 50, 0));
		}

		_instance._systemCount++;
	}

	// handle new ChoiceLine
	public static void GetNewChoice(ChoiceLine choiceLine)
	{
		_instance.GameState = State.WaitForChoice;

		_instance._nowChoiceLine = choiceLine;
		_instance._nowAffIndex = choiceLine._affindex;
		_instance._nowChoiceMax = choiceLine._list.Count;
		_instance._nowChoice = 0;

		ShowChoiceBoard();
	}

	// clear TypeBoard
	private static void ClearTypeBoard()
	{
		TypeBoard().text = "";
	}

	// flush on TypeBoard
	private static void ShowChoiceBoard()
	{
		TypeBoard().text = "";
		for (var i = 0; i < _instance._nowChoiceMax; i++)
		{
			TypeBoard().text += _instance._nowChoiceLine._list[i].Content;

			// symbol of now_choice
			if (_instance._nowChoice == i)
			{
				TypeBoard().text += "  ←";
			}
			TypeBoard().text += "\n";
		}
	}

	// get user choice
	private static void GetUserChoice()
	{
		// adjust varible
		SetGlobalVarible(_instance._nowAffIndex, _instance._nowChoice);

		// show choice 
		GetNewSpeech(new SpeechLine("You", _instance._nowChoiceLine._list[_instance._nowChoice].Content));

		// un-pause time flow
		_instance.GameState = State.Flowing;

		// clear typeboard
		ClearTypeBoard();
	}

	public static void SetGlobalVarible(int index, int value)
	{
		_instance._globalVaribles[index] = value;
	}

	// if the varible asked is OK
	public static bool FitAsked(int askVar, int askVal)
	{
		return _instance._globalVaribles[askVar] == askVal;
	}

	// return current state of game
	public static State GetState()
	{
		return _instance.GameState;
	}

	private void Init()
	{
		// typeboard
		_typeBoard = GameObject.Find("TypeBoard").GetComponent<Text>();
		ClearTypeBoard();

		// state
		GameState = State.Flowing;

		// instance of line prefab
		_speechIns = FindObjectOfType<Speech>();
		_youLineIns = FindObjectOfType<YouLine>();
		_sysMessageIns = FindObjectOfType<SystemMessage>();

		// get scroll
		_scrollTransorm = GameObject.Find("Content").transform;
		_systemCount = 0;
		_speechCount = 0;

		// speaker image manager
		_imageManager = new ImageManager();
		_imageManager.LoadImage();

		// plot manager(s)
		// TODO: more plot managers
		_plotManager = new PlotManager("test");

	}

	// Use this for initialization
	private void Start()
	{
		// singleton
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(this);
		}

		Init();
	}

	// Update is called once per frame
	private void Update()
	{
		// Flowing: plot update
		// TODO: plotmanagers update/controll seperately
		switch (GetState())
		{
			case State.Flowing:
			{
				_plotManager.Update();
				break;
			}
			case State.WaitForChoice:
			{
				// choice prev / next - adjust choice board
				if (Input.GetKeyDown(KeyCode.DownArrow) && _nowChoice < _nowChoiceMax - 1)
				{
					_nowChoice++;
					ShowChoiceBoard();
				}
				if (Input.GetKeyDown(KeyCode.UpArrow) && _nowChoice > 0)
				{
					_nowChoice--;
					ShowChoiceBoard();
				}

				// chosen!
				if (Input.GetKey(KeyCode.Return))
				{
					GetUserChoice();
				}
				break;
			}
			default:
			{
				break;
			}
		}
	}
}