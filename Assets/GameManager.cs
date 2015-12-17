using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// skeleton
	private static GameManager _instance = null;

	private Text textBoard;

	private static Text TextBoard() {
		return _instance.textBoard;
	}
	
	public static void GetNewDialog(string name, string content) {
		TextBoard().text += name + ": " + content + "\n";
	}

	public static void ClearDialog() {
		TextBoard().text = "";
	}

	void Init() {
		_instance.textBoard = GameObject.Find("TextBoard").GetComponent<Text>();
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

		ClearDialog();
		GetNewDialog("nagi", "it works!");
		GetNewDialog("GameManager", "Yes it does.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
