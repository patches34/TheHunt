using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;
using UnityEngine.UI;

public enum MenuTypes
{
	None,
	GameOver,
	Pause
}

public class MenuManager : Singleton<MenuManager>
{
	[SerializeField]
	RectTransform canvasRectTransform;
	public Rect CanvasRect
	{
		get
		{
			return canvasRectTransform.rect;
		}
	}

	Dictionary<MenuTypes, UIMenu> menus = new Dictionary<MenuTypes, UIMenu>();

	public Text debugLabel;

	[SerializeField]
	RectTransform boardRect;

	#region Initialization
	// Use this for initialization
	protected MenuManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
		
	}

	public void AddMenu(MenuTypes type, UIMenu menu)
	{
		menus.Add(type, menu);
	}

	void Update()
	{
		
	}

	public void ShowMenu(int type)
	{
		ShowMenu((MenuTypes)type);
	}
	public void ShowMenu(MenuTypes type)
	{
		menus[type].SetVisible(true);
	}

	public void ResizeGameBoard(Vector2 boardSize)
	{
		debugLabel.text = boardSize.ToString();

		boardRect.offsetMax = boardSize;
		boardRect.offsetMin = -boardSize;
	}
}