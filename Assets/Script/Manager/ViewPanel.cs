using UnityEngine;
using System.Collections;

public class ViewPanel : UnityEngine.Object
{
	public Transform ChoiceBoardTransform;
	public Transform ScrollTransorm;

	public ViewPanel(string name = "test")
	{
		ScrollTransorm = Instantiate(GameManager.Instance.ScrollTransorm);
		ScrollTransorm.SetParent(GameObject.Find("GameManager").transform);

		ChoiceBoardTransform = Instantiate(GameManager.Instance.ChoiceBoardTransform);
		ChoiceBoardTransform.SetParent(GameObject.Find("GameManager").transform);
	}

	// set this panel as focus
	public void FocusOn()
	{
		ScrollTransorm.SetParent(GameObject.Find("Content").transform);
		ScrollTransorm.localPosition = new Vector3(0, 0, 0);

		ChoiceBoardTransform.SetParent(GameObject.Find("Canvas").transform);
		ChoiceBoardTransform.localPosition = GameManager.Instance.ChoiceBoardTransform.localPosition;
	}

	// set this panel off focus
	public void FocusOff()
	{
		ScrollTransorm.SetParent(GameObject.Find("GameManager").transform);

		ChoiceBoardTransform.SetParent(GameObject.Find("GameManager").transform);
	}
}
