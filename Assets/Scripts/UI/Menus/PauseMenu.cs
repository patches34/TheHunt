using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Slider zoomSpeedSlider;

    [SerializeField]
    InputField widthInput, heightInput;

    [SerializeField]
	Text zoomSpeedLabel;

    [SerializeField]
    Dropdown boardSetupmethodDropdown;

    [SerializeField]
    List<RectTransform> boardSetupPanels;

	// Use this for initialization
	void OnEnable()
	{
		widthInput.text = BoardManager.Instance.BoardSize.X.ToString();
		heightInput.text = BoardManager.Instance.BoardSize.Y.ToString();

		zoomSpeedSlider.value = MenuManager.Instance.zoomSpeed;

        foreach(RectTransform p in boardSetupPanels)
        {
            p.gameObject.SetActive(false);
        }

        boardSetupmethodDropdown.value = (int)BoardManager.Instance.boardSetupMethod;
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetBoardWidth(string value)
	{
		BoardManager.Instance.SetBoardSize(width:Convert.ToInt32(value));

		widthInput.text = BoardManager.Instance.BoardSize.X.ToString();
	}

	public void SetBoardHeight(string value)
	{
		BoardManager.Instance.SetBoardSize(height: Convert.ToInt32(value));

        heightInput.text = BoardManager.Instance.BoardSize.Y.ToString();
    }

	public void SetZoomSpeed(float value)
	{
		MenuManager.Instance.zoomSpeed = value;

		zoomSpeedLabel.text = value.ToString("F4");
	}

    public void OnBoardSetupValueChange(int value)
    {
        boardSetupPanels[(int)BoardManager.Instance.boardSetupMethod].gameObject.SetActive(false);

        BoardManager.Instance.boardSetupMethod = (BoardSetupMethods)value;

        boardSetupPanels[value].gameObject.SetActive(true);
    }
}
