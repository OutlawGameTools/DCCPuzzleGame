using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Com.LuisPedroFonseca.ProCamera2D;

public class Block : MonoBehaviour {

	public Text scoreText;
	public Text blocksLeftText;

	public string blockType = "giraffe";

	public BlockHit blockHit;

	public Int2 myGridPos;

	public GameObject hitPrefab;
	public GameObject halfwayPrefab;
	public GameObject extraSwapPrefab;

	bool droppingDown = false;
	bool slidingLeft = false;
	Vector3 swapButtonPos;

	List<GameObject> connectedBlocks;
	List<Int2> checkedBlocks;

	enum Direction{North, East, South, West, Ignore};

	DriftingText dt;

	Shooter shooter;

	void Start () {
	
		blockHit = GameObject.Find("SceneManager").GetComponent<BlockHit>();
		scoreText = GameObject.Find("Canvas/BlankWindow/Score/Score").GetComponent<Text>();
		blocksLeftText = GameObject.Find("Canvas/BlankWindow/NumBlocks/BlocksLeft").GetComponent<Text>();
		dt = GameObject.Find("SceneManager").GetComponent<DriftingText>();
		swapButtonPos = GameObject.Find("Canvas/BlankWindow/SwapAnimal").GetComponent<RectTransform>().TransformPoint(Camera.main.transform.position);
		shooter = GameObject.Find("SceneManager").GetComponent<CreateGrid>().shooterPrefab.GetComponent<Shooter>();
		GameData.remainingBlocks = GameData.numCols * GameData.numRows / 2;
		blocksLeftText.text = GameData.remainingBlocks.ToString("N0");

		//InvokeRepeating("CanIMove", 1f, 0.05f);
	}

	void CanIMove()
	{
		// dropping down
		if (! droppingDown && myGridPos.y > 0)
		{
			Int2 belowMe = new Int2(myGridPos.x, myGridPos.y - 1);
			if (GameData.gridBlocks[belowMe] == null)
			{
				droppingDown = true;
				GameData.gridBlocks[belowMe] = gameObject;
				GameData.gridBlocks[myGridPos] = null;
				myGridPos.y -= 1;
				transform.DOMoveY(transform.position.y - GameData.ySpacing, 0.25f).OnComplete(DoneDropping);
			}
		}

		// sliding left
		if (! slidingLeft && myGridPos.x > 0)
		{
			Int2 leftOfMe = new Int2(myGridPos.x-1, myGridPos.y);
			if (GameData.gridBlocks[leftOfMe] == null)
			{
				slidingLeft = true;
				GameData.gridBlocks[leftOfMe] = gameObject;
				GameData.gridBlocks[myGridPos] = null;
				myGridPos.x -= 1;
				transform.DOMoveX(transform.position.x - GameData.xSpacing, 0.1f).OnComplete(DoneSliding);
			}

		}
	}

	void DoneDropping()
	{
		droppingDown = false;
	}

	void DoneSliding()
	{
		slidingLeft = false;
	}

	void GetConnected(Int2 startPos)
	{
		connectedBlocks = new List<GameObject>();
		checkedBlocks = new List<Int2>();

		GetNeighbors(startPos);
	}

	void GetNeighbors(Int2 startPos)
	{
		Int2 above = startPos + Int2.up;
		Int2 below = startPos + Int2.down;
		Int2 toLeft = startPos + Int2.left;
		Int2 toRight = startPos + Int2.right;

		GameObject neighbor;
		string neighborType;

		checkedBlocks.Add(startPos);

		// check block to north (above)
		//if (ignoreDir != Direction.North)
		//{
			if (GameData.gridBlocks.ContainsKey(above) && GameData.gridBlocks[above] != null && !checkedBlocks.Contains(above))
			{
				neighbor = GameData.gridBlocks[above];
				neighborType = neighbor.GetComponent<Block>().blockType;
				if (neighborType == blockType)
				{
					connectedBlocks.Add(neighbor);
					GetNeighbors(neighbor.GetComponent<Block>().myGridPos);
				}
			}
		//}

		// check block to south (below)
		//if (ignoreDir != Direction.South)
		//{
			if (GameData.gridBlocks.ContainsKey(below) && GameData.gridBlocks[below] != null && !checkedBlocks.Contains(below))
			{
				neighbor = GameData.gridBlocks[below];
				neighborType = neighbor.GetComponent<Block>().blockType;
				if (neighborType == blockType)
				{
					connectedBlocks.Add(neighbor);
					GetNeighbors(neighbor.GetComponent<Block>().myGridPos);
				}
			}
		//}

		// check block to east (toRight)
		//if (ignoreDir != Direction.East)
		//{
			if (GameData.gridBlocks.ContainsKey(toRight) && GameData.gridBlocks[toRight] != null && !checkedBlocks.Contains(toRight))
			{
				neighbor = GameData.gridBlocks[toRight];
				neighborType = neighbor.GetComponent<Block>().blockType;
				if (neighborType == blockType)
				{
					connectedBlocks.Add(neighbor);
					GetNeighbors(neighbor.GetComponent<Block>().myGridPos);
				}
			}
		//}

		// check block to west (toLeft)
		//if (ignoreDir != Direction.West)
		//{
			if (GameData.gridBlocks.ContainsKey(toLeft) && GameData.gridBlocks[toLeft] != null && !checkedBlocks.Contains(toLeft))
			{
				neighbor = GameData.gridBlocks[toLeft];
				neighborType = neighbor.GetComponent<Block>().blockType;
				if (neighborType == blockType)
				{
					connectedBlocks.Add(neighbor);
					GetNeighbors(neighbor.GetComponent<Block>().myGridPos);
				}
			}
		//}


//		print (above.ToString());
//		print (below.ToString());
//		print (toLeft.ToString());
//		print (toRight.ToString());


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Shooter"))
		{
			if (blockType == other.gameObject.GetComponent<Shooter>().blockType)
			{
				// hide and move the shooter block
				//Destroy(other.gameObject, 1f);
				other.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
				other.gameObject.GetComponent<Collider2D>().enabled = false;
				other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
				other.gameObject.GetComponent<Shooter>().ResetShooter();

				GotAHit(other.gameObject);
			}
			else // we shot at something that didn't match
			{
				other.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
				other.gameObject.GetComponent<Shooter>().RetryShot();
			}
		}
	}

