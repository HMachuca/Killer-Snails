using UnityEngine;
using System.Collections;

public class CActionEndPrompt : CBasePanel
{
    public GameObject FeedButton;
    public UILabel lblFlavorText;
    public CPreyCard currentPreyCard;
    private int currentPreyIndex;
    

    public void ExpandPanel() { CGlobals.TweenScale(anchorPanelBackground.gameObject, Vector3.one, 0.5f, iTween.EaseType.easeInSine, true); }
	public IEnumerator CollapsePanel()
    {
        CGlobals.TweenScale(anchorPanelBackground.gameObject, new Vector3(0f, 0.9f, 0.9f), 0.5f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator CollapsePanel_External()
    {
        CGlobals.TweenScale(anchorPanelBackground.gameObject, new Vector3(0f, 0.9f, 0.9f), 0.5f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.5f);
    }


	public void MovePreyCardToPanel(CPreyCard preyCard) { StartCoroutine(MovePreyCardToPanel_CR(preyCard)); }
    IEnumerator MovePreyCardToPanel_CR(CPreyCard preyCard)
    {
        currentPreyCard = preyCard;
        currentPreyIndex = preyCard.transform.GetSiblingIndex();
        currentPreyCard.transform.parent = this.transform;
        CGlobals.UpdateWidgets();
        StartCoroutine(FadeBG());
        
        CGlobals.TweenMove(currentPreyCard.gameObject, "position", new Vector3(-270f, 375f, 0f), 0.5f, iTween.EaseType.easeInSine, true);
        CGlobals.TweenScale(currentPreyCard.gameObject, Vector3.one * 0.87f, 0.5f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.5f);

        ExpandPanel();
        lblFlavorText.text = currentPreyCard.GetFlavorText();
	}


    public IEnumerator FadeBG(bool fadeIn = true, float duration = 1f)
    {
        uiFadeSprite.enabled = true;
        Color spriteColor = (fadeIn == true ? new Color(0f, 0f, 0f, CGlobals.FADE_BG_ALPHA) : Color.clear);
        CGlobals.iTweenValue(this.gameObject, uiFadeSprite.color, spriteColor, duration, this.gameObject, "OnUpdateFadeBGColor", iTween.EaseType.easeOutCubic, "OnUpdateBGColorComplete");
        yield return new WaitForSeconds(duration);
    }
    public void OnUpdateFadeBGColor(Color colorTo) { uiFadeSprite.color = colorTo; }
    public void OnUpdateBGColorComplete()
    {
        uiFadeSprite.enabled = (uiFadeSprite.alpha == 0) ? false : true;
    }


    public void ClosePanel() { StartCoroutine(ClosePanel_CR()); }
    public IEnumerator ClosePanel_CR()
    {
        yield return CollapsePanel();

        StartCoroutine(FadeBG(false));
        currentPreyCard.transform.parent = CUIManager.instance.activePlayerPanel.gridPrey.transform;
        currentPreyCard.transform.SetSiblingIndex(currentPreyIndex);
        CUIManager.instance.activePlayerPanel.gridPrey.UpdateGrid();
    }


    public void FeedSnail()
    {
        CUIManager.instance.actionPanel.FeedSnail(currentPreyCard);
    }
}


