using UnityEngine;
using System.Collections;

public class SceneAbout : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Debug.Log("PlayerPrefs.GetInt(\"playMusic\") " + PlayerPrefs.GetInt("playMusic"));

		if (PlayerPrefs.GetInt("playMusic", 1) == 0)
			SoundManager.MuteMusic(true);

		if (PlayerPrefs.GetInt("playSoundFX", 1) == 0)
			SoundManager.MuteSFX(true);
	}

	public void SkipTutorial(bool show)
	{
		int showInt = 1;
		if (show)
			showInt = 0;
		PlayerPrefs.SetInt("skipTutorial", showInt);
	}

}
