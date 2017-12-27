using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMenu : MonoBehaviour
{
	public MenuTypes type;

	// Use this for initialization
	void Awake ()
	{
		MenuManager.Instance.AddScreen(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void SetVisible(bool value)
	{
		gameObject.SetActive(value);
	}
}
