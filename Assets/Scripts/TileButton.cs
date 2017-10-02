using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileState
{
	None = 0,
	Food,
	Blocked,
}

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(Button))]
public class TileButton : MonoBehaviour
{
	[SerializeField]
	TileState state;
	public TileState State
	{
		get
		{
			return this.state;
		}
		set
		{
			this.state = value;
		}
	}

	[SerializeField]
	Animator anim;
	const string k_State = "State";

	[SerializeField]
	Button btn;

	public Point coord;

	public Tile tile;

	[SerializeField]
	Text label;

	// Use this for initialization
	void Start ()
	{
		coord = tile.Location;

		if(label != null)
		{
			label.text = coord.ToString();
		}
	}

	public void Click()
	{
		//	Check if if its the player's turn
		if(GameManager.Instance.IsActorTurn(TurnActor.Player))
		{
			SetState(TileState.Blocked);

			tile.Passable = false;

			GameManager.Instance.PlayerWent();
		}
	}

	public void SetState(TileState newState)
	{
		State = newState;

		anim.SetInteger(k_State, (int)newState);

		SetIsInteractable(newState == TileState.None);
	}

	public TileState GetState()
	{
		return state;
	}

	public void SetIsInteractable(bool value)
	{
		btn.interactable = value;
	}
}
