using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageManager {

	public string[] nameList = new string[] { "You", "npc1", "npc2", "npc3"};
	
	public Dictionary<string, Image> dict;

	public ImageManager() {
		dict = new Dictionary<string, Image>();
	}

	public void LoadImage() {

		foreach (string name in nameList) {
			Image temp_ins = GameObject.Find(name).GetComponent<Image>();
			dict.Add(name, temp_ins);
		}

	}

	public Image GetImageByName(string name) {
		if (dict.ContainsKey(name)) {
			return dict[name];
		}
		else {
			return null;
		}
	}
}

public class GameManager : MonoBehaviour {

	public enum State : int {
		Flowing,
		WaitForChoice
	}

	// skeleton
	private static GameManager _instance = null;

	// show area
	private Transform scrollTransorm;
	private Text typeBoard;

	// speech usage
	private Speech speech_ins;
	private YouLine youLine_ins;
	private SystemMessage sysMessage_ins;
	int system_count;
	int speech_count;

	// Image usage
	private ImageManager imageManager;

	// state
	public State state;

	// choice global
	private int[] global_varibles = new int[100];

	// choosing
	private int now_affindex;
	private int now_choice;
	private int now_choice_max;
	private ChoiceLine nowChoiceLine;

	private static Text TypeBoard() {
		return _instance.typeBoard;
	}

	public static void GetNewSpeech(SpeechLine speechLine) {

		LinePrefab lp;
		if (speechLine._person == "You") {
			lp = Instantiate(_instance.youLine_ins);
			lp.transform.position = new Vector3(_instance.scrollTransorm.position.x + Screen.width * 660 / 768, _instance.scrollTransorm.position.y - 40 - 60 * _instance.speech_count - 40 * _instance.system_count);
		}
		else {
			lp = Instantiate(_instance.speech_ins);
			lp.transform.position = new Vector3(_instance.scrollTransorm.position.x + 40, _instance.scrollTransorm.position.y - 40 - 60 * _instance.speech_count - 40 * _instance.system_count);
		}

		lp.transform.SetParent(_instance.scrollTransorm);
		lp.SetText(speechLine._content);

		Debug.Log(speechLine._content.Length.ToString());

		if (speechLine._content.Length > 12) { // vertical expand
			Vector3 vec3 = lp.GetComponentInChildren<Button>().transform.localScale;
            lp.GetComponentInChildren<Button>().transform.localScale = new Vector3(vec3.x, vec3.y * (speechLine._content.Length / 12 + 1), vec3.z);
			vec3 = lp.GetComponentInChildren<Text>().transform.localScale;
			lp.GetComponentInChildren<Text>().transform.localScale = new Vector3(vec3.x, vec3.y / (speechLine._content.Length / 12 + 1), vec3.z); 
        }

		Image ins = _instance.imageManager.GetImageByName(speechLine._person);
		if (ins != null) {
			Image ime = Instantiate(ins);
			ime.transform.SetParent(lp.transform);
			ime.transform.localPosition = new Vector3(0, 0, 0);
		}
		
		_instance.speech_count++;
	}

	public static void GetNewSystemMessage(SystemLine sysLine) {
		SystemMessage smsg = Instantiate(_instance.sysMessage_ins);
		smsg.transform.SetParent(_instance.scrollTransorm);
		smsg.SetText(sysLine._content);

		smsg.transform.position = new Vector3(_instance.scrollTransorm.position.x + 340 * Screen.width / 768, _instance.scrollTransorm.position.y - 40 - 60 * _instance.speech_count - 40 * _instance.system_count);

		_instance.system_count++;
	}

	public static void GetNewChoice(ChoiceLine choiceLine) {
		_instance.state = State.WaitForChoice;

		_instance.nowChoiceLine = choiceLine;
		_instance.now_affindex = choiceLine._affindex;
		_instance.now_choice_max = choiceLine._list.Count;
		_instance.now_choice = 0;

		ShowChoiceBoard();
	}

	

	public static void ClearTypeBoard() {
		TypeBoard().text = "";
	}

	public static void ShowChoiceBoard() {
		TypeBoard().text = "";
		for (int i = 0; i < _instance.now_choice_max; i++) {
			TypeBoard().text += _instance.nowChoiceLine._list[i]._content;
			if (_instance.now_choice == i) {
				TypeBoard().text += "  <--";
			}
			TypeBoard().text += "\n";
		}
	}

	public static void GetUserChoice() {
		_instance.global_varibles[_instance.now_affindex] = _instance.now_choice;
		GetNewSpeech(new SpeechLine("You", _instance.nowChoiceLine._list[_instance.now_choice]._content));
		_instance.state = State.Flowing;
		ClearTypeBoard();
	}

	public static bool FitAsked(int ask_var, int ask_val) {
		return _instance.global_varibles[ask_var] == ask_val;
	}

	public static State GetState() {
		return _instance.state;
	}

	void Init() {
		typeBoard = GameObject.Find("TypeBoard").GetComponent<Text>();
		ClearTypeBoard();
		state = State.Flowing;

		speech_ins = FindObjectOfType<Speech>();
		youLine_ins = FindObjectOfType<YouLine>();
		sysMessage_ins = FindObjectOfType<SystemMessage>();
		scrollTransorm = GameObject.Find("Content").transform;
		system_count = 0;
		speech_count = 0;

		imageManager = new ImageManager();
		imageManager.LoadImage();
	}

	// Use this for initialization
	void Start () {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Destroy(this);
		}

		Init();
	}
	
	// Update is called once per frame
	void Update () {
		if (state == State.WaitForChoice) {
			if (Input.GetKeyDown(KeyCode.DownArrow) && now_choice < now_choice_max - 1) {
				now_choice++;
				ShowChoiceBoard();
			}
			if (Input.GetKeyDown(KeyCode.UpArrow) && now_choice > 0) {
				now_choice--;
				ShowChoiceBoard();
			}
			if (Input.GetKey(KeyCode.Return)) {
				GetUserChoice();
			}
		}
	}
}
