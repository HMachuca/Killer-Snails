/*
Ugur:
this will scale content based on screen dpi with the thought that buttons, unless wanted otherwise for special cases, should always have the same physical dimensions, no matter which resolution, dpi, screen dimension or screen orientation
that way getting around the issues of UIRoot scaling style PixelPerfect (too small on high dpi screen) and FixedSize (buttons getting scaled based on screen height without taking dpi into consideration, so one then suddenly has different button dimensions in portrait and landscape screen orientation)

usage: 
-select the gameObject which has the UIRoot script on it
-set UIRoot scaling style to FixedSize
-Add this script to the gameObject which has the UIRoot script on it

Note: in editor, desktop and some specific devices Screen.dpi returns the value 0, in that case either provide a hardcoded value for targetDpi or else in that case the scaling result will be the same as setting PixelPerfect as scaling style in the UIRoot.
*/

using UnityEngine;
using System.Collections;


[RequireComponent(typeof(UIRoot))]

[ExecuteInEditMode]

public class UIRootFixedPhysicalSizeEnforcer : MonoBehaviour
{
	
	public UIRoot mRoot;

	public bool scaleDownIfLowResScreen = true; //when the res is lower than 1024x768 and this is toggled on, it will scale down the UI to fit better for the low res. You can toggle this off but then have to make sure you reposition/rescale down all to fit for the too low res yourself


	/*
	DPIs on various platforms:
	-pc: 			72
	-iPhone 4 / iPod Touch	4 / 4th Gen	8: 		326
	-iPhone 4S / iPod Touch	4S / 4th Gen: 326
	-iPhone 5 / iPod Touch	5 / 5th Gen: 326
	-iPhone 5C	5C / 5th Gen: 326
	-iPhone 5S	5S / 6th Gen: 326
	-iPad Mini	1st Gen: 163
	-iPad Mini	2nd Gen: 326
	-iPad	1st Gen, 2: 132
	-iPad / iPad Air 3rd Gen, 4th Gen: 264
	*/

	public float scaleFactorForLargeTablets = 1;//1.4F;//by default we adjust the UI to be same physical dimensions accross all devices, but Dave wants it to be scaled larger on large tablets, so one can do that by changing this value..
	public float targetScaleFactor= 1;
	private float storedScaleFactor= 1;

	public bool alsoScaleUpForLargeScreensInEditor = false;


	[HideInInspector]public float targetDpi;

	private const float defaultDpi = 160;//72;//132;//groeserer wert->kleiner
	private float storedScreenWidth;
	private float storedScreenHeight;

	private bool isEditor;


	void Awake () { 
		Application.targetFrameRate = 60;

		if(!mRoot){mRoot = GetComponent<UIRoot>(); }
		targetDpi = Screen.dpi;
		if (targetDpi == 0f) targetDpi = defaultDpi;
	}

	//allows setting the scale factor for the ui, by default we run it at scale setting 1 which rescales all automatically to have the same physical dimensions for controls no matter which
	//screen size/orientation/dpi acrros all devices. then we set it to 1.4 for all other menus than launch menu on tablet since dave wanted controls there to be a tad bigger
	//when showing the launch menu we set it to 1 for tablets, too though, since for that menu, unlike for other menus we ahave a special large screen layout, no use to on top scale it up when running on tablet
	public void SetUIScale(float targetScale){
		//#if UNITY_EDITOR
		Debug.Log("Unity: gameObject.name:"+gameObject.name+",SetUIScale(),targetScale:"+targetScale);
		//#endif

		targetDpi = Screen.dpi;
		if (targetDpi == 0f) targetDpi = defaultDpi;



		targetScaleFactor = targetScale;
		targetDpi = targetDpi*targetScaleFactor;
	}

	public void ScaleUpFroTablets(){
		Debug.Log("ScaleUpForTablets(): Unity: gameObject.name:"+gameObject.name+",ScaleUpFroTablets(),scaleFactorForLargeTablets:"+scaleFactorForLargeTablets);
		SetUIScale(scaleFactorForLargeTablets);

		//1.3 is iPad 1024/768 //kindle fire etc small and widescreen
		if(scaleDownIfLowResScreen && (Screen.width<1024 || Screen.height<768 || Screen.width/Screen.height>1.3F || (DisplayManager.instance.Device==DisplayManager.DisplayType.MiniTablet || DisplayManager.instance.Device==DisplayManager.DisplayType.Phone))){
			#if !UNITY_IPHONE
				//on iOS we have the iPads and know it works well for them, so for those we don't scale down the UI
				Debug.Log("Unity: !!!scaling down UI for low res or small screen");
				SetUIScale(0.7F);
			#endif
		}

		if(DisplayManager.instance.Device==DisplayManager.DisplayType.Phone){
			SetUIScale(0.4F);
		}

	}


