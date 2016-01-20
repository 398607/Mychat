using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	private List<PlotManager> _plotManagers = new List<PlotManager>();
	private int _nowFocusPlot;

	// plot name board
	private Text _plotNameBoard;

	// choice global
	private readonly int[] _globalVaribles = new int[100];

	// Image usage
	private ImageManager _imageManager;

	// choosing
	private Button _choiceButton;
	private Transform _choiceBoardTransform;
	private int _nowAffIndex;
	private ChoiceLine _nowChoiceLine;

	// show area
	private Transform _scrollTransorm;
	private float _nextLinePositionY;
	private float _targetPositionY;
	private float _scrollVelocity;

	// speech usage
	private Speech _speechIns;

	// state
	public State GameState;

	private SystemMessage _sysMessageIns;
	private YouLine _youLineIns;

	// get the statci plotnameboard
	private static Text PlotNameBoard()
	{
		return _instance._plotNameBoard;
	}

	// (re)set plot name
	private static void SetPlotName()
	{
		PlotNameBoard().text = _instance._plotManagers[_instance._nowFocusPlot].PlotName;
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
				_instance._scrollTransorm.position.y - 40 - _instance._nextLinePositionY);
		}
		else // npc talking
		{
			lp = Instantiate(_instance._speechIns);
			lp.transform.position = new Vector3(_instance._scrollTransorm.position.x + 25,
				_instance._scrollTransorm.position.y - 40 - _instance._nextLinePositionY);
		}

		
		// set parent to scroll content, set text
		lp.transform.SetParent(_instance._scrollTransorm);
		lp.SetText(speechLine._content);
		
		var deltaY = 1f;
		var deltaX = 1f;
		// vertical expand when content is too long
		if (speechLine._content.Length > 11)
		{
			var vec3 = lp.GetComponentInChildren<Button>().transform.localScale;
			deltaY = speechLine._content.Length / 11 + 1;
			lp.GetComponentInChildren<Button>().transform.localScale = new Vector3(vec3.x, vec3.y * deltaY, vec3.z);
			vec3 = lp.GetComponentInChildren<Text>().transform.localScale;
			lp.GetComponentInChildren<Text>().transform.localScale = new Vector3(vec3.x, vec3.y / deltaY, vec3.z);
		}
		// horizontal shrink when content is too short
		else if (speechLine._content.Length < 11)
		{
			var vec3 = lp.GetComponentInChildren<Button>().transform.localScale;
			deltaX = (speechLine._content.Length + 0.8f) / 11f;
			lp.GetComponentInChildren<Button>().transform.localScale = new Vector3(vec3.x * deltaX, vec3.y , vec3.z);
			vec3 = lp.GetComponentInChildren<Text>().transform.localScale;
			lp.GetComponentInChildren<Text>().transform.localScale = new Vector3(vec3.x / deltaX, vec3.y, vec3.z);
		}

		// update next line position
		_instance._nextLinePositionY += 60 + 33 * (deltaY - 1);

		// show speaker image
		var ins = _instance._imageManager.GetImageByName(speechLine._person);
		if (ins != null)
		{
			var ime = Instantiate(ins);
			ime.transform.SetParent(lp.transform);
			ime.transform.localPosition = new Vector3(0, 0, 0);
		}

		// when hidden, scroll to show
		if (lp.transform.position.y - 21 * deltaY - 12 < 125)
		{
			// _instance._scrollTransorm.Translate(new Vector3(0, 150 - lp.transform.position.y, 0));
			_instance._targetPositionY = _instance._scrollTransorm.position.y + 125 - lp.transform.position.y + 21 * deltaY + 12;
		}
	}

	// handle new SystemLine
	public static void GetNewSystemMessage(SystemLine sysLine)
	{
		var smsg = Instantiate(_instance._sysMessageIns);
		smsg.transform.SetParent(_instance._scrollTransorm);
		smsg.SetText(sysLine._content);

		smsg.transform.position = new Vector3(_instance._scrollTransorm.position.x + 350f*Screen.width/1024,
			_instance._scrollTransorm.position.y - 40 - _instance._nextLinePositionY);

		if (smsg.transform.position.y < 160)
		{
			_instance._scrollTransorm.Translate(new Vector3(0, 50, 0));
		}

		_instance._nextLinePositionY += 40;
		
	}

	// handle new ChoiceLine
	public static void GetNewChoice(ChoiceLine choiceLine)
	{
		_instance.GameState = State.WaitForChoice;

		_instance._nowChoiceLine = choiceLine;
		_instance._nowAffIndex = choiceLine._affindex;

		for (var i = 0; i < choiceLine._list.Count; i++)
		{
			var btn = Instantiate(_instance._choiceButton);
			btn.transform.SetParent(_instance._choiceBoardTransform);
			btn.name = i.ToString();

			btn.transform.localPosition = new Vector3(0, -10);
			btn.transform.Translate(new Vector2(0, -i * 30));

			btn.GetComponentInChildren<Text>().text = choiceLine._list[i].Content;
			btn.onClick.AddListener(delegate() { GetUserChoice(btn);});
		}
	}

	// get user choice
	public static void GetUserChoice(Button btnObj = null)
	{
		// adjust varible
		SetGlobalVarible(_instance._nowAffIndex, Convert.ToInt32(btnObj.name));

		// show choice 
		GetNewSpeech(new SpeechLine("You", _instance._nowChoiceLine._list[Convert.ToInt32(btnObj.name)].Content));

		// un-pause time flow
		_instance.GameState = State.Flowing;
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

	private void AutoScroll()
	{
		if (_instance._scrollTransorm.position.y < _targetPositionY)
		{
			_instance._scrollTransorm.Translate(new Vector3(0, Mathf.Min(_scrollVelocity * Time.deltaTime, _targetPositionY - _instance._scrollTransorm.position.y), 0));
		}
	}

	private void Init()
	{
		// plot name board
		_plotNameBoard = GameObject.Find("PlotNameBoard").GetComponent<Text>();

		// state
		GameState = State.Flowing;

		// instance of line prefab
		_speechIns = FindObjectOfType<Speech>();
		_youLineIns = FindObjectOfType<YouLine>();
		_sysMessageIns = FindObjectOfType<SystemMessage>();

		// instance of choice button
		_choiceButton = GameObject.Find("ChoiceButton").GetComponent<Button>();
		_choiceBoardTransform = GameObject.Find("ChoiceBoard").transform;

		// get scroll
		_scrollTransorm = GameObject.Find("Content").transform;
		_targetPositionY = -1f;
		_scrollVelocity = 100f;
		_nextLinePositionY = 0;

		// speaker image manager
		_imageManager = new ImageManager();
		_imageManager.LoadImage();

		// plot manager(s)
		// TODO: more plot managers
		_plotManagers.Add(new PlotManager("test"));
		_nowFocusPlot = 0;
		SetPlotName();
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
		// Auto scroll
		AutoScroll();
		
		// TODO: plotmanagers update/controll seperately
		switch (GetState())
		{
			// Flowing: plot update
			case State.Flowing:
			{
				foreach (var plotManager in _plotManagers)
				{
					plotManager.Update();
				}
				break;
			}
			// WaitForChoice: handle user action
			case State.WaitForChoice:
			{
				break;
			}
			default:
			{
				break;
			}
		}
	}
}