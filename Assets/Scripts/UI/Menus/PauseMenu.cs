using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Slider widthSlider, heightSlider, zoomSpeedSlider;

	[SerializeField]
	Text widthLabel, heightLabel, zoomSpeedLabel;

	// Use this for initialization
	void OnEnable()
	{
		widthSlider.value = BoardManager.Instance.BoardSize.X;
		heightSlider.value = BoardManager.Instance.BoardSize.Y;

		zoomSpeedSlider.value = MenuManager.Instance.zoomSpeed;
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

	public void SetZoomSpeed(float value)
	{
		MenuManager.Instance.zoomSpeed = value;

		zoomSpeedLabel.text = value.ToString("F4");
	}
}
