using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CActionPanel : CBasePanel
{
    public enum BUTTON { NONE = -1, FEED, USE, BUY, UNHIBERNATE };

    public UILabel lblFlavorText;
    public CInstructionText InstructionText;
    public Transform trInstructionBG;
    public UILabel lblInstructionText;
    public GameObject goActionContainer;
    public CActionEndPrompt ActionEndPrompt;
    //public GameObject goMainBGSprite;
    //public GameObject goCabalExtractionBGSprite;
    public GameObject goPreySwapBGSprite;
    public GameObject goHandSwapBGSprite;
    public UIButton[] bttnHandSwap;
    public CPeptideExtractionPanel PeptideExtractionPanel;
    public CPeptidesForPublishing PeptidesForPublishing;
    public CPeptidesForPresentation PeptidesForPresentation;
    

    public Grid gridPreySwap;
    //public UIButton bttnAlpha, bttnDelta, bttnMu, bttnOmega, bttnRandom;
    public UIButton bttnClose;
	public GameObject goFeedButton, goUseButton, goBuyButton, goUnhibernateButton;
    public UISprite TapToContinuePrompt;

    // Ocean waves
    public CPreyCard preyCardToSwap;
    public CBaseCard handCardToSwap;

    public bool canPlayerFeed = false;
    public AudioClip acAddPeptide;
    public AudioClip acSwapCard;
    

    protected override void Start()
    {
        base.Start();
    }

    public void SetFlavorText(string text)
    {
        lblFlavorText.text = text;
    }

    public void EnableButton(CBaseCard card)
    {
        CGameManager gameMan = CGameManager.instance;
        CSnailCard snailCard = null;
        CInstantCard instantCard = null;
        CPreyCard preyCard = null;

        switch(card.cardType) {
            case CardData.CardType.Snail:   snailCard = (CSnailCard)card;       break;
            case CardData.CardType.Instant: instantCard = (CInstantCard)card;   break;
            case CardData.CardType.Prey:    preyCard = (CPreyCard)card;         break;
        }

        switch(card.CurrentParentPanel)
        {
            case CardData.ParentPanel.HAND:
            {
                if(snailCard != null)
                {
                    EnableButton(BUTTON.USE);
                }
                else if(instantCard != null)
                {
                    if (instantCard.playStateToBeActive == GameData.PlayState.IDLE)
                    {
                        switch(instantCard.instantType)
                        {
                            case CardData.InstantType.Stingray: {
                                int snails = 0;
						        foreach(CPlayer players in gameMan.Players)
                                    if(players != gameMan.activePlayer)
    							        foreach(CBaseCard snail in players.snails)
		                    		        snails++;

                    	        bool enableButton = (snails > 0) ? true : false;
                                if (enableButton)
                                    EnableButton(BUTTON.USE);
                                else
                                    EnableButton(BUTTON.NONE);
                            }
                            break;

                            case CardData.InstantType.Lobster:{
                                int snails = 0;
						        foreach(CPlayer players in gameMan.Players)
							        if(players != gameMan.activePlayer)
    							        foreach(CBaseCard snail in players.snails)
		                    		        snails++;

                    	        bool enableButton = (snails > 0) ? true : false;
                                if (enableButton)
                                    EnableButton(BUTTON.USE);
                                else
                                    EnableButton(BUTTON.NONE);
                            }
                            break;

                            case CardData.InstantType.Publishing: {
                                if (gameMan.activePlayer.snails.Count > 0)
                                    EnableButton(BUTTON.USE);
                                else
                                    EnableButton(BUTTON.NONE);
                            }
                            break;

                            case CardData.InstantType.Presentation: {
                                if (gameMan.activePlayer.snails.Count > 0)
                                    EnableButton(BUTTON.USE);
                                else
                                    EnableButton(BUTTON.NONE);
                            }
                            break;

                            default:
                                EnableButton(BUTTON.USE);
                                break;
                        }
                    }
                    else if (instantCard.playStateToBeActive == GameData.PlayState.SELECTING_PREY)
                        EnableButton(BUTTON.NONE);
                }
            }
            break;

            case CardData.ParentPanel.SNAIL:
            {
                if (snailCard.fedState == CardData.FedState.Hibernating)
                    EnableButton(BUTTON.UNHIBERNATE);
                else if (snailCard.fedState == CardData.FedState.Unfed)
                    EnableButton(BUTTON.FEED);
                else
                    EnableButton(BUTTON.NONE);
            }
            break;

            case CardData.ParentPanel.PREY:
                EnableButton(BUTTON.NONE);
            break;

            case CardData.ParentPanel.MARKET:
                EnableButton(BUTTON.NONE);
            break;

            case CardData.ParentPanel.DISCARD:
            break;

            case CardData.ParentPanel.ACTION:
            break;
        }
    }

    public IEnumerator FadeContinuePrompt(bool fadeIn = true)
    {
        if (CAssessmentPanel.instance.lstLastPlayedCards.Count > 0)
            yield return StartCoroutine(CAssessmentPanel.instance.ShowPanel(fadeIn));
        else
            yield return StartCoroutine(CGameManager.instance.BeginNextTurn_CR());

        //StartCoroutine(FadeBG(fadeIn, 0.5f));
        //TapToContinuePrompt.enabled = true;
        //  	Color fadeColor = (fadeIn) ? Color.white : Color.clear;
        //  	CGlobals.iTweenValue(this.gameObject, TapToContinuePrompt.color, fadeColor, 0.5f, this.gameObject, "UpdateContinuePrompt", iTween.EaseType.easeOutSine);

        //  	yield return new WaitForSeconds(0.6f);
        //  	TapToContinuePrompt.enabled = fadeIn;
        //TapToContinuePrompt.GetComponent<BoxCollider>().enabled = fadeIn;
        //  	TapToContinuePrompt.GetComponent<UIButton>().enabled = fadeIn;
    }
    //private void UpdateContinuePrompt(Color color) { TapToContinuePrompt.color = color; }

    public void UseCard() { StartCoroutine(UseCard_CR()); }
    public IEnumerator UseCard_CR()
    {
        // needed to allow UIPlaySound script on buttons to play sound before disabling them
        yield return new WaitForEndOfFrame();

    	CUIManager uiManager = CUIManager.instance;
        CGameManager gameManager = CGameManager.instance;
        COpponentsPanel opponentPanel = COpponentsPanel.instance;

		CBaseCard selectedCard = uiManager.SelectedCard;
		selectedCard.EnableButton(false);

		switch (selectedCard.cardType)
        {
            case CardData.CardType.Snail:
                CLogfile.instance.Append("Play Snail Card: " + ((CSnailCard)selectedCard).snailSpecies);

                uiManager.DisableAllCollidersForEndTurn();
				opponentPanel.AddSnailToOpponent(selectedCard);
				uiManager.SelectedCard = null;
				yield return StartCoroutine(uiManager.MoveCardToSnailsPanel_CR(selectedCard));
				yield return StartCoroutine(gameManager.EndTurn());
                break;

            case CardData.CardType.Instant:
            {
				CInstantCard instantCard = ((CInstantCard)selectedCard);
                switch(instantCard.instantType)
		        {
		        case CardData.InstantType.Lobster:
                    CLogfile.instance.Append("Play Lobster Card");

                    CUIManager.instance.actionPanel.InstructionText.ShowTopHeader("Lower the fed state of an opponent's snail");

					opponentPanel.EnableSnailButtons();
					yield return StartCoroutine(ExpandActionPanelBackground(false));
					CGlobals.TweenMove(selectedCard.gameObject, "position", new Vector3(-485f, -6f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
					CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
					foreach(CPlayer player in gameManager.Players)
						player.SetSnailCardButtonTargetForFedState();

					opponentPanel.anchorPanelBackground.transform.parent = this.transform;

                    // Disable Opponent Prey cards
                    foreach(CPlayer player in CGameManager.instance.Players)
                        if(player.player != CGameManager.instance.activePlayer.player)
                            player.gridPrey.gameObject.SetActive(false);
                    
					CGlobals.UpdateWidgets();
		        break;

		        case CardData.InstantType.Stingray:
                    CLogfile.instance.Append("Play Stingray Card");

                    CUIManager.instance.actionPanel.InstructionText.ShowTopHeader("Lower the fed state of an opponent's snail");

		        	opponentPanel.EnableSnailButtons();
					yield return StartCoroutine(ExpandActionPanelBackground(false));
					CGlobals.TweenMove(selectedCard.gameObject, "position", new Vector3(-485f, -6f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
					CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
					foreach(CPlayer player in gameManager.Players)
						player.SetSnailCardButtonTargetForFedState();

					opponentPanel.anchorPanelBackground.transform.parent = this.transform;

                    // Disable Opponent Prey cards
                    foreach(CPlayer player in CGameManager.instance.Players)
                        if(player.player != CGameManager.instance.activePlayer.player)
                            player.gridPrey.gameObject.SetActive(false);

					CGlobals.UpdateWidgets();
		        break;

		        case CardData.InstantType.Meeting: {
                    CLogfile.instance.Append("Play Meeting Card");
                    // Look at 1 opponent's hand. Swap 1 card with a card in your hand.

                    yield return StartCoroutine(ExpandActionPanelBackground(false));
                    CGlobals.TweenMove(selectedCard.gameObject, "position", new Vector3(-485f, -500f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
					CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);

                    // Change Button Colors
                    ChangeHandSwapButtonColors();

                    foreach (CPlayer player in CGameManager.instance.Players) {
                        player.gridHand.mv2GridDimensions.x = 0f;
                        player.gridHand.mv2CellMaxDimensions.x = 0f;
                        player.gridHand.mnBorder = 0;
                        player.gridHand.UpdateGrid();
                        player.gridHand.transform.parent = goHandSwapBGSprite.transform;
                        player.gridHand.transform.localPosition = new Vector3(0f, -113f, 0f);
                    }

                    CGlobals.UpdateWidgets();

			        #if UNITY_IOS
					    CGlobals.TweenMove(goHandSwapBGSprite, "y", 500f, 0.5f, iTween.EaseType.easeOutSine, true);
			        #else
					    CGlobals.TweenMove(goHandSwapBGSprite, "y", 404f, 0.5f, iTween.EaseType.easeOutSine, true);
			        #endif
                }
		        break;

		        case CardData.InstantType.Ocean_Waves: {
                    CLogfile.instance.Append("Play Ocean Waves Card");

                    Debug.Log("Ocean_Waves");
			        // Swap prey with any prey on table or from top of deck
					yield return StartCoroutine(ExpandActionPanelBackground(false));
                    
                    InstructionText.ShowBotHeader("Choose a prey card to swap");
                    //InstructionText.ShowBotHeader("Swap prey cards with an opponent or choose one from the top of the Prey deck");
					CGlobals.TweenMove(selectedCard.gameObject, "position", new Vector3(-575f, 250f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
					CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.5f, 0.5f, iTween.EaseType.easeOutSine, true);
					CGlobals.UpdateWidgets();

                    bool moveBasicPreyCard = false;
					yield return StartCoroutine(MovePreyPanelToActionPanel(moveBasicPreyCard));
                    
					foreach(CPreyCard preyCard in gameManager.activePlayer.prey) {
						if(preyCard.preyName == CardData.PreyName.Basic_Prey)
							CGlobals.AssignNewUIButtonOnClickTarget(null, null, preyCard.Button, "");
						else
							CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "ShowOpponentsPrey");
					}
				}
		        break;

		        case CardData.InstantType.Presentation:
                    CLogfile.instance.Append("Play Presentation Card");

                    // Each player chooses a peptide. reveal 1 peptide from any cabal. if peptides match, add peptides to snail in pool
                    yield return StartCoroutine(RunPresentationCard());

		        break;

		        case CardData.InstantType.Publishing:
                    CLogfile.instance.Append("Play Publishing Card");

                    // Choose a peptide then reveal 1 peptide from any cabal. If match, add to snail in pool
                    CGlobals.AssignNewUIButtonOnClickTarget(PeptidesForPublishing, selectedCard, selectedCard.Button, "MoveCardToActionPanel");
                    yield return StartCoroutine(ExpandActionPanelBackground(false));

                    // Move Card
                    CGlobals.TweenMove(uiManager.SelectedCard.gameObject, "position", new Vector3(-500f, -150f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
                    CGlobals.TweenScale(uiManager.SelectedCard.gameObject, Vector3.one * 0.267f, 0.5f, iTween.EaseType.easeOutSine, true);

                    // Show "Peptides for Publishing" Panel
                    yield return StartCoroutine(PeptidesForPublishing.ShowPanel());
                    uiManager.SelectedCard.EnableButton();
		        break;

		        case CardData.InstantType.Research:
                    CLogfile.instance.Append("Play Research Card");

                    // Peek at a peptide in any cabal
                    yield return StartCoroutine(ExpandActionPanelBackground(false));
                    CCabalManager.instance.SetButtonFunctionTargetForResearchCard();

                    InstructionText.ShowTopHeader("Peek at any peptide in the left-hand panel");

					CGlobals.TweenMove(selectedCard.gameObject, "position", new Vector3(180f, 320f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
                    CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.8f, 0.5f, iTween.EaseType.easeOutSine, true);

                    yield return new WaitForSeconds(0.25f);
					uiManager.cabalPanel.anchorPanelBackground.transform.parent = uiManager.actionPanel.goActionContainer.transform;
                    CGlobals.UpdateWidgets();

                    yield return new WaitForSeconds(0.25f);
                    CCabalManager.instance.EnableButtons();

		        break;

		        case CardData.InstantType.Tsunami:
                    CLogfile.instance.Append("Play Tsunami Card");

                    CUIManager uiMan = CUIManager.instance;
                    CGameManager gameMan = CGameManager.instance;
                    CCardManager cardMan = CCardManager.instance;
                    yield return StartCoroutine(ExpandActionPanelBackground(false));

                    CGlobals.TweenMove(selectedCard.gameObject, "x", -480f, 0.5f, iTween.EaseType.easeOutSine, true);
                    CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * .25f, 0.5f, iTween.EaseType.easeOutSine, true);

                    List<CPreyCard> preyActivePlayer = new List<CPreyCard>();
                    List<CPreyCard>[] preyOpponent = new List<CPreyCard>[gameMan.numPlayers];

                    for (int i = 0; i < gameMan.numPlayers; ++i)
                        preyOpponent[i] = new List<CPreyCard>();

                    // Move cards out of screen view
                    int nOpp = 0;
                    foreach(CPlayer player in gameMan.Players)
                    {
                        if(player == gameMan.activePlayer) {
                            foreach(CPreyCard preyCard in player.prey) {
                                if(preyCard.preyName != CardData.PreyName.Basic_Prey) {
                                    CGlobals.TweenMove(preyCard.gameObject, "x", 1000f, 0.5f, iTween.EaseType.easeOutSine, true);
                                    preyActivePlayer.Add(preyCard);
                                }
                            }
                        }
                        else {
                            foreach (CPreyCard preyCard in player.prey) {
                                if (preyCard.preyName != CardData.PreyName.Basic_Prey) {
                                    CGlobals.TweenMove(preyCard.gameObject, "y", 200f, 0.5f, iTween.EaseType.easeOutSine, true);
                                    preyOpponent[nOpp].Add(preyCard);
                                }
                            }

                            nOpp++;
                        }
                    }

                    yield return new WaitForSeconds(1f);

                    // Replace old prey cards with new ones from the deck
                    nOpp = 0;
                    foreach (CPlayer player in gameMan.Players)
                    {
                        if (player == gameMan.activePlayer)
                        {
                            Transform oldParent = null;
                            Vector3[] oldPositions = new Vector3[2];

                            //foreach (CPreyCard preyCard in preyActivePlayer)
                            for(int i = 0; i < preyActivePlayer.Count; ++i) {
                                oldParent = preyActivePlayer[i].transform.parent;
                                oldPositions[i] = preyActivePlayer[i].transform.localPosition;
                                preyActivePlayer[i].transform.parent = cardMan.goPreyDrawPile.transform;
                                preyActivePlayer[i].transform.localPosition = Vector3.zero;
                                
                                player.prey.Remove(preyActivePlayer[i]);
                            }

                            // add 2 cards to player prey, remove them from deck
                            CBaseCard prey1 = cardMan.lstDrawPile_Prey[0];
                            CBaseCard prey2 = cardMan.lstDrawPile_Prey[1];
                            
                            prey1.transform.parent = oldParent;
                            prey2.transform.parent = oldParent;
                            prey1.transform.localPosition = oldPositions[0];
                            prey2.transform.localPosition = oldPositions[1];
                            prey1.transform.localEulerAngles = Vector3.zero;
                            prey2.transform.localEulerAngles = Vector3.zero;
                            prey1.Texture.enabled = true;
                            prey2.Texture.enabled = true;
                            prey1.TextureBack.enabled = false;
                            prey2.TextureBack.enabled = false;
                            prey1.ChangeWidgetDepth(52);
                            prey2.ChangeWidgetDepth(52);
                            //prey1.EnableButton();
                            //prey2.EnableButton();

                            player.prey.Add(prey1);
                            player.prey.Add(prey2);
                            cardMan.lstDrawPile_Prey.Remove(prey1);
                            cardMan.lstDrawPile_Prey.Remove(prey2);

                            CGlobals.UpdateWidgets();
                            CActivePlayerPanel.instance.gridPrey.UpdateGrid();
                        }
                        else
                        {
                            Transform oldParent = null;
                            Vector3[] oldPositions = new Vector3[2];

                            for(int i = 0; i < preyOpponent[nOpp].Count; ++i) {
                                oldParent = preyOpponent[nOpp][i].transform.parent;
                                oldPositions[i] = preyOpponent[nOpp][i].transform.localPosition;
                                preyOpponent[nOpp][i].transform.parent = cardMan.goPreyDrawPile.transform;
                                preyOpponent[nOpp][i].transform.localPosition = Vector3.zero;
                                
                                player.prey.Remove(preyOpponent[nOpp][i]);
                            }

                            // add 2 cards to player prey, remove them from deck
                            CBaseCard prey1 = cardMan.lstDrawPile_Prey[0];
                            CBaseCard prey2 = cardMan.lstDrawPile_Prey[1];
                            
                            prey1.transform.parent = oldParent;
                            prey2.transform.parent = oldParent;
                            prey1.transform.localPosition = Vector3.zero;
                            prey2.transform.localPosition = Vector3.zero;
                            prey1.transform.localScale = Vector3.zero;
                            prey2.transform.localScale = Vector3.zero;
                            prey1.transform.localEulerAngles = Vector3.zero;
                            prey2.transform.localEulerAngles = Vector3.zero;
                            prey1.Texture.enabled = true;
                            prey2.Texture.enabled = true;
                            prey1.TextureBack.enabled = false;
                            prey2.TextureBack.enabled = false;
                            prey1.ChangeWidgetDepth(41);
                            prey2.ChangeWidgetDepth(41);
                            //prey1.EnableButton();
                            //prey2.EnableButton();

                            player.prey.Add(prey1);
                            player.prey.Add(prey2);
                            cardMan.lstDrawPile_Prey.Remove(prey1);
                            cardMan.lstDrawPile_Prey.Remove(prey2);

                            CGlobals.UpdateWidgets();
                            yield return new WaitForEndOfFrame();

                            CActivePlayerPanel.instance.gridPrey.UpdateGrid();
                            player.gridPrey.UpdateGrid();

                            nOpp++;
                        }
                    }

                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(uiMan.SelectedCard));
                    yield return StartCoroutine(CGameManager.instance.EndTurn());
                    break;

		        case CardData.InstantType.Turtle:
                    CLogfile.instance.Append("Play Turtle Card");

			        // Hibernate all unfed snails and kill asll hibernated
					yield return StartCoroutine(ExpandActionPanelBackground(false));
                    yield return StartCoroutine(RunTurtleCard());
		            break;
		        }
                break;
            }
        }

        yield break;
    }

    #region Hand Swap

    public void SwapHandCard(CBaseCard cardToAcquire) { StartCoroutine(SwapHandCard_CR(cardToAcquire)); }
    public IEnumerator SwapHandCard_CR(CBaseCard cardToAcquire)
    {
        if (handCardToSwap == null)
            yield break;

        CLogfile.instance.Append("Swap Hand Card");

        yield return new WaitForEndOfFrame();

        CGameManager gameMan = CGameManager.instance;
        int acquiredOldIndex = cardToAcquire.transform.GetSiblingIndex();
		int swappedOldIndex = handCardToSwap.transform.GetSiblingIndex();
		Transform acquiredOldParent = cardToAcquire.transform.parent;
		Transform swappedOldParent = handCardToSwap.transform.parent;
		Vector3 v3AcquiredOldPos = cardToAcquire.transform.localPosition;
		Vector3 v3SwappedOldPos = handCardToSwap.transform.localPosition;
		Vector3 v3AcquiredOldScale = cardToAcquire.transform.localScale;
		Vector3 v3SwappedOldScale = handCardToSwap.transform.localScale;

        // Reset owners, hand lists
		handCardToSwap.owner = cardToAcquire.owner;
		cardToAcquire.owner = gameMan.activePlayer.player;

        CPlayer targetPlayer = gameMan.Players[(int)handCardToSwap.owner];
		targetPlayer.hand.Add(handCardToSwap);
		targetPlayer.hand.Remove(cardToAcquire);

		gameMan.activePlayer.hand.Remove(handCardToSwap);
		gameMan.activePlayer.hand.Add(cardToAcquire);

        foreach (CBaseCard handCard in targetPlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, handCard, handCard.Button, "MoveCardToActionPanel");

        foreach (CBaseCard handCard in gameMan.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, handCard, handCard.Button, "MoveCardToActionPanel");

        // Set newCard's parent to oldCard's parent. Tween to position
		cardToAcquire.transform.parent = swappedOldParent;
		cardToAcquire.transform.SetSiblingIndex(swappedOldIndex);
		CGlobals.TweenMove(cardToAcquire.gameObject, "position", v3SwappedOldPos, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(cardToAcquire.gameObject, v3SwappedOldScale, 0.5f, iTween.EaseType.easeOutSine, true);

        // Set oldCard's parent to newCard's parent. Tween to position
		handCardToSwap.transform.parent = acquiredOldParent;
		handCardToSwap.transform.SetSiblingIndex(acquiredOldIndex);
		CGlobals.TweenMove(handCardToSwap.gameObject, "position", v3AcquiredOldPos, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(handCardToSwap.gameObject, v3AcquiredOldScale, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.45f);
        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.uiPanelHand.transform.parent = CActivePlayerPanel.instance.panelHand.transform;
        CGlobals.UpdateWidgets();        

        yield return new WaitForSeconds(0.1f);
        CGlobals.TweenMove(goHandSwapBGSprite, "y", 1000f, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(CUIManager.instance.SelectedCard.gameObject, "position", new Vector3(0f, 300f, 0f), 0.75f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(1.25f);

        //StartCoroutine(FadeBG(false));


        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(CUIManager.instance.SelectedCard));
        yield return StartCoroutine(CGameManager.instance.EndTurn());


    }

    public void ShrinkButtonsExpandCards_P1() { StartCoroutine(ShrinkButtonsExpandCards_P1_CR()); }
    public IEnumerator ShrinkButtonsExpandCards_P1_CR()
    {
        CPlayer player = CGameManager.instance.Players[0];
        foreach (UIButton bttn in bttnHandSwap) {
            CGlobals.TweenMove(bttn.gameObject, "position", bttnHandSwap[1].transform.localPosition, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(bttn.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        }
        yield return new WaitForSeconds(1f);
        player.gridHand.mv2GridDimensions.x = 1250;
        player.gridHand.mv2CellMaxDimensions.x = 1000;
        player.gridHand.mnBorder = 20;
        player.gridHand.UpdateGrid();

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goHandSwapBGSprite.transform;
        CGlobals.UpdateWidgets();

        foreach (CBaseCard handCard in player.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(this, handCard, handCard.Button, "SwapHandCard");

        foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(handCard, null, handCard.Button, "SelectedCardToSwap");
    }

    public void ShrinkButtonsExpandCards_P2() { StartCoroutine(ShrinkButtonsExpandCards_P2_CR()); }
    public IEnumerator ShrinkButtonsExpandCards_P2_CR()
    {
        CPlayer player = CGameManager.instance.Players[1];
        foreach (UIButton bttn in bttnHandSwap) {
            CGlobals.TweenMove(bttn.gameObject, "position", bttnHandSwap[1].transform.localPosition, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(bttn.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        }
        yield return new WaitForSeconds(0.5f);
        player.gridHand.mv2GridDimensions.x = 1250;
        player.gridHand.mv2CellMaxDimensions.x = 1000;
        player.gridHand.mnBorder = 20;
        player.gridHand.UpdateGrid();

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goHandSwapBGSprite.transform;
        CGlobals.UpdateWidgets();

        foreach (CBaseCard handCard in player.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(this, handCard, handCard.Button, "SwapHandCard");

        foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(handCard, null, handCard.Button, "SelectedCardToSwap");
    }

    public void ShrinkButtonsExpandCards_P3() { StartCoroutine(ShrinkButtonsExpandCards_P3_CR()); }
    public IEnumerator ShrinkButtonsExpandCards_P3_CR()
    {
        CPlayer player = CGameManager.instance.Players[2];
        foreach (UIButton bttn in bttnHandSwap) {
            CGlobals.TweenMove(bttn.gameObject, "position", bttnHandSwap[1].transform.localPosition, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(bttn.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        }
        yield return new WaitForSeconds(0.5f);
        player.gridHand.mv2GridDimensions.x = 1250;
        player.gridHand.mv2CellMaxDimensions.x = 1000;
        player.gridHand.mnBorder = 20;
        player.gridHand.UpdateGrid();

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goHandSwapBGSprite.transform;
        CGlobals.UpdateWidgets();

        foreach (CBaseCard handCard in player.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(this, handCard, handCard.Button, "SwapHandCard");
        
        foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(handCard, null, handCard.Button, "SelectedCardToSwap");
    }

    public void ShrinkButtonsExpandCards_P4() { StartCoroutine(ShrinkButtonsExpandCards_P4_CR()); }
    public IEnumerator ShrinkButtonsExpandCards_P4_CR()
    {
        CPlayer player = CGameManager.instance.Players[3];
        foreach (UIButton bttn in bttnHandSwap) {
            CGlobals.TweenMove(bttn.gameObject, "position", bttnHandSwap[1].transform.localPosition, 0.5f, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(bttn.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        }
        yield return new WaitForSeconds(0.5f);
        player.gridHand.mv2GridDimensions.x = 1250;
        player.gridHand.mv2CellMaxDimensions.x = 1000;
        player.gridHand.mnBorder = 20;
        player.gridHand.UpdateGrid();

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goHandSwapBGSprite.transform;
        CGlobals.UpdateWidgets();

        foreach (CBaseCard handCard in player.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(this, handCard, handCard.Button, "SwapHandCard");

        foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(handCard, null, handCard.Button, "SelectedCardToSwap");
    }

    private void ChangeHandSwapButtonColors()
    {
        Color p1 = CGlobals.Player1Color;
        Color p2 = CGlobals.Player2Color;
        Color p3 = CGlobals.Player3Color;
        Color p4 = CGlobals.Player4Color;
        int nButton = 0;

        foreach(CPlayer player in CGameManager.instance.Players)
            if(player != CGameManager.instance.activePlayer)
            {
                UIButton button = bttnHandSwap[nButton];
                switch(player.player)
                {
                    case PlayerData.PLAYER.ONE:
                    button.defaultColor = CGlobals.Player1Color;
                    button.hover = new Color(p1.r-(p1.r * 0.15f), p1.g-(p1.g * 0.15f), p1.b-(p1.b * 0.15f));
                    button.pressed = new Color(p1.r-(p1.r * 0.25f), p1.g-(p1.g * 0.25f), p1.b-(p1.b * 0.25f));
                    button.GetComponentInChildren<UILabel>().text = "Player One";
                    button.GetComponentInChildren<UILabel>().color = CGlobals.Player1Color;
                    CGlobals.AssignNewUIButtonOnClickTarget(this, null, button, "ShrinkButtonsExpandCards_P1");
                    break;

                    case PlayerData.PLAYER.TWO:
                    button.defaultColor = CGlobals.Player2Color;
                    button.hover = new Color(p2.r-(p2.r * 0.15f), p2.g-(p2.g * 0.15f), p2.b-(p2.b * 0.15f));
                    button.pressed = new Color(p2.r-(p2.r * 0.25f), p2.g-(p2.g * 0.25f), p2.b-(p2.b * 0.25f));
                    button.GetComponentInChildren<UILabel>().text = "Player Two";
                    button.GetComponentInChildren<UILabel>().color = CGlobals.Player2Color;
                    CGlobals.AssignNewUIButtonOnClickTarget(this, null, button, "ShrinkButtonsExpandCards_P2");
                    break;

                    case PlayerData.PLAYER.THREE:
                    bttnHandSwap[nButton].defaultColor = CGlobals.Player3Color;
                    bttnHandSwap[nButton].hover = new Color(p3.r-(p3.r * 0.15f), p3.g-(p3.g * 0.15f), p3.b-(p3.b * 0.15f));
                    bttnHandSwap[nButton].pressed = new Color(p3.r-(p3.r * 0.25f), p3.g-(p3.g * 0.25f), p3.b-(p3.b * 0.25f));
                    bttnHandSwap[nButton].GetComponentInChildren<UILabel>().text = "Player Three";
                    bttnHandSwap[nButton].GetComponentInChildren<UILabel>().color = CGlobals.Player3Color;
                    CGlobals.AssignNewUIButtonOnClickTarget(this, null, button, "ShrinkButtonsExpandCards_P3");
                    break;

                    case PlayerData.PLAYER.FOUR:
                    bttnHandSwap[nButton].defaultColor = CGlobals.Player4Color;
                    bttnHandSwap[nButton].hover = new Color(p4.r-(p4.r * 0.15f), p4.g-(p4.g * 0.15f), p4.b-(p4.b * 0.15f));
                    bttnHandSwap[nButton].pressed = new Color(p4.r-(p4.r * 0.25f), p4.g-(p4.g * 0.25f), p4.b-(p4.b * 0.25f));
                    bttnHandSwap[nButton].GetComponentInChildren<UILabel>().text = "Player Four";
                    bttnHandSwap[nButton].GetComponentInChildren<UILabel>().color = CGlobals.Player4Color;
                    CGlobals.AssignNewUIButtonOnClickTarget(this, null, button, "ShrinkButtonsExpandCards_P4");
                    break;
                }
                nButton++;
            }
    }

    #endregion

    public IEnumerator RunTurtleCard()
    {
        CLogfile.instance.Append("RunTurtleCard");

        yield return new WaitForEndOfFrame();
        CGameManager gameManager = CGameManager.instance;
        CUIManager uiManager = CUIManager.instance;

        uiManager.DisableAllCollidersForEndTurn();

        List<List<CSnailCard>> playerSnailsToHibernate = new List<List<CSnailCard>>(gameManager.Players.Count);
        List<List<CSnailCard>> playerSnailsToKill = new List<List<CSnailCard>>(gameManager.Players.Count);

        for (int i = 0; i < gameManager.Players.Count; ++i)
            playerSnailsToHibernate.Add(new List<CSnailCard>());
        for (int i = 0; i < gameManager.Players.Count; ++i)
            playerSnailsToKill.Add(new List<CSnailCard>());

        List<CSnailCard> activeSnailsToHibernate = new List<CSnailCard>();
        List<CSnailCard> activeSnailsToKill = new List<CSnailCard>();

        int nPlayer = 0;
        int nActivePlayer = 0;
        foreach(CPlayer player in gameManager.Players)
		{
            foreach(CBaseCard baseCard in player.snails)
            {
                if(player == gameManager.activePlayer)
                {
                    nActivePlayer = nPlayer;
                    CSnailCard snailCard = (CSnailCard)baseCard;
                    if (snailCard.fedState == CardData.FedState.Unfed) {
                        playerSnailsToHibernate[nPlayer].Add(snailCard);
                        activeSnailsToHibernate.Add(snailCard);
                    }
                    else if (snailCard.fedState == CardData.FedState.Hibernating) {
                        playerSnailsToKill[nPlayer].Add(snailCard);
                        activeSnailsToKill.Add(snailCard);
                    }
                }
                else {
                    CSnailCard snailCard = (CSnailCard)baseCard;
                    if (snailCard.fedState == CardData.FedState.Unfed)
                        playerSnailsToHibernate[nPlayer].Add(snailCard);
                    else if (snailCard.fedState == CardData.FedState.Hibernating) {
                        playerSnailsToKill[nPlayer].Add(snailCard);
                        
                    }
                }
            }

            nPlayer++;
        }

        StartCoroutine(FadeBG(false, 0.5f));
        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        float duration = 0f;
        for(int i = 0; i < playerSnailsToHibernate.Count; ++i)
            for(int j = 0; j < playerSnailsToHibernate[i].Count; ++j)
            {
                CSnailCard snailCard = playerSnailsToHibernate[i][j];
                if (snailCard != null)
                {
                    if(i != nActivePlayer) {
                        duration += 0.25f;
                        StartCoroutine(snailCard.opponentSnail.LowerFedState_CR());
                    }
                    else {
                        duration += 0.25f;
                        StartCoroutine(snailCard.SetFedState(CardData.FedState.Hibernating));
                    }
                }
            }

        for(int i = 0; i < playerSnailsToKill.Count; ++i)
            for(int j = 0; j < playerSnailsToKill[i].Count; ++j)
            {
                CSnailCard snailCard = playerSnailsToKill[i][j];
                if (snailCard != null)
                {
                    Debug.Log(snailCard.name);
                    if(i != nActivePlayer) {
                        duration += 0.5f;
                        yield return snailCard.SetNewParentPanel(CardData.ParentPanel.HAND);
                        yield return StartCoroutine(snailCard.KillSnail_CR(0f, 0.5f));
                    }
                    else {
                        duration += 0.5f;
                        yield return snailCard.SetNewParentPanel(CardData.ParentPanel.HAND);
                        yield return StartCoroutine(snailCard.KillSnail_CR(0.5f, 0f));
                        
                    }
                }
            }

        //yield return new WaitForSeconds(duration);

        CGlobals.UpdateWidgets();
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        yield return StartCoroutine(gameManager.EndTurn());
    }

    public IEnumerator RunPresentationCard()
    {
        CLogfile.instance.Append("RunPresentationCard");

        yield return new WaitForEndOfFrame();
        CGameManager gameManager = CGameManager.instance;
        CUIManager uiManager = CUIManager.instance;

        yield return StartCoroutine(ExpandActionPanelBackground(false));

        // Move Card
        CGlobals.TweenMove(uiManager.SelectedCard.gameObject, "position", new Vector3(-500f, -150f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(uiManager.SelectedCard.gameObject, Vector3.one * 0.267f, 0.5f, iTween.EaseType.easeOutSine, true);

        PeptidesForPresentation.currentPlayer = gameManager.activePlayer.player;
        PeptidesForPresentation.PopulateListOfPlayersWithSnails();
        yield return StartCoroutine(PeptidesForPresentation.ShowPanel());
    }

	public void ShowOpponentsPrey(CPreyCard cardToSwap) { StartCoroutine(ShowOpponentsPrey_CR(cardToSwap)); }
	public IEnumerator ShowOpponentsPrey_CR(CPreyCard cardToSwap)
   	{
        Debug.Log("1");

   		CGameManager gameMan = CGameManager.instance;
   		CUIManager uiMan = CUIManager.instance;
        CLogfile.instance.Append("ShowOpponentsPreyCards");

   		preyCardToSwap = cardToSwap;
		PlayerData.PLAYER currentPlayer = gameMan.activePlayer.player;
		List<CPreyCard> opponentPreyCards = new List<CPreyCard>();

		// Get list of prey cards
		foreach(COpponent opponent in COpponentsPanel.instance.lstOpponents) {
			CPlayer player = opponent.GetComponent<CPlayer>();
			if(player.player != currentPlayer) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey) {
						CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "SwapCard");
						preyCard.EnableButton(true);
						opponentPreyCards.Add(preyCard);
					}
			}
		}

        Debug.Log("2");
		// Disable unselected cards
		foreach(CPreyCard preyCard in gameMan.activePlayer.prey)
			if(preyCard != preyCardToSwap)
				preyCard.gameObject.SetActive(false);



		// Update grid
		
		CActivePlayerPanel.instance.gridPrey.UpdateGrid();

		// Move gridPrey
		//CGlobals.TweenMove(uiMan.activePlayerPanel.gridPrey.gameObject,
							//"position", new Vector3(-38f, 20f, 0), 0.5f, iTween.EaseType.easeOutSine, true);
		
		//foreach(CPreyCard card in opponentPreyCards)
			//CGlobals.TweenMove(card.gameObject, "y", 200f, 0.5f, iTween.EaseType.easeOutSine, true);
		//foreach(CPlayer player in gameMan.Players)
			//CGlobals.TweenMove(player.gridPrey.gameObject, "y", 400f, 0.5f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.5f);

		foreach(CPreyCard preyCard in opponentPreyCards)
			preyCard.transform.parent = gridPreySwap.transform;

        Debug.Log("3");
		
		gridPreySwap.UpdateGrid(0.1f);

		yield return new WaitForSeconds(0.15f);
        CGlobals.UpdateWidgets();

        InstructionText.ShowBotHeader("Swap prey cards with an opponent or choose one from the top of the Prey deck");

        // Transform Selected Card
        CGlobals.TweenScale(CUIManager.instance.SelectedCard.gameObject, Vector3.one * 0.5f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(CUIManager.instance.SelectedCard.gameObject, "position", new Vector3(-635f, 330f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(preyCardToSwap.gameObject, "position", new Vector3(-250f, -158f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);

		//#if UNITY_IOS
		CGlobals.TweenMove(goPreySwapBGSprite, "y", 480f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#else
		//CGlobals.TweenMove(goPreySwapBGSprite, "y", 390f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#endif

        Debug.Log("4");
		CCardManager.instance.goPreyDrawPile.transform.parent = uiMan.actionPanel.transform;
		CCardManager.instance.goPreyDrawPile.transform.localPosition = new Vector3(1000f, 40f, 0f);
		CGlobals.UpdateWidgets();

        yield return new WaitForSeconds(0.5f);

        // Move cards to positions
        float x1 = 347f, x2 = 547f;
        float time = 0.25f;
        float waitTime = 0.15f;
        if(gameMan.numPlayers == 2)
        {
            CGlobals.TweenMove(opponentPreyCards[0].gameObject, "position", new Vector3(x1, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[0].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[0].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[1].EnableButton();
            yield return new WaitForSeconds(waitTime);
        }
        else if(gameMan.numPlayers == 3)
        {
            CGlobals.TweenMove(opponentPreyCards[0].gameObject, "position", new Vector3(x1, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[0].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[0].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[1].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[2].gameObject, "position", new Vector3(x1, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[2].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[2].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[3].gameObject, "position", new Vector3(x2, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[3].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[3].EnableButton();
            yield return new WaitForSeconds(waitTime);
        }
        else if(gameMan.numPlayers == 4)
        {
            CGlobals.TweenMove(opponentPreyCards[0].gameObject, "position", new Vector3(x1, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[0].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[0].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[1].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[2].gameObject, "position", new Vector3(x1, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[2].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[2].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[3].gameObject, "position", new Vector3(x2, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[3].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[3].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[4].gameObject, "position", new Vector3(x1, -475f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[4].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[4].EnableButton();
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[5].gameObject, "position", new Vector3(x2, -475f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[5].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            opponentPreyCards[5].EnableButton();
            yield return new WaitForSeconds(waitTime);
        }

        Debug.Log("6");
        
        // Get list of prey cards
        TweenAllOpponentPreyOutlineColor();

        // move preyCardToSwap up on screen and change outline color
        CGlobals.TweenMove(preyCardToSwap.gameObject, "position", new Vector3(-207, 113f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(preyCardToSwap.gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
        StartCoroutine(preyCardToSwap.TweenOutlineOverlayColor(CGameManager.instance.activePlayer.player));
        yield return new WaitForSeconds(0.75f);

		CCardManager.instance.EnableTopCardInPreyDeck();
        CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "position", new Vector3(-100f, 40f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    public void TweenAllOpponentPreyOutlineColor(bool clear = false)
    {
        // Get list of prey cards
        foreach (COpponent opponent in COpponentsPanel.instance.lstOpponents) {
            CPlayer player = opponent.GetComponent<CPlayer>();
            if (player.player != CGameManager.instance.activePlayer.player) {
                foreach (CPreyCard preyCard in player.prey)
                    if (preyCard.preyName != CardData.PreyName.Basic_Prey)
                        StartCoroutine(preyCard.TweenOutlineOverlayColor(player.player, clear));
            }
        }
    }

   	public void SwapCard(CPreyCard preyCardToAcquire) { StartCoroutine(SwapCard_CR(preyCardToAcquire)); }
   	public IEnumerator SwapCard_CR(CPreyCard preyCardToAcquire)
   	{
        yield return new WaitForEndOfFrame();
        CLogfile.instance.Append("Swap Prey");

        CAudioManager.instance.PlaySound(acSwapCard);
   		CGameManager gameMan = CGameManager.instance;
		List<CPreyCard> opponentPreyCards = new List<CPreyCard>();

        CUIManager.instance.DisableAllCollidersForEndTurn();
        preyCardToSwap.ChangeWidgetDepth(41);

		int acquiredOldIndex = preyCardToAcquire.transform.GetSiblingIndex();
		int swappedOldIndex = preyCardToSwap.transform.GetSiblingIndex();
		Transform acquiredOldParent = preyCardToAcquire.transform.parent;
		Transform swappedOldParent = preyCardToSwap.transform.parent;
		Vector3 v3AcquiredOldPos = preyCardToAcquire.transform.localPosition;
		Vector3 v3SwappedOldPos = preyCardToSwap.transform.localPosition;
		Vector3 v3AcquiredOldScale = preyCardToAcquire.transform.localScale;
		Vector3 v3SwappedOldScale = preyCardToSwap.transform.localScale;
        
        // Reset owners, prey lists
		preyCardToSwap.owner = preyCardToAcquire.owner;
		preyCardToAcquire.owner = gameMan.activePlayer.player;
		gameMan.Players[(int)preyCardToSwap.owner].prey.Add(preyCardToSwap);
		gameMan.Players[(int)preyCardToSwap.owner].prey.Remove(preyCardToAcquire);
		gameMan.activePlayer.prey.Remove(preyCardToSwap);
		gameMan.activePlayer.prey.Add(preyCardToAcquire);

        // Set newCard's parent to oldCard's parent. Tween to position
		preyCardToAcquire.transform.parent = swappedOldParent;
		preyCardToAcquire.transform.SetSiblingIndex(swappedOldIndex);
		CGlobals.TweenMove(preyCardToAcquire.gameObject, "position", v3SwappedOldPos, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(preyCardToAcquire.gameObject, v3SwappedOldScale, 0.5f, iTween.EaseType.easeOutSine, true);

        // Set oldCard's parent to newCard's parent. Tween to position
		preyCardToSwap.transform.parent = acquiredOldParent;
		preyCardToSwap.transform.SetSiblingIndex(acquiredOldIndex);
		CGlobals.TweenMove(preyCardToSwap.gameObject, "position", v3AcquiredOldPos, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(preyCardToSwap.gameObject, v3AcquiredOldScale, 0.5f, iTween.EaseType.easeOutSine, true);

        // Tween Outline color of card
        TweenAllOpponentPreyOutlineColor();
        StartCoroutine(preyCardToAcquire.TweenOutlineOverlayColor(gameMan.activePlayer.player));

		yield return new WaitForSeconds(0.75f);
        TweenAllOpponentPreyOutlineColor(true);
        StartCoroutine(preyCardToAcquire.TweenOutlineOverlayColor(gameMan.activePlayer.player, true));

		//#if UNITY_IOS
		CGlobals.TweenMove(goPreySwapBGSprite, "y", 1400f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#else
		//CGlobals.TweenMove(goPreySwapBGSprite, "y", 1200f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#endif

		//CGlobals.TweenMove(goPreySwapBGSprite, "y", 1200f, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "x", 1000f, 0.5f, iTween.EaseType.easeOutSine, true);

		yield return new WaitForSeconds(0.25f);
        InstructionText.HideBotHeader();

		// Put prey back into opponent grids
		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey)
						opponentPreyCards.Add(preyCard);
			}
		}

		foreach(CPreyCard preyCard in opponentPreyCards) {
			CGlobals.TweenScale(preyCard.gameObject, Vector3.one * 0.13f, 0.1f, iTween.EaseType.easeOutSine, true);
			preyCard.transform.parent = gameMan.Players[(int)preyCard.owner].gridPrey.transform;
		}
		yield return new WaitForSeconds(0.1f);
        CGlobals.UpdateWidgets();

		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				player.EnableBasicPreyCard(false);
				yield return new WaitForEndOfFrame();
				player.gridPrey.UpdateGrid();
			}

			foreach(CPreyCard preyCard in player.prey)
				CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, preyCard, preyCard.Button, "MoveCardToActionPanel");
		}

		//foreach(CPlayer player in gameMan.Players)
			//CGlobals.TweenMove(player.gridPrey.gameObject, "y", 67f, 0.5f, iTween.EaseType.easeOutSine, true);

		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

		preyCardToSwap.ChangeWidgetDepth(CGlobals.OPPONENT_PREY_BG_DEPTH + 1);

        StartCoroutine(preyCardToAcquire.SetNewParentPanel(CardData.ParentPanel.PREY));
        CGlobals.UpdateWidgets();

		StartCoroutine(FadeBG(false));

		// Enable unselected cards
		foreach(CPreyCard preyCard in CGameManager.instance.activePlayer.prey)
			if(preyCard != preyCardToSwap)
				preyCard.gameObject.SetActive(true);

		yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());



		yield return new WaitForSeconds(0.9f);
		yield return StartCoroutine(CGameManager.instance.EndTurn());
   	}

    public void SwapCardAfterEating(CPreyCard preyCardToAcquire) { StartCoroutine(SwapCardAfterEating_CR(preyCardToAcquire)); }
   	public IEnumerator SwapCardAfterEating_CR(CPreyCard preyCardToAcquire)
   	{
        yield return new WaitForEndOfFrame();
        CLogfile.instance.Append("Swap Prey after feeding");

        CAudioManager.instance.PlaySound(acSwapCard);
   		CGameManager gameMan = CGameManager.instance;
		List<CPreyCard> opponentPreyCards = new List<CPreyCard>();

        CUIManager.instance.DisableAllCollidersForEndTurn();

		int acquiredOldIndex = preyCardToAcquire.transform.GetSiblingIndex();
		Transform acquiredOldParent = preyCardToAcquire.transform.parent;
		Vector3 v3AcquiredOldPos = preyCardToAcquire.transform.localPosition;
		Vector3 v3AcquiredOldScale = preyCardToAcquire.transform.localScale;
        
        // Reset owners, prey lists
		preyCardToAcquire.owner = gameMan.activePlayer.player;
		gameMan.activePlayer.prey.Add(preyCardToAcquire);

        // Set newCard's parent to oldCard's parent. Tween to position
        preyCardToAcquire.transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
		preyCardToAcquire.transform.SetSiblingIndex(2);
		//CGlobals.TweenMove(preyCardToAcquire.gameObject, "position", v3SwappedOldPos, 0.5f, iTween.EaseType.easeOutSine, true);
		//CGlobals.TweenScale(preyCardToAcquire.gameObject, v3SwappedOldScale, 0.5f, iTween.EaseType.easeOutSine, true);

        // Tween Outline color of card
        TweenAllOpponentPreyOutlineColor();
        StartCoroutine(preyCardToAcquire.TweenOutlineOverlayColor(gameMan.activePlayer.player));

		yield return new WaitForSeconds(0.75f);
        TweenAllOpponentPreyOutlineColor(true);
        StartCoroutine(preyCardToAcquire.TweenOutlineOverlayColor(gameMan.activePlayer.player, true));

		CGlobals.TweenMove(goPreySwapBGSprite, "y", 1400f, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "x", 1000f, 0.5f, iTween.EaseType.easeOutSine, true);

		yield return new WaitForSeconds(0.25f);

		// Put prey back into opponent grids
		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey)
						opponentPreyCards.Add(preyCard);
			}
		}

		foreach(CPreyCard preyCard in opponentPreyCards) {
			CGlobals.TweenScale(preyCard.gameObject, Vector3.one * 0.13f, 0.1f, iTween.EaseType.easeOutSine, true);
			preyCard.transform.parent = gameMan.Players[(int)preyCard.owner].gridPrey.transform;
		}
		yield return new WaitForSeconds(0.1f);

		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				player.EnableBasicPreyCard(false);
				yield return new WaitForEndOfFrame();
				player.gridPrey.UpdateGrid();
			}

			foreach(CPreyCard preyCard in player.prey)
				CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, preyCard, preyCard.Button, "MoveCardToActionPanel");
		}

		//foreach(CPlayer player in gameMan.Players)
			//CGlobals.TweenMove(player.gridPrey.gameObject, "y", 67f, 0.5f, iTween.EaseType.easeOutSine, true);

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

		//preyCardToSwap.ChangeWidgetDepth(CGlobals.OPPONENT_PREY_BG_DEPTH + 1);

        StartCoroutine(preyCardToAcquire.SetNewParentPanel(CardData.ParentPanel.PREY));
        CGlobals.UpdateWidgets();

		StartCoroutine(FadeBG(false));

		// Enable unselected cards
		foreach(CPreyCard preyCard in CGameManager.instance.activePlayer.prey)
			if(preyCard != preyCardToSwap)
				preyCard.gameObject.SetActive(true);

		yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());

        CCardManager.instance.ReplacePreyForActivePlayer_Swap(preyCardToAcquire);



		yield return new WaitForSeconds(0.9f);
		yield return StartCoroutine(CGameManager.instance.EndTurn());
   	}

    public void TakeFromTopOPreyDeck(CPreyCard preyCardToAcquire) { StartCoroutine(TakeFromTopOfPreyDeck_CR(preyCardToAcquire)); }
    public IEnumerator TakeFromTopOfPreyDeck_CR(CPreyCard preyCardToAcquire)
    {
        CGameManager gameMan = CGameManager.instance;
        CLogfile.instance.Append("Prey Swap: Take from top of prey deck");

        CUIManager.instance.DisableAllCollidersForEndTurn();

        Vector3 v3OldAcquirePosition = preyCardToAcquire.transform.localPosition;
        Vector3 v3OldAcquireScale = preyCardToAcquire.transform.localScale;
        Vector3 v3OldSwapPosition = preyCardToSwap.transform.localPosition;
        Vector3 v3OldSwapcale = preyCardToSwap.transform.localScale;
        int oldSiblingIndex = preyCardToSwap.transform.GetSiblingIndex();

        preyCardToAcquire.transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
        preyCardToAcquire.transform.SetSiblingIndex(oldSiblingIndex);
        StartCoroutine(preyCardToAcquire.SetNewParentPanel(CardData.ParentPanel.PREY));

        preyCardToSwap.transform.parent = CCardManager.instance.goPreyDrawPile.transform;
        preyCardToSwap.transform.SetAsLastSibling();

        gameMan.activePlayer.prey.Remove(preyCardToSwap);
        gameMan.activePlayer.prey.Add(preyCardToAcquire);

        CCardManager.instance.lstDrawPile_Prey.Remove(preyCardToAcquire);
        CCardManager.instance.lstDrawPile_Prey.Add(preyCardToSwap);

        CGlobals.UpdateWidgets();

        StartCoroutine(preyCardToAcquire.FlipCard(0f, 0.5f, false));
        CGlobals.TweenMove(preyCardToAcquire.gameObject, "position", v3OldSwapPosition, 0.75f, iTween.EaseType.linear, true);
        CGlobals.TweenScale(preyCardToAcquire.gameObject, v3OldSwapcale, 0.75f, iTween.EaseType.easeOutSine, true);

        StartCoroutine(preyCardToSwap.FlipCard(180f, 0.5f, false));
        CGlobals.TweenMove(preyCardToSwap.gameObject, "position", v3OldAcquirePosition, 0.75f, iTween.EaseType.linear, true);
        CGlobals.TweenScale(preyCardToSwap.gameObject, v3OldAcquireScale, 0.75f, iTween.EaseType.easeOutSine, true);

        TweenAllOpponentPreyOutlineColor(true);
        StartCoroutine(preyCardToSwap.TweenOutlineOverlayColor(gameMan.activePlayer.player, true));

        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "x", 2000f, 1f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(goPreySwapBGSprite, "y", 1300f, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeBG(false));
        StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());
        InstructionText.HideBotHeader();

        yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());

        //CActivePlayerPanel.instance.gridPrey.transform.parent = CActivePlayerPanel.instance.uiPanelPrey.transform;
        //CActivePlayerPanel.instance.gridPrey.transform.localPosition = Vector3.up * -25f;

        //foreach (CPreyCard preyCard in gameMan.activePlayer.prey)
        //    preyCard.gameObject.SetActive(true);

        //CActivePlayerPanel.instance.gridPrey.UpdateGrid();

       // Put prey back into opponent grids
       List<CPreyCard> opponentPreyCards = new List<CPreyCard>();
		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey)
						opponentPreyCards.Add(preyCard);
			}
		}

		foreach(CPreyCard preyCard in opponentPreyCards) {
			CGlobals.TweenScale(preyCard.gameObject, Vector3.one * 0.13f, 0.1f, iTween.EaseType.easeOutSine, true);
			preyCard.transform.parent = gameMan.Players[(int)preyCard.owner].gridPrey.transform;
		}
        yield return new WaitForSeconds(0.1f);

		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				player.EnableBasicPreyCard(false);
				yield return new WaitForEndOfFrame();
				player.gridPrey.UpdateGrid();
			}

			foreach(CPreyCard preyCard in player.prey)
				CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, preyCard, preyCard.Button, "MoveCardToActionPanel");
		}

        CCardManager.instance.goPreyDrawPile.transform.parent = CCardManager.instance.transform.parent;
        CGlobals.UpdateWidgets();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(gameMan.EndTurn());
    }

    public IEnumerator TakeFromTopOfPreyDeck_NoSwapCR(CPreyCard preyCardToAcquire)
    {
        CGameManager gameMan = CGameManager.instance;
        CUIManager.instance.DisableAllCollidersForEndTurn();

        Vector3 v3OldAcquirePosition = preyCardToAcquire.transform.localPosition;
        Vector3 v3OldAcquireScale = preyCardToAcquire.transform.localScale;

        preyCardToAcquire.transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
        StartCoroutine(preyCardToAcquire.SetNewParentPanel(CardData.ParentPanel.PREY));
        gameMan.activePlayer.prey.Add(preyCardToAcquire);

        CCardManager.instance.lstDrawPile_Prey.Remove(preyCardToAcquire);
        CGlobals.UpdateWidgets();
        CGlobals.TweenRotate(preyCardToAcquire.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        TweenAllOpponentPreyOutlineColor(true);
        yield return new WaitForSeconds(0.25f);

        preyCardToAcquire.Texture.enabled = true;
        preyCardToAcquire.TextureBack.enabled = false;

        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "x", 2000f, 1f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(goPreySwapBGSprite, "y", 1300f, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeBG(false));
        StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        CActivePlayerPanel.instance.gridPrey.transform.parent = CActivePlayerPanel.instance.uiPanelPrey.transform;
        CActivePlayerPanel.instance.gridPrey.transform.localPosition = Vector3.up * -25f;

        foreach (CPreyCard preyCard in gameMan.activePlayer.prey)
            preyCard.gameObject.SetActive(true);

        CActivePlayerPanel.instance.gridPrey.UpdateGrid();

       // Put prey back into opponent grids
       List<CPreyCard> opponentPreyCards = new List<CPreyCard>();
		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey)
						opponentPreyCards.Add(preyCard);
			}
		}

		foreach(CPreyCard preyCard in opponentPreyCards) {
			CGlobals.TweenScale(preyCard.gameObject, Vector3.one * 0.13f, 0.1f, iTween.EaseType.easeOutSine, true);
			preyCard.transform.parent = gameMan.Players[(int)preyCard.owner].gridPrey.transform;
		}
        yield return new WaitForSeconds(0.1f);

		foreach(CPlayer player in gameMan.Players) {
			if(player.player != gameMan.activePlayer.player) {
				player.EnableBasicPreyCard(false);
				yield return new WaitForEndOfFrame();
				player.gridPrey.UpdateGrid();
			}

			foreach(CPreyCard preyCard in player.prey)
				CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, preyCard, preyCard.Button, "MoveCardToActionPanel");
		}

        CCardManager.instance.goPreyDrawPile.transform.parent = CCardManager.instance.transform.parent;
        CGlobals.UpdateWidgets();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(gameMan.EndTurn());
    }

    public IEnumerator MovePreyPanelToActionPanel(bool withBasicPrey = true)
    {
        yield return new WaitForEndOfFrame();

        if(!withBasicPrey)
        {
            CBaseCard basicPrey = CGameManager.instance.activePlayer.prey[0];
            basicPrey.transform.parent = CActivePlayerPanel.instance.panelPrey.anchorPanelBackground.transform;
        }

        Grid gridPrey = CActivePlayerPanel.instance.gridPrey;
        gridPrey.transform.parent = CUIManager.instance.actionPanel.transform;
        gridPrey.mv2GridDimensions.x = 1100;
        gridPrey.mv2CellMaxDimensions.x = 300;
        gridPrey.mnBorder = 80;
        gridPrey.UpdateGrid(0.35f);

        CGlobals.TweenMove(gridPrey.gameObject, "position", new Vector3(215f, 250f, 0f), 0.35f, iTween.EaseType.easeOutSine, true);
        CGlobals.UpdateWidgets();

        yield return new WaitForSeconds(0.4f);
    }

    public IEnumerator MovePreyPanelToActivePlayerPanel()
    {
        yield return new WaitForEndOfFrame();

        foreach (CBaseCard preyCard in CGameManager.instance.activePlayer.prey)
            preyCard.gameObject.SetActive(true);

        CBaseCard basicPrey = CGameManager.instance.activePlayer.prey[0];
		Grid gridPrey = CActivePlayerPanel.instance.gridPrey;

        basicPrey.transform.parent = gridPrey.transform;
        basicPrey.transform.SetAsFirstSibling();
        CGlobals.UpdateWidgets();
        yield return new WaitForEndOfFrame();

		gridPrey.transform.parent = CActivePlayerPanel.instance.panelPrey.anchorPanelBackground.transform;
        gridPrey.mv2GridDimensions.x = 900;
        gridPrey.mv2CellMaxDimensions.x = 200;
        gridPrey.mnBorder = 55;
        gridPrey.UpdateGrid(0.75f);

        CGlobals.TweenMove(gridPrey.gameObject, "position", Vector3.up * -25f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.UpdateWidgets();
        yield return new WaitForSeconds(0.75f);
    }

    public void MoveHandPanelToActivePlayerPanel()
    {
    	CUIManager uiManager = CUIManager.instance;
		CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;

		foreach(CBaseCard handCard in CGameManager.instance.activePlayer.hand)
			CGlobals.AssignNewUIButtonOnClickTarget(uiManager, handCard, handCard.Button, "MoveCardToActionPanel");

		CGlobals.UpdateWidgets();
    }

    // Called when instant cards are clicked while feeding snail
	public void AddUpgradeToFeedingSnail(CInstantCard instantCard) { StartCoroutine(AddUpgradeToFeedingSnail_CR(instantCard)); }
    public IEnumerator AddUpgradeToFeedingSnail_CR(CInstantCard instantCard)
    {
        CLogfile.instance.Append("Adding upgrade: " + instantCard.instantType);

		instantCard.EnableButton(false);
    	//instantCard.transform.parent = goActionContainer.transform;
    	//CGlobals.TweenMove(instantCard.gameObject, "position", new Vector3(-484f, 129f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
    	CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
    	switch(instantCard.instantType)
    	{
    	case CardData.InstantType.Potency:
			snailCard.HasPotency = true;
			break;

		case CardData.InstantType.Starvation:
			snailCard.HasStarvation = true;
			break;
		}

    	yield return new WaitForSeconds(0.5f);
		//instantCard.EnableButton(true);
    }

    public IEnumerator RemoveUpgradeFromFeedingSnail_CR(CInstantCard instantCard)
    {
        yield return new WaitForEndOfFrame();

        CLogfile.instance.Append("Removing upgrade: " + instantCard.instantType);

		instantCard.EnableButton(false);
    	CUIManager.instance.SelectedCards.Remove(instantCard);

    	CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
    	switch(instantCard.instantType)
    	{
    	case CardData.InstantType.Potency:
			snailCard.HasPotency = false;
			break;

		case CardData.InstantType.Starvation:
			snailCard.HasStarvation = false;
			break;
		}
    }


    public void FeedCard() { StartCoroutine(CR_FeedCard()); }
    public IEnumerator CR_FeedCard()
    {
        CLogfile.instance.Append("Clicked feed button");

    	CGameManager gameMan = CGameManager.instance;
		//CAudioManager.instance.PlaySound(gameMan.acSuccess);
		gameMan.CurrentPlayState = GameData.PlayState.SELECTING_PREY;

		CActivePlayerPanel.instance.panelHand.transform.parent = this.transform;

        // Set Hand card button targets
		foreach(CBaseCard handCard in CGameManager.instance.activePlayer.hand)
        {
            if(handCard.cardType == CardData.CardType.Instant)
                CGlobals.AssignNewUIButtonOnClickTarget(handCard, (CInstantCard)handCard, handCard.Button, "SelectedCardToDiscard");
            else
                CGlobals.AssignNewUIButtonOnClickTarget(handCard, null, handCard.Button, "SelectedCardToDiscard");
		}

		CActivePlayerPanel.instance.discardCollider.boxCollider.enabled = true;
		CGlobals.UpdateWidgets();

        CPlayer activePlayer = CGameManager.instance.activePlayer;
        CSnailCard card = ((CSnailCard)CUIManager.instance.SelectedCard);
		card.EnableButton(false);

        //  Old function target
        //foreach (CBaseCard preyCard in activePlayer.prey)
        //    CGlobals.AssignNewUIButtonOnClickTarget(this, preyCard, preyCard.Button, "FeedSnail");

        // New function target
        foreach (CBaseCard preyCard in activePlayer.prey)
            CGlobals.AssignNewUIButtonOnClickTarget(ActionEndPrompt, preyCard, preyCard.Button, "MovePreyCardToPanel");

        CUIManager.instance.actionPanel.InstructionText.ShowTopHeader("Select your prey");

        yield return StartCoroutine(ExpandActionPanelBackground(false));
        CGlobals.TweenMove(card.gameObject, "position", new Vector3(-638f, 345f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(card.gameObject, Vector3.one * 0.5f, 0.5f, iTween.EaseType.easeOutSine, true);

        switch (card.cardType) {
            case CardData.CardType.Snail:
            yield return StartCoroutine(MovePreyPanelToActionPanel());
            break;
        }

        yield return new WaitForSeconds(0.25f);
        CGlobals.TweenScale(anchorPanelBackground.gameObject, Vector3.zero, 0.01f, iTween.EaseType.linear, true);

        yield return new WaitForSeconds(0.03f);
		card.EnableButton(true);
    }

    private bool ComparePreyType(CPreyCard preyCard)
    {
    	CSnailCard snail = (CSnailCard)CUIManager.instance.SelectedCard;
    	if(snail.preyType == CardData.PreyType.All || snail.preyType == preyCard.preyType)
    		return true;
    	return false;
    }

    private bool CompareAttackDefense(CPreyCard preyCard)
    {
		CSnailCard snail = (CSnailCard)CUIManager.instance.SelectedCard;
    	if(snail.strength == preyCard.resistance)
    		return true;
		return false;
    }

    public void FeedSnail(CBaseCard preyCard) { StartCoroutine(FeedSnail_CR(preyCard)); }
    private IEnumerator FeedSnail_CR(CBaseCard card)
    {
    	if(canPlayerFeed) {
			CUIManager uiMan = CUIManager.instance;
	    	CPreyCard preyCard = (CPreyCard)card;
			CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);

	    	bool strongEnough = CompareAttackDefense(preyCard);
	    	bool validPreyType = (ComparePreyType(preyCard) || snailCard.HasStarvation);

	    	if(preyCard.preyName == CardData.PreyName.Basic_Prey)
	    	{
                int numDiscards = uiMan.lstSelectedCards.Count - 1;

                if(numDiscards == 1) {
				    canPlayerFeed = false;
                    CLogfile.instance.Append("Fed on Basic Prey");
                    yield return StartCoroutine(ActionEndPrompt.ClosePanel_CR());

                    CAudioManager.instance.PlaySound(CGameManager.instance.acEatPrey);
				    StartCoroutine(snailCard.SetFedState(CardData.FedState.Fed));
				    yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());
				    yield return StartCoroutine(EndFeedSnail_CR());
                }
                else {
                    StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.DECREASE_STRENGTH));
                    CLogfile.instance.Append("Failed feed attempt: Too many discarded cards");
                }
	    	}
	    	else
	    	{
				if(strongEnough && validPreyType)
		    	{
					canPlayerFeed = false;
                    CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;

                    yield return StartCoroutine(ActionEndPrompt.ClosePanel_CR());
                    CAudioManager.instance.PlaySound(CGameManager.instance.acEatPrey);
			        uiMan.EnablePreyButtons(false);

					CGlobals.TweenMove(card.gameObject, "x", 900f, 0.5f, iTween.EaseType.easeOutSine, true);
		            yield return new WaitForSeconds(0.5f);
                    
                    
                    ////////////////////////////////////////////////////////////////////////////////
                    // Prey Rewards
                    ////////////////////////////////////////////////////////////////////////////////
                    //
                    //
                    CardData.PreyName name = preyCard.preyName;
                    CLogfile.instance.Append(name.ToString());

                    // if no peptides gained, do not show peptide panel
                    bool peptidesGained = (name != CardData.PreyName.Stripey &&
                                            name != CardData.PreyName.Common_Periwinkle &&
                                            name != CardData.PreyName.Bearded_Fireworm);

                    // Gain two peptides, reveal peptide from # cabal
                    bool gainTwoPeptidesAndReveal = (name == CardData.PreyName.Conus_kinoshitai ||
                                                    name == CardData.PreyName.Butterfly_Fish ||
                                                    name == CardData.PreyName.Arrow_Worm);

                    bool peekAndReveal = (name == CardData.PreyName.Goldfish ||
                                            name == CardData.PreyName.Serpents_Head_Cowry ||
                                            name == CardData.PreyName.Ragworm);

                    // steal one of opponents' prey after eating
                    bool stealPreyFromOpp = (name == CardData.PreyName.Clownfish ||
                                                    name == CardData.PreyName.Bobbit_Worm ||
                                                    name == CardData.PreyName.Venus_Comb_Murex);

                    // pick random card from market after feeding
                    bool randomMarketCard = (name == CardData.PreyName.Hebrew_Volute ||
                                                name == CardData.PreyName.Bloodworm ||
                                                name == CardData.PreyName.Gold_Belly_Damsel_Fish);

                    bool drawTwoCards = (name == CardData.PreyName.Blue_Devil_Fish ||
                                                name == CardData.PreyName.Rusty_Scale_Worm ||
                                                name == CardData.PreyName.Turbo_Snail);

                    bool feedOrUnhibernate = (name == CardData.PreyName.Lugworm ||
                                                name == CardData.PreyName.Dusky_Frillgoby ||
                                                name == CardData.PreyName.Olive_Shells);

                    // initialize variables used when adding peptides to snail card
                    if (gainTwoPeptidesAndReveal) {
                        PeptideExtractionPanel.peptidesToExtract = 2;
                        PeptideExtractionPanel.gainTwoPeptides_RevealPeptide = true;
                    }
                    else if (peekAndReveal)
                        PeptideExtractionPanel.peekAt_thenRevealPeptide = true;
                    else if (stealPreyFromOpp)
                        PeptideExtractionPanel.stealPreyFromOpponent = true;
                    else if (randomMarketCard)
                        PeptideExtractionPanel.pickRandomMarketCard = true;
                    else if (drawTwoCards)
                        PeptideExtractionPanel.drawTwoCardsFromDeck = true;
                    else if (feedOrUnhibernate)
                        PeptideExtractionPanel.feedOrUnhibernateAnotherSnailInPool = true;
                    
                    if(peptidesGained)
                    {
                        InstructionText.HideTopHeader();
                        InstructionText.ShowBotHeader("Select a peptide");

                        // Potency allows player to choose peptide
                        if(snailCard.HasPotency)
                        {
                            //bttnAlpha.isEnabled = bttnDelta.isEnabled = bttnMu.isEnabled = bttnOmega.isEnabled = true;
                            StartCoroutine(PeptideExtractionPanel.ShowPanel(snailCard.lstPeptideRewards));

                            if (!stealPreyFromOpp) {
    					        CCardManager.instance.ReplacePreyForActivePlayer(card);
                                yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());
                            }
                            else
                                preyCardToSwap = (CPreyCard)card;
					    }
					    else
                        {
							StartCoroutine(PeptideExtractionPanel.ShowPanel(snailCard.lstPeptideRewards));

                            if (!stealPreyFromOpp)
                            {
                                CCardManager.instance.ReplacePreyForActivePlayer(card);
                                yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());
                            }
                            else
                                preyCardToSwap = (CPreyCard)card;
                        }

                        StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCardsExceptFeedingSnail());
					}
                    else
                    {
                        // Snail does not gain a peptide, Kill all hibernating snails
                        //
                        StartCoroutine(FadeBG(false, 0.25f));
                        CCardManager.instance.ReplacePreyForActivePlayer(card);
                        yield return StartCoroutine(MovePreyPanelToActivePlayerPanel());
                        yield return StartCoroutine(CPreyReward.RunReward_NoPeptideKillAllHibernatingSnails());
                        yield return StartCoroutine(snailCard.SetFedState(CardData.FedState.Fed));
                        yield return StartCoroutine(EndFeedSnail_CR());
                    }
		        }
				else if(!strongEnough && !validPreyType)
		        {
		        	// Error
                    if(snailCard.strength < preyCard.resistance) {
                        StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.INCREASE_STRENGTH));
                        CLogfile.instance.Append("Failed feed attempt: Must increase strength or use matching prey type");
                    }
                    else if(snailCard.strength > preyCard.resistance) {
                        StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.DECREASE_STRENGTH));
                        CLogfile.instance.Append("Failed feed attempt: Must reduce # of discarded cards or use matching prey type");
                    }
		        }
				else if(!strongEnough)
		        {
					// Error
                    if(snailCard.strength < preyCard.resistance) {
                        StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.INCREASE_STRENGTH));
                        CLogfile.instance.Append("Failed feed attempt: Too many discarded cards");
                    }
                    else if(snailCard.strength > preyCard.resistance) {
                        StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.DECREASE_STRENGTH));
                        CLogfile.instance.Append("Failed feed attempt: Must increase strength");
                    }
		        }
				else if(!validPreyType)
		        {
					// Error
                    StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.WRONG_TYPE));
                    CLogfile.instance.Append("Failed feed attempt: Must use matching prey type");
		        }
	        }
		}
        else
        {
            // Error - Discard card to feed;
            StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.DISCARD_CARD));
            CLogfile.instance.Append("Failed feed attempt: Must discard card to feed");
        }
    }

    public IEnumerator EndFeedSnail_CR()
    {
    	CUIManager uiMan = CUIManager.instance;
        uiMan.DisableAllCollidersForEndTurn();
		CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
		uiMan.SelectedCards.Remove(snailCard);

        yield return StartCoroutine(snailCard.ResetStrengthAnimation());
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

		MoveHandPanelToActivePlayerPanel();
		yield return StartCoroutine(CUIManager.instance.MoveCardToSnailsPanel_CR(snailCard));

        foreach (CBaseCard card in CGameManager.instance.activePlayer.prey)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

        yield return new WaitForSeconds(1.0f);
		CUIManager.instance.EnablePreyButtons();

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

	public void UnhibernateButton() { StartCoroutine(UnhibernateButton_CR()); }
    public IEnumerator UnhibernateButton_CR()
    {
        CLogfile.instance.Append("Click Unhibernate button");

    	CUIManager uiMan = CUIManager.instance;
		CPlayer activePlayer = CGameManager.instance.activePlayer;
		CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
		CGameManager.instance.CurrentPlayState = GameData.PlayState.UNHIBERNATING_SNAIL;

		//snailCard.EnableButton(false);

        // Set function target for hand cards
        foreach(CBaseCard card in activePlayer.hand)
            CGlobals.AssignNewUIButtonOnClickTarget(card, null, card.Button, "SelectedCardToUnhibernateSnail");

		CGlobals.AssignNewUIButtonOnClickTarget(uiMan, snailCard, snailCard.Button, "MoveCardToSnailsPanel");

        CGlobals.TweenMove(goActionContainer, "y", 144f, 0.5f, iTween.EaseType.easeInSine, true);
        CGlobals.TweenScale(goActionContainer, Vector3.one * 0.75f, 0.5f, iTween.EaseType.easeInBack, true);
        yield return new WaitForSeconds(0.5f);

        BoxCollider unhibCollider = goUnhibernateButton.GetComponent<BoxCollider>();
        UIButton unhibButton = goUnhibernateButton.GetComponent<UIButton>();
        CGlobals.AssignNewUIButtonOnClickTarget(this, null, unhibButton, "UnhibernateSnail");
        unhibCollider.enabled = false;
        unhibButton.enabled = false;
        yield return new WaitForEndOfFrame();
        unhibButton.enabled = true;

        //yield return StartCoroutine(ExpandActionPanelBackground(false));

        //CGlobals.TweenMove(snailCard.gameObject, "position", Vector3.up * 350f, 0.5f, iTween.EaseType.easeOutSine, true);
		//CGlobals.TweenScale(snailCard.gameObject, Vector3.one * 0.5f, 0.5f, iTween.EaseType.easeOutSine, true);
        //yield return new WaitForSeconds(0.5f);

		CActivePlayerPanel.instance.panelHand.transform.parent = this.transform;
		//CActivePlayerPanel.instance.discardCollider.boxCollider.enabled = true;
		CGlobals.UpdateWidgets();
    }


    public void UnhibernateSnail() { StartCoroutine(UnhibernateSnail_CR()); }
    public IEnumerator UnhibernateSnail_CR()
    {
        CUIManager uiMan = CUIManager.instance;

        yield return StartCoroutine(ExpandActionPanelBackground(false));
        UIButton unhibButton = goUnhibernateButton.GetComponent<UIButton>();
        CGlobals.AssignNewUIButtonOnClickTarget(this, null, unhibButton, "UnhibernateButton");

        CSnailCard snailCard = (CSnailCard)uiMan.SelectedCard;
        snailCard.SetFedStateToUnfed();

		yield return new WaitForSeconds(0.5f);
        CBaseCard discard = uiMan.SelectedCards[1];
        uiMan.SelectedCards.Remove(discard);
        CGameManager.instance.activePlayer.hand.Remove(discard);
        CGameManager.instance.activePlayer.discard.Add(discard);
        StartCoroutine(CActivePlayerPanel.instance.DiscardCard(discard));
        yield return new WaitForSeconds(0.5f);

		yield return StartCoroutine(uiMan.MoveCardToSnailsPanel_CR(snailCard));
		yield return StartCoroutine(CGameManager.instance.EndTurn());
    }
    
    public void ResetActionContainer()
    {
        goActionContainer.transform.localPosition = Vector3.zero;
        goActionContainer.transform.localScale = Vector3.one;

        CGlobals.AssignNewUIButtonOnClickTarget(this, null, goUnhibernateButton.GetComponent<UIButton>(), "UnhibernateButton");
        goUnhibernateButton.GetComponent<BoxCollider>().enabled = true;
        goUnhibernateButton.GetComponent<UISprite>().color = Color.white;
    }

    public void EnableButton(BUTTON button)
    {
        
        switch (button)
        {
            case BUTTON.FEED:
                goFeedButton.SetActive(true);
                goUseButton.SetActive(false);
                //goBuyButton.SetActive(false);
			goUnhibernateButton.SetActive(false);
                break;

            case BUTTON.USE:
                goFeedButton.SetActive(false);
                goUseButton.SetActive(true);
                //goBuyButton.SetActive(false);
			goUnhibernateButton.SetActive(false);
                break;

            case BUTTON.BUY:
                goFeedButton.SetActive(false);
                goUseButton.SetActive(false);
                //goBuyButton.SetActive(true);
			goUnhibernateButton.SetActive(false);
                break;

			case BUTTON.UNHIBERNATE:
                goFeedButton.SetActive(false);
                goUseButton.SetActive(false);
                //goBuyButton.SetActive(false);
			    goUnhibernateButton.SetActive(true);
                break;

            case BUTTON.NONE:
                goFeedButton.SetActive(false);
                goUseButton.SetActive(false);
                //goBuyButton.SetActive(false);
			goUnhibernateButton.SetActive(false);
                break;
        }
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

    public void OnUpdatePreyPanel(int value)
    {
        CGameManager.instance.activePlayer.PreyPanel.height = value;
    }

    public IEnumerator ExpandActionPanelBackground(bool expand = true)
    {
        float time = 0.35f;

        if(expand) {
			CGlobals.TweenMove(anchorPanelBackground.gameObject, "position", new Vector3(-280f, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
			CGlobals.TweenScale(anchorPanelBackground.gameObject, Vector3.one, time, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(time);
        }
        else {
			CGlobals.TweenMove(anchorPanelBackground.gameObject, "position", new Vector3(-366f, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
			CGlobals.TweenScale(anchorPanelBackground.gameObject, new Vector3(0f, 0.9f, 1f), time, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(time);
        }
    }
}
