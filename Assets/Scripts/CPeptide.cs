using UnityEngine;
using System.Collections;


public class CPeptide : MonoBehaviour
{
    public PlayerData.PLAYER owner; // used for Presentation card

    public CardData.PeptideType peptide;
    private UISprite uiPeptideSprite;
    public UISprite uiBGSprite;
    public UISprite peptideBorder;

    public UISprite PeptideSprite {
    	get { return uiPeptideSprite; } }


    void Awake()
    {
        uiPeptideSprite = GetComponent<UISprite>();
    }

    public void Enable(bool enable = true)
    {
        uiPeptideSprite.enabled = enable;
        uiBGSprite.enabled = enable;
        peptideBorder.enabled = enable;
    }

    public void EnableButton(bool enable = true)
    {
        UIButton button = GetComponent<UIButton>();
        if(button)
            button.enabled = enable;
    }

    public void SetPeptideAs(CardData.PeptideType peptide)
    {
        if (peptide == CardData.PeptideType.Unknown) {
            uiPeptideSprite.spriteName = "random";
            uiPeptideSprite.GetComponent<UIButton>().normalSprite = "random";
        }
        else {
            uiPeptideSprite.spriteName = peptide.ToString().ToLower();
            uiPeptideSprite.GetComponent<UIButton>().normalSprite = peptide.ToString().ToLower();
        }
    }

	public void ChangeBorderColor(bool resetColor = false, float duration = 0.25f)
    {
	    CGameManager gameMan = CGameManager.instance;

	    if(resetColor) {
			CGlobals.iTweenValue(this.gameObject, peptideBorder.color, Color.white, duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
		}
		else {
			if(gameMan.activePlayer.player == PlayerData.PLAYER.ONE)
				CGlobals.iTweenValue(this.gameObject, peptideBorder.color, Color.red, duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
			else if(gameMan.activePlayer.player == PlayerData.PLAYER.TWO)
				CGlobals.iTweenValue(this.gameObject, peptideBorder.color, new Color(0.086f, 0.73f, 0f, 1f), duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
			else if(gameMan.activePlayer.player == PlayerData.PLAYER.THREE)
				CGlobals.iTweenValue(this.gameObject, peptideBorder.color, new Color(1f, 0.73f, 0.46f, 1f), duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
			else if(gameMan.activePlayer.player == PlayerData.PLAYER.FOUR)
				CGlobals.iTweenValue(this.gameObject, peptideBorder.color, new Color(0.77f, 0f, 0.89f, 1f), duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
		}
    }

    public void ChangeBorderColor(Color newColor, float duration = 0.25f)
    {
	    CGameManager gameMan = CGameManager.instance;
		CGlobals.iTweenValue(this.gameObject, peptideBorder.color, newColor, duration, this.gameObject, "ChangeBorderColorTween", iTween.EaseType.easeOutSine);
    }

    public void ChangeBorderColorTween(Color color)
    {
    	peptideBorder.color = color;
    	//Debug.Log(color);
    }

    public void SetWidgetDepth(int depth)
    {
        uiPeptideSprite.depth = depth + 1;
        uiBGSprite.depth = depth;
        peptideBorder.depth = depth + 1;
    }


    // Used for peptides in cabal panel. Peptides on cards only use type.
    //public GameObject[] goPeptides;
    //public void GeneratePeptide()
    //{
    //    CardData.PeptideType randomPeptide = (CardData.PeptideType)Random.Range(0, (int)CardData.PeptideType.Total);

    //    foreach (GameObject goPeptide in goPeptides) {
    //        if (goPeptide.name.ToLower() == randomPeptide.ToString().ToLower()) {
    //            goPeptide.GetComponent<UITexture>().enabled = true;
    //            peptide = randomPeptide;
    //        }
    //        else {
    //            goPeptide.GetComponent<UITexture>().enabled = false;
    //        }
    //    }
    //}
}
