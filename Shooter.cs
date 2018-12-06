using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;

public class Shooter : MonoBehaviour {

	public Text msg;
	public Text swapBtnText;
	public Text	swapText;
	public Text highScoreText;

	public Sprite[] animalSprites;

	public string blockType = "animal";

	public GameData gameData;

	[SerializeField]
	float speed = 0.1f;
	public float moveDistance = 1.5f;

	[SerializeField]
	int myRow = 0;
	[SerializeField]
	int myCol = -1;	//-1 means it's not over a column; ready to push sideways

	bool isMoving = false;

	float endPosY;
	float endPosX;

	public int numberSwaps = 3;

	Vector3 lastPosition;

	public GameObject noMatchPrefab;
	public GameObject wonLevelPrefab;
	public GameObject lostLevelPrefab;
	public GameObject sparklePrefab;
	public GameObject shooterPrefab;
	public GameObject nextLevelBtn;
	public Text winLoseMsg;

	int lastSpriteIdx = -1;

	int numberOfShooters = 0;

	GameObject redX;

	CreateGrid createGrid;

	void Start () 
	{
		numberSwaps = 3;
		//swapText = GameObject.Find("Canvas/Swaps").GetComponent<Text>();
		swapBtnText = GameObject.Find("Canvas/BlankWindow/SwapAnimal/Text").GetComponent<Text>();
		nextLevelBtn =  GameObject.Find("Canvas/NextPuzzle");
		nextLevelBtn.SetActive(false);
		msg = GameObject.Find("Canvas/Message").GetComponent<Text>();
		winLoseMsg = GameObject.Find("Canvas/WinLoseMessage").GetComponent<Text>();
		gameData = GameObject.Find("SceneManager").GetComponent<GameData>();
		highScoreText = GameObject.Find("Canvas/BlankWindow/HighScore/Score").GetComponent<Text>();

		highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString("N0");

		redX = GameObject.Find("redx");
		Vector3 swapButtonPos = GameObject.Find("Canvas/BlankWindow/SwapAnimal").GetComponent<RectTransform>().TransformPoint(Camera.main.transform.position);
		redX.transform.position = swapButtonPos + Vector3.up;

		GameData.shooter.transform.position = redX.transform.position;
		redX.SetActive(false);

		ResetShooter();
		while (!CheckForEdgeMatch())
			GetRandomAnimalSprite();

		#if UNITY_IOS || UNITY_ANDROID
			//GameObject.Find("SwipeController").GetComponent<SwipeControl>().SetMethodToCall( SwipeCallback );
		#endif

		createGrid = GameObject.Find("SceneManager").GetComponent<CreateGrid>();

		//Debug.Log ( "GameCenterManager.IsPlayerAuthenticated: " + GameCenterManager.IsPlayerAuthenticated );
		//GameCenterPlatform.ShowLeaderboardUI("dccpuzzlemain", TimeScope.AllTime);

	}


//	private void SwipeCallback( SwipeControl.SWIPE_DIRECTION iDirection ) 
//	{
//		switch ( iDirection ) {
//		case SwipeControl.SWIPE_DIRECTION.SD_UP:
//			MoveUpOrLeft();
//			break;
//		case SwipeControl.SWIPE_DIRECTION.SD_DOWN:
//			MoveDownOrRight();
//			break;
//		}
//	}

