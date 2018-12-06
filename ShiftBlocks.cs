using UnityEngine;
using System.Collections;
using DG.Tweening;

// look for empty spaces and shift blocks down and left
// to fill those spaces.

public class ShiftBlocks : MonoBehaviour {


	public void ShiftDown()
	{
		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				GameObject thisBlock = GameData.gridBlocks[gridPos];

				if (thisBlock != null)
				{
					float numOpen = 0;
					Int2 below = gridPos + Int2.down;

					for(int y = j; y > 0; y--)
					{
						if (GameData.gridBlocks.ContainsKey(below) && GameData.gridBlocks[below] == null)
							numOpen++;
						below += Int2.down;
					}

					if (numOpen > 0)
					{
						Int2 prevPos = thisBlock.GetComponent<Block>().myGridPos;
						Int2 nextPos = new Int2(prevPos.x, prevPos.y - (int) numOpen);

						GameData.gridBlocks[prevPos] = null;
						GameData.gridBlocks[nextPos] = thisBlock;
						thisBlock.GetComponent<Block>().myGridPos = nextPos;

						thisBlock.transform.DOMoveY(thisBlock.transform.position.y - GameData.ySpacing * numOpen, GameData.shiftSpeed);
					}
				}
			}
		}

	}

	public void ShiftLeft()
	{
		for (int i = 0; i < GameData.numCols; i++)
		{
			for(int j = 0; j < GameData.numRows; j++)
			{
				Int2 gridPos = new Int2(i,j);
				GameObject thisBlock = GameData.gridBlocks[gridPos];

				if (thisBlock != null)
				{
					float numOpen = 0;
					Int2 left = gridPos + Int2.left;

					for(int x = i; x > 0; x--)
					{
						if (GameData.gridBlocks.ContainsKey(left) && GameData.gridBlocks[left] == null)
							numOpen++;
						left += Int2.left;
					}

					if (numOpen > 0)
					{
						Int2 prevPos = thisBlock.GetComponent<Block>().myGridPos;
						Int2 nextPos = new Int2(prevPos.x  - (int) numOpen, prevPos.y);

						GameData.gridBlocks[prevPos] = null;
						GameData.gridBlocks[nextPos] = thisBlock;
						thisBlock.GetComponent<Block>().myGridPos = nextPos;

						thisBlock.transform.DOMoveX(thisBlock.transform.position.x - GameData.xSpacing * numOpen, GameData.shiftSpeed);
					}
				}
			}
		}

	}

}
