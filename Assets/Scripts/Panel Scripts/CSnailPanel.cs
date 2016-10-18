using UnityEngine;
using System.Collections;

public class CSnailPanel : CBasePanel
{
    public UITexture uiWaves;
    public UITexture uiBG;

	void Start ()
    {
	
	}
	
	public override void EnableFlashBG(bool enable)
    {
        base.EnableFlashBG(enable);

        if(!enable) {
            uiWaves.color = Color.white;
            uiBG.color = Color.white;
        }
	}
}
 