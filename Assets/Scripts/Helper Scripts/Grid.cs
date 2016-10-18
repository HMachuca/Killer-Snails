using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public enum DIRECTION { HORIZONTAL, VERTICAL, CENTERED };
    public DIRECTION meDirection;

    private UITexture DEBUG_TEXTURE;
    private Vector2 mv2DEBUG_TEXTURE_DIMENSIONS;

    private float MOVE_TIME = 0.45f;
    private float SCALE_TIME = 0.45f;

    public Vector2 mv2GridDimensions;
    public Vector2 mv2CellMaxDimensions;
    private Vector2 mv2CCellDimensions;
    
    public int mnBorder;
    private int mnBorderWidth;

    private float mfCellWidth;
    private float mfCellHeight;

    private List<Transform> mlstChildren;
    private List<Vector3> mv3MidpointPositions;  // Contains mid-point between each item. Helps for drag and drop into grid.

    public bool mbLiveUpdate;

    public List<Vector3> lstCurrentPositions;


    public Vector2 CurrentCellScale {
        get { return mv2CCellDimensions; }
        private set { }
    }

    public List<Vector3> MidpointPositions {
        get { return mv3MidpointPositions; }
    }

    public float MoveTime {
        get { return MOVE_TIME; }
        private set { }
    }

    public float ScaleTime {
        get { return SCALE_TIME; }
        private set { }
    }

 
	void Awake()
    {
        mlstChildren = new List<Transform>();
        mv3MidpointPositions = new List<Vector3>();
        lstCurrentPositions = new List<Vector3>();
        mv2CCellDimensions = new Vector2();

#if UNITY_EDITOR
            DEBUG_TEXTURE = this.gameObject.AddComponent<UITexture>();
            DEBUG_TEXTURE.mainTexture = (Texture)Resources.Load("BackgroundPanelSmaller") as Texture;
            DEBUG_TEXTURE.enabled = false;
#endif

        UpdateGrid();
    }

	
	void Update ()
    {
	    if(mbLiveUpdate)
            UpdateGrid();

        #if UNITY_EDITOR
            DEBUG_TEXTURE.width = (int)mv2GridDimensions.x;
            DEBUG_TEXTURE.height = (int)mv2GridDimensions.y;
        #endif
	}


    ///////////////////////////////////////////////////////
    // This function MUST run AFTER any other tweens! 
    ///////////////////////////////////////////////////////
    public void UpdateGrid(float fTime = -1f,  bool ControlAllMovementExternally = false, bool ControlHorizontalMovement = false, bool ControlVerticalMovement = false)
    {
        float fUpdateTime = fTime;
        if (fUpdateTime == -1f)
            fUpdateTime = MOVE_TIME;

        UpdateChildren();

        // Get pixel space for all borders
        // Calculate cell width for grid objects
        mnBorderWidth = mnBorder * (mlstChildren.Count + 1);
        mfCellWidth = (mv2GridDimensions.x - mnBorderWidth) / mlstChildren.Count;
        mfCellHeight = (mv2GridDimensions.y - mnBorderWidth) / mlstChildren.Count;

        if (mfCellWidth > mv2CellMaxDimensions.x)
            mfCellWidth = mv2CellMaxDimensions.x;
        if (mfCellHeight > mv2CellMaxDimensions.y)
            mfCellHeight = mv2CellMaxDimensions.y;

        ScaleChildren(fUpdateTime);
        MoveChildren(fUpdateTime);
    }


    private void UpdateChildren()
    {
        mlstChildren.Clear();
        mv3MidpointPositions.Clear();
        foreach (Transform child in this.transform)
        	if(child.gameObject.activeSelf)
	            mlstChildren.Add(child);
    }


    public void SetChildrenPositionInstant(bool zeroVector = true, Vector3 position = default(Vector3))
    {
        foreach (Transform child in this.transform)
            child.localPosition = (zeroVector) ? Vector3.zero : position;
    }


    private void MoveChildren(float fUpdateTime)
    {
        lstCurrentPositions.Clear();

        for (int i = 0; i < mlstChildren.Count; ++i) {
            // negFactor helps with anchoring from the center (0.5f) of the list.
            // for more anchoring options, this variable will have to be configured to do so.
            float negFactor = (i < mlstChildren.Count * 0.5f) ? -1f : 1f;
            float diff = Mathf.Abs(i - ((mlstChildren.Count - 1) * 0.5f));

            float targetXPos = (mfCellWidth * diff) * negFactor;
            targetXPos += (mnBorder * diff) * negFactor;

            float targetYPos = (mfCellHeight * diff) * negFactor;
            targetYPos += (mnBorder * diff) * negFactor;
            
            if(meDirection == DIRECTION.HORIZONTAL) {
                Vector3 newPos = new Vector3(targetXPos, 0f, 0f);
                iTween.MoveTo(mlstChildren[i].gameObject, iTween.Hash("position", newPos,
                                                                    "time", fUpdateTime,
                                                                    "easetype", iTween.EaseType.easeInSine,
                                                                    "isLocal", true));
                lstCurrentPositions.Add(newPos);
            }
            else if(meDirection == DIRECTION.VERTICAL) {
                Vector3 newPos = new Vector3(0f, targetYPos, 0f);
                iTween.MoveTo(mlstChildren[i].gameObject, iTween.Hash("position", newPos,
                                                                    "time", fUpdateTime,
                                                                    "easetype", iTween.EaseType.easeInSine,
                                                                    "isLocal", true));
                lstCurrentPositions.Add(newPos);
            }
        }

        for(int i = 0; i < lstCurrentPositions.Count-1; ++i)
        {
            Vector3 midPoint = (lstCurrentPositions[i] + lstCurrentPositions[i + 1]) / 2;
            mv3MidpointPositions.Add(midPoint);
        }


    }


    private void ScaleChildren(float fUpdateTime)
    {
        foreach (Transform child in mlstChildren)
        {
            UISprite uiTex = child.GetComponent<UISprite>();
            Grid grid = child.GetComponent<Grid>();
            if (uiTex) {
                mv2CCellDimensions.x = mfCellWidth / uiTex.width;
                mv2CCellDimensions.y = mfCellHeight / uiTex.height;
            }
            else if (grid) {
                mv2CCellDimensions.x = mfCellWidth / ((grid.mv2CellMaxDimensions.x == 0) ? 1 : grid.mv2CellMaxDimensions.x);
                mv2CCellDimensions.y = mfCellHeight / ((grid.mv2CellMaxDimensions.x == 0) ? 1 : grid.mv2CellMaxDimensions.x);
            }

            if (meDirection == DIRECTION.HORIZONTAL)
            {
                iTween.ScaleTo(child.gameObject, iTween.Hash("scale", Vector3.one * mv2CCellDimensions.x,
                                                            "time", fUpdateTime,
                                                            "islocal", true));
            }
            else if (meDirection == DIRECTION.VERTICAL)
            {
                iTween.ScaleTo(child.gameObject, iTween.Hash("scale", Vector3.one * mv2CCellDimensions.y,
                                                            "time", fUpdateTime,
                                                            "islocal", true));
            }
        }
    }
}
