using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Load scene manager loads scenes and adds them to one main scene.
/// </summary>
public class LoadSceneManager : Singleton<LoadSceneManager>
{
	void Awake()
	{
		//Move through and load each scene
		for(int i = 1; i < SceneManager.sceneCountInBuildSettings; ++i)
		{
			SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
		}
	}

	/// <summary>
	/// Unloads a scene by getting a reference through its gameobject
	/// </summary>
	/// <param name="root">The game object which scene we want to unload</param>
	public void UnloadScene(Scene sceneToUnload)
	{
		//Unload the scene
		if (sceneToUnload.name != "NewMain")
		{
			StartCoroutine(UnloadSceneCouritine(sceneToUnload));
		}
	}

	/// <summary>
	/// Unloads a scene by waiting for it to be loaded. doesnt use and index because the index can become out of sync
	/// </summary>
	/// <returns>The scene.</returns>
	/// <param name="sceneToUnload">Scene to unload.</param>
	IEnumerator UnloadSceneCouritine(Scene sceneToUnload)
	{
		while (!sceneToUnload.isLoaded)
		{
			yield return null;
		}
		SceneManager.UnloadScene(sceneToUnload.buildIndex);
	}
}
