using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	static SettingsManager current;

	//Player Setting Variables
	int musicSetting, soundSetting, vibrationSetting;

	[Header ("Settings OFF Sprites")]
	public GameObject musicOFF;
	public GameObject soundOFF;
	public GameObject vibrationOFF;

	// Use this for initialization
	void Awake () {

		current = this;

		//Get Player Settings From System Memory
		musicSetting = PlayerPrefs.GetInt ("MusicSetting", 1);
		soundSetting = PlayerPrefs.GetInt ("SoundSetting", 1);
		vibrationSetting = PlayerPrefs.GetInt ("VibrationSetting", 1);

		//Blocking Sprites Will Be Active
		//Only If The Respective Setting Is 0
		musicOFF.SetActive (musicSetting == 0);
		soundOFF.SetActive (soundSetting == 0);
		vibrationOFF.SetActive (vibrationSetting == 0);
	}


	public static int CheckMusicSettings()
	{
		return current.musicSetting;
	}

	public static int CheckSoundSettings()
	{
		return current.soundSetting;
	}

	public static int CheckVibrationSettings()
	{
		return current.vibrationSetting;
	}

	public void MusicSetting()
	{
		musicSetting = PlayerPrefs.GetInt ("MusicSetting", 1);
		if(musicSetting == 0)
		{
			musicSetting = 1;
			musicOFF.SetActive (false);
			PlayerPrefs.SetInt ("MusicSetting", 1);

			//Play Music From Audio Manager
			AudioManager.ToggleMusic ();
		}
		else
		{
			musicSetting = 0;
			musicOFF.SetActive (true);
			PlayerPrefs.SetInt ("MusicSetting", 0);

			//Stop Music From Audio Manager
			AudioManager.ToggleMusic ();
		}
	}

	public void SoundSetting()
	{
		soundSetting = PlayerPrefs.GetInt ("SoundSetting", 1);
		if(soundSetting == 0)
		{
			soundSetting = 1;
			soundOFF.SetActive (false);
			PlayerPrefs.SetInt ("SoundSetting", 1);
		}
		else
		{
			soundSetting = 0;
			soundOFF.SetActive (true);
			PlayerPrefs.SetInt ("SoundSetting", 0);
		}
	}

	public void VibrationSetting()
	{
		vibrationSetting = PlayerPrefs.GetInt ("VibrationSetting", 1);
		if(vibrationSetting == 0)
		{
			vibrationSetting = 1;
			vibrationOFF.SetActive (false);
			PlayerPrefs.SetInt ("VibrationSetting", 1);
			Handheld.Vibrate ();
		}
		else
		{
			vibrationSetting = 0;
			vibrationOFF.SetActive (true);
			PlayerPrefs.SetInt ("VibrationSetting", 0);
		}
	}
}