	void Update ()
	{
		if (GameData.gameState == "waiting" || GameData.gameState == "playing")
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				//print("SWAP");
				gameData.ResetShooter();
				msg.text = "";
			}
		}
	}

	void SlideBlock()
	{
		float addX = 0;
		float addY = 0;

		// shoot block here
		if (myRow < 0)
		{
			//print ("Shoot down");
			addY = -600; //-GameData.ySpacing;
		}

		if (myCol < 0)
		{
			//print ("Shoot left");
			addX = -600; //-GameData.xSpacing;
		}

		if (ABlockAhead(myRow, myCol))
		{
			GetComponent<Rigidbody2D>().AddForce(new Vector2(addX, addY));

			isMoving = true;
			if (GameData.gameState == "waiting")
			{
				GameData.lastGameState = GameData.gameState;
				GameData.gameState = "playing";
			}
		}

	}


	void MoveDownOrRight()
	{
		bool switching = false;

		if (myCol == GameData.numCols-1) // need to switch here from going right to going down
		{
			switching = true;
			myCol = -1;
			myRow = GameData.numRows-1;
			endPosY = transform.position.y - (GameData.ySpacing * 1);
			endPosX = transform.position.x + (GameData.ySpacing * 1);
		}
		else if (myRow != 0) // don't move down past the bottom
		{
			if (myCol < 0) // moving down
			{
				endPosY = transform.position.y - (GameData.ySpacing * 1);
				endPosX = transform.position.x;
				myRow--;
			}
			else // moving sideways
			{
				endPosY = transform.position.y;
				endPosX = transform.position.x + (GameData.xSpacing * 1);
				myCol++;
			}
		}

		if (switching)
		{
			float addX = 0;
			float addY = 0;

			if (myCol < 0)
				addY = GameData.xSpacing;
			if (myRow < 0)
				addX = GameData.xSpacing;

			print (addX + " " + addY);

			Sequence mySequence = DOTween.Sequence();
			mySequence.Append(transform.DOMove(new Vector3(endPosX + addX, endPosY + addY, 0), speed));
			mySequence.Append(transform.DOMove(new Vector3(endPosX, endPosY, 0), speed ).OnComplete(DoneMoving));

			//					transform.DOJump(new Vector3(endPosX, endPosY, 0), 10f, 1, speed ).OnComplete(DoneMoving);

		}
		else
			transform.DOMove(new Vector3(endPosX, endPosY, 0), speed ).OnComplete(DoneMoving);

		isMoving = true;
		if (GameData.gameState == "waiting")
		{
			GameData.lastGameState = GameData.gameState;
			GameData.gameState = "playing";
		}

	}

	void MoveUpOrLeft()
	{
		bool switching = false;

		if (myRow == GameData.numRows-1)
		{
			switching = true;
			myRow = -1;
			myCol = GameData.numCols-1;
			endPosY = transform.position.y + (GameData.ySpacing * 1);
			endPosX = transform.position.x - (GameData.ySpacing * 1);
		}
		else if (myCol != 0)
		{
			if (myRow < 0) // moving sideways
			{
				endPosY = transform.position.y;
				endPosX = transform.position.x - (GameData.xSpacing * 1);
				myCol--;
			}
			else // moving up and down
			{
				endPosY = transform.position.y + (GameData.ySpacing * 1);
				endPosX = transform.position.x;
				myRow++;
			}
		}

		if (switching)
		{
			float addX = 0;
			float addY = 0;

			if (myCol < 0)
				addY = GameData.xSpacing;
			if (myRow < 0)
				addX = GameData.xSpacing;

			print (addX + " " + addY);

			Sequence mySequence = DOTween.Sequence();
			mySequence.Append(transform.DOMove(new Vector3(endPosX + addX, endPosY + addY, 0), speed));
			mySequence.Append(transform.DOMove(new Vector3(endPosX, endPosY, 0), speed ).OnComplete(DoneMoving));
		}
		else
			transform.DOMove(new Vector3(endPosX, endPosY, 0), speed ).OnComplete(DoneMoving);

		isMoving = true;
		if (GameData.gameState == "waiting")
		{
			GameData.lastGameState = GameData.gameState;
			GameData.gameState = "playing";
		}
			
	}

	void CheckForHighScore()
	{
		// see if we have a high score
		int oldScore = PlayerPrefs.GetInt("HighScore");
		if (GameData.score > oldScore)
		{
			// new high score!
			highScoreText = GameObject.Find("Canvas/BlankWindow/HighScore/Score").GetComponent<Text>();
			highScoreText.text = GameData.score.ToString("N0");
			PlayerPrefs.SetInt("HighScore", GameData.score);
			PlayerPrefs.SetInt("Level", GameData.dungeonLevel);

			// now get the data into GameCenter
			string iOS_LeaderboardID = "dccpuzzlemain"; //Same as Leaderboard ID you setup in iTunes Connect.

			bool isGCAuthenticated = Social.localUser.authenticated;

			if (isGCAuthenticated) {
				Social.ReportScore(GameData.score, iOS_LeaderboardID, success => { if (success) { Debug.Log("==iOS GC report score ok: " + GameData.score + "\n"); } else { Debug.Log("==iOS GC report score Failed: " + iOS_LeaderboardID + "\n"); } } );

			} else {
				Debug.Log("==iOS GC can't report score, not authenticated\n");

			}
		}
	}

	public void SkippingToNextLevel()
	{
		CheckForHighScore();
		GameData.dungeonLevel++;

		// increment number of items in grid 5, 6, 7, 8 (the max)
		if (GameData.dungeonLevel == 3)
			GameData.numberOfAnimals++;			// 6
		else if (GameData.dungeonLevel == 5)
			GameData.numberOfAnimals++;			// 7
		else if (GameData.dungeonLevel == 8)
			GameData.numberOfAnimals++;			// 8

		numberOfShooters = 0;

		SetNextLevel();

		createGrid = GameObject.Find("SceneManager").GetComponent<CreateGrid>();
		createGrid.StartLevel();

	}

	public void WonLevel()
	{
		print("WON!!! GameData.remainingBlocks " + GameData.remainingBlocks);

		if (GetRemainingBlockCount() > 0)
			redX.SetActive(true);
		
		SoundManager.PlaySFX("Event 13");

		GameData.gameState = "waiting";

		GameObject wonParticles = CFX_SpawnSystem.GetNextObject(wonLevelPrefab);
		wonParticles.transform.position = nextLevelBtn.gameObject.transform.position;

		CheckForHighScore();

		//GameObject.Find("Canvas/NextPuzzle").gameObject.SetActive(true);

		string tMsg = "Keep Going!";
		if (GetRemainingBlockCount() > 0)
			tMsg = "No More Matches!\n" + tMsg;
		else
			tMsg = "Cleared All Loot! " + 5000 * GameData.dungeonLevel + " Bonus Points!\nGreat Job! " + tMsg;
		SetWinLoseMessage(tMsg);

		GameData.dungeonLevel++;

		// increment number of items in grid 5, 6, 7, 8 (the max)
		if (GameData.dungeonLevel == 3)
			GameData.numberOfAnimals++;			// 6
		else if (GameData.dungeonLevel == 5)
			GameData.numberOfAnimals++;			// 7
		else if (GameData.dungeonLevel == 8)
			GameData.numberOfAnimals++;			// 8

		numberOfShooters = 0;

		if (true)
		{
			Invoke("GoToNextLevel", 2.5f);
		}
		else
		{
			nextLevelBtn.SetActive(true);
			nextLevelBtn.GetComponentInChildren<Text>().text = "Next Level";
		}

	}

	// go to next level without clicking a button
	void GoToNextLevel()
	{
		Invoke("ClearWinLoseMessage", 1.0f);
		SetNextLevel();
		createGrid.StartLevel();
		Invoke("ShowMatchingEdgePieces", 3f);
	}

	void SetWinLoseMessage(string msg = "")
	{
		Debug.Log("SetWinLoseMessage " + msg);
		winLoseMsg.text = msg;
		//winLoseMsg.GetComponent<Text>().color.a = 1f;
		//winLoseMsg.canvasRenderer.SetAlpha(255);
//		winLoseMsg.CrossFadeAlpha(1.0f, 0.00f, false);
//		winLoseMsg.gameObject.SetActive(false);
//		winLoseMsg.gameObject.SetActive(true);
	}

	void ClearWinLoseMessage()
	{
		Debug.Log("ClearWinLoseMessage");
		winLoseMsg.text = "";
		//winLoseMsg.CrossFadeAlpha(0.01f, 0.5f, false);
	}

	// when we lose a level, we've also finished the game
	public void LostLevel()
	{
		print("LOST!!!");

		if (GetRemainingBlockCount() > 0)
			ProCamera2DShake.Instance.ShakeUsingPreset("ShakePreset0");
		
		redX.SetActive(true);

		SoundManager.PlaySFX("lose");

		GameData.gameState = "lost";

		SetWinLoseMessage("Sorry, Game Over!\nYou died heroically\non dungeon level " + GameData.dungeonLevel);

		CheckForHighScore();

		nextLevelBtn.SetActive(true);
		nextLevelBtn.GetComponentInChildren<Text>().text = "Try Again";

		GameObject lostParticles = CFX_SpawnSystem.GetNextObject(lostLevelPrefab);
		lostParticles.transform.position = nextLevelBtn.gameObject.transform.position + (Vector3.down*2);

		ResetGameStuff();
	}

	// stuff that needs to get set for the next level to play correcctly
	public void SetNextLevel()
	{
		GameObject.Find("Canvas/LevelNumber").GetComponent<Text>().text = "Level " + GameData.dungeonLevel;

	}

	// stuff to reset when we're starting completely over (like when we've lost)
	public void ResetGameStuff()
	{
		GameData.dungeonLevel = 1;

		GameData.numberOfAnimals = 5; //we start with 5 different things

		numberOfShooters = 0;
	}

	bool ABlockAhead(int row, int col)
	{
		bool foundBlock = false;
		if (row < 0)
		{
			//print ("Shoot down");
			for(int y = 0; y < GameData.numRows-1; y++)
				if (GameData.gridBlocks[new Int2(col,y)] != null)
					foundBlock = true;
		}

		if (myCol < 0)
		{
			//print ("Shoot left");
			for(int x = 0; x < GameData.numCols-1; x++)
				if (GameData.gridBlocks[new Int2(x,row)] != null)
					foundBlock = true;
		}
		return (foundBlock);
	}

	void DoneMoving()
	{
		lastPosition = transform.position;
		isMoving = false;
	}

	public int GetRemainingBlockCount()
	{
		int numBlocks = 0;

		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				if (GameData.gridBlocks[gridPos] != null)
					numBlocks++;
			}
		}

		return numBlocks;
	}


	public void ResetShooter()
	{
		print("ResetShooter");
//		if (GameData.gameState == "playing")
//			numberSwaps--;

		myRow = 0;
		myCol = -1;
		isMoving = true;

		GetRandomAnimalSprite();

		GetComponent<Collider2D>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<Rigidbody2D>().isKinematic = false;

		//transform.position = new Vector3(5.87f, -3.42f, 0f); // x was GameData.shooterStartX + 4f, y was GameData.shooterStartY
		//print("GameData.screenTop " + GameData.screenTop/2);
		//transform.position = new Vector3(GameData.screenRight-1.7f, GameData.screenTop-2f, 0f); // x was GameData.shooterStartX + 4f, y was GameData.shooterStartY
		transform.position = redX.transform.position;

		//print("GameData.gameState " + GameData.gameState);
		// make sure there's a matching animal on the edge for first three tries
		if (GameData.gameState == "waiting" || numberOfShooters < 3)
			Invoke("CanWeShoot", 0.1f);
		else
			Invoke("MoveShooterIntoPosition", 0.3f);

		numberOfShooters++;

//		if (EventSystem.current.currentSelectedGameObject)
//			Debug.Log("currentSelectedGameObject: " + EventSystem.current.currentSelectedGameObject.name);
//		
//		EventSystem.current.SetSelectedGameObject(Camera.main.gameObject);
//
//		if (EventSystem.current.currentSelectedGameObject)
//			Debug.Log("currentSelectedGameObject: " + EventSystem.current.currentSelectedGameObject.name);

		//if (winLoseMsg.text != "")
		//	Invoke("ClearWinLoseMessage", 2f);

		//Invoke("ShowMatchingEdgePieces", 1f);
	}

	void CanWeShoot()
	{
		//if (GameData.gameState != "playing" || GameData.gameState != "waiting")
		//	return;
		
		print("CanWeShoot");
		while (!CheckForEdgeMatch())
			GetRandomAnimalSprite();
		Invoke("MoveShooterIntoPosition", 0.3f);
	}

	void MoveShooterIntoPosition()
	{
		if (GameData.gameState == "lost")
			return;
		
		if (CheckForEdgeMatch())
		{
			//transform.DOMoveX(GameData.shooter.transform.position.x - 2.5f, 0.3f).OnComplete(NotMoving);
			NotMoving();
			redX.SetActive(false);
			msg.text = "";

			// we have a good shooter, so clear the list of swapped ones
			GameData.swappingShooters.Clear();
		}
		else if (numberSwaps > 0 && GameData.gameState == "playing")
		{
			GameObject redXParticles = CFX_SpawnSystem.GetNextObject(noMatchPrefab);
			redXParticles.transform.position = redX.transform.position;

			redX.SetActive(true);
			//msg.text = "No Match,\nSwap >>";
			SetWinLoseMessage("No Match, Swapping...");
			SoundManager.PlaySFX("nomatchsubtle");
			Invoke("DoSwap", 1.5f);
		}
		else if (numberSwaps == 0)
		{
			msg.text = ""; 
			CheckForWinOrLose();
		}
	}

	void DoSwap()
	{
		redX.SetActive(false);
		//Invoke("ClearWinLoseMessage", 0.5f);
		ClearWinLoseMessage();
		msg.text = "";
		gameData.ResetShooter();
	}

	void CheckForWinOrLose()
	{
		GameData.skipLevelBtn.SetActive(false);

		if (GameData.remainingBlocks > 0)
			redX.SetActive(true);
		
		if (GameData.remainingBlocks == 0 && !CheckForEdgeMatch())	//(GameData.numCols * GameData.numRows / 2 >= GameData.remainingBlocks)
		{
			// we can move on to the next puzzle
			GameData.gameState = "won";
			WonLevel();
		}
		else
		{
			// we're done, so do high score stuff, etc.
			GameData.gameState = "lost";
			LostLevel();
		}
	}

	void OnMouseUpAsButton()
	{
		ShowMatchingEdgePieces();
	}

	// sparkle all the matching edge pieces
	public void ShowMatchingEdgePieces()
	{
		int numBlocks = 0;
		float gridCount = 0.2f;

		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				if (GameData.gridBlocks[gridPos] != null && GameData.gridBlocks[gridPos].GetComponent<Block>().blockType == GameData.shooterType)
				{
					bool onEdge = false;

					//print ("Look up");
					if (!GameData.gridBlocks.ContainsKey(gridPos+Int2.up) || GameData.gridBlocks[gridPos+Int2.up] == null)
						onEdge = true;
					//print ("Look right");
					if (!GameData.gridBlocks.ContainsKey(gridPos+Int2.right) || GameData.gridBlocks[gridPos+Int2.right] == null)
						onEdge = true;

					if (onEdge)
					{
						//GameObject sparkle = CFX_SpawnSystem.GetNextObject(sparklePrefab);
						//sparkle.transform.position = GameData.gridBlocks[gridPos].transform.position;

						GameData.gridBlocks[gridPos].transform.DOPunchScale(new Vector3(0.2f, 0.2f, 1f), 1.5f, 4).SetDelay(gridCount);
						gridCount += 0.2f;

					}
				}
			}
		}

	}

	void NotMoving()
	{
		lastPosition = transform.position;
		isMoving = false;
	}

	void GetRandomAnimalSprite()
	{
		int arrayIdx;
		//grab a random animal sprite, but make sure we
		// don't randomly pick the one we just had
		do
		{
			arrayIdx = Random.Range (0, GameData.numberOfAnimals);
		} while (arrayIdx == lastSpriteIdx || GameData.swappingShooters.Contains(arrayIdx));
		lastSpriteIdx = arrayIdx;
		GameData.swappingShooters.Add(arrayIdx);

		Sprite shooterSprite = animalSprites[arrayIdx];
		string pickupName = shooterSprite.name;

		blockType = shooterSprite.name;
		GameData.shooterType = blockType;
		GetComponent<SpriteRenderer>().sprite = shooterSprite;

		Invoke("CheckForLose", 0.5f);
	}

	void CheckForLose()
	{
		// if there are no edge matches and we have no more swaps, we lost!
		if (GameData.gameState == "playing" && !CheckForEdgeMatch() && numberSwaps == 0)
			GameData.gameState = "lost";	
	}

	public void UpdateSwaps()
	{
		if (numberSwaps < 0)
			numberSwaps = 0;
		
		//swapText.text = "Swaps: " + numberSwaps;
		if (swapBtnText != null)
			swapBtnText.text = "Swaps: " + numberSwaps;
	}

	// look at all of the edge pieces and see if there's a block that matches the shooter
	public bool CheckForEdgeMatch()
	{
		bool foundMatch = false;

		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				if (GameData.gridBlocks[gridPos] != null)
				{
					Int2 above = gridPos + Int2.up;
					Int2 toRight = gridPos + Int2.right;

					GameObject block = GameData.gridBlocks[gridPos] as GameObject;
					string gridBlock = block.GetComponent<Block>().blockType;
					if (gridBlock == blockType)
					{
						if ( (!GameData.gridBlocks.ContainsKey(above) || GameData.gridBlocks[above] == null)  || (!GameData.gridBlocks.ContainsKey(toRight) || GameData.gridBlocks[toRight] == null) )
						{
							foundMatch = true;
						}
					}
				}
			}
		}

		return (foundMatch);
	}


	// for when we try to shoot something that's not a match
	public void RetryShot()
	{
		isMoving = false;
		GetComponent<Rigidbody2D>().isKinematic = false;
		transform.position = lastPosition;
	}

	public void CreateShooter()
	{
		GameData.shooter = Instantiate(shooterPrefab);
	}


}
