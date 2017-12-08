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

	[SerializeField]
	GameObject animalPath, hunterPath;

	// Use this for initialization
	void Start ()
	{
		coord = tile.Location;

		if(label != null)
		{
			label.text = coord.ToString();
		}

		SetAsPathNodeFor(TurnActor.None);
	}

    void OnEnable()
    {
        tile.Passable = true;
    }

    void OnDisable()
    {
        tile.Passable = false;
        btn.interactable = true;

		SetAsPathNodeFor(TurnActor.None);
    }

    public void Click()
	{
		//	Check if if its the player's turn
		if(GameManager.Instance.IsActorTurn(TurnActor.Player))
		{
			SetState(TileState.Blocked);

			GameManager.Instance.BlockTile(tile.Location);

			GameManager.Instance.PlayerWent();
		}
	}

	public void SetState(TileState newState)
	{
		State = newState;

		anim.SetInteger(k_State, (int)newState);

		SetIsInteractable(newState == TileState.None);

		tile.Passable = newState != TileState.Blocked;
	}

	public TileState GetState()
	{
		return state;
	}

	public void SetIsInteractable(bool value)
	{
		if(state != TileState.Food)
		{
			btn.interactable = value;
		}
	}

	public void SetAsPathNodeFor(TurnActor actor)
	{
		switch(actor)
		{
		case TurnActor.Animal:
			animalPath.SetActive(true);
			break;
		case TurnActor.Hunter:
			hunterPath.SetActive(true);
			break;
		default:
			animalPath.SetActive(false);
			hunterPath.SetActive(false);
			break;
		}
	}
}
