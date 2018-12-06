using UnityEngine;
using System.Collections;

public class ScenePlay : MonoBehaviour {

	// Use this for initialization
	void Start () {

		if (PlayerPrefs.GetInt("playMusic", 1) == 0)
			SoundManager.MuteMusic(true);

		if (PlayerPrefs.GetInt("playSoundFX", 1) == 0)
			SoundManager.MuteSFX(true);
	}

}