	void GotAHit(GameObject animal)
	{
		if (GameData.gameState != "playing")
			return;
		
		int scoreAmount = 10 * GameData.dungeonLevel; // larger points for higher levels

		// see if any matching animals are touching and collect them
		GetConnected(myGridPos);

		// give bonus for more connecting blocks
		if (connectedBlocks.Count > 0)
		{
			scoreAmount = (connectedBlocks.Count+1) * 100 * GameData.dungeonLevel;
		}
			
		GameData.score += scoreAmount;
		dt.MakeDriftingText(scoreAmount.ToString(), transform.position );

		if ((connectedBlocks.Count+1) >= 3)
		{
			//print("Bonus: " + (connectedBlocks.Count+1)/3 + " added to " + GameData.shooter.GetComponent<Shooter>().numberSwaps);
			GameData.shooter.GetComponent<Shooter>().numberSwaps += (connectedBlocks.Count+1)/3; //a swap for every 3
			GameData.shooter.GetComponent<Shooter>().UpdateSwaps();

			dt.MakeDriftingText("Extra Swap!", transform.position + Vector3.right, 2.0f );
			GameObject extraSwap = CFX_SpawnSystem.GetNextObject(extraSwapPrefab);
			extraSwap.transform.position = swapButtonPos;

			SoundManager.PlaySFX("Swish 2");
		}
		else
			SoundManager.PlaySFX("Hit 1"); // sound for just a single or double match.

		// set the to-be-deleted blocks to null in the grid
		foreach(GameObject gObj in connectedBlocks)
		{
			Int2 pos = gObj.GetComponent<Block>().myGridPos;
			GameData.gridBlocks[pos] = null;
		}

		// show particles for each to-be-deleted block and destroy it.
		foreach(GameObject gObj in connectedBlocks)
		{
			dt.MakeDriftingText(scoreAmount.ToString(), gObj.transform.position );
			GameObject leaves1 = CFX_SpawnSystem.GetNextObject(hitPrefab);
			leaves1.transform.position = gObj.transform.position;
			Destroy(gObj, 0.2f);
			GameData.score += scoreAmount;
		}
		// update the score on the screen
		if (scoreText != null)
			scoreText.text = GameData.score.ToString("N0");

		GameData.gridBlocks[myGridPos] = null;
		GameObject leaves2 = CFX_SpawnSystem.GetNextObject(hitPrefab);
		leaves2.transform.position = gameObject.transform.position;

		SpriteRenderer[] spriteRenderers;
		spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spr in spriteRenderers) {
			spr.enabled = false;
		}

		Destroy(gameObject, 0.5f); // delay killing us so other code can get finished

		//Invoke("MoveBlocksDown", 0.2f);
		//Invoke("MoveBlocksLeft", 0.2f);
		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftDown();
		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftLeft();

