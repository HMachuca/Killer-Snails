using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMarketPanel : CBasePanel
{
    public Grid gridMarketCards;
    public List<CBaseCard> lstMarketCards;
    public List<UISprite> lstBuyButtons;
    public List<UISprite> lstActiveBuyButtons;

    private float MARKET_TWEEN_TIME = 0.75f;
    private float INITIAL_MARKET_Y_POS = 0f;

    public Color colorDisabledButton = new Color(0.3f, 0.3f, 0.3f, 1.0f);
    

    protected override void Start()
    {
        base.Start();

        lstMarketCards = new List<CBaseCard>(5);
		INITIAL_MARKET_Y_POS = anchorPanelBackground.transform.localPosition.y;

        foreach (UISprite button in lstBuyButtons)
            CGlobals.AssignNewUIButtonOnClickTarget(this, button.GetComponent<UIButton>(), button.GetComponent<UIButton>(), "BuyCard");
    }
    

	public void ViewMarket()
    {
		anchorPanelBackground.ReleaseAnchors(NGUIAnchorController.Anchor.All);
        CGameManager.instance.CurrentPlayState = GameData.PlayState.VIEWING_MARKET;

        foreach (UISprite buybttn in lstBuyButtons)
            buybttn.color = colorDisabledButton;

        Vector3 v3NewPos = new Vector3(0f, 520f, 0f);
		CGlobals.TweenMove(anchorPanelBackground.gameObject, "position", v3NewPos, MARKET_TWEEN_TIME, iTween.EaseType.easeOutSine, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acOpenMarket);

        CPlayer activePlayer = CGameManager.instance.activePlayer;
        CActivePlayerPanel.instance.panelHand.transform.parent = this.transform;

        foreach (CBaseCard handCard in activePlayer.hand)
            handCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__MARKET_PANEL + 2);

        CGlobals.UpdateWidgets();

        uiFadeSprite.enabled = true;
        Color from = uiFadeSprite.color;
        Color to = new Color(0f, 0f, 0f, 0.6f);
        CGlobals.iTweenValue(this.gameObject, from, to, MARKET_TWEEN_TIME, this.gameObject, "OnUpdateMarketBGColor", iTween.EaseType.easeOutQuad);

        //take cards in hand. change button targets.
        foreach(CBaseCard card in activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(card, null, card.Button, "SelectedCardToSell");
    }

    public void ViewMarket_AfterFeeding()
    {
        CGameManager.instance.CurrentPlayState = GameData.PlayState.VIEWING_MARKET;

        foreach (CBaseCard card in lstMarketCards) {
            card.transform.localEulerAngles = Vector3.up * 180f;
            card.Texture.enabled = false;
            card.TextureBack.enabled = true;

            CGlobals.AssignNewUIButtonOnClickTarget(this, card, card.TextureBack.GetComponent<UIButton>(), "TakeMarketCard");
        }

        ShowBuyButtions(false);


        anchorPanelBackground.ReleaseAnchors(NGUIAnchorController.Anchor.All);
        Vector3 v3NewPos = new Vector3(0f, 520f, 0f);
		CGlobals.TweenMove(anchorPanelBackground.gameObject, "position", v3NewPos, MARKET_TWEEN_TIME, iTween.EaseType.easeOutSine, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acOpenMarket);

        CGlobals.UpdateWidgets();

        uiFadeSprite.enabled = true;
        Color from = uiFadeSprite.color;
        Color to = new Color(0f, 0f, 0f, 0.6f);
        CGlobals.iTweenValue(this.gameObject, from, to, MARKET_TWEEN_TIME, this.gameObject, "OnUpdateMarketBGColor", iTween.EaseType.easeOutQuad);

        //ViewMarket();
    }

    public void ShowBuyButtions(bool show = true)
    {
        foreach (UISprite sprite in lstBuyButtons)
            sprite.gameObject.SetActive(show);
    }


    public void CloseMarket() { StartCoroutine(CloseMarket_CR()); }
    public IEnumerator CloseMarket_CR()
    {
        //CGameManager.instance.audiosource.PlayOneShot(CGameManager.instance.acCloseMarket);
        CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;

        foreach(CBaseCard card in CUIManager.instance.SelectedCards)
        {
            card.Texture.color = Color.white;
            card.Button.defaultColor = Color.white;
        }

        CUIManager.instance.marketPanel.AssignTargetFunction_MoveToActionPanel();
		CGlobals.TweenMove(anchorPanelBackground.gameObject, "y", INITIAL_MARKET_Y_POS, MARKET_TWEEN_TIME, iTween.EaseType.easeOutSine, true);

        foreach (UISprite button in lstBuyButtons) {
            button.color = colorDisabledButton;
            button.GetComponent<UIButton>().enabled = false;
        }
        CUIManager.instance.SelectedCards.Clear();

        Color from = uiFadeSprite.color;
        Color to = new Color(0f, 0f, 0f, 0f);
        CGlobals.iTweenValue(this.gameObject, from, to, MARKET_TWEEN_TIME, this.gameObject, "OnUpdateMarketBGColor", iTween.EaseType.easeOutQuad);
		CActivePlayerPanel.instance.gridHand.UpdateGrid();

        yield return new WaitForSeconds(MARKET_TWEEN_TIME);
        uiFadeSprite.enabled = false;

        CPlayer activePlayer = CGameManager.instance.activePlayer;
		CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;


        foreach (CBaseCard handCard in activePlayer.hand)
            StartCoroutine(handCard.SetNewParentPanel(CardData.ParentPanel.HAND));

        CGlobals.UpdateWidgets();

        foreach(CBaseCard card in activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

		anchorPanelBackground.RestoreAnchors();
    }

    public void UpdateHandPanel(int value)
    {
        CGameManager.instance.activePlayer.HandPanel.height = value;
    }


    public void OnUpdateMarketBGColor(Color colorTo)
    {
        uiFadeSprite.color = colorTo;
    }


    public void UpdateListOrder()
    {
        lstMarketCards.Clear();
        foreach (Transform transform in gridMarketCards.transform)
            lstMarketCards.Add(transform.GetComponent<CBaseCard>());
    }

    public void TakeMarketCard(CBaseCard marketCard) { StartCoroutine(TakeMarketCard_CR(marketCard)); }
    public IEnumerator TakeMarketCard_CR(CBaseCard marketCard)
    {
        yield return new WaitForEndOfFrame();
        CUIManager.instance.DisableAllCollidersForEndTurn();

        //CAudioManager.instance.PlaySound(CGameManager.instance.acBuyCard);
        lstMarketCards.Remove(marketCard);
        marketCard.transform.parent = CUIManager.instance.actionPanel.transform;
        CGlobals.UpdateWidgets();

        StartCoroutine(marketCard.FlipCard(0f, 0.5f, false));
        CGlobals.TweenMove(marketCard.gameObject, "position", Vector3.up * 305f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(marketCard.gameObject, Vector3.one * 0.75f, 0.4f, iTween.EaseType.easeOutSine, true);
        //CGlobals.TweenRotate(marketCard.gameObject, Vector3.zero, 0.5f, iTween.EaseType.linear, true);

        yield return new WaitForSeconds(0.25f);
        CUIManager.instance.marketPanel.gridMarketCards.UpdateGrid();
		yield return StartCoroutine(PositionBuyButtons());
        yield return new WaitForSeconds(1.5f);
        
        int oldDepth = marketCard.Texture.depth;
        marketCard.ChangeWidgetDepth(200);

        yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(CloseMarket_CR());
        foreach(CBaseCard baseCard in lstMarketCards)
        {
            baseCard.transform.localEulerAngles = Vector3.zero;
            baseCard.Texture.enabled = true;
            baseCard.TextureBack.enabled = false;
            baseCard.EnableButton();
            //baseCard.TextureBack.GetComponent<UIButton>().enabled = false;
        }
        
		marketCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;
		CGlobals.UpdateWidgets();
        CGameManager.instance.activePlayer.discard.Add(marketCard);
        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(marketCard)); 

        foreach(CBaseCard card in lstMarketCards)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");
		foreach(CBaseCard card in CGameManager.instance.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

        marketCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + CGameManager.instance.activePlayer.discard.Count + 1);

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    public void AssignTargetFunction_BuyCard()
    {
        foreach(CBaseCard marketCard in lstMarketCards)
            CGlobals.AssignNewUIButtonOnClickTarget(this, marketCard, marketCard.Button, "BuyCard");
    }

    public void AssignTargetFunction_MoveToActionPanel()
    {
        foreach(CBaseCard marketCard in lstMarketCards)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, marketCard, marketCard.Button, "MoveCardToActionPanel");
    }


    void BuyCard(UIButton BuyButton) { StartCoroutine(BuyCard_CR(BuyButton)); }
    private IEnumerator BuyCard_CR(UIButton BuyButton)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

    	CUIManager uiMan = CUIManager.instance;
		CPlayer activePlayer = CGameManager.instance.activePlayer;

        uiMan.DisableAllCollidersForEndTurn();

        yield return new WaitForEndOfFrame();
        //CAudioManager.instance.PlaySound(CGameManager.instance.acBuyCard);
        CBaseCard marketCard = lstMarketCards[BuyButton.transform.GetSiblingIndex()];
        foreach (UISprite button in lstBuyButtons)
            button.GetComponent<UIButton>().enabled = false;

        CAssessmentPanel.instance.lstLastPlayedCards.Add(marketCard);
        lstMarketCards.Remove(marketCard);
        marketCard.transform.parent = uiMan.actionPanel.transform;
        CGlobals.UpdateWidgets();
        CGlobals.TweenMove(marketCard.gameObject, "position", Vector3.up * 305f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(marketCard.gameObject, Vector3.one * 0.75f, 0.4f, iTween.EaseType.easeOutSine, true);
		
		float fBufferTimeForCards = 0.15f * CUIManager.instance.SelectedCards.Count;
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        yield return new WaitForSeconds(1.2f - fBufferTimeForCards);
        uiMan.marketPanel.gridMarketCards.UpdateGrid();
		CActivePlayerPanel.instance.gridHand.UpdateGrid();
		yield return StartCoroutine(PositionBuyButtons());  

        int oldDepth = marketCard.Texture.depth;
        marketCard.ChangeWidgetDepth(200);

        yield return new WaitForSeconds(0.5f);
		StartCoroutine(CloseMarket_CR());
        
		marketCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;
		CGlobals.UpdateWidgets();
        activePlayer.discard.Add(marketCard);
        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(marketCard));

        foreach (CBaseCard removeFromHand in uiMan.SelectedCards)
            activePlayer.hand.Remove(removeFromHand);
    
        uiMan.SelectedCards.Clear();
        CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

        foreach (UISprite button in lstBuyButtons) {
            button.color = colorDisabledButton;
            button.GetComponent<UIButton>().enabled = false;
        }

        foreach(CBaseCard card in lstMarketCards)
            CGlobals.AssignNewUIButtonOnClickTarget(uiMan, card, card.Button, "MoveCardToActionPanel");
		foreach(CBaseCard card in activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

        marketCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + activePlayer.discard.Count + 1);

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    //public void BuyCard(CBaseCard marketCard) { StartCoroutine(BuyCard_CR(marketCard)); }
    public IEnumerator BuyCard_CR(CBaseCard marketCard)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

    	CUIManager uiMan = CUIManager.instance;
		CPlayer activePlayer = CGameManager.instance.activePlayer;

        uiMan.DisableAllCollidersForEndTurn();

        yield return new WaitForEndOfFrame();
        //CAudioManager.instance.PlaySound(CGameManager.instance.acBuyCard);
        foreach (UISprite button in lstBuyButtons)
            button.GetComponent<UIButton>().enabled = false;

        CAssessmentPanel.instance.lstLastPlayedCards.Add(marketCard);
        lstMarketCards.Remove(marketCard);
        marketCard.transform.parent = uiMan.actionPanel.transform;
        CGlobals.UpdateWidgets();
        CGlobals.TweenMove(marketCard.gameObject, "position", Vector3.up * 305f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(marketCard.gameObject, Vector3.one * 0.75f, 0.4f, iTween.EaseType.easeOutSine, true);
		
		float fBufferTimeForCards = 0.15f * CUIManager.instance.SelectedCards.Count;
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        yield return new WaitForSeconds(1.2f - fBufferTimeForCards);
        uiMan.marketPanel.gridMarketCards.UpdateGrid();
		CActivePlayerPanel.instance.gridHand.UpdateGrid();
		yield return StartCoroutine(PositionBuyButtons());  

        int oldDepth = marketCard.Texture.depth;
        marketCard.ChangeWidgetDepth(200);

        yield return new WaitForSeconds(0.5f);
		StartCoroutine(CloseMarket_CR());
        
		marketCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;
		CGlobals.UpdateWidgets();
        activePlayer.discard.Add(marketCard);
        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(marketCard));

        foreach (CBaseCard removeFromHand in uiMan.SelectedCards)
            activePlayer.hand.Remove(removeFromHand);
    
        uiMan.SelectedCards.Clear();
        CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

        foreach (UISprite button in lstBuyButtons) {
            button.color = colorDisabledButton;
            button.GetComponent<UIButton>().enabled = false;
        }

        foreach(CBaseCard card in lstMarketCards)
            CGlobals.AssignNewUIButtonOnClickTarget(uiMan, card, card.Button, "MoveCardToActionPanel");
		foreach(CBaseCard card in activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

        marketCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + activePlayer.discard.Count + 1);

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    public void UpdateMarketCardButtonsByCost()
    {
        for(int i = 0; i < lstMarketCards.Count; ++i)
        {
            CInstantCard instant = null;
            CSnailCard snail = null;

            if (lstMarketCards[i].cardType == CardData.CardType.Instant) {
                instant = (CInstantCard)lstMarketCards[i];
                if (instant.cost == CUIManager.instance.SelectedCards.Count) {
                    lstBuyButtons[i].GetComponent<UIButton>().enabled = true;
                    lstBuyButtons[i].color = Color.white;
                }
                else {
                    lstBuyButtons[i].GetComponent<UIButton>().enabled = false;
                    lstBuyButtons[i].color = colorDisabledButton;
                }
            }
            else if (lstMarketCards[i].cardType == CardData.CardType.Snail) {
                snail = (CSnailCard)lstMarketCards[i];
                if (snail.cost == CUIManager.instance.SelectedCards.Count) {
                    lstBuyButtons[i].GetComponent<UIButton>().enabled = true;
                    lstBuyButtons[i].color = Color.white;
                }
                else {
                    lstBuyButtons[i].GetComponent<UIButton>().enabled = false;
                    lstBuyButtons[i].color = colorDisabledButton;
                }
            }
        }
    }

    public IEnumerator PositionBuyButtons()
    {
        int i = 0;
        for (i = 0; i < gridMarketCards.lstCurrentPositions.Count; ++i) {
            lstBuyButtons[i].gameObject.SetActive(true);
            CGlobals.TweenMove(lstBuyButtons[i].gameObject, "x", gridMarketCards.lstCurrentPositions[i].x, 0.5f, iTween.EaseType.easeOutSine, true);
        }

        for(i = i; i < CGlobals.MAX_MARKET_CARD_COUNT; ++i) {
            lstBuyButtons[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
    }
}
