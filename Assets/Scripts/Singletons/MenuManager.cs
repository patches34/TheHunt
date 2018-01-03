using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public enum MenuTypes
{
	None,
	GameOver,
	Pause,
	Info,
    Settings
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
	float zoomSpeed, mouseWheelSpeed, zoomMax;
	float zoomMin;

	public GameObject loadingSpinner, playerInputBlock;

	[SerializeField]
	Button playerPassBtn, playerForfeitBtn, playerRetryBtn;

    const string k_IS_NEW_PLAYER = "isNewPlayer";

	#region Initialization
	// Use this for initialization
	protected MenuManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
        if (PlayerPrefs.GetInt(k_IS_NEW_PLAYER, -1) < 0)
        {
            ShowMenu(MenuTypes.Info);
            PlayerPrefs.SetInt(k_IS_NEW_PLAYER, 0);
        }
	}

	public string BuildVersionStr;

	/// <summary>
	/// Adds a UI screen to the stack and deletes its scene
	/// </summary>
	/// <param name="screen">Screen.</param>
	public void AddScreen(UIMenu screen)
	{
		//Store a ref to the screen's go
		UnityEngine.SceneManagement.Scene rootScene = screen.gameObject.scene;

		//Set the screens transform to the main scene's canvas
		screen.transform.SetParent(transform);
		screen.transform.SetAsLastSibling();

		//Make sure the screen is scaled and positioned correctly
		screen.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
		screen.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		screen.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
		screen.gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		screen.gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;

		//Hide the screen
		screen.gameObject.SetActive(false);

		//Add this screen to the screens dictionary
		if(!menus.ContainsKey(screen.type))
		{
			menus.Add(screen.type, screen);
		}
		else
		{
			Debug.LogError("Duplicate screens: " + screen.type);
		}

		//Remove the scene the screen came from
		LoadSceneManager.Instance.UnloadScene(rootScene);
	}

	void Update()
	{
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

			SetZoom(deltaMagnitudeDiff, zoomSpeed);
		}

		#if UNITY_EDITOR
		if(Input.mouseScrollDelta.y != 0)
		{
			SetZoom(-Input.mouseScrollDelta.y, mouseWheelSpeed);
		}
		#endif
		#endregion

		#region Game Buttons Activation Status
		playerPassBtn.interactable = GameManager.Instance.IsPlayerTurn();

		playerForfeitBtn.interactable = GameManager.Instance.IsGameActive();

		playerInputBlock.SetActive(!GameManager.Instance.IsPlayerTurn());

        playerRetryBtn.interactable = GameManager.Instance.IsGameActive();
		#endregion
	}

	void SetZoom(float delta, float zoomSpeed)
	{
		Vector3 rectScale = boardPaddingRect.localScale;

		//	X
		rectScale.x -= delta * zoomSpeed;
		if(rectScale.x > zoomMax)
		{
			rectScale.x = zoomMax;
		}
		else if(rectScale.x < zoomMin)
		{
			rectScale.x = zoomMin;
		}

		//	Y
		rectScale.y -= delta * zoomSpeed;
		if(rectScale.y > zoomMax)
		{
			rectScale.y = zoomMax;
		}
		else if(rectScale.y < zoomMin)
		{
			rectScale.y = zoomMin;
		}

		boardPaddingRect.localScale = rectScale;
	}

	public void ShowMenu(int type)
	{
		ShowMenu((MenuTypes)type);
	}
	public void ShowMenu(MenuTypes type)
	{
		menus[type].SetVisible(true);
	}

    public void HideMenu(MenuTypes type)
    {
        menus[type].SetVisible(false);
    }

	public void ResizeGameBoard(Vector2 boardSize)
	{
		boardRect.offsetMax = boardSize;
		boardRect.offsetMin = -boardSize;

		boardPaddingRect.offsetMax = boardSize + boardPadding;
		boardPaddingRect.offsetMin = -boardSize - boardPadding;

		zoomMin = Mathf.Min(CanvasRect.width / boardPaddingRect.rect.width, CanvasRect.height / boardPaddingRect.rect.height);
	}

	public void SetZoomSpeed(float value)
	{
		zoomSpeed = value;
	}

	public void OpenFeedback()
	{
		Application.OpenURL("https://github.com/patches34/TheHunt/issues/new");
	}
}