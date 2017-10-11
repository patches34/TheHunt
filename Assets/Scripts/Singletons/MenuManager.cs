﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;

public enum MenuTypes
{
	None,
	GameOver
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

	public void ShowMenu(MenuTypes type)
	{
		menus[type].SetVisible(true);
	}
}