using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CTutorialDirector : MonoBehaviour
{
    #region Singleton Instance
    private static CTutorialDirector _instance;
    public static CTutorialDirector instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CTutorialDirector>();
            }
            return _instance;
        }
    }
    #endregion

    // WFI = WAIT FOR INPUT

    public enum TutorialScene { TS_INTRO, TS_EXPLAINHAND, TS_PEEKATPEPTIDE };
    public enum TS_INTRO { SETUP_HAND, SHOW_WELCOME_TEXT, EXPLAIN_GOAL, ENDSCENE };
    public enum TS_EXPLAINHAND { EXPLAIN_HAND, WFI_CLICKHANDCARD, MOVE_TO_ACTION_PANEL, WFI_CLICKCLOSEBUTTON, ENDSCENE };
    public enum TS_PEEKATPEPTIDE { EXPLAIN_INSTANT_CARDS, WFI_CLICKRESEARCHCARD, EXPLAIN_RESEARCH_CARD,
                                    WFI_CLICKPEPTIDE, EXPLAIN_PEEKEDPEPTIDE, ENDTURN_HIGHLIGHTOTHERPLAYERS, ENDSCENE };



    public CArrowManager ArrowManager;
    public UIPanel uiPanel;
    public UISprite uiFadeSprite;
    public UILabel uiText;
    public UISprite uiTextBoxSprite;
    public UISprite uiArrowSprite;
    public UIButton uiContinueBttn;

    public GameObject goArrows;
    public List<CTutorialScene> lstScenes;

    public CTutorialScene PreviousTutorialScene;
    public CTutorialScene CurrentTutorialScene;



    IEnumerator Start()
    {
        CurrentTutorialScene = lstScenes[0];
        CurrentTutorialScene.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        CGlobals.iTweenValue(gameObject, 0, 1, 1f, gameObject, "UpdateUIPanelAlpha", iTween.EaseType.easeInSine);
        yield return new WaitForSeconds(1f);

        StartCoroutine(CurrentTutorialScene.StartScene());
    }

    public void UpdateUIPanelAlpha(float alpha) { uiPanel.alpha = alpha; }
    public void SetText(string txt) { uiText.text = txt; }

    public IEnumerator EnableButton()
    {
        yield return new WaitForEndOfFrame();

        uiContinueBttn.enabled = true;
        uiContinueBttn.GetComponent<BoxCollider>().enabled = true;
        CGlobals.TweenScale(uiContinueBttn.gameObject, Vector3.one, 0.4f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.4f);
    }

    public IEnumerator DisableButton()
    {
        yield return new WaitForEndOfFrame();
        if(uiContinueBttn.enabled)
        {
            CGlobals.TweenScale(uiContinueBttn.gameObject, Vector3.one*1.2f, 0.1f, iTween.EaseType.easeInSine, true);
            yield return new WaitForSeconds(0.1f);

            CGlobals.TweenScale(uiContinueBttn.gameObject, Vector3.zero, 0.25f, iTween.EaseType.easeInSine, true);
            yield return new WaitForSeconds(0.25f);

            uiContinueBttn.enabled = false;
        }
    }

    public IEnumerator ExpandTextbox()
    {
        yield return new WaitForEndOfFrame();
        CGlobals.TweenScale(uiTextBoxSprite.gameObject, Vector3.one, 0.4f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.4f);
    }

    public IEnumerator ShrinkTextbox()
    {
        CGlobals.TweenScale(uiTextBoxSprite.gameObject, Vector3.one*1.15f, 0.1f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.1f);

        CGlobals.TweenScale(uiTextBoxSprite.gameObject, Vector3.zero, 0.25f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator TransformTextbox(Vector3 v3Position, Vector3 v3Scale, float duration)
    {
        CGlobals.TweenMove(uiTextBoxSprite.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);
        CGlobals.TweenScale(uiTextBoxSprite.gameObject, v3Scale, duration, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(duration);
    }

    public void AdvanceScene()
    {
        StartCoroutine(CurrentTutorialScene.AdvanceScene());
    }

    public void StartNextScene()
    {
        switch(CurrentTutorialScene.SceneType)
        {
            case TutorialScene.TS_INTRO:
                CurrentTutorialScene.gameObject.SetActive(false);
                CurrentTutorialScene = lstScenes[1];
                CurrentTutorialScene.gameObject.SetActive(true);
                StartCoroutine(CurrentTutorialScene.StartScene());
                break;

            case TutorialScene.TS_EXPLAINHAND:
                CurrentTutorialScene.gameObject.SetActive(false);
                CurrentTutorialScene = lstScenes[2];
                CurrentTutorialScene.gameObject.SetActive(true);
                StartCoroutine(CurrentTutorialScene.StartScene());
                break;

            case TutorialScene.TS_PEEKATPEPTIDE:
                //CurrentTutorialScene.gameObject.SetActive(false);
                //CurrentTutorialScene = lstScenes[0];
                //CurrentTutorialScene.gameObject.SetActive(true);
                break;
        }
    }

    public IEnumerator BeginTutorial()
    {
        yield return new WaitForEndOfFrame();

        CGameManager.instance.CreatePlayers();
		CCardManager.instance.CreateDeck();
		CCardManager.instance.DealHandCardsToPlayers();
        CCardManager.instance.DealPreyCardsToPlayers();
        CCardManager.instance.DealCardsToMarket();
        CCabalManager.instance.GenerateCabals();

        yield return StartCoroutine(CUIManager.instance.UpdateResolution());

        yield return new WaitForSeconds(0.75f);
        StartCoroutine(CUIManager.instance.marketPanel.PositionBuyButtons());
        List<CBaseCard> marketCards = CUIManager.instance.marketPanel.lstMarketCards;
        List<UISprite> buyButtons = CUIManager.instance.marketPanel.lstBuyButtons;

        for (int i = 0; i < CUIManager.instance.marketPanel.lstMarketCards.Count; ++i)
            buyButtons[i].transform.localPosition = marketCards[i].transform.localPosition;

        yield return new WaitForEndOfFrame();
        CGlobals.AutoResizeTextureColliders(this.transform);
        //CUIManager.instance.CenterPlayerOneUI(activePlayer.SnailsPanel.width);

        yield return StartCoroutine(CGameManager.instance.SetNextActivePlayer(null, true));
        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        CActivePlayerPanel.instance.gridPrey.UpdateGrid();
    }

    


    // functions with type 'void' are needed for NGUI buttons to access them through the inspector.
    // NGUI Buttons will not be able to access IEnumerator type functions so we have a helper function that
    // just calls our Coroutine function
    //
    // Call 'IEnumerator' functions with StartCoroutine(MoveCardToCenter_CR(card, duration));
    public void MoveCardTo_Center(CBaseCard card) { StartCoroutine(MoveCardTo_Center_CR(card, 0.5f)); }

    // Coroutine functions act as a way to suspend & resume code execution for a particular action
    // and allows more control to contain a sequence of events
    public IEnumerator MoveCardTo_Center_CR(CBaseCard card, float duration = 0.5f)
    {
        // Always call below function at the start of every Coroutine function or else you may get some weird errors
        yield return new WaitForEndOfFrame();

        /*
            StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.ACTION))
            ^
            The above function calls the SetNewParentPanel() with return type IEnumerator
            You can also use "yield return StartCoroutine()". This will force the function to wait
            until the function has completed before executing any code after it.
        */

        // Sets card.parent to action panel. The action panel renders above all other game UI
        yield return StartCoroutine(card.SetNewParentPanel_Tutorial(CardData.ParentPanel.ACTION));

        // This updates NGUI's interal render hierarchy
        CGlobals.UpdateWidgets();

        // Since the card is parented to the action panel, let's Fade in the ActionPanel BG
        // Every Main Panel has their own FadeBG()
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(true, 0.75f));

        // Moves the card.gameObject to the center of the screen over 0.5f seconds
        float y = card.Texture.height * 0.5f;
        CGlobals.TweenMove(card.gameObject, "position", new Vector3(0f, y, 0f), duration, iTween.EaseType.easeInSine, true);
        CGlobals.TweenScale(card.gameObject, Vector3.one * 0.75f, duration, iTween.EaseType.easeInSine, true);

        // This pauses the function for 0.5 seconds. After it will continue executing the rest of the function
        yield return new WaitForSeconds(duration);

        // Exit function
    }



    // Below are some functions you can play with to begin moving the cards around.
    // If you want to move other objects around, follow the same



    public IEnumerator MoveCardTo_Position(CBaseCard card, Vector3 v3Position, float duration = 0.5f)
    {
        yield return StartCoroutine(card.SetNewParentPanel_Tutorial(CardData.ParentPanel.ACTION));
        CGlobals.UpdateWidgets();
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(true, 0.75f));
        CGlobals.TweenMove(card.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator MoveCardTo_Position(CBaseCard card, Vector3 v3Position, Vector3 v3Scale = default(Vector3), float duration = 0.5f)
    {
        yield return StartCoroutine(card.SetNewParentPanel_Tutorial(CardData.ParentPanel.ACTION));
        CGlobals.UpdateWidgets();
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(true, 0.75f));
        CGlobals.TweenMove(card.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);

        // Scale the card.gameObject to the center of the screen over 0.5f seconds
        if (v3Scale != default(Vector3))
            CGlobals.TweenScale(card.gameObject, v3Scale, duration, iTween.EaseType.easeInSine, true);

        yield return new WaitForSeconds(duration);
    }

 //   public void MoveCardToSnailsPanel(CBaseCard baseCard) { StartCoroutine(MoveCardToSnailsPanel_CR(baseCard)); }
	//public IEnumerator MoveCardToSnailsPanel_CR(CBaseCard baseCard)
 //   {
 //       // needed to allow UIPlaySound script on buttons to play sound before disabling them
 //       yield return new WaitForEndOfFrame();
 //       CUIManager uiMan = CUIManager.instance;

 //   	CSnailCard snailCard = (CSnailCard)baseCard;
 //   	snailCard.HasPotency = false;
 //   	snailCard.HasStarvation = false;
 //       //snailCard.SetButtonSound(acSelectCard_playerPanel);

 //   	foreach(CBaseCard selectedCard in SelectedCards)
	//		if(selectedCard != baseCard)
	//			StartCoroutine(selectedCard.SetNewParentPanel(CardData.ParentPanel.HAND));
    		
 //       SelectedCard = null;

 //       CPlayer activePlayer = CGameManager.instance.activePlayer;
	//	activePlayer.hand.Remove(baseCard);

 //       // Check to see if card was previously in this panel
 //       // if true, set sibling index to original value
 //       for(int i = 0; i < activePlayer.snails.Count; ++i)
	//		if(activePlayer.snails[i] == baseCard) {
	//			baseCard.transform.SetSiblingIndex(i);
 //               CAssessmentPanel.instance.lstLastPlayedCards.Clear();
 //               break;
 //           }

 //       if (!activePlayer.snails.Contains(baseCard))
 //   		activePlayer.snails.Add(baseCard);

 //       StartCoroutine(actionPanel.FadeBG(false));

 //       actionPanel.MoveHandPanelToActivePlayerPanel();

 //       yield return StartCoroutine(actionPanel.ExpandActionPanelBackground(false));
 //       yield return new WaitForEndOfFrame();

	//	baseCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL + 1);
	//	yield return StartCoroutine(baseCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));

 //       CUIManager.instance.activePlayerPanel.lblHandCount.text = activePlayer.hand.Count.ToString();
	//	CGlobals.TweenMove(baseCard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
	//	activePlayerPanel.gridSnails.UpdateGrid();
 //       activePlayerPanel.gridHand.UpdateGrid();

 //       // Reset prey card target function
 //       if(CGameManager.instance.CurrentPlayState == GameData.PlayState.SELECTING_PREY)
 //       {
 //           foreach(CBaseCard preyCard in CGameManager.instance.activePlayer.prey)
 //               CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "MoveCardToActionPanel");

 //           CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;
 //           yield return StartCoroutine(actionPanel.MovePreyPanelToActivePlayerPanel());
 //       }

 //       CGlobals.UpdateWidgets();
	//	baseCard.Button.enabled = true;
		
	//	yield return new WaitForSeconds(0.1f);
	//	if(baseCard.cardType == CardData.CardType.Snail)
	//		snailCard.UpdateWidgetDepths();

	//	yield return new WaitForSeconds(1.5f);
 //   }




    //public IEnumerator MoveCardTo_Position(GameObject arrow, Vector3 v3Position, float duration = 0.5f)
    //{
    //    yield return StartCoroutine(card.SetNewParentPanel_Tutorial(CardData.ParentPanel.ACTION));
    //    CGlobals.UpdateWidgets();
    //    StartCoroutine(CUIManager.instance.actionPanel.FadeBG(true, 0.75f));
    //    CGlobals.TweenMove(card.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);

    //    yield return new WaitForSeconds(duration);
    //}

    //public IEnumerator MoveCardTo_Position(GameObject arrow, Vector3 v3Position, Vector3 v3Scale = default(Vector3), float duration = 0.5f)
    //{
    //    //yield return StartCoroutine(card.SetNewParentPanel_Tutorial(CardData.ParentPanel.ACTION));
    //    //arrow.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
    //    //CGlobals.UpdateWidgets();

    //    StartCoroutine(CUIManager.instance.actionPanel.FadeBG(true, 0.75f));
    //    CGlobals.TweenMove(card.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);

    //    // Scale the card.gameObject to the center of the screen over 0.5f seconds
    //    if (v3Scale != default(Vector3))
    //        CGlobals.TweenScale(card.gameObject, v3Scale, duration, iTween.EaseType.easeInSine, true);

    //    yield return new WaitForSeconds(duration);
    //}


}


