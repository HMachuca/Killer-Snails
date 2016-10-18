//ugur: work here: i use this in a few projects, get rid of the parts not needed for tiggly numbers

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using AssemblyCSharp;

public class DisplayManager : MonoBehaviour 
{

	public bool loadTextSizesOnStart = true;//ugur: added this since we also need the DisplayManager in the launch scene but there don't want it to load the fonts etc so one can toggle this off there


	public enum FontScale { VerySmall, Small, Medium, Large, VeryLarge }
	public enum DisplayType { Phone, MiniTablet, Tablet, Laptop, Desktop }
	
	public float ScaleFactor;
	public float ButtonScaleFactor;
	public FontScale FontSetting;
	public DisplayType Device;
	public float Dpi;
	
	public float ScreenPixels;
	public float ScreenPhysicalSize;
	
	public bool ScalePhysicalObjects = true;
	
	private List<float> textureStandardSizes;

	public const float defaultDpi = 130f;
	
	private const string fontPrefabsPath = "Prefabs/UI/Fonts/";
	private GameObject prefab;
	
	private GameObject goFontFolder;
	public GameObject FontFolder 
	{
		get
		{
			if (goFontFolder == null)
			{
				goFontFolder = GameObject.Find("Fonts");
			}
			return goFontFolder;
		}
	}

	//ugur: added this so i can access this easily from other places
	protected static DisplayManager _instance;
	public static DisplayManager instance{
		get {
			if (_instance == null){
				_instance = FindObjectOfType(typeof(DisplayManager)) as DisplayManager;//new DisplayManager();
				if(_instance== null){
					//Debug.Log("Warning: there should always be an instance of DisplayManager in the scene.");
				}
			}
			return _instance;
		}
	}


	
	#region Standard pre-packaged Text sizes
	public List<UIFont> TextSizes;










	#endregion
	
	
	#region MonoBehaviour Events
	
	void Awake()
	{
		SetDevice();
	}
	
	void Start () 
	{	
		LoadStandardTextureSizes();	
		if(loadTextSizesOnStart){

		}
		
		// TODO - Set up MaxHeight
	}
	#endregion
	
	
	#region Private Methods
	
	private void SetDevice()
	{
		// Determine sceen dpi
		Dpi = Screen.dpi;
		//Debug.Log("DPI: " + Screen.dpi);
		if (Dpi == 0f) Dpi = defaultDpi;
		
		// TEST CODE: 
		// Dpi = 320f;
		
		// Determine screen larger dimension size
		ScreenPixels = Screen.width > Screen.height ? Screen.width : Screen.height;
		ScreenPhysicalSize = ScreenPixels / Dpi;
		
		// Determine raw scaling factors
		ScaleFactor = Dpi / defaultDpi;
		ButtonScaleFactor = ScaleFactor;
		
		// Set device type and adjust scaling factor
		if (ScreenPhysicalSize < 5f) 
		{
			Device = DisplayType.Phone;
			ScaleFactor *= 0.8f;	
			ButtonScaleFactor *= 0.8f;
		}
		else if (ScreenPhysicalSize < 7f) 
		{
			Device = DisplayType.MiniTablet;
			ScaleFactor *= 0.9f;	
			ButtonScaleFactor *= 0.9f;
		}
		else if (ScreenPhysicalSize < 10f) 
		{
			Device = DisplayType.Tablet; // This is the standard we're working from
		}
		else if (ScreenPhysicalSize < 17f)
		{
			Device = DisplayType.Laptop;
			ScaleFactor *= 1.2f;
			ButtonScaleFactor *= 1.1f;
		}
		else
		{
			Device = DisplayType.Desktop;
			ScaleFactor *= 1.4f;
			ButtonScaleFactor *= 1.2f;
		}


		FontSetting = FontScale.VerySmall;
		//ugur: since we globally scale the ui screen size and dpi based now, no need to set the font large here anymore
		/*
		// Now set the Font Scale setting
		if (ScaleFactor < 1.15f) FontSetting = FontScale.VerySmall;
		else if (ScaleFactor < 1.35f) FontSetting = FontScale.Small;
		else if (ScaleFactor < 1.75f) FontSetting = FontScale.Medium;
		else if (ScaleFactor < 2.3f) FontSetting = FontScale.Medium;//FontScale.Large;
		else FontSetting = FontScale.Large;//FontScale.VeryLarge;

		Debug.Log("ScaleFactor:"+ScaleFactor+",FontSetting:"+FontSetting);
		*/


		// 
	}
	
	private void LoadStandardTextureSizes()
	{
		textureStandardSizes = new List<float>();
		textureStandardSizes.Add(8f);
		textureStandardSizes.Add(12f);
		textureStandardSizes.Add(16f);
		textureStandardSizes.Add(24f);
		textureStandardSizes.Add(32f);
		textureStandardSizes.Add(48f);
		textureStandardSizes.Add(64f);
		textureStandardSizes.Add(96f);
		textureStandardSizes.Add(128f);
		textureStandardSizes.Add(192f);
		textureStandardSizes.Add(256f);
	}
	


	#endregion
	
	
	#region Public Methods
	public float GetTextureSize(float defaultSize)
	{
		float sizeToUse = defaultSize;
		
		if (ScaleFactor != 1f)
		{
			// Get the closest standard texture size to the size we actually want
			float targetSize = defaultSize * ScaleFactor;	
			float minRatio = 999f;
			
			foreach (float size in textureStandardSizes)
			{
				float ratio = targetSize > size ? targetSize / size : size / targetSize;
				if (ratio < minRatio)
				{
					minRatio = ratio;
					sizeToUse = size;
				}
			}
						
			// This doesn't work, but it should..
			//float textureSize = textureStandardSizes.Min(item => Mathf.Abs(item - targetSize));
		}
					
		return sizeToUse;
	}
	

	
	public float GetDeviceObjectScalingFactor()
	{
		float factor = 1f;
		if (ScalePhysicalObjects)
		{
			// Use iPad4 screen as the standard 
			const float standard = 2048f / 264f;
			float height = Screen.height / Dpi;
			
			// Calculate the raw factor required to make objects the same physical size as the standard
			factor = standard / height;
			
			// Now adjust size according to the device type
			factor *= GetDeviceScreenObjectScalingFactor();
		}
		
		return factor;
	}
	
	public float GetDeviceScreenObjectScalingFactor()
	{
		float factor = 1.0f;
		
		switch (Device)
		{
			case DisplayType.Phone:
				factor *= 0.8f;
				break;
			case DisplayType.MiniTablet:
				factor *= 0.9f;
				break;
			case DisplayType.Tablet:	
				break;
			case DisplayType.Laptop:
				factor *= 1.1f;
				break;
			case DisplayType.Desktop:
				factor *= 1.2f;
				break;
			default:
				break;
		}
		
		return factor;
	}
	
	public float GetPixelSize(float physicalSizeInches, bool adjustForDevice)
	{
		float pixels = Dpi * physicalSizeInches;
		if (adjustForDevice) pixels *= GetDeviceScreenObjectScalingFactor();
		return pixels;
	}
	#endregion
}
