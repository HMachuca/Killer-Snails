using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CBasePanel : MonoBehaviour
{
    public NGUIAnchorController anchorPanelBackground;
	public BoxCollider bgCollider;
    public UISprite uiFadeSprite;

    public List<TweenColor> lstTweenColor;

    //public List<Transform> lstAnchors;

    protected virtual void Start()
    {
    }

    public virtual void EnableFlashBG(bool enable)
    {
        foreach (TweenColor tc in lstTweenColor)
            tc.enabled = enable;
    }

	public virtual void EnableCollider(bool enable)
	{
		if(!bgCollider)
			return;

		bgCollider.enabled = enable;
	}
}
