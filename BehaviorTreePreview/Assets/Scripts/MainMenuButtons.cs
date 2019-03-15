using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
	public enum EMenuState { MainMenu, Options, Credits }

	public AudioMixer m_MasterMixer;
	public AudioSource m_Music;
	public Slider[] m_VolumeSliders;

	public GameObject[] m_MainMenuObjects;
	public GameObject[] m_OptionsObjects;
	public GameObject[] m_CreditsObjects;

	private EMenuState m_MenuState = EMenuState.MainMenu;

	public void Start()
	{
		// Set Volume Sliders
		if (m_MasterMixer.GetFloat("MasterMixerVolume", out float vol) && m_VolumeSliders[0] != null)
			m_VolumeSliders[0].value = Mathf.Pow(10, vol / 20);

		if (m_MasterMixer.GetFloat("MusicMixerVolume", out vol) && m_VolumeSliders[1] != null)
			m_VolumeSliders[1].value = Mathf.Pow(10, vol/20);

		if (m_MasterMixer.GetFloat("SoundMixerVolume", out vol) && m_VolumeSliders[2] != null)
			m_VolumeSliders[2].value = Mathf.Pow(10, vol / 20);

		// Go to main menu
		foreach (GameObject obj in m_OptionsObjects)
			obj.SetActive(false);
		foreach (GameObject obj in m_CreditsObjects)
			obj.SetActive(false);
		ChangeMenu(EMenuState.MainMenu);
	}

	public void LoadSceneIndex(int index)
	{
		Debug.Log("Loading Scene: " + index);
		SceneManager.LoadScene(index);
	}

	public void CloseApp()
	{
		Application.Quit();
	}

	public void ChangeMenu(EMenuState menuState)
	{
		// turn off old items
		switch (m_MenuState)
		{
			case EMenuState.MainMenu:
				foreach (GameObject obj in m_MainMenuObjects)
					obj.SetActive(false);
				break;
			case EMenuState.Options:
				foreach (GameObject obj in m_OptionsObjects)
					obj.SetActive(false);
				break;
			case EMenuState.Credits:
				foreach (GameObject obj in m_CreditsObjects)
					obj.SetActive(false);
				break;
			default:
				break;
		}

		// update state
		m_MenuState = menuState;

		// turn on new items
		switch (m_MenuState)
		{
			case EMenuState.MainMenu:
				foreach (GameObject obj in m_MainMenuObjects)
					obj.SetActive(true);
				break;
			case EMenuState.Options:
				foreach (GameObject obj in m_OptionsObjects)
					obj.SetActive(true);
				break;
			case EMenuState.Credits:
				foreach (GameObject obj in m_CreditsObjects)
					obj.SetActive(true);
				break;
			default:
				break;
		}
	}

	public void SwapMenuMain()
	{
		ChangeMenu(EMenuState.MainMenu);
	}

	public void SwapMenuOptions()
	{
		ChangeMenu(EMenuState.Options);
	}
	public void SwapMenuCredits()
	{
		ChangeMenu(EMenuState.Credits);
	}

	public void SetMasterVol(float volume)
	{
		m_MasterMixer.SetFloat("MasterMixerVolume", Mathf.Log10(volume) * 20);
	}

	public void SetMusicVol(float volume)
	{
		m_MasterMixer.SetFloat("MusicMixerVolume", Mathf.Log10(volume) * 20);
	}

	public void SetSoundVol(float volume)
	{
		m_MasterMixer.SetFloat("SoundMixerVolume", Mathf.Log10(volume) * 20);
	}
}
