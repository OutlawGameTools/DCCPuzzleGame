using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DriftingText : MonoBehaviour {

	public GameObject canvas;
	public GameObject textPrefab;

	public float driftTime = 1f;
	public float relativeYEndPos = 1.5f;

	private Text msgTxt;
	
	void Awake()
	{
		if (canvas == null)
			canvas = GameObject.Find("Canvas");

		if (textPrefab == null)
			textPrefab = GameObject.Find ("Canvas/DriftingText");
	}

	// Use this for initialization
	void Start () {
	
	}
	

	public void MakeDriftingText (string msg, Vector3 pos, float dTime = 1.5f, float relYEndPos = 1.5f) 
	{
		GameObject ptTxt = Instantiate(textPrefab) as GameObject;
		msgTxt = ptTxt.GetComponent<Text>();
		ptTxt.transform.SetParent(canvas.transform, false);
		ptTxt.transform.position = pos;

		//Vector2 newPos = new Vector2(Tile.WorldToMapPosition(pos).x, Tile.);
		//ptTxt.transform.position = pos; //new Vector2(0,0); 

		//Int2 mapPos = Tile.WorldToMapPosition(pos);
		//print ("mapPos " + mapPos);

		//ptTxt.transform.position = new Vector3(mapPos.x, mapPos.y, 0);
		//ptTxt.transform.position = Tile.MapToWorldPosition(new Int2(pos));

		msgTxt.text = msg;
		
		float endYPos = pos.y + relYEndPos;

		ptTxt.transform.DOMoveY(endYPos, dTime).SetEase(Ease.OutQuint);
		msgTxt.DOFade(0f, dTime);
		Destroy(ptTxt, dTime);
	}

}
