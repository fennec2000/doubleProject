using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
	public void LoadSceneIndex(int index)
	{
		Debug.Log("Loading Scene: " + index);
		SceneManager.LoadScene(index);
	}

	public void CloseApp()
	{
		Application.Quit();
	}
}
