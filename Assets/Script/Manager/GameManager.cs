using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	[HideInInspector]
	private Speech speech_ins;
	private SystemMessage sysMessage_ins;
	int system_count;
	int speech_count;

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

		Speech spe = Instantiate(_instance.speech_ins);
		spe.transform.SetParent(_instance.scrollTransorm);
		// spe.transform.SetAsLastSibling();
		spe.SetText(speechLine._content);
		
		spe.transform.position = new Vector3(_instance.scrollTransorm.position.x + 40, _instance.scrollTransorm.position.y - 40 - 60 * _instance.speech_count - 40 * _instance.system_count);
			
		_instance.speech_count++;
	}

	public static void GetNewSystemMessage(SystemLine sysLine) {
		SystemMessage smsg = Instantiate(_instance.sysMessage_ins);
		smsg.transform.SetParent(_instance.scrollTransorm);
		smsg.SetText(sysLine._content);

		smsg.transform.position = new Vector3(_instance.scrollTransorm.position.x + 240, _instance.scrollTransorm.position.y - 40 - 60 * _instance.speech_count - 40 * _instance.system_count);

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

		speech_ins = GameObject.FindObjectOfType<Speech>();
		sysMessage_ins = FindObjectOfType<SystemMessage>();
		scrollTransorm = GameObject.Find("Content").transform;
		system_count = 0;
		speech_count = 0;
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
