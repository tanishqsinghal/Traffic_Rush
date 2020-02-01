using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
	private int _currentTutorialNumber = 0;

	public float nextTriggerDistance;
	public GameObject[] tutorialCanvases;
	public GameObject tutorialCompletionCanvas;

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer == LevelController.layerPlayerToInt)
		{
			ShowTutorial ();
		}
	}

	void ShowTutorial()
	{
		if(_currentTutorialNumber < tutorialCanvases.Length)
		{
			Vector3 newTriggerPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z + nextTriggerDistance);

			transform.position = newTriggerPosition;

			for(int i = 0; i < tutorialCanvases.Length; i++)
			{
				tutorialCanvases [i].SetActive (false);
			}

			tutorialCanvases [_currentTutorialNumber].SetActive (true);
			_currentTutorialNumber++;
		}
		else
		{
			for(int i = 0; i < tutorialCanvases.Length; i++)
			{
				tutorialCanvases [i].SetActive (false);
			}

			Time.timeScale = 0f;

			tutorialCompletionCanvas.SetActive (true);
		}
	}
}
