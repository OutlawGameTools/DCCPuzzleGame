using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToSceneButton : MonoBehaviour {
	
	public void GoToScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}

	public void GoToURL(string url)
	{
		Application.OpenURL(url);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

}
