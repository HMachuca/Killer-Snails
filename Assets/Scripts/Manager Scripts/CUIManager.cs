using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CUIManager : MonoBehaviour
{
    #region Singleton Instance
    private static CUIManager _instance;
    public static CUIManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CUIManager>();
            }
            return _instance;
        }
    }
    #endregion

	public static float REAL_SCREEN_WIDTH = 0;
	public static float REAL_SCREEN_HEIGHT = 0;

	public UIRoot uiRoot;
    public UICamera uiCamera;
    public CCabalPanel cabalPanel;
    public CActionPanel actionPanel;
    public CMarketPanel marketPanel;
    public CActivePlayerPanel activePlayerPanel;

    public List<CBaseCard> lstSelectedCards;
    public CBaseCard draggedCard;
    public CardData.ParentPanel draggedPanel;

    private Vector2 v2LastScreenWH = Vector2.zero;
    public float opponentPanelWidth;
	public float preyWidth;
    public float headerWidth;

    public struct tBoxCollider {
        public BoxCollider boxCollider;
        public bool enabled;
    }
    public List<tBoxCollider> lstBoxColliders;
	public bool isDragging = false;


    public CBaseCard SelectedCard {
        get {
            if (lstSelectedCards.Count > 0)
                return lstSelectedCards[0];
            else
                return null;
        }
        set {
            lstSelectedCards.Clear();
            if(value != null)
                lstSelectedCards.Add(value);
        }
    }

    public List<CBaseCard> SelectedCards {
        get { return lstSelectedCards; }
    }

    public CBaseCard DraggedCard { 
    	get { return draggedCard; } 
    	set { draggedCard = value; }}


    public AudioClip acSelectCard_actionPanel, acSelectCard1, acSelectCard_playerPanel;



	////////////////////////////////////////////////////////////////////////
	/// THIS FUNCTION GETS THE REAL SCREEN DIMENSIONS.
	/// USE REAL_SCREEN_WIDTH AND REAL_SCREEN_HEIGHT WHEN WORKING WITH THE SCREEN
	/// 
	////////////////////////////////////////////////////////////////////////
	//
	private void CalculateScreenDimensions()
	{
		UIRoot root = NGUITools.FindInParents<UIRoot>(this.gameObject);
		float ratio = (float)root.activeHeight / Screen.height;
		CUIManager.REAL_SCREEN_WIDTH = Mathf.Ceil(Screen.width * ratio);
		CUIManager.REAL_SCREEN_HEIGHT = Mathf.Ceil(Screen.height * ratio);
	}
	//
	////////////////////////////////////////////////////////////////////////


	void Awake() { CalculateScreenDimensions(); }


    void Start()
    {
        lstSelectedCards = new List<CBaseCard>();

        Debug.Log("W: " + REAL_SCREEN_WIDTH);        Debug.Log("H: " + REAL_SCREEN_HEIGHT);
        Debug.Log("Aspect Ratio: " + (REAL_SCREEN_WIDTH / REAL_SCREEN_HEIGHT));
    }

    public void ResetUI()
    {
        StartCoroutine(actionPanel.FadeBG(false, 0f));
        actionPanel.InstructionText.HideTopHeader();
        actionPanel.InstructionText.HideBotHeader();
    }


    #region Collider Management 
    public void EnableAllBoxCollidersForBeginTurn()
    {
        if (lstBoxColliders == null)
            return;

        foreach(tBoxCollider tBox in lstBoxColliders)
            if(tBox.boxCollider != null && tBox.boxCollider.gameObject.layer != LayerMask.NameToLayer("Tutorial"))
                tBox.boxCollider.enabled = tBox.enabled;

        // Clear list so it's ready to be populated for CGameManager.EndTurn()
        lstBoxColliders.Clear();
    }

    public void DisableAllCollidersForEndTurn()
    {
        if (lstBoxColliders == null)
            lstBoxColliders = new List<tBoxCollider>();

        BoxCollider[] colliders = GameObject.FindObjectsOfType<BoxCollider>();
        foreach (BoxCollider boxCollider in colliders)
        {
            if(boxCollider.gameObject.layer != LayerMask.NameToLayer("Tutorial"))
            {
                tBoxCollider tBox;
                tBox.boxCollider = boxCollider;
                tBox.enabled = boxCollider.enabled;
                lstBoxColliders.Add(tBox);

                boxCollider.enabled = false;
            }
        }
    }

    #endregion


    void Update()
    {
//#if UNITY_EDITOR
//        if (v2LastScreenWH.x != CGlobals.SCREEN_WIDTH || v2LastScreenWH.y != Screen.height)
//        {
//            v2LastScreenWH = new Vector2(CGlobals.SCREEN_WIDTH, Screen.height);
//            StartCoroutine(UpdateResolution());
//        }
//#else
//        if (v2LastScreenWH.x != Screen.width || v2LastScreenWH.y != Screen.height)
//        {
//            v2LastScreenWH = new Vector2(Screen.width, Screen.height);
//            UpdateResolution();
//        }
//#endif

        //if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //    Screen.SetResolution(1024, 768, false);
        //    UpdateResolution();
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //    Screen.SetResolution(1280, 768, false);
        //    UpdateResolution();
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //    Screen.SetResolution(1600, 1024, false);
        //    UpdateResolution();
        //}
        //else 
        //    if (Input.GetKeyDown(KeyCode.Alpha4)) {
        //    Screen.SetResolution(1920, 1080, false);
        //    UpdateResolution();
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //    Screen.SetResolution(1920, 1080, false);
        //    UpdateResolution();
        }

    }

    public IEnumerator UpdateResolution()
    {
		float fCabalWidth = 0.255f * CUIManager.REAL_SCREEN_WIDTH;
		float fLeftoverScreenWidth = CUIManager.REAL_SCREEN_WIDTH - fCabalWidth;

        opponentPanelWidth = fLeftoverScreenWidth / (CGameManager.instance.numPlayers - 1);
        preyWidth = 0.91f * Mathf.RoundToInt(opponentPanelWidth) + 1;
        headerWidth = 0.1f * opponentPanelWidth;
        
		int nPreyWidth = (int)preyWidth;
        int nHeaderWidth = (int)headerWidth;

		UITexture cabalBG = cabalPanel.anchorPanelBackground.GetComponent<UITexture>();
        cabalBG.rightAnchor.target = null;
        yield return new WaitForEndOfFrame();
       	
        cabalBG.rightAnchor.Set(CUIManager.instance.uiCamera.transform, 0f, fCabalWidth);
        cabalBG.ResetAndUpdateAnchors();

		COpponentsPanel oppPanel = COpponentsPanel.instance;
        if (CGameManager.instance.numPlayers == 2)
        {
            oppPanel.SetToFirstPosition(PlayerData.PLAYER.TWO);
            oppPanel.SetToSecondPosition(PlayerData.PLAYER.ONE);
        }
        else if (CGameManager.instance.numPlayers == 3)
        {
            oppPanel.SetToFirstPosition(PlayerData.PLAYER.TWO);
		    oppPanel.SetToSecondPosition(PlayerData.PLAYER.THREE);
            oppPanel.SetToThirdPosition(PlayerData.PLAYER.ONE);
        }
        else if(CGameManager.instance.numPlayers == 4)
        {
            oppPanel.SetToFirstPosition(PlayerData.PLAYER.TWO);
		    oppPanel.SetToSecondPosition(PlayerData.PLAYER.THREE);
		    oppPanel.SetToThirdPosition(PlayerData.PLAYER.FOUR);
		    oppPanel.SetToFourthPosition(PlayerData.PLAYER.ONE);
        }

        // Update grid dimensions for opponent panel's prey based on width of bg texture
        foreach(CPlayer player in CGameManager.instance.Players)
        {
            int newGridDimensionWidth = nPreyWidth;
            int cellMaxDimensionWidth = (int)(nPreyWidth * 0.425f);

            player.gridPrey.mv2GridDimensions.x = newGridDimensionWidth;
            player.gridPrey.mv2CellMaxDimensions.x = cellMaxDimensionWidth;
            player.gridPrey.UpdateGrid(0.1f);
        }
    }

    public IEnumerator AddCardToMarket(CBaseCard card)
    {
        yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.MARKET));
        yield return new WaitForEndOfFrame();
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = Vector3.one * 0.375f;
    }

    public void EnablePreyButtons(bool enable = true)
    {
        foreach (Transform child in CGameManager.instance.activePlayer.gridPrey.transform)
            child.GetComponent<UIButton>().enabled = enable;
    }

	public IEnumerator DiscardSnailCardInstantly(CPlayer playerToRemove, CSnailCard cardToDiscard)
	{
		yield return new WaitForEndOfFrame();

		playerToRemove.snails.Remove(cardToDiscard);
		cardToDiscard.EnableButton(false);
		cardToDiscard.transform.parent = playerToRemove.goDiscard.transform;
		cardToDiscard.transform.localPosition = Vector3.zero;
		cardToDiscard.transform.localEulerAngles = Vector3.zero;
		cardToDiscard.transform.localScale = Vector3.one * 0.1f;
		cardToDiscard.opponentSnail.transform.parent = CGameManager.instance.transform;
		cardToDiscard.opponentSnail.transform.localPosition = Vector3.up * 1500f;

		yield return new WaitForSeconds(0.15f);
		cardToDiscard.TextureBack.enabled = true;
		cardToDiscard.Texture.enabled = false;

		CGlobals.UpdateWidgets();
	}


