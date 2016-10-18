using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


//TODO:  a script that that the slide colliders to update the active item when it collides with the vertical slider.  Raead up on ontrigger enger.
public class CTutorial : MonoBehaviour
{
	#region Singleton Instance
	
	private static CTutorial _instance;
	public static CTutorial instance
	{
		get{
			if (_instance == null){
				_instance = FindObjectOfType<CTutorial>();}
			return _instance;}
	}
	
	#endregion
	
	private readonly int THUMBNAIL_WIDTH = 2048;
	private readonly int THUMBNAIL_HEIGHT = 1536;
	
	private readonly int MAX_LIMIT = 10;
	

    // Needed variables
    private readonly int UNITS_BETWEEN = 1300;              // Units between objects
    private readonly float MAXIMUM_SWIPE_DURATION = 0.3f;   // max swipe duration
	private readonly float MINIMUM_SWIPE_DISTANCE = 50.0f;  // min swipe duration
	private readonly float SWIPE_SCROLL_DURATION = 0.7f;    // how long swipe is after release of touch

    // struct to hold swipe data
    [System.Serializable]
    public struct tSwipeDetails
    {
        public float _fSwipeStartPosX;
        public float _fSwipeStartTime;
        public bool _bSwipeDirectionLeft;

        // TODO: Debugging purposes only. Remove when not needed.
        public float _fSwipeDist;
        public float _fSwipeDuration;
    }

    [System.Serializable]
    public struct tActiveItemDetails  //create a transform o
    {
        public Transform _slide;  //ctutoriial_slide;
        public int _nIndex;
        //public bool _bVideo;

        public void Clear() { _slide = null; _nIndex = 0; }
    }
    
    public tActiveItemDetails m_tActiveItemDetails;

    public Vector3 m_v3InitialOffset;
	public Vector3 m_v3TouchOffset;
	public Vector3 m_v3TouchWorldPosition;
	
	public Transform m_trItemsParent;
	private tSwipeDetails m_tSwipeDetails;
	
	private AudioClip m_acSFXNewItem;

    public Camera nguicamera;

	public List<Collider> UIColliders;
    private bool freezeInput;
    private float freezeCounter;

    public List<Transform> gallery = new List<Transform>();  //for each transform in a parent transform, detect all transfor
    public GameObject TutorialPrompt;
    int a = 0;

    public Vector3 InitialOffset {
		get { return m_v3InitialOffset; }
		set { m_v3InitialOffset = value; }}
	
	public Vector3 TouchOffset {
		get { return m_v3TouchOffset; }
		set { m_v3TouchOffset = value; }}
	
	public Vector3 TouchWorldPosition {
		get { return m_v3TouchWorldPosition; }
		set { m_v3TouchWorldPosition = value; }}

    public void SetNewActiveItem(Transform slide, int index)
    {
        m_tActiveItemDetails._slide = slide;
        m_tActiveItemDetails._nIndex = index;
    }
	
	public void Start()
    {
        foreach(Transform child in m_trItemsParent) {
            gallery.Add(child);
        }

        SetNewActiveItem(gallery[0], 0);
    }


	void LateUpdate()
	{
        if(freezeInput)
        {
            freezeCounter += Time.deltaTime;

            if(freezeCounter >= SWIPE_SCROLL_DURATION + 0.1f)
            {
                freezeCounter = 0f;
                freezeInput = false;
            }
        }
        else
        {
		    HandleInput();
        }
	}

