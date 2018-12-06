using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameData : MonoBehaviour {

	public static Dictionary<Int2, GameObject> gridBlocks;
	public static List<int> swappingShooters;

	// col,row num
	// easy 6x5 5 animals
	// medium 7x6 6 animals
	// hard 8x7 7 animals

	// easy 6x5 5 animals
	// medium 8x7 6 animals
	// hard 8x7 8 animals

	public static int numCols = 8;
	public static int numRows = 7;
	public static int numberOfAnimals = 5;

	public static string gridType = "easy";	// easy, medium, hard

	public static float xSpacing = 1.25f;
	public static float ySpacing = 1.25f;

	public static float shiftSpeed = 0.5f;

	public static float shooterStartX;
	public static float shooterStartY;

	public static GameObject shooter;
	public static string shooterType;
	public static bool swapping = false;
	//public static GameObject[] swappingShooters;

	public static int dungeonLevel = 1;
	public static int remainingBlocks = 0;
	public static bool toldUserHalfway = false;
	public static bool firstTimeIn = true;

	public static int score = 0;

	public static string gameState = "waiting";
	public static string lastGameState = "";
	public static bool instructionsGone = false;

	public static bool playMusic = true;
	public static bool playSoundFX = true;
	public Button musicOnSwitch;
	public Button soundFXOnSwitch;
	public Button musicOffSwitch;
	public Button soundFXOffSwitch;

	public static GameObject skipLevelBtn;

	public static float screenTop;
	public static float screenLeft;
	public static float screenBottom;
	public static float screenRight;
	public static Resolution screenRes;

	void Start()
	{
		swappingShooters = new List<int>();
		GetScreenCoords();

		playMusic = PlayerPrefs.GetInt("playMusic") == 1;
		playSoundFX = PlayerPrefs.GetInt("playSoundFX") == 1;

		if (playMusic)
			SoundManager.MuteMusic(false);
		else
			SoundManager.MuteMusic(true);

		if (playSoundFX)
			SoundManager.MuteSFX(false);
		else
			SoundManager.MuteSFX(true);

		skipLevelBtn = GameObject.Find("Canvas/Skip to Next");
		skipLevelBtn.SetActive(false);
	}

	void GetScreenCoords()
	{
		Camera camera = Camera.main;
		Vector3 p;

		p = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
		screenTop = p.y;
		screenRight = p.x;

		p = camera.ViewportToWorldPoint(new Vector3(0, 0f, 0));
		screenLeft = p.x;
		screenBottom = p.y;

		screenRes = Screen.currentResolution;
	}

	// called from Swap button
	public void ResetShooter()
	{
		if (GameData.gameState == "playing" && shooter.GetComponent<Shooter>().numberSwaps > 0)
		{
			//swapping = true;
			shooter.GetComponent<Shooter>().ResetShooter();
			shooter.GetComponent<Shooter>().numberSwaps--;
			shooter.GetComponent<Shooter>().UpdateSwaps();
			//swappingShooters[swappingShooters.Length+1] = shooter;
		}
	}
		
	public void PauseGame()
	{
		if (playMusic)
		{
			musicOnSwitch.gameObject.SetActive(true);
			musicOffSwitch.gameObject.SetActive(false);
		}
		else
		{
			musicOnSwitch.gameObject.SetActive(false);
			musicOffSwitch.gameObject.SetActive(true);
		}

		if (playSoundFX)
		{
			soundFXOnSwitch.gameObject.SetActive(true);
			soundFXOffSwitch.gameObject.SetActive(false);
		}
		else
		{
			soundFXOnSwitch.gameObject.SetActive(false);
			soundFXOffSwitch.gameObject.SetActive(true);
		}

		Time.timeScale = 0;

	}
	public void ResumeGame()
	{
		Time.timeScale = 1;
	}

}
