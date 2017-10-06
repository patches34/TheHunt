using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : UIMenu
{
	public Text reasonLabel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void SetVisible (bool value)
	{
		switch(GameManager.Instance.GameOverState)
		{
		case TurnActor.Player:
			reasonLabel.text = "Player Won!";
			break;
		case TurnActor.Animal:
			reasonLabel.text = "Animal Starved";
			break;
		case TurnActor.Hunter:
			reasonLabel.text = "Animal Killed";
			break;
		}

		base.SetVisible (value);
	}

	/*public void SetVisible(bool value)
	{
		Debug.Log("Game Over");
		switch(GameManager.Instance.GameOverState)
		{
		case TurnActor.Player:
			reasonLabel.text = "Player Won!";
			break;
		case TurnActor.Animal:
			reasonLabel.text = "Animal Starved";
			break;
		case TurnActor.Hunter:
			reasonLabel.text = "Animal Killed";
			break;
		}

		base.SetVisible(value);
	}*/

	public void Dismiss()
	{
		SetVisible(false);
	}
}
