using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
	private List<int> _lastSpawnPoints;									//Spawn point where the last vehicle was spawned
	private float _lastSpawnedPositionZ = 0f;							//Z position where the last vehicle was Spawned
	private float _distanceForNextSpawn = 50f;							//Distance between this spawn and next spawn points

	[SerializeField] float minDistanceBetweenSpawns = 0.5f;				//Minimum distance between each spawn
	[SerializeField] float maxDistanceBetweenSpawns = 5f;				//Maximum distance between each spawn
	[SerializeField] float speedThreshold = 5f;							//Maximum time delay between each spawn

	public Transform[] vehicleSpawnPoints;								//All locations in the level where the vehicles can be spawned
	public GameObject[] vehicles;										//List of all vehicles which can be spawned

    // Start is called before the first frame update
    void Start()
    {
		_lastSpawnPoints = new List<int> ();

		SpawnVehicle ();

		//Store the current position as last spawned position
		_lastSpawnedPositionZ = vehicleSpawnPoints [0].position.z;
    }

	void Update()
	{
		//If the distance between last spawn point and current spawn point is greater than the calculated distance for next spawn
		//Then spawn new vehicle
		if(Mathf.Abs (vehicleSpawnPoints [0].position.z - _lastSpawnedPositionZ) > _distanceForNextSpawn)
		{
			SpawnVehicle ();

			//Store the current position as last spawned position
			_lastSpawnedPositionZ = vehicleSpawnPoints [0].position.z;
		}
	}

	void SpawnVehicle()
	{
		//Spawn vehicles only if the current velocity of the racer is greater than the threshold speed
		if(Forward_Movement.GetRacerSpeed () > speedThreshold)
		{
			//Select random spawn point
			int randomSpawnPoint = Random.Range (0, vehicleSpawnPoints.Length);

			if(_lastSpawnPoints.Count > 1)
			{
				//Do not select the same spawn point continuously
				while(randomSpawnPoint == _lastSpawnPoints[0] || randomSpawnPoint == _lastSpawnPoints[1])
				{
					//Select random spawn point again
					randomSpawnPoint = Random.Range (0, vehicleSpawnPoints.Length);
				}

				_lastSpawnPoints.RemoveAt (0);
				_lastSpawnPoints.Add (randomSpawnPoint);
			}
			else
			{
				_lastSpawnPoints.Add (randomSpawnPoint);
			}

			//Select random vehicle from the list of all vehicles
			int randomVehicle = Random.Range (0, vehicles.Length);

			GameObject spawnedVehicle = Instantiate (vehicles[randomVehicle], vehicleSpawnPoints[randomSpawnPoint].position, Quaternion.identity);

			//Set the lane number for the spawned vehicle
			spawnedVehicle.GetComponent<VehicleController> ().laneNumber = _lastSpawnPoints[_lastSpawnPoints.Count - 1];

			//Calculate random distance for the next spawn
			_distanceForNextSpawn = Random.Range (minDistanceBetweenSpawns, maxDistanceBetweenSpawns);
		}
	}
}
