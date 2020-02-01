using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroSpawner : MonoBehaviour
{
	private float _minSpawnDistance = 0f;									//Minimum spawn distance between each nitro spawned
	private float _maxSpawnDistance = 0f;									//Maximum spawn distance between each nitro spawned
	private float _distanceForNextSpawn = 0f;								//Calculated distance for next nitro spawn
	private float _lastNitroPositionZ = 0f;									//Position in Z axis where the last nitro was spawned

	public Transform[] nitroSpawnPoints;									//All locations in the level where the nitro can be spawned
	public GameObject nitroPrefab;											//Nitro prefab that has to be spawned

    // Start is called before the first frame update
    void Start()
    {
        //Calculate min and max distance between spawns
		//According to game mode to balance difficulty
		if(GameManager.gameMode == 0)
		{
			//One way mode (Normal)
			_maxSpawnDistance = LevelController.Mileage () / 2f;
			_minSpawnDistance = _maxSpawnDistance / 3f;
		}
		else if(GameManager.gameMode == 1)
		{
			//Two way mode (Difficult)
			_maxSpawnDistance = LevelController.Mileage () / 2f;
			_minSpawnDistance = _maxSpawnDistance / 3.5f;
		}
		else if(GameManager.gameMode == 2)
		{
			//Free ride mode (Easy)
			_maxSpawnDistance = LevelController.Mileage () / 1.25f;
			_minSpawnDistance = _maxSpawnDistance / 2.5f;
		}

		//Spawn nitro in the start
		SpawnNitro ();

		//Store the current position as last spawned position
		_lastNitroPositionZ = nitroSpawnPoints [0].position.z;
    }

    // Update is called once per frame
    void Update()
    {
		//If the distance between last spawn point and current spawn point is greater than the calculated distance for next spawn
		//Then spawn new vehicle
		if(Mathf.Abs (nitroSpawnPoints [0].position.z - _lastNitroPositionZ) > _distanceForNextSpawn)
		{
			SpawnNitro ();

			//Store the current position as last spawned position
			_lastNitroPositionZ = nitroSpawnPoints [0].position.z;
		}
    }

	void SpawnNitro()
	{
		int randomSpawnPoint = 0;

		//Select random spawn point
		if(GameManager.gameMode == 1)
		{
			randomSpawnPoint = Random.Range (2, nitroSpawnPoints.Length);
		}
		else
		{
			randomSpawnPoint = Random.Range (0, nitroSpawnPoints.Length);
		}

		Instantiate (nitroPrefab, nitroSpawnPoints[randomSpawnPoint].position, Quaternion.identity);

		//Calculate random distance for the next spawn
		_distanceForNextSpawn = Random.Range (_minSpawnDistance, _maxSpawnDistance);
	}
}
