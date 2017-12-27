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

	public void Restart()
	{
		GameManager.Instance.Restart();
	}

	public override void SetVisible (bool value)
	{
		switch(GameManager.Instance.gameOverReason)
		{
		case GameOverReason.PLAYER_WON:
			reasonLabel.text = "Player Won!";
			break;
		case GameOverReason.ANIMAL_STARVED:
			reasonLabel.text = "Animal Starved";
			break;
		case GameOverReason.HUNTER_WON:
			reasonLabel.text = "Animal Killed";
			break;
		}

		base.SetVisible (value);
	}
}