		CountRemainingBlocks();
	}
		
	// see how many blocks are left after the last collision
	void CountRemainingBlocks()
	{
		int numBlocks = GameData.shooter.GetComponent<Shooter>().GetRemainingBlockCount();

		// show how many blocks are remaining
		GameData.remainingBlocks = numBlocks - (GameData.numCols * GameData.numRows / 2); //numBlocks;
		if (GameData.remainingBlocks < 0)
			GameData.remainingBlocks = 0;
		blocksLeftText.text = GameData.remainingBlocks.ToString();

		if (!GameData.toldUserHalfway && GameData.remainingBlocks == 0)
		{
			GameData.toldUserHalfway = true;
			SoundManager.PlaySFX("Halfway");
			// show particle effect on numblocks field
			Vector3 buttonPos = GameObject.Find("Canvas/BlankWindow/NumBlocks").GetComponent<RectTransform>().TransformPoint(Camera.main.transform.position);
			GameObject halfway = CFX_SpawnSystem.GetNextObject(halfwayPrefab);
			halfway.transform.position = buttonPos;

			GameData.skipLevelBtn.SetActive(true);
		}

		if (numBlocks == 0)
		{
			// bonus for getting all blocks in this level
			Vector3 bonusPos = new Vector3(-8.3f, -4.1f, 0);
			int finishBonus = 5000 * GameData.dungeonLevel;
			dt.MakeDriftingText(finishBonus.ToString(), bonusPos, 2f );
			GameData.score += finishBonus;
			// update the score on the screen
			if (scoreText != null)
				scoreText.text = GameData.score.ToString("N0");
			
			GameData.lastGameState = GameData.gameState;
			GameData.gameState = "won";
			print("Sending to GoToWonLevel");
			GameData.skipLevelBtn.SetActive(false);
			Invoke("GoToWonLevel", 0f);
		}
	}

	void GoToWonLevel()
	{
		print("Inside to GoToWonLevel");
		GameData.shooter.GetComponent<Shooter>().WonLevel();
	}

	void MoveBlocksDown()
	{
		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftDown();
	}

	void MoveBlocksLeft()
	{
		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftLeft();
	}

	void xCanIMove()
	{
		// dropping down
		if (! droppingDown && myGridPos.y > 0)
		{
			Int2 belowMe = new Int2(myGridPos.x, myGridPos.y - 1);
			if (GameData.gridBlocks[belowMe] == null)
			{
				droppingDown = true;
				GameData.gridBlocks[belowMe] = gameObject;
				GameData.gridBlocks[myGridPos] = null;
				myGridPos.y -= 1;
				transform.DOMoveY(transform.position.y - GameData.ySpacing, 0.25f).OnComplete(DoneDropping);
			}
		}

		// sliding left
		if (! slidingLeft && myGridPos.x > 0)
		{
			Int2 leftOfMe = new Int2(myGridPos.x-1, myGridPos.y);
			if (GameData.gridBlocks[leftOfMe] == null)
			{
				slidingLeft = true;
				GameData.gridBlocks[leftOfMe] = gameObject;
				GameData.gridBlocks[myGridPos] = null;
				myGridPos.x -= 1;
				transform.DOMoveX(transform.position.x - GameData.xSpacing, 0.1f).OnComplete(DoneSliding);
			}

		}
	}


	public void OnMouseUpAsButton()
	{
		//Debug.Log("in OnMouseUpAsButton EventSystem.current.IsPointerOverGameObject(): " + EventSystem.current.IsPointerOverGameObject());

		string otherBlockType;
		if (GameData.shooter != null)
			otherBlockType = GameData.shooter.GetComponent<Shooter>().blockType;
		else
			return;

		if (GameData.gameState != "playing" || Time.timeScale == 0)
			return;

		if (!GameData.instructionsGone)
			return;
		


		GameData.swapping = false;

		Vector3 msgPos = transform.position;
		if (msgPos.x < GameData.screenLeft + 1)
			msgPos += Vector3.right;
		
		if ( blockType == otherBlockType && ThisBlockOnEdge() && GameData.gameState == "playing" )
		{
			//print ("MATCH!");
			//GameData.shooter.transform.DOMove(transform.position, 0.2f).OnComplete(ShooterDoneMoving);

			GotAHit(gameObject);
			GameData.shooter.GetComponent<Shooter>().ResetShooter();

		}
		else if (blockType == otherBlockType && !ThisBlockOnEdge())
		{
			dt.MakeDriftingText("Blocked In!", msgPos );
			shooter.ShowMatchingEdgePieces();
		}
		else if (blockType != otherBlockType)
		{
			dt.MakeDriftingText("Not A Match!", msgPos );
			shooter.ShowMatchingEdgePieces();
		}
	}

	void ShooterDoneMoving()
	{
		GotAHit(gameObject);
		GameData.shooter.GetComponent<Shooter>().ResetShooter();
	}

	bool ThisBlockOnEdge()
	{
		bool onEdge = false;

		//print ("Look up");
		if (!GameData.gridBlocks.ContainsKey(myGridPos+Int2.up) || GameData.gridBlocks[myGridPos+Int2.up] == null)
			onEdge = true;

		//print ("Look right");
		if (!GameData.gridBlocks.ContainsKey(myGridPos+Int2.right) || GameData.gridBlocks[myGridPos+Int2.right] == null)
			onEdge = true;

		return (onEdge);
	}

}
