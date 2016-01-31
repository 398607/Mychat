using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageManager
{
	private Dictionary<string, Image> _dict;

	public string[] NameList = {"You", "jf", "bt"};

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
	// skeleton
	public static GameManager Instance;

	// variables

	// plot manager and plot choose
	private readonly Dictionary<string, PlotDisplay> _plotDisplays = new Dictionary<string, PlotDisplay>();
	private PlotChoose _plotChoose;

	// view panel manager
	private readonly Dictionary<string, ViewPanel> _viewPanels = new Dictionary<string, ViewPanel>();
	private string _nowFocusPanel;

	// plot name board
	private Text _plotNameBoard;

	// global varibles
	private readonly int[] _globalVaribles = new int[100];

	// Image usage
	private ImageManager _imageManager;

	// choosing and choiceBoardTransform for plotdisplay
	private Button _choiceButton;
	public Transform ChoiceBoardTransform;

	// scrollTransform for plotdisplay
	public Transform ScrollTransorm;
	// auto scroll speed
	private readonly float _scrollVelocity = 1000f;
	
	// line perfab usage
	private Speech _speechIns;
	private SystemMessage _sysMessageIns;
	private YouLine _youLineIns;
	
	// (re)set plot name
	private static void SetPlotName(string newName)
	{
		Instance._plotNameBoard.text = newName;
	}

	// functions

	// set focus
	public static void SetFocusPlot(string plotName)
	{
		// focus off previous plot
		if (plotName != Instance._nowFocusPanel && Instance._nowFocusPanel != null)
			Instance._viewPanels[Instance._nowFocusPanel].FocusOff();

		// move to target pos
		Instance._viewPanels[plotName].ScrollTransorm.transform.Translate(0, Instance._viewPanels[plotName].TargetPositionY - Instance._viewPanels[plotName].ScrollTransorm.position.y, 0);

		// focus on
		Instance._viewPanels[plotName].FocusOn();

		// let gamemanager know
		Instance._nowFocusPanel = plotName;
		SetPlotName(plotName);

	}

	// handle new SpeechLine
	public static void GetNewSpeech(SpeechLine speechLine, string plotName)
	{
		// build line prefab 
		LinePrefab lp;
		if (speechLine._person == "You")
		{
			lp = Instantiate(Instance._youLineIns);
			lp.transform.position = new Vector3(Instance._plotDisplays[plotName].ScrollTransorm.position.x + 320,
				Instance._plotDisplays[plotName].ScrollTransorm.position.y - Instance._plotDisplays[plotName].NextLinePositionY);
		}
		else // npc talking
		{
			lp = Instantiate(Instance._speechIns);
			lp.transform.position = new Vector3(Instance._plotDisplays[plotName].ScrollTransorm.position.x + 25,
				Instance._plotDisplays[plotName].ScrollTransorm.position.y - Instance._plotDisplays[plotName].NextLinePositionY);
		}

		// Debug.Log(speechLine._content + lp.transform.position.ToString());
		// set parent to scroll content, set text
		lp.transform.SetParent(Instance._plotDisplays[plotName].ScrollTransorm);
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
		Instance._plotDisplays[plotName].NextLinePositionY += 60 + 33 * (deltaY - 1);

		// show speaker image
		var ins = Instance._imageManager.GetImageByName(speechLine._person);
		if (ins != null)
		{
			var ime = Instantiate(ins);
			ime.transform.SetParent(lp.transform);
			ime.transform.localPosition = new Vector3(0, 0, 0);
		}

		// when hidden, scroll to show (magic but works)
		if (lp.transform.position.y - 21 * deltaY - 12 < 125)
		{
			// _instance._scrollTransorm.Translate(new Vector3(0, 150 - lp.transform.position.y, 0));
			Instance._plotDisplays[plotName].TargetPositionY = Instance._plotDisplays[plotName].ScrollTransorm.position.y + 125 - lp.transform.position.y + 21 * deltaY - 40;
		}
	}

	// handle new SystemLine
	public static void GetNewSystemMessage(SystemLine sysLine, string plotName)
	{
		var smsg = Instantiate(Instance._sysMessageIns);
		smsg.transform.SetParent(Instance._plotDisplays[plotName].ScrollTransorm);
		smsg.SetText(sysLine._content);

		smsg.transform.position = new Vector3(Instance._plotDisplays[plotName].ScrollTransorm.position.x + 140,
			Instance._plotDisplays[plotName].ScrollTransorm.position.y - Instance._plotDisplays[plotName].NextLinePositionY);

		// when hidden, scroll to show (magic but works)
		if (smsg.transform.position.y < 150)
		{
			Instance._plotDisplays[plotName].TargetPositionY = Instance._plotDisplays[plotName].ScrollTransorm.position.y + 150 - smsg.transform.position.y - 40;
		}

		Instance._plotDisplays[plotName].NextLinePositionY += 40;
	}

	// handle new ChoiceLine
	public static void GetNewChoice(ChoiceLine choiceLine, string plotName)
	{
		Instance._plotDisplays[plotName].PlotState = PlotDisplay.State.WaitForChoice;

		Instance._plotDisplays[plotName].NowChoiceLine = choiceLine;
		Instance._plotDisplays[plotName].NowAffIndex = choiceLine._affindex;

		for (var i = 0; i < choiceLine._list.Count; i++)
		{
			var btn = Instantiate(Instance._choiceButton);
			btn.transform.SetParent(Instance._plotDisplays[plotName].ChoiceBoardTransform);
			btn.name = i.ToString();

			btn.transform.localPosition = new Vector3(0, -10);
			btn.transform.Translate(new Vector2(0, -i * 30));

			btn.GetComponentInChildren<Text>().text = choiceLine._list[i].Content;
			btn.onClick.AddListener(delegate() { GetUserChoice(btn);});
		}
	}

	// handle new WaitLine
	public static void GetNewWait(WaitLine waitLine, string plotName)
	{
		Instance._plotDisplays[plotName].GetWait(waitLine.WaitIndex, waitLine.WaitValue);
	}

	// get user choice
	public static void GetUserChoice(Button btnObj = null)
	{
		// avoid null
		if (btnObj == null)
			return;

		// adjust varible
		SetGlobalVarible(Instance._plotDisplays[Instance._nowFocusPanel].NowAffIndex, Convert.ToInt32(btnObj.name));

		// show choice 
		GetNewSpeech(new SpeechLine("You", Instance._plotDisplays[Instance._nowFocusPanel].NowChoiceLine._list[Convert.ToInt32(btnObj.name)].Content), Instance._nowFocusPanel);

		// clear choice board by killing all buttons on it
		Button[] btnObjects =
			Instance._plotDisplays[Instance._nowFocusPanel].ChoiceBoardTransform.GetComponentsInChildren<Button>();
		
		foreach (var btn in btnObjects)
		{
			Destroy(btn.gameObject);
		}

		// back to flow
		Instance._plotDisplays[Instance._nowFocusPanel].PlotState = PlotDisplay.State.Flowing;
	}

	// set global varible
	public static void SetGlobalVarible(int index, int value)
	{
		Instance._globalVaribles[index] = value;
	}

	// if the varible asked is OK
	public static bool FitAsked(int askVar, int askVal)
	{
		return Instance._globalVaribles[askVar] == askVal;
	}
	
	// showboard auto scroll (with update)
	private void AutoScroll()
	{
		foreach (var plot in _plotDisplays.Values)
		{
			var delta = plot.TargetPositionY - plot.ScrollTransorm.position.y;
			if (Mathf.Abs(delta) > 0)
			{
				plot.ScrollTransorm.Translate(new Vector3(0, Mathf.Min(_scrollVelocity * Time.deltaTime, delta), 0));
			}
		}
	}

	// init
	private void Init()
	{
		// plot name board
		_plotNameBoard = GameObject.Find("PlotNameBoard").GetComponent<Text>();
		
		// instance of line prefab
		_speechIns = FindObjectOfType<Speech>();
		_youLineIns = FindObjectOfType<YouLine>();
		_sysMessageIns = FindObjectOfType<SystemMessage>();

		// instance of choice button
		_choiceButton = GameObject.Find("ChoiceButton").GetComponent<Button>();
		ChoiceBoardTransform = GameObject.Find("ChoiceBoard").transform;

		// get scroll
		ScrollTransorm = GameObject.Find("Content").transform;

		// speaker image manager
		_imageManager = new ImageManager();
		_imageManager.LoadImage();

		// plot choose panel
		_plotChoose = new PlotChoose();
		_viewPanels.Add("Home", _plotChoose);

		// home button
		var homeBtn = GameObject.Find("Home").GetComponent<Button>();
		homeBtn.onClick.AddListener(delegate() {SetFocusPlot("Home"); });
		
		// load plots
		LoadPlots();
		
		// set focus to home (plot choose)
		_nowFocusPanel = "Home"; // avoid bug when SetFocusPlot() is first called
		SetFocusPlot("Home");
	}

	// find plot list and load all plots written
	private void LoadPlots()
	{
		var fileName = @"PlotFile\plot_name_info.ini";

		if (!File.Exists(fileName))
			return;

		var plotNames = File.ReadAllLines(fileName);

		foreach (var plotName in plotNames)
		{
			if (plotName.Length > 0)
			{
				LoadPlot(plotName);
			}
		}
	}

	// load a plot
	private void LoadPlot(string plotName)
	{
		var plotDisplay = new PlotDisplay(plotName);
		_plotDisplays.Add(plotName, plotDisplay);
		_viewPanels.Add(plotName, plotDisplay);
		_plotChoose.GetPlot(plotName);
	}

	// Use this for initialization
	private void Start()
	{
		// singleton
		if (Instance == null)
		{
			Instance = this;
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

		// plots update
		foreach (var plot in _plotDisplays.Values)
		{
			plot.Update();
		}
		
	}
}