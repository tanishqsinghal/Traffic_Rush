// This script is a Manager that controls all of the audio for the project. All audio
// commands are issued through the static methods of this class. Additionally, this 
// class creates AudioSource "channels" at runtime and manages them
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	static AudioManager current;

	[Header("Ambient Audio")]
	public AudioClip musicClip;					//The background music
	public AudioClip menuClip;					//Main Menu background music

	[Header("Stings")]
	public AudioClip levelStingClip;			//The sting played when the scene loads
	public AudioClip highScoreClip;				//The sting played when the player get high score
	public AudioClip uiTransitionClip;          //The Sting Played On UI Transition
	public AudioClip[] rewardClip;          	//The Sting Played When Player Gets A Reward
	public AudioClip purchaseClip;				//The Sting Played When Player successfully purches some item in the game
	public AudioClip selectionClip;				//The Sting Played When Player Selects UI Element
	public AudioClip countClip;					//The Sting Played When there is some calculation happening in the ui
	public AudioClip errorClip;					//The Sting Played When there is some error
	public AudioClip nitroPickupClip;			//The Sting Played When the racer picks up nitro
	public AudioClip levelIncreamentClip;		//The Sting Played When the level is increamented

	[Header("Player Audio")]
	public AudioClip nitrousClip;				//The boost activation sound effect
	public AudioClip closePassClip;				//The close overtake sound effect
	public AudioClip brakesClip;				//The vehicle brakes sound effect
	public AudioClip deathClip;					//The player death sound effect
	public AudioClip vehicleAccidentClip;		//The vehicle accident sound effect

	public AudioClip normalEngineClip;			//Engine sounds effect without boost
	public AudioClip boostedEngineClip;			//Engine sounds effect with boost

	[Header("Mixer Groups")]
	public AudioMixerGroup musicGroup;  		//The music mixer group
	public AudioMixerGroup stingGroup;  		//The sting mixer group
	public AudioMixerGroup engineGroup;  		//The vehicle engine mixer group
	public AudioMixerGroup playerGroup; 		//The player mixer group
	public AudioMixerGroup uiGroup;  			//The UI mixer group
	public AudioMixerGroup environmentGroup;  	//The Environment mixer group
	public AudioMixerGroup enemyGroup;  		//The Enemy mixer group

	AudioSource musicSource;            		//Reference to the generated music Audio Source
	AudioSource stingSource;            		//Reference to the generated sting Audio Source
	AudioSource engineSource;           		//Reference to the generated engine Audio Source
	AudioSource playerSource;           		//Reference to the generated player Audio Source
	AudioSource uiSource;						//Reference to the generated UI Audio Source
	AudioSource environmentSource;				//Reference to the generated Environment Audio Source
	AudioSource enemySource;					//Reference to the generated Enemy Audio Source

	void Awake()
	{
		//If an AudioManager exists and it is not this...
		if (current != null && current != this)
		{
			//...destroy this. There can be only one AudioManager
			Destroy(gameObject);
			return;
		}

		//This is the current AudioManager and it should persist between scene loads
		current = this;
		DontDestroyOnLoad(gameObject);

		//Generate the Audio Source "channels" for our game's audio
		musicSource			= gameObject.AddComponent<AudioSource>() as AudioSource;
		stingSource			= gameObject.AddComponent<AudioSource>() as AudioSource;
		playerSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
		engineSource		= gameObject.AddComponent<AudioSource>() as AudioSource;
		uiSource			= gameObject.AddComponent<AudioSource>() as AudioSource;
		environmentSource	= gameObject.AddComponent<AudioSource>() as AudioSource;
		enemySource			= gameObject.AddComponent<AudioSource>() as AudioSource;

		//Assign each audio source to its respective mixer group so that it is
		//routed and controlled by the audio mixer
		musicSource.outputAudioMixerGroup		= musicGroup;
		stingSource.outputAudioMixerGroup		= stingGroup;
		playerSource.outputAudioMixerGroup		= playerGroup;
		engineSource.outputAudioMixerGroup		= engineGroup;
		uiSource.outputAudioMixerGroup 			= uiGroup;
		environmentSource.outputAudioMixerGroup = environmentGroup;
		enemySource.outputAudioMixerGroup 		= enemyGroup;

		//Being playing the game audio
		//StartLevelAudio();
	}

	public static void StartLevelAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null || (current.musicSource.clip == current.musicClip && current.musicSource.isPlaying))
			return;

		//Set the clip for music audio, tell it to loop, and then tell it to play
		current.musicSource.clip = current.musicClip;
		current.musicSource.loop = true;

		if(SettingsManager.CheckMusicSettings () == 1)
		{
			current.musicSource.Play();
		}
		else
		{
			current.musicSource.Play();
			current.musicSource.mute = true;
		}

		//Play the audio that repeats whenever the level reloads
		PlaySceneRestartAudio();
	}

	public static void PlaySceneRestartAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the level reload sting clip and tell the source to play
			current.stingSource.clip = current.levelStingClip;
			current.stingSource.Play();
		}
	}

	public static void StartMenuAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Set the clip for music audio, tell it to loop, and then tell it to play
		current.musicSource.clip = current.menuClip;
		current.musicSource.loop = true;

		if(SettingsManager.CheckMusicSettings () == 1)
		{
			current.musicSource.Play();
		}
		else
		{
			current.musicSource.Play();
			current.musicSource.mute = true;
		}

		//Play the audio that repeats whenever the level reloads
		//PlaySceneRestartAudio();
	}

	public static void PauseAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckMusicSettings () == 1)
		{
			current.musicSource.Pause ();
		}

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			current.engineSource.Pause ();
		}
	}

	public static void ResumeAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckMusicSettings () == 1)
		{
			current.musicSource.Play ();
		}

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			current.engineSource.Play ();
		}
	}

	public static void StartNormalEngineAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null || (current.engineSource.clip == current.normalEngineClip && current.engineSource.isPlaying))
			return;

		//Set the clip for music audio, tell it to loop, and then tell it to play
		current.engineSource.clip = current.normalEngineClip;
		current.engineSource.loop = true;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			current.engineSource.Play();
		}
	}

	public static void StartBoostedEngineAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null || (current.engineSource.clip == current.boostedEngineClip && current.engineSource.isPlaying))
			return;

		//Set the clip for music audio, tell it to loop, and then tell it to play
		current.engineSource.clip = current.boostedEngineClip;
		current.engineSource.loop = true;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			current.engineSource.Play();
		}
	}

	public static void StopEngineAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			current.engineSource.Stop ();
		}
	}

	public static void PlayNitrousAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the death SFX clip and tell the source to play
			current.playerSource.clip = current.nitrousClip;
			current.playerSource.Play();
		}
	}

	public static void PlayClosePassAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the death SFX clip and tell the source to play
			current.playerSource.clip = current.closePassClip;
			current.playerSource.Play();
		}
	}

	public static void PlayBrakesAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the death SFX clip and tell the source to play
			current.playerSource.clip = current.brakesClip;
			current.playerSource.Play();
		}
	}

	public static void ToggleMusic()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Toggle Music
		current.musicSource.mute = !current.musicSource.mute;
	}

	public static void PlayNitroPickUpAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the nitro pickup sting clip and tell the source to play
			current.stingSource.clip = current.nitroPickupClip;
			current.stingSource.Play();
		}
	}

	public static void PlayLevelIncreamentAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the nitro pickup clip and tell the source to play
			current.uiSource.clip = current.levelIncreamentClip;
			current.uiSource.Play();
		}
	}

	public static void PlayDeathAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the death SFX clip and tell the source to play
			current.playerSource.clip = current.deathClip;
			current.playerSource.Play();
		}
	}

	public static void PlayVehicleAccidentAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the Attack SFX clip and tell the source to play
			current.enemySource.clip = current.vehicleAccidentClip;
			current.enemySource.Play();
		}
	}

	public static void PlayHighScoreAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		//Stop the ambient sound
		//current.ambientSource.Stop();

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set the player won clip and tell the source to play
			current.uiSource.clip = current.highScoreClip;
			current.uiSource.Play();
		}
	}

	public void PlayUISound(AudioClip audioClip)
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The sting Clip And Tell The Source To Play
			current.uiSource.clip = audioClip;
			current.uiSource.Play ();
		}
	}

	public void PlayButtonClickSound()
	{
		PlaySelectionSound ();
	}

	public static void PlaySelectionSound()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.uiSource.clip = current.selectionClip;
			current.uiSource.Play ();
		}
	}

	public static void PlayCountSound()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.stingSource.clip = current.countClip;
			current.stingSource.Play ();
		}
	}

	public static void PlayPurchaseSound()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.uiSource.clip = current.purchaseClip;
			current.uiSource.Play ();
		}
	}

	public static void PlayErrorSound()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.uiSource.clip = current.errorClip;
			current.uiSource.Play ();
		}
	}

	public static void PlayTransitionSound()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.uiSource.clip = current.uiTransitionClip;
			current.uiSource.Play ();
		}
	}

	public static void PlayRewardAudio()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Pick a random Reeward sound
			int index = Random.Range(0, current.rewardClip.Length);

			//Set The Reward Sound Clip And Tell The Source To Play
			current.uiSource.clip = current.rewardClip[index];
			current.uiSource.Play ();
		}
	}

	public static void PlayAnySound(AudioClip audioClip)
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckSoundSettings () == 1)
		{
			//Set The UI Sound Clip And Tell The Source To Play
			current.uiSource.clip = audioClip;
			current.uiSource.Play ();
		}
	}

	public static void VibratePhone()
	{
		//If there is no current AudioManager, exit
		if (current == null)
			return;

		if(SettingsManager.CheckVibrationSettings () == 1)
		{
			//Vibrate The Phone
			Handheld.Vibrate ();
		}
	}
}