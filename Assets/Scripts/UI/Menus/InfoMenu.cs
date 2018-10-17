using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : UIMenu
{
    [SerializeField]
    TileButton blockTile;

    public void GoToInfo()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Info);
    }

    public void GoToSettings()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Settings);
    }

    public void OnEnable()
    {
        blockTile.SetState(TileState.Blocked);
    }
}