	public void SetUIScaleAutomatically(){

		Debug.Log("SetUIScaleAutomatically(),Screen.width:"+Screen.width+"Screen.height:"+Screen.height+"Screen.width/Screen.height:"+(Screen.width/Screen.height));

		Debug.Log("DisplayManager.instance.Device:"+DisplayManager.instance.Device);

		if((!isEditor || alsoScaleUpForLargeScreensInEditor) && DisplayManager.instance.Device== DisplayManager.DisplayType.Tablet){
			//Debug.Log("DisplayType is DisplayType.Tablet, scaling to scaleFactorForLargeTablets: "+scaleFactorForLargeTablets);
			//targetDpi = targetDpi*scaleFactorForLargeTablets;

				//SetUIScale(1);
				ScaleUpFroTablets();

		}else{
			SetUIScale(1);
		}

		if(scaleDownIfLowResScreen || (Screen.width<1024 || Screen.height<768 || Screen.width/Screen.height>1.3F || DisplayManager.instance.Device==DisplayManager.DisplayType.MiniTablet || DisplayManager.instance.Device==DisplayManager.DisplayType.Phone)){
			#if !UNITY_IPHONE
				Debug.Log("Unity: !!!scaling down UI for low res screen");
				SetUIScale(0.7F);
			#endif
		}

		if(DisplayManager.instance.Device==DisplayManager.DisplayType.Phone){
			SetUIScale(0.4F);
		}

	}


	void Start(){
		#if UNITY_EDITOR
			isEditor = true;
		#endif

		if(!isEditor || alsoScaleUpForLargeScreensInEditor){
			/*
			if(DisplayManager.instance.Device== DisplayManager.DisplayType.Tablet){
				#if UNITY_EDITOR
					Debug.Log("DisplayType is DisplayType.Tablet, scaling to scaleFactorForLargeTablets: "+scaleFactorForLargeTablets);
				#endif
				//targetDpi = targetDpi*scaleFactorForLargeTablets;
				ScaleUpFroTablets();
			}
			*/
			//in the launch scene we don't want it to scale larger since showing launch menu there
			//if(Application.loadedLevelName!=DataStorage.launchSceneName){
				SetUIScaleAutomatically();
			//}
		}
	}
	
	void Update ()
	{
		//when the screen dimensions/orientations changes or a new target scalefactor was set, trigger scale/re layout the ui accordingly:
		if(storedScreenWidth!= Screen.width || storedScreenHeight!= Screen.height || targetScaleFactor!=storedScaleFactor){ 
			// needed? : || (Screen.dpi!=0 && targetDpi!=Screen.dpi) ){
			float usedTargetDpi = targetDpi;
//			if (DisplayManager.instance.Device==DisplayManager.DisplayType.Phone)
//				usedTargetDpi *= 0.7f;

			float factor = usedTargetDpi / defaultDpi;



			mRoot.manualHeight = Mathf.FloorToInt(Screen.height/factor);


//			Debug.Log("targetDpi:"+targetDpi+";defaultDpi:"+defaultDpi+";factor:"+factor+",mRoot.manualHeight:"+mRoot.manualHeight);

			//Debug.Log("current screen dpi:"+targetDpi+",factor:"+factor+",manualHeight:"+mRoot.manualHeight);

			storedScreenWidth = Screen.width;
			storedScreenHeight = Screen.height;
			storedScaleFactor = targetScaleFactor;

		}

	}




	//ugur: added this so i can access this easily from other places
	protected static UIRootFixedPhysicalSizeEnforcer _instance;
	public static UIRootFixedPhysicalSizeEnforcer instance{
		get {
			if (_instance == null){
				_instance = FindObjectOfType(typeof(UIRootFixedPhysicalSizeEnforcer)) as UIRootFixedPhysicalSizeEnforcer;
				if(_instance== null){
					//Debug.Log("Warning: there should always be an instance of UIRootFixedPhysicalSizeEnforcer in the scene.");
				}
			}
			return _instance;
		}
	}



}
