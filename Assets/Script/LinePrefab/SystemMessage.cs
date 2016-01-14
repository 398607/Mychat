using UnityEngine;
using UnityEngine.UI;

public class SystemMessage : MonoBehaviour
{
	public void SetText(string text)
	{
		GetComponentInChildren<Text>().text = text;
	}

	// Use this for initialization
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
	}
}