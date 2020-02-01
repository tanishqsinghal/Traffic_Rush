using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer_Controller : MonoBehaviour
{
	static Racer_Controller current;							//The class holds static reference to itself

	private Vector3 _direction = Vector3.zero;					//Direction of the device rotation obtained from accelerometer input
	private float _turn_directionThreshold = 0.01f;				//Minimum value for which the device rotation has no effect on movement
	private Rigidbody _racerRigidBody;							//Racer rigidbody
	private int _gameMode = 0;									//Current game mode
	private float _timeInOppositeLane = 0f;						//Time when the racer is riding in the opposite lane
	private float _currentTimeInOppositeLane = 0f;				//Time when the racer enter and start riding in the opposite lane 

	[SerializeField]
	float turnspeed = 1f;										//Turn speed of the racer
	[SerializeField] float minForce = 1f;						//Minimum force applied on the racer when collided with another vehicle
	[SerializeField] float maxForce = 9f;						//Maximum force applied on the racer when collided with another vehicle

	public GameObject racerModel;								//3D model of the racer

	void Awake()
	{
		current = this;
	}

	void Start()
	{
		_racerRigidBody = GetComponent<Rigidbody> ();

		//Get the current accelerometer sensitivity
		turnspeed = PlayerPrefs.GetFloat ("Sensitivity", turnspeed);
	
		_gameMode = GameManager.gameMode;
	}

    // Update is called once per frame
    void Update()
    {
		//Accelerometer input from mobile device on each frame and the game is not over yet
		if(!LevelController.IsGameOver ())
		{
			AccelerometerInput ();

			//Always set the racer physics velocity to 0 to stop it from misbehaving
			_racerRigidBody.velocity = Vector3.zero;

			//If the game mode is two way and the racer is boosting in the opposite lane
			//Then increament the time in opposite lane
			if(_gameMode == 1 && (Forward_Movement.IsBoosting () && transform.position.x < -0.5f))
			{
				_timeInOppositeLane += Time.deltaTime;

				//Increament current time in the opposite lane
				_currentTimeInOppositeLane += Time.deltaTime;
			}
			else
			{
				//If the racer is not riding in the opposite lane
				//Then reset the timer
				_currentTimeInOppositeLane = 0f;
			}
			//Update Opposite lane timer UI
			LevelController.UpdateOppositeLaneTimer (_currentTimeInOppositeLane);
		}
    }

	void LateUpdate()
	{
		//Always set the racer physics velocity to 0 to stop it from misbehaving
		_racerRigidBody.velocity = Vector3.zero;
	}

	void AccelerometerInput()
	{
		//clamp the position of the racer to certain boundaries
		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -5f, 5f), transform.position.y, transform.position.z);

		//Take input _direction from mobile accelerometer sensor
		_direction.x = Input.acceleration.x;

		//Turn the racer only if
		//The device rotation is greather than the threshold
		//and the racer is moving forward
		if(Mathf.Abs (_direction.x) > _turn_directionThreshold && Forward_Movement.GetRacerSpeed () > 1f)
		{
			//Turn the racer according to the device rotation input
			transform.Translate (_direction * turnspeed * Time.deltaTime);

			if(_direction.x > 0)
			{
				//Turn the racer model
				racerModel.transform.rotation = Quaternion.Slerp (racerModel.transform.rotation, Quaternion.Euler (0f, 5f, -15f), turnspeed * _direction.x * Time.deltaTime);
				//racerModel.transform.eulerAngles = Vector3.Lerp (racerModel.transform.eulerAngles, new Vector3 (-90f, 35f, 10f), turnspeed * Time.deltaTime);
			}
			else
			{
				//Turn the racer model
				racerModel.transform.rotation = Quaternion.Slerp (racerModel.transform.rotation, Quaternion.Euler (0f, -5f, 15f), turnspeed * (- _direction.x) * Time.deltaTime);
				//racerModel.transform.eulerAngles = Vector3.Lerp (racerModel.transform.eulerAngles, new Vector3 (-90f, -35f, -10f), turnspeed * Time.deltaTime);
			}
		}
		else
		{
			//Do not turn the racer
			//Turn the racer model
			racerModel.transform.rotation = Quaternion.Slerp (racerModel.transform.rotation, Quaternion.Euler (0f, 0f, 0f), turnspeed * Time.deltaTime / 2.5f);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		//Check collision if the game is not already over
		if(!LevelController.IsGameOver ())
		{
			GameObject collidedObject = col.gameObject;

			//When the racer collides with an enemy vehicle
			//then initiate accident situation
			if(collidedObject.layer == LevelController.layerEnemyToInt)
			{
				//Get the reference to the coliided vehicle's script
				VehicleController vehicleScript = collidedObject.GetComponent<VehicleController> ();

				//Get the collision direction
				Vector3 collisionDirection = col.gameObject.transform.position - transform.position;

				//If this vehicle can be destroyed and the racer is boosting
				//Then destroy this vehicle
				if(vehicleScript.IsDestructible () && Forward_Movement.IsBoosting ())
				{
					//collisionDirection = collisionDirection.normalized;

					vehicleScript.DestroyVehicle (collisionDirection);
				}
				else
				{
					if(!LevelController.IsGameOver ())
					{
						//Play collision audio
						AudioManager.PlayDeathAudio ();

						LevelController.GameOver ();
					}

					//Damage the collided vehicle
					vehicleScript.VehicleAccident (collisionDirection);
				}
			}

			//If the player has collided with the guardrails
			//then also accident situation
			if(collidedObject.layer == LevelController.layerGuardRailToInt)
			{
				if(!LevelController.IsGameOver ())
				{
					//Play collision audio
					AudioManager.PlayDeathAudio ();

					LevelController.GameOver ();
				}
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		//If the racer has picked up a nitro
		//Then refill its nitro amount
		if(col.gameObject.layer == LevelController.layerNitroToInt && !LevelController.IsGameOver ())
		{
			//Play nitro pick-up audio
			AudioManager.PlayNitroPickUpAudio ();

			//Call the pickup nitro function on the player wrapper
			GetComponentInParent<Forward_Movement> ().PickupNitro ();

			//Destroy nitro object
			Destroy (col.gameObject.transform.parent.gameObject);
		}

		//If the racer is near the guardrails
		//then enable danger effect
		if(col.gameObject.layer == LevelController.layerGuardRailToInt)
		{
			LevelController.GuardRailEffect ().SetActive (true);
		}
	}

	void OnTriggerExit(Collider col)
	{
		//If the racer moves away from the guardrails
		//then disable danger effect
		if(col.gameObject.layer == LevelController.layerGuardRailToInt)
		{
			LevelController.GuardRailEffect ().SetActive (false);
		}
	}

	public static void GameOver()
	{
		current._racerRigidBody.constraints = RigidbodyConstraints.None;

		//float appliedForce = Random.Range (minForce, maxForce);

		//_racerRigidBody.AddForce ((Vector3.forward + Vector3.up) * appliedForce, ForceMode.Impulse);
	}

	public void ControlSensitivity(float sensitivity)
	{
		//Adjust accelerometer sensitivity
		turnspeed = sensitivity;

		//Save the current sensitivity
		PlayerPrefs.SetFloat ("Sensitivity", sensitivity);
	}

	public static float TimeInOppositeLane()
	{
		if(GameManager.gameMode == 1)
		{
			return current._timeInOppositeLane;
		}
		else
		{
			return 0f;
		}
	}
}
