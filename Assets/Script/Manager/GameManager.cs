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
	private Text textBoard;
	private Text typeBoard;

	// state
	public State state;

	// choice global
	private int[] global_varibles = new int[100];

	// choosing
	private int now_affindex;
	private int now_choice;
	private int now_choice_max;
	private ChoiceLine nowChoiceLine;

	private static Text TextBoard() {
		return _instance.textBoard;
	}

	private static Text TypeBoard() {
		return _instance.typeBoard;
	}

	public static void GetNewSpeech(SpeechLine speechLine) {
		TextBoard().text += speechLine._person + " : " + speechLine._content + "\n";
	}

	public static void GetNewChoice(ChoiceLine choiceLine) {
		_instance.state = State.WaitForChoice;

		_instance.nowChoiceLine = choiceLine;
		_instance.now_affindex = choiceLine._affindex;
		_instance.now_choice_max = choiceLine._list.Count;
		_instance.now_choice = 0;

		ShowChoiceBoard();
	}

	public static void ClearDialog() {
		TextBoard().text = "";
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
		_instance.textBoard = GameObject.Find("TextBoard").GetComponent<Text>();
		_instance.typeBoard = GameObject.Find("TypeBoard").GetComponent<Text>();
		ClearDialog();
		ClearTypeBoard();
		_instance.state = State.Flowing;
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
