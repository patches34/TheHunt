using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Collections;
using UnityEngine.UI;

public enum MenuTypes
{
	None,
	GameOver,
	Pause,
	Info
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
	RectTransform boardRect, boardPaddingRect;
	[SerializeField]
	Vector2 boardPadding;

	[SerializeField]
	public float zoomSpeed;
	[SerializeField]
	float zoonMin, zoomMax;

	public GameObject loadingSpinner;

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
		debugLabel.text = string.Format("Touches = {0}", Input.touchCount);

		#region Zooming
		if(Input.touchCount >= 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			Vector3 rectScale = boardRect.localScale;
			rectScale.x -= deltaMagnitudeDiff * zoomSpeed;
			if(rectScale.x > zoomMax)
			{
				rectScale.x = zoomMax;
			}
			else if(rectScale.x < zoonMin)
			{
				rectScale.x = zoonMin;
			}

			rectScale.y -= deltaMagnitudeDiff * zoomSpeed;
			if(rectScale.y > zoomMax)
			{
				rectScale.y = zoomMax;
			}
			else if(rectScale.y < zoonMin)
			{
				rectScale.y = zoonMin;
			}

			boardRect.localScale = rectScale;
		}
		#endregion
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
		boardRect.offsetMax = boardSize;
		boardRect.offsetMin = -boardSize;

		boardPaddingRect.offsetMax = boardSize + boardPadding;
		boardPaddingRect.offsetMin = -boardSize - boardPadding;
	}

	public void SetZoomSpeed(float value)
	{
		zoomSpeed = value;
	}
}