#region Card Movement

    public IEnumerator EnableActionBackground(bool enable, CBaseCard card)
    {
        if (enable)
        {
            Vector3 v3NewPos = new Vector3(-240f, card.Texture.height * 0.45f, 0f);
            CGlobals.TweenMove(card.gameObject, "position", v3NewPos, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(card.gameObject, Vector3.one * 0.87f, 0.5f, iTween.EaseType.easeOutSine, true);

            yield return new WaitForSeconds(0.5f);
			actionPanel.anchorPanelBackground.gameObject.SetActive(true);

            yield return StartCoroutine(actionPanel.ExpandActionPanelBackground());
        }
        else
        {
            Vector3 v3NewPos = new Vector3(0f, card.Texture.height * 0.33f, 0f);
            CGlobals.TweenMove(card.gameObject, "position", v3NewPos, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(card.gameObject, Vector3.one * 0.67f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void MoveToCenterOfActionPanel(CBaseCard card) { StartCoroutine(MoveToCenterOfActionPanel_CR(card)); }
    private IEnumerator MoveToCenterOfActionPanel_CR(CBaseCard card)
    {
        Vector3 v3NewPos = new Vector3(0f, SelectedCard.Texture.height * 0.33f, 0f);
        CGlobals.TweenMove(SelectedCard.gameObject, "position", v3NewPos, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(SelectedCard.gameObject, Vector3.one * 0.67f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    public void MoveCardToActionPanel(CBaseCard card) { StartCoroutine(MoveCardToActionPanel_CR(card)); }
    public IEnumerator MoveCardToActionPanel_CR(CBaseCard card)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

    	CGameManager gameMan = CGameManager.instance;
        CAssessmentPanel.instance.lstLastPlayedCards.Add(card);
        SelectedCard = card;
        SelectedCard.ChangeWidgetDepth(100);
        SelectedCard.Button.enabled = false;
        SelectedCard.SetButtonSound(acSelectCard_actionPanel);

        marketPanel.gridMarketCards.UpdateGrid();
		actionPanel.anchorPanelBackground.gameObject.SetActive(true);
		actionPanel.anchorPanelBackground.transform.localPosition = Vector3.zero;
        StartCoroutine(actionPanel.FadeBG());
        
        yield return new WaitForEndOfFrame();
        actionPanel.EnableButton(SelectedCard);
        StartCoroutine(SelectedCard.SetNewParentPanel(CardData.ParentPanel.ACTION));

        CGlobals.UpdateWidgets();
        actionPanel.SetFlavorText(SelectedCard.GetFlavorText());

        // Used to set widget depth for all sprites on card gameobject
        if(SelectedCard.cardType == CardData.CardType.Snail)
            ((CSnailCard)SelectedCard).UpdateWidgetDepths();

        if(SelectedCard.cardType == CardData.CardType.Snail)
        {
            CSnailCard snailCard = (CSnailCard)SelectedCard;
            bool canAct = (snailCard.fedState != CardData.FedState.Fed) ? true : false;
			yield return StartCoroutine(EnableActionBackground(canAct, SelectedCard));
        }
        else if(SelectedCard.cardType == CardData.CardType.Prey)
        {
            actionPanel.EnableButton(SelectedCard);
            yield return StartCoroutine(EnableActionBackground(true, SelectedCard));
        }
        else if(SelectedCard.cardType == CardData.CardType.Instant)
        {
            CInstantCard instantCard = (CInstantCard)SelectedCard;
            yield return StartCoroutine(EnableActionBackground(true, SelectedCard));

            //default:
            //    Vector3 v3NewPos = new Vector3(0f, SelectedCard.Texture.height * 0.33f, 0f);
            //    CGlobals.TweenMove(SelectedCard.gameObject, "position", v3NewPos, 0.5f, iTween.EaseType.easeOutSine, true);
            //    CGlobals.TweenScale(SelectedCard.gameObject, Vector3.one * 0.67f, 0.5f, iTween.EaseType.easeOutSine, true);
            //    yield return new WaitForSeconds(0.5f);
            //    break;
        }
        
        card.Button.enabled = true;
    }

    public void MoveCardToMarketPanel(CBaseCard card) { StartCoroutine(MoveCardToMarketPanel_CR(card)); }
    public IEnumerator MoveCardToMarketPanel_CR(CBaseCard card)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(actionPanel.ExpandActionPanelBackground(false));
        card.SetButtonSound(acSelectCard_playerPanel);
    	SelectedCard = null;
        StartCoroutine(actionPanel.FadeBG(false));

        if(IsCardInMarketOrPreyPanel(card))
        {
			actionPanel.anchorPanelBackground.gameObject.SetActive(false);
            yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.MARKET));
            for(int i = 0; i < marketPanel.lstMarketCards.Count; ++i)
            if(marketPanel.lstMarketCards[i] == card) {
                card.transform.SetSiblingIndex(i);
                break;
            }
            CGlobals.UpdateWidgets();

            yield return new WaitForEndOfFrame();
            CGlobals.TweenMove(card.gameObject, "y", 0f, marketPanel.gridMarketCards.MoveTime, iTween.EaseType.easeOutSine, true);
        }
        else
        {
            Vector3 v3NewPos = new Vector3(card.transform.localPosition.x, 0f, 0f);
			CGlobals.TweenMove(actionPanel.anchorPanelBackground.gameObject, "position", v3NewPos, 0.25f, iTween.EaseType.easeOutSine, true);

            yield return new WaitForSeconds(0.25f);
			CGlobals.TweenScale(actionPanel.anchorPanelBackground.gameObject, Vector3.zero, 0.01f, iTween.EaseType.easeOutSine, true);

            yield return new WaitForSeconds(0.02f);
			actionPanel.anchorPanelBackground.gameObject.SetActive(false);
            yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.MARKET));
            CGlobals.UpdateWidgets();

            yield return new WaitForEndOfFrame();
            CGlobals.TweenMove(card.gameObject, "y", 0f, marketPanel.gridMarketCards.MoveTime, iTween.EaseType.easeOutSine, true);
        }

        marketPanel.gridMarketCards.UpdateGrid();
        marketPanel.UpdateListOrder();

        yield return new WaitForSeconds(0.35f);
        card.Button.enabled = true;
    }

    public void MoveCardToHandPanel(CBaseCard card) { StartCoroutine(MoveCardToHandPanel_CR(card)); }
    public IEnumerator MoveCardToHandPanel_CR(CBaseCard card)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();
        
        card.SetButtonSound(acSelectCard_playerPanel);
        card.Button.enabled = false;

        StartCoroutine(actionPanel.FadeBG(false, 0.75f));
        yield return StartCoroutine(actionPanel.ExpandActionPanelBackground(false));

        if(CGlobals.TUTORIAL_ACTIVE) {
            yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.HAND));
        }
        else {
            foreach(CBaseCard handCard in SelectedCards)
			    yield return StartCoroutine(handCard.SetNewParentPanel(CardData.ParentPanel.HAND));
        }

		SelectedCard = null;

        // Check to see if card was previously in this panel
        // if true, set sibling index to original value
        //if(CGameManager.instance.activePlayer.hand.Count == 1) {
            for(int i = 0; i < CGameManager.instance.activePlayer.hand.Count; ++i) {
                if(CGameManager.instance.activePlayer.hand[i] == card) {
                    //CAssessmentPanel.instance.lstLastPlayedCards.Clear();
                    card.transform.SetSiblingIndex(i);
                    break;
                }
            }
        //}

		// Reset prey card target function
