using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : UIMenu
{
	[SerializeField]
	Text overLabel, reasonLabel;

	[SerializeField]
	GameObject restartObj;

	const string k_PLAYER_WON = "Animal Save";
	const string k_ANIMAL_STARVED = "Animal Died";
	const string k_HUNTER_WON = "Animal Killed";
	const string k_WON = "YOU WON!";
	const string k_LOST = "GAME OVER";

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

    public void NewGame()
    {
        GameManager.Instance.NewGame();
    }

	public override void SetVisible (bool value)
	{
		if(value)
		{
			switch(GameManager.Instance.gameOverReason)
			{
			case GameOverReason.PLAYER_WON:
				overLabel.text = k_WON;
				reasonLabel.text = k_PLAYER_WON;
				restartObj.SetActive(false);
				break;
			case GameOverReason.ANIMAL_STARVED:
				overLabel.text = k_LOST;
				reasonLabel.text = k_ANIMAL_STARVED;
				break;
			case GameOverReason.HUNTER_WON:
				overLabel.text = k_LOST;
				reasonLabel.text = k_HUNTER_WON;
				break;
			}
		}
		else
		{
			restartObj.SetActive(true);
		}

		base.SetVisible (value);
	}
}
