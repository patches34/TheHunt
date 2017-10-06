using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMenu : MonoBehaviour
{
	[SerializeField]
	MenuTypes type;

	// Use this for initialization
	void Awake ()
	{
		MenuManager.Instance.AddMenu(type, this);

		SetVisible(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void SetVisible(bool value)
	{
		gameObject.SetActive(value);
	}
}
