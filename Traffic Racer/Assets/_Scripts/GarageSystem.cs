using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageSystem : MonoBehaviour
{
	private bool _quitBool = false;						//Check if the player has pressed the escape button to quit the game
	private int _selectedBikeNumber = 0;				//The index of the currently displayed bike

	[SerializeField] float countAnimationTime = 1.5f;	//Total time of the text count animation

	public Bike_Object[] bikes;							//Bike scriptable objects

	public GameObject[] showcasedBikes;					//All showcased bikes

	public TextMeshProUGUI bikeNameText;				//UI text which displayes current bike name
	public Button startButton;							//Start Game UI button

	public GameObject unlockSection;					//Unlock section for the bike displayed if the bike is locked
	public TextMeshProUGUI bikePriceText;				//UI text which displayes current bike price
	public TextMeshProUGUI totalMoneyText;				//UI text which displays total money that the player has

	public GameObject garageSystem;						//Garage object in the scene
	public GameObject gameModeSelector;					//Game mode selection UI

	void Start()
	{
		PlayerPrefs.SetInt ("BikeUnlockStatus0", 1);

		//Display the last selected bike
		_selectedBikeNumber = PlayerPrefs.GetInt ("SelectedBikeNumber", 0);

		//Set total money
		totalMoneyText.text = PlayerPrefs.GetInt ("AvailableMoney", 0).ToString ();

		//Start menu music
		AudioManager.StartMenuAudio ();

		DisplayBike ();
	}

	void Update()
	{
		//If the escape button is pressed twice
		//Then quit the game
		if(Input.GetKeyDown (KeyCode.Escape) && _quitBool)
		{
			Application.Quit ();
		}

		if(Input.anyKey)
		{
			if(Input.GetKey (KeyCode.Escape))
			{
				_quitBool = true;
			}
			else
			{
				_quitBool = false;
			}
		}
	}

	public void DisplayNextBike()
	{
		//Hide the currently displayed bike
		showcasedBikes [_selectedBikeNumber].SetActive (false);

		//If the player has reached the end of the list of all bikes
		//Then reset the bike index
		if(_selectedBikeNumber == showcasedBikes.Length - 1)
		{
			_selectedBikeNumber = 0;
		}
		else
		{
			//Otherwise increament selected bike number
			_selectedBikeNumber++;
		}

		DisplayBike ();
	}

	public void DisplayPreviousBike()
	{
		//Hide the currently displayed bike
		showcasedBikes [_selectedBikeNumber].SetActive (false);

		//If the player has reached the start of the list of all bikes
		//Then set the bike index to the last bike in the list
		if(_selectedBikeNumber == 0)
		{
			_selectedBikeNumber = showcasedBikes.Length - 1;
		}
		else
		{
			//Otherwise decreament selected bike number
			_selectedBikeNumber--;
		}

		DisplayBike ();
	}

	private void DisplayBike()
	{
		//Display the current bike
		showcasedBikes [_selectedBikeNumber].SetActive (true);

		//Set name of the next bike
		bikeNameText.text = bikes [_selectedBikeNumber].bikeName;

		//Display unlock section only if the bike is locked
		if(PlayerPrefs.GetInt ("BikeUnlockStatus" + _selectedBikeNumber.ToString (), 0) == 0)
		{
			//Set price of the bike
			bikePriceText.text = bikes [_selectedBikeNumber].bikePrice.ToString ();

			unlockSection.SetActive (true);

			//Disable the start game button
			startButton.gameObject.SetActive (false);
		}
		else
		{
			unlockSection.SetActive (false);

			//Enable the start game button
			startButton.gameObject.SetActive (true);
		}
	}

	public void UnlockBike()
	{
		//Get the price of the current bike
		int price = bikes [_selectedBikeNumber].bikePrice;

		//Get the money that the player has
		int availableMoney = PlayerPrefs.GetInt ("AvailableMoney", 0);

		//Unlock the bike only if the player has enough money to buy the vehicle
		if(availableMoney >= price)
		{
			//Play successful purchase sound
			AudioManager.PlayPurchaseSound ();

			//Count down the available money
			StartCoroutine (CountDownAnimation (availableMoney, (availableMoney - price), totalMoneyText));

			//Deduct the money from available money
			availableMoney -= price;
			PlayerPrefs.SetInt ("AvailableMoney", availableMoney);



			//Set unlock status
			PlayerPrefs.SetInt ("BikeUnlockStatus" + _selectedBikeNumber.ToString (), 1);

			//Disbale unlock section
			unlockSection.SetActive (false);

			//Enable the start game button
			startButton.gameObject.SetActive (true);

			//Update total money text
			//totalMoneyText.text = availableMoney.ToString ();
		}
		else
		{
			//Player does not have enough money to buy the vehicle


			//Play error clip
			AudioManager.PlayErrorSound ();
		}
	}

	IEnumerator CountDownAnimation(float initialValue, float targetValue, TextMeshProUGUI targetText)
	{
		float displayNumber = initialValue;
		for(float timer = 0; timer <= countAnimationTime; timer += Time.deltaTime)
		{
			float progress = timer / countAnimationTime;

			displayNumber = (int)Mathf.Lerp (initialValue, targetValue, progress);

			//Play count audio
			AudioManager.PlayCountSound ();

			targetText.text = displayNumber.ToString ();

			//yield return null;
			yield return null;
		}

		targetText.text = targetValue.ToString ();
	}

	public void SelectBike()
	{
		//Update the bike number in Game Manager
		GameManager.selectedBikeNumber = _selectedBikeNumber;

		//Store the selected bike number
		PlayerPrefs.SetInt ("SelectedBikeNumber", _selectedBikeNumber);

		//Disable the garage system
		garageSystem.SetActive (false);

		//Enable game mode selection UI
		gameModeSelector.SetActive (true);
	}

	public void SelectGameMode(int gameModeNumber)
	{
		GameManager.gameMode = gameModeNumber;
	}
}
