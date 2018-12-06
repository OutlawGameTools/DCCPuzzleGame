using UnityEngine;
using System.Collections;

public class BlockHit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

//	void GotAHit(GameObject animal)
//	{
//		if (GameData.gameState != "playing")
//			return;
//
//		int scoreAmount = 10 * GameData.dungeonLevel; // larger points for higher levels
//
//		// see if any matching animals are touching and collect them
//		GetConnected(myGridPos, Direction.Ignore);
//
//		// give bonus for more connecting blocks
//		if (connectedBlocks.Count > 0)
//		{
//			scoreAmount = (connectedBlocks.Count+1) * 100 * GameData.dungeonLevel;
//		}
//
//		GameData.score += scoreAmount;
//		dt.MakeDriftingText(scoreAmount.ToString(), transform.position );
//
//		if ((connectedBlocks.Count+1) >= 3)
//		{
//			//print("Bonus: " + (connectedBlocks.Count+1)/3 + " added to " + GameData.shooter.GetComponent<Shooter>().numberSwaps);
//			GameData.shooter.GetComponent<Shooter>().numberSwaps += (connectedBlocks.Count+1)/3; //a swap for every 3
//			GameData.shooter.GetComponent<Shooter>().UpdateSwaps();
//
//			dt.MakeDriftingText("Extra Swap!", transform.position + Vector3.right, 2.0f );
//			GameObject extraSwap = CFX_SpawnSystem.GetNextObject(extraSwapPrefab);
//			extraSwap.transform.position = swapButtonPos;
//
//			SoundManager.PlaySFX("Swish 2");
//		}
//		else
//			SoundManager.PlaySFX("Hit 1"); // sound for just a single or double match.
//
//		// set the to-be-deleted blocks to null in the grid
//		foreach(GameObject gObj in connectedBlocks)
//		{
//			Int2 pos = gObj.GetComponent<Block>().myGridPos;
//			GameData.gridBlocks[pos] = null;
//		}
//
//		// show particles for each to-be-deleted block and destroy it.
//		foreach(GameObject gObj in connectedBlocks)
//		{
//			dt.MakeDriftingText(scoreAmount.ToString(), gObj.transform.position );
//			GameObject leaves1 = CFX_SpawnSystem.GetNextObject(hitPrefab);
//			leaves1.transform.position = gObj.transform.position;
//			Destroy(gObj, 0.2f);
//			GameData.score += scoreAmount;
//		}
//		// update the score on the screen
//		if (scoreText != null)
//			scoreText.text = GameData.score.ToString("N0");
//
//		GameData.gridBlocks[myGridPos] = null;
//		GameObject leaves2 = CFX_SpawnSystem.GetNextObject(hitPrefab);
//		leaves2.transform.position = gameObject.transform.position;
//
//		SpriteRenderer[] spriteRenderers;
//		spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
//		foreach (SpriteRenderer spr in spriteRenderers) {
//			spr.enabled = false;
//		}
//
//		Destroy(gameObject, 0.3f); // delay killing us so other code can get finished
//
//		//Invoke("MoveBlocksDown", 0.2f);
//		//Invoke("MoveBlocksLeft", 0.2f);
//		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftDown();
//		GameObject.Find("SceneManager").GetComponent<ShiftBlocks>().ShiftLeft();
//
//		CountRemainingBlocks();
//	}

}
