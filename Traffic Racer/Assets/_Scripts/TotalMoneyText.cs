using UnityEngine;
using TMPro;

public class TotalMoneyText : MonoBehaviour
{
	void Start()
	{
		//Set the total money text in game manager
		GameManager.totalMoneyText = GetComponent<TextMeshProUGUI> ();

		//Update the total money text
		GameManager.totalMoneyText.text = PlayerPrefs.GetInt ("AvailableMoney", 0).ToString ();
	}
}
