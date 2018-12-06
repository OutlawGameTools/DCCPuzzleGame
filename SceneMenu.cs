using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class SceneMenu : MonoBehaviour {

	public Button scoreButton;
	public GameObject quitButton;

	// Use this for initialization
	void Start () {
	
		//Debug.Log("PlayerPrefs.GetInt(\"playMusic\") " + PlayerPrefs.GetInt("playMusic"));

		if (PlayerPrefs.GetInt("playMusic", 1) == 0)
			SoundManager.MuteMusic(true);
		
		if (PlayerPrefs.GetInt("playSoundFX", 1) == 0)
			SoundManager.MuteSFX(true);

		#if !UNITY_STANDALONE
			Social.localUser.Authenticate (ProcessAuthentication);
		#endif

		//GameObject.Find("GCScores").GetComponent<Button>().interactable = GameCenter.gcIsInitialized;

		#if UNITY_ANDROID || UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			scoreButton.gameObject.SetActive(false);
		#endif

		// turn it off for mobile builds
		quitButton.SetActive(false);
		#if UNITY_STANDALONE
		quitButton.SetActive(true);
		#endif


		}

	// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server.
	void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("GC: Authenticated");

			GameObject.Find("GCScores").GetComponent<Button>().interactable = true;
			// Request loaded achievements, and register a callback for processing them
			//Social.LoadAchievements (ProcessLoadedAchievements);
		}
		else
		{
			Debug.Log ("GC: Failed to authenticate");
			GameObject.Find("GCScores").GetComponent<Button>().interactable = false;
		}
	}


	public void GoPlayMaybe()
	{
		if (PlayerPrefs.GetInt("skipTutorial") == 1)
			SceneManager.LoadScene("Play");
		else
			SceneManager.LoadScene ("Tutorial");
	}

}
