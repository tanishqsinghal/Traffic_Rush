using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	static LevelController current;							//This static class holds reference to itself and mainly used to get values or references from anywhere

	[SerializeField] float accelerationSpeed = 1f;			//Time taken in transition from current speed to accelerated speed
	[SerializeField] float deAccelerationSpeed = 1f;		//Time taken in transition from current speed to deaccelerated speed

	[SerializeField] float realWorldMultiplier = 10f;		//Multiply the game speed to make it look like real world
	[SerializeField] float distanceToDestroy = 1f;			//Distance of any object from the player to destory itself

	public static int layerEnemyToInt;						//Integer representation of the layer "Enemy"
	public static int layerPlayerToInt;						//Integer representation of the layer "Player"

	public Transform racerTransform;						//Transform of the racer object

	void Awake()
	{
		current = this;

		//Get reference to Enemy layer
		layerEnemyToInt = LayerMask.NameToLayer ("Enemy");

		//Get reference to Player layer
		layerPlayerToInt = LayerMask.NameToLayer ("Player");
	}

	public static void RestartScene()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public static float AccelerationSpeed()
	{
		return current.accelerationSpeed;
	}

	public static float DeAccelerationSpeed()
	{
		return current.deAccelerationSpeed;
	}

	public static float RealWorldMultiplier()
	{
		return current.realWorldMultiplier;
	}

	public static float DistanceToDestroy()
	{
		return current.distanceToDestroy;
	}

	public static Transform RacerTransform()
	{
		return current.racerTransform;
	}
}