using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowTutorialPics : MonoBehaviour {

	public GameObject tutorialPic;
	public Sprite[] tutorialPics;
	int tutorialIdx = 0;

	public Button nextButton;
	public Button prevButton;
	public Button startButton;

	// Use this for initialization
	void Start () {
	
		prevButton.interactable = false;
		nextButton.interactable = false;

		// if there are more pics, enable the next button
		if (tutorialPics.Length-1 > tutorialIdx)
			nextButton.interactable = true;
	}

	public void Next()
	{
		tutorialIdx++;
		SetButtonsInteractable();
		ShowNewSprite();
	}

	public void Prev()
	{
		tutorialIdx--;
		SetButtonsInteractable();
		ShowNewSprite();
	}

	void SetButtonsInteractable()
	{
		nextButton.interactable = (tutorialIdx < tutorialPics.Length-1);
		prevButton.interactable = (tutorialIdx > 0);
	}

	void ShowNewSprite()
	{
		Sprite newSprite = tutorialPics[tutorialIdx];
		tutorialPic.GetComponent<SpriteRenderer>().sprite = newSprite;
	}
}
