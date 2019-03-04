using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{

	public enum EGameState { Simulating, Observing, MapMaker }

	private int m_AIUpdates = 20;
	private int m_GameSpeed = 0;
	private EGameState m_GameState = EGameState.Simulating;

	public int GameSpeed { get => m_GameSpeed; set => m_GameSpeed = value; }
	public EGameState GameState { get => m_GameState; set => m_GameState = value; }
	public int AIUpdates { get => m_AIUpdates; set => m_AIUpdates = value; }

	private void Update()
	{
		if (Input.GetButtonDown("StopSpeed"))
			m_GameSpeed = 0;
		else if (Input.GetButtonDown("NormalSpeed"))
			m_GameSpeed = 1;
		else if (Input.GetButtonDown("FastSpeed"))
			m_GameSpeed = 2;
		else if (Input.GetButtonDown("FastestSpeed"))
			m_GameSpeed = 3;
	}
}
