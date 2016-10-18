//helper class to get ngui's dimensions since we can't use Screen.width , Screen.height to position ngui stuff when ngui stuff is scaled for different resolutions etc
//put this script on the gameobject containing the ngui uiroot, then access DisplaySize.instance.width and ..height instead of Screen.width and Screen.height for positioning ngui stuff for that UIRoot (all that if the uiroot is scaled using fixedsize with manualheight instead of pixelperfect setting
//see http://www.gianser.com/index.php/2d-3d-photography-graphics/adobe-flash/52668-ngui-unity3d-getting-screen-size


using UnityEngine;
using System.Collections;

public class DisplaySize: MonoBehaviour{

	UIRoot mRoot;

	protected static DisplaySize _instance;
	public static DisplaySize instance{
		get {
			if (_instance == null){
				_instance = FindObjectOfType(typeof(DisplaySize)) as DisplaySize;//new DisplaySize();
			}
			return _instance;
		}
	}

	void Start(){
		CalculateSize();
	}
	
	//public GameObject gameObject;
	/*
	public float width;
	public float height;
	*/
	private float w;
	private float h;
	[SerializeField]
	public float width
	{
		get { 
			CalculateSize();//doesnt get the correct value right away on orientation change, so no use: if(Screen.width!=storedScreenWidth){CalculateSize();}
			return w; 
		}
		set { w = value; }
	}
	[SerializeField]
	public float height
	{
		get { 
			CalculateSize();//doesnt get the correct value right away on orientation change, so no use: if(Screen.height!=storedScreenHeight){CalculateSize();}
			return h; }
		set { h = value; }
	}

	public float ratio = 1;

	private float storedScreenWidth;
	private float storedScreenHeight;

	void Update(){
		if(Screen.width!=storedScreenWidth || Screen.height!=storedScreenHeight){
			w = storedScreenWidth= Screen.width;
			h = storedScreenHeight= Screen.height;
			CalculateSize();
		}
	}
	
	public void CalculateSize(){
		if(mRoot==null){
			//= GetComponent<UIRoot>(); //NGUITools.FindInParents<UIRoot>(gameObject);
			mRoot= FindObjectOfType(typeof(UIRoot)) as UIRoot;
		}
		ratio = (float)mRoot.activeHeight / Screen.height;
		
		width = Mathf.Ceil(Screen.width * ratio);
		height = Mathf.Ceil(Screen.height * ratio);
	}
}