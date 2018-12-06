using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTutorial : MonoBehaviour {

	void Awake()
	{
		if (PlayerPrefs.GetInt("skipTutorial") == 1)
			SceneManager.LoadScene("Play");
	}

	void Start () {
		
		if (PlayerPrefs.GetInt("playMusic", 1) == 0)
			SoundManager.MuteMusic(true);

		if (PlayerPrefs.GetInt("playSoundFX", 1) == 0)
			SoundManager.MuteSFX(true);	
	}

	public void SkipTutorial(bool skip)
	{
		int skipInt = 0;
		if (skip)
			skipInt = 1;
		PlayerPrefs.SetInt("skipTutorial", skipInt);
	}

}
