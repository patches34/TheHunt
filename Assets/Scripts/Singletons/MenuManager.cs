using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
		#region Game Buttons Activation Status
		playerPassBtn.interactable = GameManager.Instance.IsPlayerTurn();

		playerForfeitBtn.interactable = GameManager.Instance.IsGameActive();

		playerInputBlock.SetActive(!GameManager.Instance.IsPlayerTurn());

        playerRetryBtn.interactable = GameManager.Instance.IsGameActive();
		#endregion
	}

    #region Zoom Logic
    public void OnZoom(InputAction.CallbackContext context)
	{
		Vector2 zoomScroll = context.ReadValue<Vector2>();

		SetZoom(zoomScroll.y, mouseWheelSpeed);
	}

	void SetZoom(float newZoom)
	{
		SetZoom(new Vector2(newZoom, newZoom));
	}
	void SetZoom(float delta, float zoomSpeed)
	{
		Vector3 rectScale = boardPaddingRect.localScale;

		//	X
		rectScale.x -= delta * zoomSpeed;

		//	Y
		rectScale.y -= delta * zoomSpeed;

		SetZoom(rectScale);
	}
	void SetZoom(Vector2 newZoom)
	{
        //	X
        if (newZoom.x > zoomMax)
        {
            newZoom.x = zoomMax;
        }
        else if (newZoom.x < zoomMin)
        {
            newZoom.x = zoomMin;
        }

        //	Y
        if (newZoom.y > zoomMax)
        {
            newZoom.y = zoomMax;
        }
        else if (newZoom.y < zoomMin)
        {
            newZoom.y = zoomMin;
        }

        boardPaddingRect.localScale = newZoom;
    }
    #endregion

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

		SetZoom(zoomMin);
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