using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Slider widthSlider, heightSlider;

	[SerializeField]
	Text widthLabel, heightLabel;

	// Use this for initialization
	void OnEnable()
	{
		widthSlider.value = BoardManager.Instance.BoardSize.X;
		heightSlider.value = BoardManager.Instance.BoardSize.Y;
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetBoardWidth(float value)
	{
		BoardManager.Instance.SetBoardSize(width:(int)value);

		widthLabel.text = value.ToString();
	}

	public void SetBoardHeight(float value)
	{
		BoardManager.Instance.SetBoardSize(height:(int)value);

		heightLabel.text = value.ToString();
	}
}
