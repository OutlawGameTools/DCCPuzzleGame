using UnityEngine;
using System.Collections;

public class GameStuff : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
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
					if (block.GetComponent<Block>().blockType == GameData.shooterType)
					{
						if (GameData.gridBlocks.ContainsKey(above) && GameData.gridBlocks[above] != null || GameData.gridBlocks.ContainsKey(toRight) && GameData.gridBlocks[toRight] != null)
						{
							foundMatch = true;
						}
					}
				}
			}
		}

		return (foundMatch);
	}
}
