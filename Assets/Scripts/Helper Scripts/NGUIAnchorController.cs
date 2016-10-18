using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UIWidget))]
public class NGUIAnchorController : MonoBehaviour 
{
	// Struct used to store anchor data. Anchor data is restored to NGUI after panel transitions.
	[System.Serializable]
	public struct tAnchorData {
		public Transform target;
		public float relative;
		public float absolute;

		public tAnchorData(Transform t, float r, float a) { target = t; relative = r; absolute = a; }
		public void SetAnchorData(Transform t, float r, float a) { target = t; relative = r; absolute = a; }
		public void SetTarget(Transform t) { target = t; }
		public void SetRelative(float r) { relative = r; }
		public void SetAbsolute(float a) { absolute = a; }
	}

	public enum Anchor { Top, Bottom, Left, Right, All };
	private UISprite uiSprite;
	private UITexture uiTexture;
	private bool isUsingSprite;
	public tAnchorData anchorTop, anchorBottom, anchorLeft, anchorRight;


	public UISprite Sprite {
		get { return uiSprite; }
		set { uiSprite = value; }} 

	public UITexture Texture {
		get { return uiTexture; }
		set { uiTexture = value; }}


	void Start()
	{
		Sprite = this.GetComponent<UISprite>();
		Texture = this.GetComponent<UITexture>();

		if(Texture == null) {
			isUsingSprite = true;
			anchorTop = new tAnchorData(Sprite.topAnchor.target, Sprite.topAnchor.relative, Sprite.topAnchor.absolute);
			anchorBottom = new tAnchorData(Sprite.bottomAnchor.target, Sprite.bottomAnchor.relative, Sprite.bottomAnchor.absolute);
			anchorLeft = new tAnchorData(Sprite.leftAnchor.target, Sprite.leftAnchor.relative, Sprite.leftAnchor.absolute);
			anchorRight = new tAnchorData(Sprite.rightAnchor.target, Sprite.rightAnchor.relative, Sprite.rightAnchor.absolute);
		}
		else {
			isUsingSprite = false;
			anchorTop = new tAnchorData(Texture.topAnchor.target, Texture.topAnchor.relative, Texture.topAnchor.absolute);
			anchorBottom = new tAnchorData(Texture.bottomAnchor.target, Texture.bottomAnchor.relative, Texture.bottomAnchor.absolute);
			anchorLeft = new tAnchorData(Texture.leftAnchor.target, Texture.leftAnchor.relative, Texture.leftAnchor.absolute);
			anchorRight = new tAnchorData(Texture.rightAnchor.target, Texture.rightAnchor.relative, Texture.rightAnchor.absolute);
		}

	}

	public void UpdateAnchors() 
	{ 
        if(Application.isPlaying) {
		    if(isUsingSprite)
			    Sprite.ResetAndUpdateAnchors();
		    else
			    Texture.ResetAndUpdateAnchors();
        }
	}

	public void ReleaseAnchors(Anchor anchor) 
	{
		switch(anchor) {
			case Anchor.All:
				Sprite.topAnchor.target = null;
				Sprite.bottomAnchor.target = null;
				Sprite.leftAnchor.target = null;
				Sprite.rightAnchor.target = null;
	 			break;

			case Anchor.Top: Sprite.topAnchor.target = null; break;
			case Anchor.Bottom: Sprite.bottomAnchor.target = null; break;
			case Anchor.Left: Sprite.leftAnchor.target = null; break;
			case Anchor.Right: Sprite.rightAnchor.target = null; break;
		}
	}

	public void SetAnchor(Anchor side, Transform target, float anchorSide, float anchorValue) 
	{
		switch(side)
		{
			case Anchor.Top:
				Sprite.topAnchor.Set(target, anchorSide, anchorValue);
				anchorTop.SetAnchorData(target, anchorSide, anchorValue);
				break;
			case Anchor.Bottom:
				Sprite.bottomAnchor.Set(target, anchorSide, anchorValue);
				anchorBottom.SetAnchorData(target, anchorSide, anchorValue);
				break;
			case Anchor.Left:
				Sprite.leftAnchor.Set(target, anchorSide, anchorValue);
				anchorLeft.SetAnchorData(target, anchorSide, anchorValue);
				break;
			case Anchor.Right:
				Sprite.rightAnchor.Set(target, anchorSide, anchorValue);
				anchorRight.SetAnchorData(target, anchorSide, anchorValue);
				break;
		}
	}

	public void RestoreAnchors()
	{
		Sprite.leftAnchor.Set(anchorLeft.target, anchorLeft.relative, anchorLeft.absolute);
		Sprite.rightAnchor.Set(anchorRight.target, anchorRight.relative, anchorRight.absolute);
		Sprite.topAnchor.Set(anchorTop.target, anchorTop.relative, anchorTop.absolute);
		Sprite.bottomAnchor.Set(anchorBottom.target, anchorBottom.relative, anchorBottom.absolute);
	}
}
