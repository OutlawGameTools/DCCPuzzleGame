using UnityEngine;
using System.Collections;

public class DragWithMouse : MonoBehaviour {

	private Vector3 offset;

	void OnMouseDown () {
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		offset = transform.position - Camera.main.ScreenToWorldPoint(mousePos);
	}
	
	void OnMouseDrag () {
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		transform.position =  Camera.main.ScreenToWorldPoint(mousePos) + offset;
	}
}