//        if(CGameManager.instance.CurrentPlayState == GameData.PlayState.SELECTING_PREY)
//        {
//            foreach(CBaseCard preyCard in CGameManager.instance.activePlayer.prey)
//                CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "MoveCardToActionPanel");
//
//            CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;
//			actionPanel.MoveHandPanelToActivePlayerPanel();
//            yield return StartCoroutine(actionPanel.MovePreyPanelToActivePlayerPanel());
//        }

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.gridPrey.UpdateGrid();
        
		actionPanel.anchorPanelBackground.gameObject.SetActive(false);
		activePlayerPanel.lblHandCount.text = CGameManager.instance.activePlayer.hand.Count.ToString();
        card.Button.enabled = true;

        yield return new WaitForSeconds(0.25f);
        CGlobals.UpdateWidgets();
    }

	public void MoveCardToSnailsPanel(CBaseCard baseCard) { StartCoroutine(MoveCardToSnailsPanel_CR(baseCard)); }
	public IEnumerator MoveCardToSnailsPanel_CR(CBaseCard baseCard)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

    	CSnailCard snailCard = (CSnailCard)baseCard;
    	snailCard.HasPotency = false;
    	snailCard.HasStarvation = false;
        snailCard.SetButtonSound(acSelectCard_playerPanel);

    	foreach(CBaseCard selectedCard in SelectedCards)
			if(selectedCard != baseCard)
				StartCoroutine(selectedCard.SetNewParentPanel(CardData.ParentPanel.HAND));
    		
        SelectedCard = null;

        CPlayer activePlayer = CGameManager.instance.activePlayer;
		activePlayer.hand.Remove(baseCard);

        // Check to see if card was previously in this panel
        // if true, set sibling index to original value
        for(int i = 0; i < activePlayer.snails.Count; ++i)
			if(activePlayer.snails[i] == baseCard) {
				baseCard.transform.SetSiblingIndex(i);
                //CAssessmentPanel.instance.lstLastPlayedCards.Clear();
                break;
            }

        if (!activePlayer.snails.Contains(baseCard))
    		activePlayer.snails.Add(baseCard);

        StartCoroutine(actionPanel.FadeBG(false));
        actionPanel.MoveHandPanelToActivePlayerPanel();
        StartCoroutine(actionPanel.MovePreyPanelToActivePlayerPanel());

        actionPanel.InstructionText.HideTopHeader();

        yield return StartCoroutine(actionPanel.ExpandActionPanelBackground(false));
        yield return new WaitForEndOfFrame();

		baseCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL + 1);
		yield return StartCoroutine(baseCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
        actionPanel.ResetActionContainer();

        activePlayerPanel.lblHandCount.text = activePlayer.hand.Count.ToString();
		CGlobals.TweenMove(baseCard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
		activePlayerPanel.gridSnails.UpdateGrid();
        activePlayerPanel.gridHand.UpdateGrid();

        // Reset prey card target function
        if(CGameManager.instance.CurrentPlayState == GameData.PlayState.SELECTING_PREY)
        {
            foreach(CBaseCard preyCard in CGameManager.instance.activePlayer.prey)
                CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "MoveCardToActionPanel");

            CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;

            //// Instruction text
            //actionPanel.InstructionText.ShowTopHeader();
            //yield return StartCoroutine(actionPanel.MovePreyPanelToActivePlayerPanel());

            //actionPanel.InstructionText.SetTopInstructionText("Select your prey");
            //actionPanel.lblInstructionText.gradientBottom = Color.white;
        }



        CGlobals.UpdateWidgets();
		baseCard.Button.enabled = true;
		
		yield return new WaitForSeconds(0.1f);
		if(baseCard.cardType == CardData.CardType.Snail)
			snailCard.UpdateWidgetDepths();

		yield return new WaitForSeconds(1.5f);
    }

    public void MoveCardToPreyPanel(CBaseCard card) { StartCoroutine(MoveCardToPreyPanel_CR(card)); }
    public IEnumerator MoveCardToPreyPanel_CR(CBaseCard card)
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

        card.SetButtonSound(acSelectCard_playerPanel);
    	yield return StartCoroutine(actionPanel.ExpandActionPanelBackground(false));
        SelectedCard = null;
        CPlayer activePlayer = CGameManager.instance.activePlayer;
        yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.PREY));


        for (int i = 0; i < CGameManager.instance.activePlayer.prey.Count; ++i)
            if(CGameManager.instance.activePlayer.prey[i] == card) {
                card.transform.SetSiblingIndex(i);
                //CAssessmentPanel.instance.lstLastPlayedCards.Clear();
                break;
            }

        card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 2);
        CGlobals.UpdateWidgets();
        StartCoroutine(actionPanel.FadeBG(false, 0.75f));

        activePlayerPanel.gridPrey.UpdateGrid();
        CGlobals.UpdateWidgets();
        //card.Button.enabled = true;

        if (card.cardType == CardData.CardType.Snail)
            ((CSnailCard)card).UpdateWidgetDepths();

    }

