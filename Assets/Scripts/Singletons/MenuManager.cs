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
    Settings,
    Credits
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

    public List<TileButton> paths;

	[SerializeField]
	RectTransform boardRect, boardPaddingRect;
	[SerializeField]
	Vector2 boardPadding;

	[SerializeField]
	float zoomSpeed, mouseWheelSpeed, zoomMax;
	float zoomMin;

	public GameObject loadingSpinner, playerInputBlock;

	[SerializeField]
	Button playerPassBtn, playerForfeitBtn, playerRetryBtn, fastForwardBtn;

    const string k_IS_NEW_PLAYER = "isNewPlayer";
	const string k_IS_ENGAGED = "isEngaged";

    [SerializeField]
    Text blockNumberLabel;

	#region Initialization
	// Use this for initialization
	protected MenuManager()
	{
		// guarantee this will be always a singleton only - can't use the constructor!
	}
	#endregion

	void Start()
	{
		Application.targetFrameRate = 30;



        for(int i = 0; i < transform.childCount; ++i)
        {
            UIMenu childScreen = transform.GetChild(i).GetComponent<UIMenu>();
            if (childScreen != null)
            {
                AddScreen(childScreen);
            }
        }

        #if !UNITY_EDITOR
        ShowMenu(MenuTypes.Info);
        #endif
    }

    public string BuildVersionStr;

	/// <summary>
	/// Adds a UI screen to the stack and deletes its scene
	/// </summary>
	/// <param name="screen">Screen.</param>
	void AddScreen(UIMenu screen)
	{
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
	}

	void Update()
	{
#region Zooming
#if UNITY_ANDROID
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
#else
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
		if(menus.ContainsKey(type))
		{
            menus[type].SetVisible(true);
        }
	}

    public void HideMenu(MenuTypes type)
    {
		if (menus.ContainsKey(type))
		{
			menus[type].SetVisible(false);
		}
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

	public void SetFastForwardBtnState()
	{
        fastForwardBtn.GetComponent<Animator>().SetBool("IsActive", GameManager.Instance.IsFastForward);
	}

    public void SetBlockTileCount(int count)
    {
        if (GameManager.Instance.MaxPlayerBlocks == 0)
        {
            blockNumberLabel.text = "unlimited";
        }
        else
        {
            blockNumberLabel.text = (GameManager.Instance.MaxPlayerBlocks - count).ToString();
        }
    }
}