using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer_Controller : MonoBehaviour
{
	private Vector3 _direction = Vector3.zero;			//Direction of the device rotation obtained from accelerometer input
	private float _turn_directionThreshold = 0.05f;		//Minimum value for which the device rotation has no effect on movement

	[SerializeField]
	float turnspeed = 1f;								//Turn speed of the racer

    // Update is called once per frame
    void Update()
    {
		//Accelerometer input from mobile device on each frame
		AccelerometerInput ();
    }

	void AccelerometerInput()
	{
		//clamp the position of the racer to certain boundaries
		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -4f, 4f), transform.position.y, transform.position.z);

		//Take input _direction from mobile accelerometer sensor
		_direction.x = Input.acceleration.x;

		//Turn the racer only if
		//The device rotation is greather than the threshold
		if(Mathf.Abs (_direction.x) > _turn_directionThreshold)
		{
			//Turn the racer according to the device rotation input
			transform.Translate (_direction * turnspeed * Time.deltaTime);
		}
		else
		{
			//Do not turn the racer
		}
	}

	void OnCollisionEnter(Collision col)
	{
		GameObject collidedObject = col.gameObject;

		//When the racer collides with an enemy vehicle
		//then initiate accident situation
		if(collidedObject.layer == LevelController.layerEnemyToInt)
		{
			//Get the reference to the coliided vehicle's script
			VehicleController vehicleScript = collidedObject.GetComponent<VehicleController> ();

			//If this vehicle can be destroyed and the racer is boosting
			//Then destroy this vehicle
			if(vehicleScript.IsDestructible () && Forward_Movement.IsBoosting ())
			{
				vehicleScript.DestroyVehicle ();
			}
			else
			{
				GameOver ();
			}
		}
	}

	void GameOver()
	{
		LevelController.RestartScene ();
	}
}
