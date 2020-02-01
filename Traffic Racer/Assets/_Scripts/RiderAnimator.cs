using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderAnimator : MonoBehaviour
{
	Animator riderAnimator;

	//public Rigidbody racerBodyRoot;						//Root of the racer body

	[Header ("Bike Set Positions")]
	Transform leftFootRest;									//Left foot rest on the bike
	Transform rightFootRest;								//Right foot rest on the bike
	Transform leftPalmRest;									//Left palm rest on the bike
	Transform rightPalmRest;								//Right Palm rest on the bike
	Transform hipsRest;										//hips rest on the bike
	//public Transform headRest;							//head rotation on the bike

	//public Transform riderHead;							//Head of the racer

    // Start is called before the first frame update
    void Start()
    {
		//Get the active bike object
		GameObject activeBikeObject = LevelController.activeBikeObject;

		//Get the rest positions of the currently active bike model
		leftFootRest  = activeBikeObject.transform.GetChild (1).transform.GetChild (0).transform;
		rightFootRest = activeBikeObject.transform.GetChild (1).transform.GetChild (1).transform;
		leftPalmRest  = activeBikeObject.transform.GetChild (1).transform.GetChild (2).transform;
		rightPalmRest = activeBikeObject.transform.GetChild (1).transform.GetChild (3).transform;
		hipsRest 	  = activeBikeObject.transform.GetChild (1).transform.GetChild (4).transform;

		riderAnimator = GetComponent<Animator> ();
    }

	void OnAnimatorIK()
	{
		if(!LevelController.IsGameOver ())
		{
			riderAnimator.bodyPosition = hipsRest.position;
			riderAnimator.bodyRotation = hipsRest.rotation;

			//Set rider's head rotatio
			//riderHead.rotation = headRest.rotation;

			riderAnimator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, 1);
			riderAnimator.SetIKPosition (AvatarIKGoal.LeftFoot, leftFootRest.position);
			riderAnimator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, 1);
			riderAnimator.SetIKRotation (AvatarIKGoal.LeftFoot, leftFootRest.rotation);

			riderAnimator.SetIKPositionWeight (AvatarIKGoal.RightFoot, 1);
			riderAnimator.SetIKPosition (AvatarIKGoal.RightFoot, rightFootRest.position);
			riderAnimator.SetIKRotationWeight (AvatarIKGoal.RightFoot, 1);
			riderAnimator.SetIKRotation (AvatarIKGoal.RightFoot, rightFootRest.rotation);

			riderAnimator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
			riderAnimator.SetIKPosition (AvatarIKGoal.LeftHand, leftPalmRest.position);
			riderAnimator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);
			riderAnimator.SetIKRotation (AvatarIKGoal.LeftHand, leftPalmRest.rotation);

			riderAnimator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
			riderAnimator.SetIKPosition (AvatarIKGoal.RightHand, rightPalmRest.position);
			riderAnimator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);
			riderAnimator.SetIKRotation (AvatarIKGoal.RightHand, rightPalmRest.rotation);
		}
		else
		{
			//transform.parent = null;

			//float appliedForce = Random.Range (50, 100);

			//racerBodyRoot.AddForce (transform.forward * appliedForce, ForceMode.Impulse);

			Destroy (GetComponent<Animator> ());
		}
	}
}
