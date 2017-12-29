using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : UIMenu
{
    public void GoToInfo()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Info);
    }

    public void GoToSettings()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Settings);
    }
}
