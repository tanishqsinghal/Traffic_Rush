using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forward_Movement : MonoBehaviour
{
	static Forward_Movement current;						//The class holds static reference to itself

	private bool _isBoosting = false;						//Check whether the boost is up or not
	private bool _isBraking = false;						//Check whether the brakes are applied or not
	private float _currentSpeed = 0f;						//Current forward moving speed of the racer


	[SerializeField] float forwardSpeed = 5f;				//Speed by which the racer move forward
	//[SerializeField] float boostMultiplier = 1.5f;			//Racer speed will be increamented by this multiplier when boosting

	void Awake()
	{
		current = this;

		//Set current speed of the racer to the base speed
		_currentSpeed = forwardSpeed;
	}

	//Get the current speed from anywhere
	public static float GetRacerSpeed()
	{
		return current._currentSpeed;
	}

    // Update is called once per frame
    void Update()
    {
		//If the racer is boosting
		//Then increament the traffic racer speed
		//else revert the speed back to normal
		if(_isBoosting)
		{
			float boostedSpeed = forwardSpeed * 2f - 2f;
			_currentSpeed = Mathf.Lerp (_currentSpeed, boostedSpeed, LevelController.AccelerationSpeed () * Time.deltaTime);
		}
		else
		{
			//If brakes are applied
			//then stop the racer
			if(_isBraking)
			{
				_currentSpeed = Mathf.Lerp (_currentSpeed, 0, LevelController.DeAccelerationSpeed () * Time.deltaTime);
			}
			else
			{
				_currentSpeed = Mathf.Lerp (_currentSpeed, forwardSpeed, LevelController.DeAccelerationSpeed () * Time.deltaTime);
			}
		}

		//Move the transform in forward direction with forward speed
		transform.Translate (Vector3.forward * _currentSpeed * Time.deltaTime);
    }

	public void EnableBoost()
	{
		//Enable Boost and disable brakes
		_isBoosting = true;
		_isBraking = false;
	}

	public void DisableBoost()
	{
		_isBoosting = false;
	}

	public void EnableBrakes()
	{
		//Enable Brakes and disable boost
		_isBraking = true;
		_isBoosting = false;
	}

	public void DisableBrakes()
	{
		_isBraking = false;
	}

	public static bool IsBoosting()
	{
		return current._isBoosting;
	}
}
