using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class CreateGrid : MonoBehaviour {

	public GameObject createGridParticle;
	public GameObject animalPrefab;
	public Sprite[] animalSprites;
	
	public float startX = 400f;
	public float startY = 0f;

	[Header("Shooter")]
	public GameObject shooterPrefab;

	GameObject animal;

	void Start()
	{
		Camera camera = Camera.main;
		Vector3 p = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0));
		startX = p.x + 0.6f;
		startY = p.y + 0.6f;

		GameObject.Find("BGCanvas/Image").GetComponent<RectTransform>().sizeDelta = new Vector2(GameData.screenRes.width*2, GameData.screenRes.height);
		//GameObject.Find("BGCanvas/Image").GetComponent<Image>().SetNativeSize();

		GameData.dungeonLevel = 1;

		StartLevel();

		//IOSRateUsPopUp rate = IOSRateUsPopUp.Create();
		//IOSDialog dialog = IOSDialog.Create("Dialog Titile", "Dialog message");

//		GameCenterManager.init();
//		string leaderBoardId= "dccpuzzlemain"; 
//		GameCenterManager.showLeaderBoard(leaderBoardId);
	}

	public void SetNotFirstStart()
	{
		GameData.firstTimeIn = false;
		GameData.instructionsGone = true;
	}
	public void SetGameState(string gState)
	{
		GameData.gameState = gState;
	}

	public void StartLevel()
	{
		if (GameData.gameState == "lost")
		{
			// do some things like reset score, etc.
			GameData.score = 0;
			GameObject.Find("Canvas/BlankWindow/Score/Score").GetComponent<Text>().text = GameData.score.ToString("N0");
		}

		GameData.gameState = "waiting";

		//Destroy(GameData.shooter);
		if (GameData.gridBlocks != null)
			DitchAllGridBlocks();

		CreateAGrid();

		if (GameData.shooter == null)
			CreateShooter();
		else
		{
			GameData.shooter.GetComponent<Shooter>().ResetShooter();
			GameData.shooter.GetComponent<Shooter>().numberSwaps = 3;
			GameData.shooter.GetComponent<Shooter>().UpdateSwaps();
		}

		GameData.toldUserHalfway = false;

		if (!GameData.firstTimeIn)
			GameData.gameState = "playing";

		GameObject.Find("Canvas/LevelNumber").GetComponent<Text>().text = "Level " + GameData.dungeonLevel;

	}
		

	public void CreateShooter()
	{
		GameData.shooter = Instantiate(shooterPrefab);
		GameData.shooterType = GameData.shooter.GetComponent<Shooter>().blockType;
	}

	void CreateAGrid () {

//		float rndX = Random.Range(GameData.screenLeft+3, GameData.screenRight-3);
//		GameObject bats = CFX_SpawnSystem.GetNextObject(createGridParticle);
//		bats.transform.position = new Vector3(rndX, GameData.screenBottom, 0);


		GameData.gridBlocks = new Dictionary<Int2, GameObject>();

		float gridCount = 0.2f;

		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				int arrayIdx = Random.Range (0, GameData.numberOfAnimals);
				Sprite pickupSprite = animalSprites[arrayIdx];
				string pickupName = pickupSprite.name;

				Vector2 endPos = new Vector2(startX + (i * GameData.xSpacing), startY + (j * GameData.ySpacing));
				Vector2 startPos = endPos + (Vector2.up * 15);

				animal = Instantiate(animalPrefab, startPos, Quaternion.identity) as GameObject;

				//Debug.Log(animal.transform.position);

				Int2 gridPos = new Int2(i,j);
				animal.name = pickupSprite.name;
				animal.GetComponent<Block>().blockType = pickupSprite.name;
				animal.GetComponent<Block>().myGridPos = gridPos;

				SpriteRenderer[] spriteRenderers;
				spriteRenderers = animal.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spr in spriteRenderers) {
					if (spr.gameObject.name == "Sprite")
						spr.sprite = pickupSprite;
				}

				//animal.GetComponentInChildren<SpriteRenderer>().sprite = pickupSprite;

				if (animal.name == "pig")
				{
					AudioClip vv = Resources.Load("Audio/pig") as AudioClip;
					animal.GetComponent<AudioSource>().clip = vv;
				}

				//print (i + " " + j + " " + pickupName);

				GameData.gridBlocks.Add(gridPos, animal);

				animal.transform.DOMoveY(endPos.y, 0.1f * gridCount);
				gridCount += 0.2f;
			}
		}

		// set shooter position based on grid size

		GameData.shooterStartX = animal.transform.position.x + (GameData.xSpacing * 1f);
		GameData.shooterStartY = startY;

		//GameData.gameState = "playing";
	}

	void DitchAllGridBlocks()
	{
		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				if (GameData.gridBlocks[gridPos] != null)
					Destroy(GameData.gridBlocks[gridPos]);
			}
		}

		GameData.gridBlocks.Clear();
	}

	public void ToggleMusic()
	{
		int playing = 0;
		if (GameData.playMusic)
		{
			GameData.playMusic = false;
			SoundManager.MuteMusic(true);
		}
		else
		{
			GameData.playMusic = true;
			SoundManager.MuteMusic(false);
			playing = 1;
		}

		PlayerPrefs.SetInt("playMusic", playing);
	}

	public void ToggleSoundFX()
	{
		int playing = 0;
		if (GameData.playSoundFX)
		{
			GameData.playSoundFX = false;
			SoundManager.MuteSFX(true);
		}
		else
		{
			GameData.playSoundFX = true;
			SoundManager.MuteSFX(false);
			playing = 1;
		}

		PlayerPrefs.SetInt("playSoundFX", playing);
	}

}
