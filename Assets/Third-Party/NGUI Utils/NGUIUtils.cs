using UnityEngine;
using System.Collections;

public static class NGUIUtils
{

	//used in fullscreen button in title scene to check if the user is pressing the settings panel or not..
	public static bool isCursorOverOverlayUI(){
		
		foreach (UICamera uiCam in GameObject.FindObjectsOfType<UICamera>())
			if (isCursorOverOverlayUI (uiCam.GetComponent<Camera> ()))
				return true;
		return false;
		
	}
	
	public static bool isCursorOverOverlayUI(Camera cam){
		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)){
			return true;
		}else{
			return false;
		}
		
	}

}