#endregion

    public void new_MoveCardToActionPanel(CBaseCard baseCard) { StartCoroutine(new_MoveCardToActionPanel_CR(baseCard)); }
    public IEnumerator new_MoveCardToActionPanel_CR(CBaseCard baseCard)
    {
        yield return new WaitForEndOfFrame();
        SelectedCard = baseCard;

        CGameManager gameMan = CGameManager.instance;
        //gameMan.audiosource.PlayOneShot(gameMan.acSelectCard);

        SelectedCard.Button.enabled = false;
        marketPanel.gridMarketCards.UpdateGrid();
		actionPanel.anchorPanelBackground.gameObject.SetActive(true);
		actionPanel.anchorPanelBackground.transform.localPosition = Vector3.zero;
        StartCoroutine(actionPanel.FadeBG());
        
        yield return new WaitForEndOfFrame();
        actionPanel.EnableButton(SelectedCard);
        StartCoroutine(SelectedCard.SetNewParentPanel(CardData.ParentPanel.ACTION));

        CGlobals.UpdateWidgets();
        actionPanel.SetFlavorText(SelectedCard.GetFlavorText());

        // Used to set widget depth for all sprites on card gameobject
        if(SelectedCard.cardType == CardData.CardType.Snail)
            ((CSnailCard)SelectedCard).UpdateWidgetDepths();
    }


    private bool IsCardInMarketOrPreyPanel(CBaseCard card)
    {
        foreach (CBaseCard marketCard in marketPanel.lstMarketCards)
            if (card == marketCard)
                return true;

        foreach (CBaseCard preyCard in CGameManager.instance.activePlayer.prey)
            if (card == preyCard)
                return true;

        return false;
    }

    // Used when putting card back into original panel
    public void ReinsertCardIntoPanel(CBaseCard card)
    {
        Grid grid = null;
        List<CBaseCard> cards = null;
        

        if (card.CurrentParentPanel == CardData.ParentPanel.HAND) {
            grid = activePlayerPanel.gridHand;
            cards = CGameManager.instance.activePlayer.hand;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.SNAIL) {
            grid = activePlayerPanel.gridSnails;
            cards = CGameManager.instance.activePlayer.snails;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.PREY) {
            grid = activePlayerPanel.gridPrey;
            cards = CGameManager.instance.activePlayer.prey;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.MARKET) {
            grid = marketPanel.gridMarketCards;
            cards = marketPanel.lstMarketCards;
        }

        int targetIndex = (grid.MidpointPositions.Count == 0) ? 0 : -1;
        for (int i = 0; i < grid.MidpointPositions.Count; ++i)
        {
            float posX = card.transform.localPosition.x;
            float midpointX = grid.MidpointPositions[i].x;
            float nextMidpointX = (i >= grid.MidpointPositions.Count - 1) ? -1f : grid.MidpointPositions[i + 1].x;

            if(i == 0 && posX <= midpointX)
            {
                targetIndex = i;
                break;
            }
            else if(i == grid.MidpointPositions.Count-1 && posX > midpointX)
            {
                targetIndex = i+1;
                break;
            }
            else if(posX > midpointX && posX <= nextMidpointX)
            {
                targetIndex = i+1;
                break;
            }
        }

        List<CBaseCard> newOrder = new List<CBaseCard>(cards.Count);
        int oldIndex = -1;

        for(int i = 0; i < cards.Count; ++i)
            if(card == cards[i]) {
                oldIndex = i;
                break;
            }

        if(oldIndex < targetIndex) {
            for (int i = 0; i < targetIndex; ++i) {
                if(i < oldIndex)
                {
                    cards[i].transform.SetSiblingIndex(i);
                    newOrder.Add(cards[i]);
                }
                else
                {
                    cards[i + 1].transform.SetSiblingIndex(i);
                    newOrder.Add(cards[i + 1]);
                }
            }

            card.transform.SetSiblingIndex(targetIndex);
            newOrder.Add(card);

            for (int i = targetIndex + 1; i < cards.Count; ++i)
            {
                cards[i].transform.SetSiblingIndex(i);
                newOrder.Add(cards[i]);
            }
        }
        else {
            for (int i = cards.Count-1; i > targetIndex; --i) {
                if(i > oldIndex)
                {
                    cards[i].transform.SetSiblingIndex(i);
                    newOrder.Add(cards[i]);
                }
                else
                {
                    cards[i - 1].transform.SetSiblingIndex(i);
                    newOrder.Add(cards[i - 1]);
                }
            }

            card.transform.SetSiblingIndex(targetIndex);
            newOrder.Add(card);

            for (int i = targetIndex-1; i >= 0; --i)
            {
                cards[i].transform.SetSiblingIndex(i);
                newOrder.Add(cards[i]);
            }

            newOrder.Reverse();
        }

        if (card.CurrentParentPanel == CardData.ParentPanel.HAND) {
            CGameManager.instance.activePlayer.hand = newOrder;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.SNAIL) {
            CGameManager.instance.activePlayer.snails = newOrder;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.PREY) {
            CGameManager.instance.activePlayer.prey = newOrder;
        }
        else if (card.CurrentParentPanel == CardData.ParentPanel.MARKET) {
            marketPanel.lstMarketCards = newOrder;
        }
        
        grid.UpdateGrid();
    }
}






