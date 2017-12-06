using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Slider zoomSpeedSlider;

    [SerializeField]
    InputField widthInput, heightInput, blockedTilesInput, blockedTilesDistanceInput;

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

		blockedTilesInput.text = BoardManager.Instance.blockPoints.ToString();
		blockedTilesDistanceInput.text = BoardManager.Instance.blockDistance.ToString();

        foreach(RectTransform p in boardSetupPanels)
        {
            p.gameObject.SetActive(false);
        }

        boardSetupmethodDropdown.value = (int)BoardManager.Instance.boardSetupMethod;

		boardSetupPanels[(int)BoardManager.Instance.boardSetupMethod].gameObject.SetActive(true);
	}

	public void SetBoardWidth(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";
		
		BoardManager.Instance.SetBoardSize(width:Convert.ToInt32(value));

		widthInput.text = BoardManager.Instance.BoardSize.X.ToString();
	}

	public void SetBoardHeight(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";
		
		BoardManager.Instance.SetBoardSize(height: Convert.ToInt32(value));

        heightInput.text = BoardManager.Instance.BoardSize.Y.ToString();
    }

    public void OnBoardSetupValueChange(int value)
    {
        boardSetupPanels[(int)BoardManager.Instance.boardSetupMethod].gameObject.SetActive(false);

        BoardManager.Instance.boardSetupMethod = (BoardSetupMethods)value;

        boardSetupPanels[value].gameObject.SetActive(true);
    }

	public void SetBlockedTiles(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";

		BoardManager.Instance.blockPoints = Convert.ToInt32(value);

		blockedTilesInput.text = BoardManager.Instance.blockPoints.ToString();
	}

	public void SetBlockedTilesDistane(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";

		BoardManager.Instance.blockDistance = Convert.ToInt32(value);

		blockedTilesDistanceInput.text = BoardManager.Instance.blockDistance.ToString();
	}
}
