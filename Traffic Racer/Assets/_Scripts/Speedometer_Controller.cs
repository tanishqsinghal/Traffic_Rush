using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer_Controller : MonoBehaviour
{
	private float _realWorldMultiplier = 10f;					//Real world multiplier to make unity values look real

	private float _minSpeed = 0f;								//Minimum speed of the racer
	private float _maxSpeed = 0f;								//Maximum speed of the racer
	private float _speedPercentage = 0f;						//How much percent the curent speed of the racer is to max speed

	[SerializeField] float _minRotation = 0f;					//Minimum rotation allowed for the pin
	[SerializeField] float _maxRotation = 0f;					//Maximum rotation allowed for the pin

	public TextMeshProUGUI speed_Text;							//UI text which displayes the current speed of the racer
	public GameObject pin;										//Speedometer pin object

    // Start is called before the first frame update
    void Start()
    {
		//Calculate speed range
		_minSpeed = 0;
		_maxSpeed = ((((LevelController.MaximumLevelNumber () - 1f) * Forward_Movement.SpeedIncreamentPerLevel ()) + Forward_Movement.BaseSpeed ())) * 2f - 2f;

		_realWorldMultiplier = LevelController.RealWorldMultiplier ();
    }

    // Update is called once per frame
    void Update()
    {
        //Update the speed of the racer in the speedometer
		speed_Text.text = (Mathf.RoundToInt(Forward_Movement.GetRacerSpeed () * _realWorldMultiplier)).ToString ();

		//Calculate the percentage of maximum speed the racer can achieve with its current speed
		_speedPercentage = (Forward_Movement.GetRacerSpeed () * 100f) / _maxSpeed;

		//Set the rotation of the pin
		pin.transform.rotation = Quaternion.Euler (0f, 0f, ((_maxRotation - _minRotation) * _speedPercentage) / 100f);
    }
}