	public void HandleInput()
	{
		Camera CameraNGUI = nguicamera;
	
        ///////////////////////////////////////////////////////////////
        // Check out this functionality for scrollable textures
        ///////////////////////////////////////////////////////////////
        // 
        //
		if(Input.GetMouseButtonDown(0))
		{
            // At first touch, we record the current swipe time and current screen x position
			m_tSwipeDetails._fSwipeStartTime = Time.time;
			m_tSwipeDetails._fSwipeStartPosX = Input.mousePosition.x;
			
            // Get ScreenToWorld position
			Vector3 v3ScreenToWorldPoint = CameraNGUI.ScreenToWorldPoint(Input.mousePosition);
			v3ScreenToWorldPoint.y = 50f;
			v3ScreenToWorldPoint.z = 0f;
			
			//Ray ray = new Ray(v3ScreenToWorldPoint, Vector3.forward*1000f);
            // Create a ray object. Cast that ray in 3d world space and we check what objects are hit (hits)
			Ray ray = CameraNGUI.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
            // for each object hit, we determine which object we want to manipulate using layers. (you dont have to use layers, i did in this case)
			foreach(RaycastHit hit in hits)
			{
				//if(LayerMask.LayerToName(hit.collider.gameObject.layer) == "NGUI" && hit.collider.name == "Gallery Items")
				{
                    // Update World Position
					TouchWorldPosition = v3ScreenToWorldPoint;

                    // Update offset from center of texture to ScreenToWorld point
                    TouchOffset = m_trItemsParent.position - TouchWorldPosition;
				}
			}
		}
		else if(Input.GetMouseButton(0))     // Dragging
		{
            // Scrolling gallery
			//ScrollGallery();
		}
		else if(Input.GetMouseButtonUp(0))   // End touch/drag
		{
            // Get swipe duration and swipe distance
			float fSwipeDuration = Time.time - m_tSwipeDetails._fSwipeStartTime;
			float fDistance = Input.mousePosition.x - m_tSwipeDetails._fSwipeStartPosX;

            if (fDistance == 0f)
                return;

            freezeInput = true;
            
			Hashtable ht = iTween.Hash("position", Vector3.zero,
			                           "time", SWIPE_SCROLL_DURATION,
			                           "easetype", iTween.EaseType.easeOutCirc,
			                           "isLocal", true,
			                           "onComplete", "SetInitialOffset",
			                           "onCompleteTarget", this.gameObject);
            
            // Check direction
			m_tSwipeDetails._bSwipeDirectionLeft = (fDistance < 0) ? true : false;
			fDistance = Mathf.Abs(fDistance);

            // Set final tSwipeDetails variables
			m_tSwipeDetails._fSwipeDist = fDistance;
			m_tSwipeDetails._fSwipeDuration = fSwipeDuration;
			
            // Check for valid swipe
			if(fSwipeDuration < MAXIMUM_SWIPE_DURATION && fDistance > MINIMUM_SWIPE_DISTANCE)
			{
                // Get index of texture of current item being viewed
                //REPLACE with your own list of items
                int currentIndex = m_tActiveItemDetails._nIndex;
               // int nIndex = 0;

				Vector3 v3ItemsParent = m_trItemsParent.transform.localPosition;
				
                // if nIndex == 0, we are looking at the first texture in slideshow
				if(currentIndex == 0)
				{
					if(m_tSwipeDetails._bSwipeDirectionLeft)
                    {	
                        // If there is more than one object in list and player swipes, we can move to the next.
						// If not, we move back to the original position.
						//ScrollGallery();
							
						v3ItemsParent.x = (currentIndex+1) * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);

                        SetNewActiveItem(gallery[currentIndex + 1], currentIndex + 1);
					}
					else
					{
						//ScrollGallery();
						
						v3ItemsParent.x = currentIndex * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);
					}
				}
                // REPLACE THIS ELSE IF WITH YOUR OWN LIST OF ITEMS
                // if nIndex == last item in slideshow, we are viewing the last item
				//else if(nIndex == m_lstGalleryItems.Count-1)
                else if(currentIndex == gallery.Count-1)
                {
					if(m_tSwipeDetails._bSwipeDirectionLeft)
					{
						v3ItemsParent.x = currentIndex * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);
					}
					else
					{
						v3ItemsParent.x = (currentIndex-1) * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);

                        SetNewActiveItem(gallery[currentIndex - 1], currentIndex - 1);
					}
				}
                // else, we are in the middle of the slideshow
				else
				{
					if(m_tSwipeDetails._bSwipeDirectionLeft)
					{
						v3ItemsParent.x = (currentIndex+1) * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);

                        SetNewActiveItem(gallery[currentIndex + 1], currentIndex + 1);
					}
					else
					{
						v3ItemsParent.x = (currentIndex-1) * -UNITS_BETWEEN;
						ht["position"] = v3ItemsParent;
						iTween.MoveTo(m_trItemsParent.gameObject, ht);

                        SetNewActiveItem(gallery[currentIndex - 1], currentIndex - 1);
					}
				}
			}  
			else // invalid swipe
			{
                //int nIndex = m_tActiveItemDetails._nIndex;

                // REPLACE "0" with the current active item' index
                int currentIndex = m_tActiveItemDetails._nIndex;

				Vector2 v2MinMax = new Vector2(currentIndex * UNITS_BETWEEN, (currentIndex+1) * UNITS_BETWEEN);
				float fLimitX = (v2MinMax.x + v2MinMax.y) * 0.5f;
				Vector3 v3ItemsParent = m_trItemsParent.transform.localPosition;
				
				// Based on position in list we have to snap back into position
				if(currentIndex == 0)
				{
					v3ItemsParent.x = 0;
				}
                // REPLACE list with your own list of items
				//else if(nIndex == m_lstGalleryItems.Count-1)
                else if(currentIndex == gallery.Count-1)
				{
					v3ItemsParent.x = currentIndex * -UNITS_BETWEEN;
				}
				else
				{
					if(v3ItemsParent.x >= fLimitX)
						v3ItemsParent.x = -v2MinMax.y;
					else if(v3ItemsParent.x < fLimitX)
						v3ItemsParent.x = -v2MinMax.x;
				}
				
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);
			}
		}
	}
	private void ScrollGallery()
	{
        // Get current world position of screen point
		TouchWorldPosition = nguicamera.ScreenToWorldPoint(Input.mousePosition);

        // Get new position with offset
		Vector3 v3NewPosition = new Vector3((TouchWorldPosition + TouchOffset).x, 0, 0);

        // Set ItemsParent object's new position;
        m_trItemsParent.position = v3NewPosition;// + InitialOffset;
		
		Vector3 v3LocalPosition = m_trItemsParent.localPosition;
		v3LocalPosition.y = 0f; v3LocalPosition.z = 0f;
		m_trItemsParent.localPosition = v3LocalPosition;
	}


    public void OnScrollGalleryButton_Left()
    {
        m_tSwipeDetails._bSwipeDirectionLeft = false;
        ScrollGalleryButton();
    }
    public void OnScrollGalleryButton_Right()
    {
        m_tSwipeDetails._bSwipeDirectionLeft = true;
        ScrollGalleryButton();
    }

    public void ScrollGalleryButton()
    {
        Hashtable ht = iTween.Hash("position", Vector3.zero,
			                        "time", SWIPE_SCROLL_DURATION,
			                        "easetype", iTween.EaseType.easeOutCirc,
			                        "isLocal", true,
			                        "onComplete", "SetInitialOffset",
			                        "onCompleteTarget", this.gameObject);

        // Get index of texture of current item being viewed
        //REPLACE with your own list of items
        int currentIndex = m_tActiveItemDetails._nIndex;
        // int nIndex = 0;

		Vector3 v3ItemsParent = m_trItemsParent.transform.localPosition;
				
        // if nIndex == 0, we are looking at the first texture in slideshow
		if(currentIndex == 0)
		{
			if(m_tSwipeDetails._bSwipeDirectionLeft)
            {	
                // If there is more than one object in list and player swipes, we can move to the next.
				// If not, we move back to the original position.
				//ScrollGallery();
							
				v3ItemsParent.x = (currentIndex+1) * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);

                SetNewActiveItem(gallery[currentIndex + 1], currentIndex + 1);
			}
			else
			{
				//ScrollGallery();
						
				v3ItemsParent.x = currentIndex * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);
			}
		}
        // REPLACE THIS ELSE IF WITH YOUR OWN LIST OF ITEMS
        // if nIndex == last item in slideshow, we are viewing the last item
		//else if(nIndex == m_lstGalleryItems.Count-1)
        else if(currentIndex == gallery.Count-1)
        {
			if(m_tSwipeDetails._bSwipeDirectionLeft)
			{
				v3ItemsParent.x = currentIndex * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);
			}
			else
			{
				v3ItemsParent.x = (currentIndex-1) * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);

                SetNewActiveItem(gallery[currentIndex - 1], currentIndex - 1);
			}
		}
        // else, we are in the middle of the slideshow
		else
		{
			if(m_tSwipeDetails._bSwipeDirectionLeft)
			{
				v3ItemsParent.x = (currentIndex+1) * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);

                SetNewActiveItem(gallery[currentIndex + 1], currentIndex + 1);
			}
			else
			{
				v3ItemsParent.x = (currentIndex-1) * -UNITS_BETWEEN;
				ht["position"] = v3ItemsParent;
				iTween.MoveTo(m_trItemsParent.gameObject, ht);

                SetNewActiveItem(gallery[currentIndex - 1], currentIndex - 1);
			}
		}
    }

    public void OnCloseTutorialPrompt()
    {
        TutorialPrompt.SetActive(false);
    }



    private void SetInitialOffset()
	{
        Vector3 v3Offset = m_trItemsParent.localPosition;
        v3Offset.y = 50f;
        v3Offset.z = 0f;
        InitialOffset = v3Offset;

        InitialOffset = nguicamera.transform.TransformPoint(m_trItemsParent.localPosition);
        TouchOffset = Vector3.zero;
    }

	
	public void ResetItemsPosition() 
	{
		Vector3 v3Pos = m_trItemsParent.localPosition;
		v3Pos.x = 0f;
		m_trItemsParent.localPosition = v3Pos;

         m_tActiveItemDetails._slide = gallery[0];
        m_tActiveItemDetails._nIndex = 0;
	}

   

	//public void OnTriggerEnter(Collider other)
	//{
	//	//if(other.gameObject.layer == LayerMask.NameToLayer("NGUI"))
	//	if(other.gameObject.tag == "Gallery")
	//	{
	//		//m_tActiveItemDetails._ActiveItem = other.GetComponent<GalleryItem>();
			
	//		for(int i = 0; i < m_lstGalleryItems.Count; ++i)
	//			if(m_lstGalleryItems[i] == m_tActiveItemDetails._ActiveItem)
	//			{
	//				m_tActiveItemDetails._nIndex = i;
	//				//m_tActiveItemDetails._bVideo = m_tActiveItemDetails._ActiveItem.IsVideo;
	//				return;
	//			}
	//	}
	//}
	

}