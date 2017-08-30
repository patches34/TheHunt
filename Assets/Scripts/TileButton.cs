using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileState
{
	None = 0,
	Blocked = 1,
	Food = 2,
	Animal = 4,
	Hunter = 8
}

public class TileButton : MonoBehaviour
{
	[SerializeField]
	Image blockedIcon;
	[SerializeField]
	Image foodIcon;
	[SerializeField]
	Image animalIcon;
	[SerializeField]
	Image hunterIcon;

	[SerializeField]
	TileState state;

	// Use this for initialization
	void Start ()
	{
		foreach(Transform child in gameObject.transform)
		{
			child.gameObject.SetActive(false);
		}

		if((state & TileState.Food) == TileState.Food)
		{
			foodIcon.gameObject.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Click()
	{
		//	Check if if its the player's turn
		if(GameManager.Instance.turn == TurnState.Player)
		{
			if((state & (TileState.Food | TileState.None)) == (TileState.None | TileState.Food))
			{
				Debug.Log("Can do something");
			}
		}
	}
}
