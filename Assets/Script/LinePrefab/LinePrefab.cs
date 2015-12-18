using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LinePrefab : MonoBehaviour {

	public void SetText(string text) {
		GetComponentInChildren<Text>().text = text;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
