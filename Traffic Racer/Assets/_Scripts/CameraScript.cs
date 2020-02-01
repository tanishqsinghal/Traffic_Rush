using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	static CameraScript current;

	Vector3 cameraInitialPosition;
	public float shakeMagnetudeX = 0.05f, shakeMagnetudeY = 0.05f, shakeMagnetudeZ = 0.05f, shakeTime = 0.5f, hardness = 0.005f;
	public Camera mainCamera;

	float startTime, thisTime;

	bool isShaking = false;

	void Awake()
	{
		current = this;
	}

	void Start()
	{
		current.cameraInitialPosition = current.mainCamera.transform.localPosition;
	}

	public static void ShakeCamera(float magnitudeX,float magnitudeY,float magnitudeZ,float shakeDuration,float shakeHardness)
	{
		//Shake the camera only if it is not already shaking
		if(!current.isShaking)
		{
			//Set the shake properties according to the parameters
			current.shakeMagnetudeX = magnitudeX;
			current.shakeMagnetudeY = magnitudeY;
			current.shakeMagnetudeZ = magnitudeZ;
			current.shakeTime = shakeDuration;
			current.hardness = shakeHardness;

			current.startTime = Time.unscaledTime;

			current.isShaking = true;

			current.StartCoroutine ("StartCameraShaking");
		}
	}

	IEnumerator StartCameraShaking()
	{
		float cameraShakingOffsetX = Random.value * shakeMagnetudeX * 2 - shakeMagnetudeX;
		float cameraShakingOffsetY = Random.value * shakeMagnetudeY * 2 - shakeMagnetudeY;
		float cameraShakingOffsetZ = Random.value * shakeMagnetudeZ * 2 - shakeMagnetudeZ;
		Vector3 cameraIntermadiatePosition = mainCamera.transform.localPosition;
		cameraIntermadiatePosition.x += cameraShakingOffsetX;
		cameraIntermadiatePosition.y += cameraShakingOffsetY;
		cameraIntermadiatePosition.z += cameraShakingOffsetZ;
		mainCamera.transform.localPosition = cameraIntermadiatePosition;

		yield return new WaitForSecondsRealtime (hardness);

		thisTime = Time.unscaledTime - startTime;
		if(thisTime < shakeTime)
		{
			StartCoroutine ("StartCameraShaking");
			yield return null;
		}
		else
		{
			StopCameraShaking ();
		}
	}

	void StopCameraShaking()
	{
		mainCamera.transform.localPosition = cameraInitialPosition;

		isShaking = false;
	}
}
