using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public GameObject initialLevelPart;					//Default Level Part
	public GameObject[] levelParts;						//Different Level Parts For Endless Level

	public Transform playerTransform;					//Player Transform

	[SerializeField] float minDistanceForSpawn = 10f;	//Minimum Distance Between Player Transform And End Point Of The Current Level Part To Spawn Next Part

	private Vector3 _lastEndPoint;						//End Point Of Last Level Part
	private GameObject[] _activeLevelParts;				//Level Parts Which Are CUrrently Active In The Scene
	private int _levelPartNumber = 0;					//Number Of Newly Spawned Level Part

	void Awake()
	{
		_activeLevelParts = new GameObject[4];

		_activeLevelParts [0] = initialLevelPart;
		_levelPartNumber++;
		//GameManager.current_levelPartNumber = _levelPartNumber;

		//Get Random Level Part From The List Of All Parts
		int randomLevelPart = Random.Range (0, levelParts.Length);

		//Instantiate New Level Part At End Point Of Last Level Part
		_activeLevelParts[1] = Instantiate (levelParts [randomLevelPart], initialLevelPart.transform.GetChild (0).transform.position, transform.rotation);

		//Set The Spawn Position For Next Level Part To The End Point Of This Level Part
		_lastEndPoint = _activeLevelParts[1].transform.GetChild (0).transform.position;
	}

	void Update()
	{
		//If The Distance Between Player And The End Point Of Current Level Part Is Less Than The Minimum Distance
		//Then Spawn New Level Part
		if((_lastEndPoint.z - playerTransform.position.z) < minDistanceForSpawn)
		{
			SpawnLevelPart ();
		}
	}

	void SpawnLevelPart()
	{
		//Get Random Level Part From The List Of All Parts
		int randomLevelPart = ChooseRandomPart (levelParts.Length);

		_levelPartNumber++;

		if(_levelPartNumber > 2)
		{
			_levelPartNumber = 0;
		}

		if(_activeLevelParts[_levelPartNumber] != null)
		{
			Destroy (_activeLevelParts[_levelPartNumber]);
		}

		//Instantiate New Level Part At End Point Of Last Level Part
		_activeLevelParts[_levelPartNumber] = Instantiate (levelParts [randomLevelPart], _lastEndPoint, transform.rotation);

		//Set The Spawn Position For Next Level Part To The End Point Of This Level Part
		_lastEndPoint = _activeLevelParts[_levelPartNumber].transform.GetChild (0).transform.position;

		//GameManager.current_levelPartNumber = _levelPartNumber;
	}

	int ChooseRandomPart(int length)
	{
		int[] randomArray = new int[length];

		for(int i = 0; i < length; i++)
		{
			randomArray [i] = Random.Range (0, length);
		}

		int randomIndex = Random.Range (0, randomArray.Length);

		return randomArray [randomIndex];
	}
}
