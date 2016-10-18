using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPreyReward : MonoBehaviour
{
    #region Singleton Instance
    private static CPreyReward _instance;
    public static CPreyReward instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CPreyReward>();
            }
            return _instance;
        }
    }
    #endregion

    public struct tHibernatingSnail
    {
        public PlayerData.PLAYER player;
        public CSnailCard snailCard;
    }

    private List<CBaseCard> selectedDiscardCards;

    void Awake()
    {
        selectedDiscardCards = new List<CBaseCard>();
    }

    // Bearded_Fireworm, Stripey, Common Periwinkle
	public static IEnumerator RunReward_NoPeptideKillAllHibernatingSnails()
    {
        List<tHibernatingSnail> hibernatingSnails = new List<tHibernatingSnail>();

        foreach(CPlayer player in CGameManager.instance.Players)
            foreach(CSnailCard snailCard in player.snails) {
                if(snailCard.fedState == CardData.FedState.Hibernating) {
                    tHibernatingSnail hibeSnail = new tHibernatingSnail();
                    hibeSnail.player = player.player;
                    hibeSnail.snailCard = snailCard;
                    hibernatingSnails.Add(hibeSnail);
                }
            }

        foreach(tHibernatingSnail hibernatingSnail in hibernatingSnails) {
            if(hibernatingSnail.player == CGameManager.instance.activePlayer.player)
                yield return CGameManager.instance.StartCoroutine(hibernatingSnail.snailCard.KillSnail_CR(0.5f, 0f));
            else
                yield return CGameManager.instance.StartCoroutine(hibernatingSnail.snailCard.KillSnail_CR(0f, 0.5f));
        }
	}

    // Blue Devil Fish, Rusty Scale Worm, Turbo Snail
    public static IEnumerator RunReward_DrawTwoCards_Setup()
    {
        yield return new WaitForEndOfFrame();
        CGameManager gameMan = CGameManager.instance;
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        yield return gameMan.StartCoroutine(CUIManager.instance.SelectedCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        //CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;
        CGlobals.UpdateWidgets();

        foreach (CBaseCard handCard in activePlayer.hand)
            CGlobals.TweenMove(handCard.gameObject, "y", -250f, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.75f);
        foreach (CBaseCard handCard in activePlayer.hand)
            handCard.transform.parent = CActivePlayerPanel.instance.panelHand.anchorPanelBackground.transform;

        foreach (CBaseCard discardCard in activePlayer.discard) {
            discardCard.transform.parent = CActivePlayerPanel.instance.gridHand.transform;
        }

        foreach (CBaseCard discardCard in activePlayer.discard) {
            UIButton bttn = discardCard.TextureBack.GetComponent<UIButton>();
            CGlobals.AssignNewUIButtonOnClickTarget(CPreyReward.instance, (CBaseCard)discardCard, bttn, "SelectCardFromDiscard");
        }

        CGlobals.UpdateWidgets();
        CActivePlayerPanel.instance.gridHand.UpdateGrid(1f);
    }

    private void SelectCardFromDiscard(CBaseCard baseCard) { StartCoroutine(SelectCardFromDiscard_CR(baseCard)); }
    private IEnumerator SelectCardFromDiscard_CR(CBaseCard baseCard)
    {
        yield return new WaitForEndOfFrame();

        CPlayer activePlayer = CGameManager.instance.activePlayer;
        selectedDiscardCards.Add(baseCard);
        activePlayer.discard.Remove(baseCard);

        Vector3 v3Pos = (selectedDiscardCards.Count == 1) ? new Vector3(0f, 460f, 0f) : new Vector3(375f, 460f, 0f);
        CGlobals.TweenMove(baseCard.gameObject, "position", v3Pos, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(baseCard.gameObject, Vector3.one * 0.35f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);

        if(selectedDiscardCards.Count == 2)
        {
            // Rotate
            // Put in hand

            foreach (CBaseCard card in activePlayer.discard) {
                StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.DISCARD));
            }
            foreach (CBaseCard card in activePlayer.hand)
                StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.HAND));

            CGlobals.UpdateWidgets();
            CActivePlayerPanel.instance.gridDiscard.UpdateGrid(0.65f);


            // Flip selected cards
            foreach (CBaseCard card in selectedDiscardCards)
                yield return StartCoroutine(card.FlipCard(0f, 0.5f, false));


            yield return new WaitForSeconds(0.5f);
            foreach (CBaseCard card in selectedDiscardCards) {
                yield return StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.HAND));
                activePlayer.hand.Add(card);
                card.transform.SetAsLastSibling();
            }

            CActivePlayerPanel.instance.gridHand.UpdateGrid(0.75f);
            yield return new WaitForSeconds(1f);
            CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;
            yield return StartCoroutine(CGameManager.instance.EndTurn());
        }
    }

    // Olive shells, Lugworm, Dusky Frillgoby
    public static IEnumerator FeedOrUnhibernateAnotherSnailInPool_Setup()
    {
        yield return new WaitForEndOfFrame();
        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        bool allSnailsFed = true;
        foreach (CSnailCard snailCard in activePlayer.snails)
            if (snailCard.fedState != CardData.FedState.Fed)
                allSnailsFed = false;

        if(activePlayer.snails.Count == 1 || allSnailsFed)
        {
            // Return snail back to snail panel
            yield return gameMan.StartCoroutine(uiMan.SelectedCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
            CActivePlayerPanel.instance.gridSnails.UpdateGrid();
            CGlobals.UpdateWidgets();

            yield return new WaitForSeconds(0.5f);
            CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;
            yield return gameMan.StartCoroutine(CGameManager.instance.EndTurn());
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
            CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;
            //CActivePlayerPanel.instance.panelSnails.transform.parent = uiMan.actionPanel.transform;

            // Return snail back to snail panel
            yield return gameMan.StartCoroutine(uiMan.SelectedCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
            CActivePlayerPanel.instance.gridSnails.UpdateGrid();
            CGlobals.UpdateWidgets();
            yield return new WaitForSeconds(0.5f);

            // move snail grid to action panel without snail that was JUST fed
            CActivePlayerPanel.instance.gridSnails.transform.parent = uiMan.actionPanel.transform;
            foreach(CSnailCard snailCard in activePlayer.snails)
            {
                if (uiMan.SelectedCard != snailCard && snailCard.fedState != CardData.FedState.Fed)
                    CGlobals.AssignNewUIButtonOnClickTarget(snailCard, null, snailCard.Button, "RaiseFedState");
                else
                {
                    snailCard.Button.enabled = false;
                    snailCard.Button.GetComponent<BoxCollider>().enabled = false;
                    yield return new WaitForEndOfFrame();
                    snailCard.Button.enabled = true;
                }

            }
            CGlobals.UpdateWidgets();
        }
    }

    // Bobbit worm, Venus comb murex, Clownfish
    public static IEnumerator StealPreyFromOpponent()
    {
        yield return new WaitForEndOfFrame();

        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;
        CGlobals.UpdateWidgets();

		Grid gridPrey = CActivePlayerPanel.instance.gridPrey;
		gridPrey.transform.parent = CActivePlayerPanel.instance.panelPrey.anchorPanelBackground.transform;
        gridPrey.mv2GridDimensions.x = 900;
        gridPrey.mv2CellMaxDimensions.x = 200;
        gridPrey.mnBorder = 55;
        gridPrey.UpdateGrid(0.75f);

        CGlobals.TweenMove(gridPrey.gameObject, "position", Vector3.up * -25f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.UpdateWidgets();
        yield return new WaitForSeconds(0.75f);

        List<CPreyCard> opponentPreyCards = new List<CPreyCard>();

		// Get list of prey cards
		foreach(COpponent opponent in COpponentsPanel.instance.lstOpponents) {
			CPlayer player = opponent.GetComponent<CPlayer>();
			if(player.player != gameMan.activePlayer.player) {
				foreach(CPreyCard preyCard in player.prey)
					if(preyCard.preyName != CardData.PreyName.Basic_Prey) {
						CGlobals.AssignNewUIButtonOnClickTarget(uiMan.actionPanel, preyCard, preyCard.Button, "SwapCard");
						preyCard.EnableButton(true);
						opponentPreyCards.Add(preyCard);
					}
			}
		}

        foreach(CPreyCard preyCard in opponentPreyCards)
			preyCard.transform.parent = CUIManager.instance.actionPanel.gridPreySwap.transform;


		//#if UNITY_IOS
		CGlobals.TweenMove(CUIManager.instance.actionPanel.goPreySwapBGSprite, "y", 480f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#else
		//CGlobals.TweenMove(CUIManager.instance.actionPanel.goPreySwapBGSprite, "y", 390f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#endif


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
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
        }
        else if(gameMan.numPlayers == 3)
        {
            CGlobals.TweenMove(opponentPreyCards[0].gameObject, "position", new Vector3(x1, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[0].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[2].gameObject, "position", new Vector3(x1, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[2].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[3].gameObject, "position", new Vector3(x2, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[3].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
        }
        else if(gameMan.numPlayers == 4)
        {
            CGlobals.TweenMove(opponentPreyCards[0].gameObject, "position", new Vector3(x1, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[0].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[1].gameObject, "position", new Vector3(x2, 0f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[1].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[2].gameObject, "position", new Vector3(x1, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[2].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[3].gameObject, "position", new Vector3(x2, -237f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[3].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);

            CGlobals.TweenMove(opponentPreyCards[4].gameObject, "position", new Vector3(x1, -475f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[4].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(waitTime);
            CGlobals.TweenMove(opponentPreyCards[5].gameObject, "position", new Vector3(x2, -475f, 0f), time, iTween.EaseType.easeOutSine, true);
            CGlobals.TweenScale(opponentPreyCards[5].gameObject, Vector3.one * 0.27f, 0.5f, iTween.EaseType.easeOutSine, true);
        }

        // Get list of prey cards
        CUIManager.instance.actionPanel.TweenAllOpponentPreyOutlineColor();
        yield return new WaitForSeconds(0.75f);

		CCardManager.instance.EnableTopCardInPreyDeck();
        CGlobals.TweenMove(CCardManager.instance.goPreyDrawPile, "position", new Vector3(-100f, 40f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    // Gold belly damsel, Hebrew volute, Bloodworm
    public static IEnumerator PickRandomMarketCard()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiMan = CUIManager.instance;
        uiMan.StartCoroutine(uiMan.actionPanel.FadeBG(false, 0.5f));

        yield return uiMan.StartCoroutine(uiMan.SelectedCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        CActivePlayerPanel.instance.panelHand.transform.parent = CActivePlayerPanel.instance.transform;

        CMarketPanel marketPanel = uiMan.marketPanel;
        marketPanel.transform.parent = uiMan.actionPanel.transform;
        CGlobals.UpdateWidgets();

        marketPanel.ViewMarket_AfterFeeding();
    }

    //Conus kinoshitai, Butterfly fish, arrow worm
    public static IEnumerator GainTwoPeptides_RevealPeptide_CR()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiMan = CUIManager.instance;

        CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
        CPlayer activePlayer = CGameManager.instance.activePlayer;
		uiMan.actionPanel.MoveHandPanelToActivePlayerPanel();

        CGlobals.TweenMove(snailCard.gameObject, "position", new Vector3(140f, 280f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(snailCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);

        CCabalManager.instance.SetButtonFunctionTargetForRevealPeptide();
        CCabalManager.instance.EnableButtons();
        yield return new WaitForSeconds(0.25f);
		uiMan.cabalPanel.anchorPanelBackground.transform.parent = uiMan.actionPanel.goActionContainer.transform;
        CGlobals.UpdateWidgets();
    }

    // Ragworm, Serpent's head, Goldfish
    public static IEnumerator SetupForPeekAtThenRevealPeptide_CR()
    {
        yield return new WaitForEndOfFrame();

        CCabalManager.instance.SetButtonFunctionTargetForResearchCard();

        CUIManager uiMan = CUIManager.instance;
        CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
        CPlayer activePlayer = CGameManager.instance.activePlayer;

		CUIManager.instance.actionPanel.MoveHandPanelToActivePlayerPanel();

        CGlobals.TweenMove(snailCard.gameObject, "position", new Vector3(140f, 280f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(snailCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);

        CCabalManager.instance.SetButtonFunctionTargetForPeekAt_thenRevealPeptide();
        CCabalManager.instance.EnableButtons();
        yield return new WaitForSeconds(0.25f);
		uiMan.cabalPanel.anchorPanelBackground.transform.parent = uiMan.actionPanel.goActionContainer.transform;
        CGlobals.UpdateWidgets();
    }

}
