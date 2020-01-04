using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	private float _currentVehicleSpeed = 0f;				//Vehicle speed randomly calculated from min and max speeds
	private float _originalVehicleSpeed = 0f;				//Vehicle speed at the start
	private bool _isChangingLane = false;					//Check whether the vehicle is currently changing the lane or not
	private bool _canChangeLanes = true;					//Can the vehicle change lanes
	private bool _isVehicleAhead = false;					//Check whether a vehicle is ahead or not

	[SerializeField] float minSpeed = 1f;					//Minimum forward speed of the vehicle
	[SerializeField] float maxSpeed = 5f;					//Maximum speed of the vehicle
	[SerializeField] float laneWidth = 2.5f;				//Width of a lane
	[SerializeField] float laneChangeSpeed = 1f;			//Lane change speed of the vehicle
	[SerializeField] bool isDestructible = false;			//Can this vehicle be destoyed by the racer

	public Transform[] forwardRaycastTransforms;			//Forward Raycast Positions on the vehicle
	public Transform laneCheckBoxTransform;					//Location from where the lanes has to be checked
	public float rayLength = 1f;							//Length of the ray
	public Vector3 laneCheckBoxSize;						//Size of the box to check if the vehicle can change its lane

	public int laneNumber = 0;								//Lane number in which the vehicle has been spawned

    // Start is called before the first frame update
    void Start()
    {
        //Calculate the random vehicle speed
		_currentVehicleSpeed = Random.Range (minSpeed, maxSpeed);

		_originalVehicleSpeed = _currentVehicleSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		//Handle all the environment checks
		HandleRayCasts ();

		//Move the vehicle forward with _currentVehicleSpeed
		transform.Translate (Vector3.forward * _currentVehicleSpeed * Time.deltaTime);

		//If the distance of this vehicle from the racer is greater than the distanceToDestroy
		//Then destroy this vehicle
		if((LevelController.RacerTransform ().position.z - transform.position.z) > LevelController.DistanceToDestroy ())
		{
			Destroy (gameObject);
		}
    }

	void HandleRayCasts()
	{
		//Get the information of the raycast hit
		RaycastHit forwardHitInformation = new RaycastHit ();

		float newVehicleSpeed = _currentVehicleSpeed;

		//Check whether the vehicle ahead is also changing its lane
		bool _isVehicleAheadChangingLane = false;

		//Shoot a ray on each ray cast position
		for(int i = 0; i < forwardRaycastTransforms.Length; i++)
		{
			if(Physics.Raycast (forwardRaycastTransforms[i].position, Vector3.forward, out forwardHitInformation, rayLength))
			{
				//Some vehicle is ahead
				_isVehicleAhead = true;

				//Get the collided object information
				GameObject collidedObject = forwardHitInformation.collider.gameObject;

				//Check whether the collided object is a vehicle or not
				if(collidedObject.layer == LevelController.layerEnemyToInt)
				{
					//Get the speed of the ray casted vehicle
					newVehicleSpeed = collidedObject.GetComponentInParent<VehicleController> ().GetVehicleSpeed ();

					//Get the lane chaging status of the vehicle ahead
					_isVehicleAheadChangingLane = collidedObject.GetComponentInParent<VehicleController> ().GetLaneChangingStatus ();
				}

				//Check if the vehicle is allowed to change the lane or not
				Collider[] colliders = Physics.OverlapBox (laneCheckBoxTransform.position, laneCheckBoxSize);

				for(int j = 0; j < colliders.Length; j++)
				{
					//If there is another object in the range other than the vehicle ahead
					//Then the vehicle is not allowed to change lanes
					/*if(colliders[j].gameObject != collidedObject)
					{
						_canChangeLanes = false;
					}*/

					if(Mathf.Abs (colliders[j].gameObject.transform.position.x - transform.position.x) > 0.05f)
					{
						_canChangeLanes = false;
					}
				}
			}
		}
		//Handle vehicle speed according to the raycast information
		HandleVehicleSpeed (newVehicleSpeed, _isVehicleAheadChangingLane);
	}

	void HandleVehicleSpeed(float newVehicleSpeed, bool _isVehicleAheadChangingLane)
	{
		//If some vehicle is ahead
		//then match its speed
		if(_isVehicleAhead)
		{
			//If the vehicle is not already changing lane and is allowed to change lanes
			//Then change its current lane
			if(!_isChangingLane && _canChangeLanes)
			{
				//If the collided object is a vehicle then match its speed
				_currentVehicleSpeed = Mathf.Lerp (_currentVehicleSpeed, newVehicleSpeed, LevelController.DeAccelerationSpeed () * Time.deltaTime);

				//If the vehicle ahead is not already changing lane
				//Then only change this vehicle lane
				if(!_isVehicleAheadChangingLane)
				{
					StartChangeLane ();
				}
			}
			else
			{
				//If the collided object is a vehicle then match its speed
				_currentVehicleSpeed = Mathf.Lerp (_currentVehicleSpeed, newVehicleSpeed, LevelController.DeAccelerationSpeed () * Time.deltaTime);
			}
		}
		else
		{
			//If the vehicle is moving freely
			//Then change its speed to original speed
			_currentVehicleSpeed = Mathf.Lerp (_currentVehicleSpeed, _originalVehicleSpeed, LevelController.AccelerationSpeed () * Time.deltaTime);
		}

		//Reset the vehicle ahead status
		//so that it can be checked again in the Update method
		_isVehicleAhead = false;

		//Reset the can lane change status
		//so that it can be checked again in the Update method
		_canChangeLanes = true;
	}

	void StartChangeLane()
	{
		//The vehicle has started changing the lane
		_isChangingLane = true;

		//Calculate the new lane number according to the current lane number
		int newLaneNumber = laneNumber;
		switch (laneNumber)
		{
		case 0:
			newLaneNumber = 1;
			break;
		case 1:
			newLaneNumber = 0;
			break;
		case 2:
			newLaneNumber = 3;
			break;
		case 3:
			newLaneNumber = 2;
			break;
		}

		//If the new lane number is less than the current lane number
		//Then move left otherwise move right
		if(newLaneNumber < laneNumber)
		{
			//Calculate the new lane position
			float newPositionX = transform.position.x - laneWidth;

			StartCoroutine (ChangeLane (newPositionX));
		}
		else
		{
			//Calculate the new lane position
			float newPositionX = transform.position.x + laneWidth;

			StartCoroutine (ChangeLane (newPositionX));
		}

		//Change the current lane number
		laneNumber = newLaneNumber;
	}

	IEnumerator ChangeLane(float newPositionX)
	{
		//Move the vehicle until it reaches its final position
		while(Mathf.Abs (transform.position.x - newPositionX) > 0.01f)
		{
			//Calculate the new position
			Vector3 newPosition = new Vector3(newPositionX, transform.position.y, transform.position.z);

			transform.position = Vector3.Lerp (transform.position, newPosition, laneChangeSpeed * Time.deltaTime);

			yield return null;
		}

		//Now the vehicle has changed its lane
		_isChangingLane = false;

		yield return null;
	}

	void OnCollisionEnter(Collision col)
	{
		//If the vehicle has collided with another vehicle
		//Then this is accident situation
		if(col.gameObject.layer == LevelController.layerEnemyToInt)
		{
			VehicleAccident ();
		}
	}

	void VehicleAccident()
	{
		Debug.Log ("Accident");
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer == LevelController.layerPlayerToInt)
		{
			Debug.Log ("Close Pass");
		}
	}

	public void DestroyVehicle()
	{
		Debug.Log ("Destroyed");
		Destroy (gameObject);
	}

	//Draw Gizmos
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (laneCheckBoxTransform.position, laneCheckBoxSize);
	}

	public float GetVehicleSpeed()
	{
		return _currentVehicleSpeed;
	}

	public bool GetLaneChangingStatus()
	{
		return _isChangingLane;
	}

	public bool IsDestructible()
	{
		return isDestructible;
	}
}
