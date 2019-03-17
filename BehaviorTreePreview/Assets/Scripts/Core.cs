using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Core : MonoBehaviour
{
	[SerializeField]
	Text m_GameSpeedText;

	public enum EGameState { Simulating, Observing, MapMaker }

	private float m_AIUpdates = 0.5f;
	private int m_GameSpeed = 0;
	private EGameState m_GameState = EGameState.Simulating;

	public int GameSpeed { get => m_GameSpeed; set => m_GameSpeed = value; }
	public EGameState GameState { get => m_GameState; set => m_GameState = value; }
	public float AIUpdates { get => m_AIUpdates; set => m_AIUpdates = value; }

	private void Start()
	{
		// set ingame text
		m_GameSpeedText.text = "Game Speed: " + m_GameSpeed + "x";
	}

	private void Update()
	{
		if (Input.GetButtonDown("StopSpeed"))
		{
			m_GameSpeed = 0;
			m_GameSpeedText.text = "Game Speed: 0x";
		}
		else if (Input.GetButtonDown("NormalSpeed"))
		{
			m_GameSpeed = 1;
			m_GameSpeedText.text = "Game Speed: 1x";
		}
		else if (Input.GetButtonDown("FastSpeed"))
		{
			m_GameSpeed = 1;
			m_GameSpeedText.text = "Game Speed: 2x";
		}
		else if (Input.GetButtonDown("FastestSpeed"))
		{
			m_GameSpeed = 3;
			m_GameSpeedText.text = "Game Speed: 3x";
		}
		else if (Input.GetButtonDown("Back"))
		{
			Debug.Log("Loading main menu");
			SceneManager.LoadScene(0);
		}
	}
}
