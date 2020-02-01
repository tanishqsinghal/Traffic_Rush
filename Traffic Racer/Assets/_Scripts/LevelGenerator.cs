using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public GameObject initialLevelPart;					//Default Level Part
	public GameObject[] levelPartsAll;					//Different Level Parts of of all types

	private GameObject[,] levelPartsByType;				//Different Level Parts grouped according to their types

	public Transform playerTransform;					//Player Transform

	[SerializeField] float minDistanceForSpawn = 10f;	//Minimum Distance Between Player Transform And End Point Of The Current Level Part To Spawn Next Part

	private Vector3 _lastEndPoint;						//End Point Of Last Level Part
	private GameObject[] _activeLevelParts;				//Level Parts Which Are CUrrently Active In The Scene
	private int _levelPartNumber = 0;					//Number Of Newly Spawned Level Part

	private int _currentLevelPartType = 0;				//The type of currently active level part series
	private int _partNumberCurrentType = 0;				//Number of instantiated part according to the part type

	void Awake()
	{
		//Initialize the levelparts array
		levelPartsByType = new GameObject[5, 6];

		int copyIndex = 0;
		for(int i = 0; i < levelPartsByType.GetLength (0); i++)
		{
			for(int j = 0; j < levelPartsByType.GetLength (1); j++)
			{
				levelPartsByType[i, j] = levelPartsAll [copyIndex];

				copyIndex++;
			}
		}

		_activeLevelParts = new GameObject[4];

		_activeLevelParts [0] = initialLevelPart;
		_levelPartNumber++;
		//GameManager.current_levelPartNumber = _levelPartNumber;

		//Get Random Level Part From The List Of All Parts
		_currentLevelPartType = Random.Range (0, levelPartsByType.GetLength (0));

		//Instantiate New Level Part At End Point Of Last Level Part
		_activeLevelParts[1] = Instantiate (levelPartsByType [_currentLevelPartType, _partNumberCurrentType], initialLevelPart.transform.GetChild (0).transform.position, transform.rotation);

		//Increament the part number of current type
		_partNumberCurrentType++;

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
		_levelPartNumber++;

		if(_levelPartNumber > 2)
		{
			_levelPartNumber = 0;
		}

		if(_activeLevelParts[_levelPartNumber] != null)
		{
			Destroy (_activeLevelParts[_levelPartNumber]);
		}

		//If the all parts of the current type have been spawned
		//Then choose another level part type randomly
		if(_partNumberCurrentType >= levelPartsByType.GetLength (1))
		{
			//Get Random Level Part From The List Of All Parts
			_currentLevelPartType = Random.Range (0, levelPartsByType.GetLength (0));

			//Reset the level part number of current type
			_partNumberCurrentType = 0;

			//Instantiate New Level Part At End Point Of Last Level Part
			_activeLevelParts[_levelPartNumber] = Instantiate (levelPartsByType [_currentLevelPartType, _partNumberCurrentType], _lastEndPoint, transform.rotation);
			_partNumberCurrentType++;

			//Set The Spawn Position For Next Level Part To The End Point Of This Level Part
			_lastEndPoint = _activeLevelParts[_levelPartNumber].transform.GetChild (0).transform.position;
		}
		else
		{
			//Otherwise spawn the next level part of same type
			_activeLevelParts[_levelPartNumber] = Instantiate (levelPartsByType [_currentLevelPartType, _partNumberCurrentType], _lastEndPoint, transform.rotation);
			_partNumberCurrentType++;

			//Set The Spawn Position For Next Level Part To The End Point Of This Level Part
			_lastEndPoint = _activeLevelParts[_levelPartNumber].transform.GetChild (0).transform.position;
		}
	}
}
