using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIMenu
{
	[SerializeField]
	Text versionlabel;

	const string k_VERSION_CODE = "V_{0} #{2}";

	void Start()
	{
		versionlabel.text = MenuManager.Instance.BuildVersionStr;
	}

    public void GoToInfo()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Info);
    }

    public void GoToSettings()
    {
        MenuManager.Instance.ShowMenu(MenuTypes.Settings);
    }
}
