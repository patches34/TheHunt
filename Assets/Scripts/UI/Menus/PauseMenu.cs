using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Slider zoomSpeedSlider;

    [SerializeField]
    InputField widthInput, heightInput, blockedTilesInput, blockedTilesDistanceInput, maxBlocksInput;

    [SerializeField]
	Text zoomSpeedLabel;

    [SerializeField]
    Dropdown boardSetupmethodDropdown;

    [SerializeField]
    List<RectTransform> boardSetupPanels;

	[SerializeField]
	Toggle showPathToggle, hunterSeeksHomeToggle;

	// Use this for initialization
	void OnEnable()
	{
		widthInput.text = BoardManager.Instance.BoardSize.X.ToString();
		heightInput.text = BoardManager.Instance.BoardSize.Y.ToString();

		blockedTilesInput.text = BoardManager.Instance.StartBlockedTiles.ToString();
		blockedTilesDistanceInput.text = BoardManager.Instance.StartBlockedTilesMinSpacing.ToString();

        foreach(RectTransform p in boardSetupPanels)
        {
            p.gameObject.SetActive(false);
        }

        boardSetupmethodDropdown.value = (int)BoardManager.Instance.boardSetupMethod;

		boardSetupPanels[(int)BoardManager.Instance.boardSetupMethod].gameObject.SetActive(true);

		showPathToggle.isOn = BoardManager.Instance.ShowAiPath;

		maxBlocksInput.text = GameManager.Instance.MaxPlayerBlocks.ToString();

		hunterSeeksHomeToggle.isOn = GameManager.Instance.DoesHunterSeeksHome;
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

		BoardManager.Instance.StartBlockedTiles = Convert.ToInt32(value);

		blockedTilesInput.text = BoardManager.Instance.StartBlockedTiles.ToString();
	}

	public void SetBlockedTilesDistane(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";

		BoardManager.Instance.StartBlockedTilesMinSpacing = Convert.ToInt32(value);

		blockedTilesDistanceInput.text = BoardManager.Instance.StartBlockedTilesMinSpacing.ToString();
	}

	public void SetMaxBlocks(string value)
	{
		if(string.IsNullOrEmpty(value))
			value = "0";

		GameManager.Instance.MaxPlayerBlocks = Convert.ToInt32(value);

		maxBlocksInput.text = GameManager.Instance.MaxPlayerBlocks.ToString();
	}

	public void OnHunterSeekHomeValueChange(bool value)
	{
		GameManager.Instance.DoesHunterSeeksHome = value;
	}

	public void Rebuild()
	{
		GameManager.Instance.Rebuild();
	}
}
