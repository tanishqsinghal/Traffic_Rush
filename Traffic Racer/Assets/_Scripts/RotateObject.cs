using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
	[SerializeField] Vector3 rotationAxis = Vector3.zero;			//Defined axis on which the object will rotate
	[SerializeField] float rotationSpeed = 0f;						//Speed by which the object will rotate

    // Update is called once per frame
    void Update()
    {
		//Rotate the object
		transform.Rotate (rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
