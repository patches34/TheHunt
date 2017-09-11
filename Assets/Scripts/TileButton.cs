using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileState
{
	None = 0,
	Hunter,
	Animal,
	Food,
	Blocked,
}

[RequireComponent (typeof(Animator))]
public class TileButton : MonoBehaviour
{
	[SerializeField]
	TileState state;

	[SerializeField]
	Animator anim;
	const string k_State = "State";

	// Use this for initialization
	void Start ()
	{
		foreach(Transform child in gameObject.transform)
		{
			child.gameObject.SetActive(false);
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

	public void SetState(TileState newState)
	{
		state = newState;

		anim.SetInteger(k_State, (int)newState);
	}

	public TileState GetState()
	{
		return state;
	}
}
