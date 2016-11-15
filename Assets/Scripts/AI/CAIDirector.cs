using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAIDirector : MonoBehaviour
{
    #region Singleton Instance
    private static CAIDirector _instance;
    public static CAIDirector instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CAIDirector>();
            }
            return _instance;
        }
    }
    #endregion

    public enum ACTIONS
    {
        UseInstant_Research = 0,
        UseInstant_OceanWaves,
        UsePredator,

        PlaySnail,

        FeedSnail_WithPeptides,
        FeedSnail_ZeroPeptides,

        UnhibernateSnail,

        BuyFromMarket,
        
        Count
    }


    public Dictionary<string, int> FrequencyTable;
    public List<ACTIONS> lstActions;
    public ACTIONS FinalDecision;

    // Action Variables
    public CSnailCard snailCardToPlay, snailCardToFeedWithPeptides, snailCardToFeedWithoutPeptides, snailCardToUnhibernate;
    public CPreyCard preyCardToEat;
    public CInstantCard instantCardToPlay;
    public List<CPeptide> lstHiddenPeptides; // of current player


    private int GetFrequencyValue(ACTIONS action, int value) {
        return FrequencyTable[action.ToString()];
    }
    private void SetFrequencyValue(ACTIONS action, int value) {
        FrequencyTable[action.ToString()] = value;
    }
    private void AddToFrequencyValue(ACTIONS action, int value) {
        FrequencyTable[action.ToString()] += value;
    }
    private void ClearFrequencyTable() {
        for (ACTIONS act = ACTIONS.UseInstant_Research; act < ACTIONS.Count; ++act)
            SetFrequencyValue(act, 0);
	}
    private ACTIONS GetHighestFrequencyAction() {
        ACTIONS highest = ACTIONS.Count;

        foreach (ACTIONS action in lstActions)
        {
            if(FrequencyTable.ContainsKey(action.ToString()))
            {
                if (highest == ACTIONS.Count)
                    highest = action;

                Debug.Log(action + ": " + FrequencyTable[action.ToString()]);
                if (FrequencyTable[action.ToString()] > FrequencyTable[highest.ToString()])
                    highest = action;
            }
        }

        return highest;
    }


    void Start ()
    {
        FrequencyTable = new Dictionary<string, int>();
        int numActions = (int)ACTIONS.Count;
        lstActions = new List<ACTIONS>(numActions);

        for (ACTIONS act = ACTIONS.UseInstant_Research; act < ACTIONS.Count; ++act)
            lstActions.Add(act);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            WeighNextAction();
            FinalDecision = GetHighestFrequencyAction();
            StartCoroutine(RunAction());
        }
    }


    public IEnumerator DecideNextMove()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);

        ClearFrequencyTable();
        WeighNextAction();
        FinalDecision = GetHighestFrequencyAction();
        Debug.Log("Action: " + FinalDecision.ToString());

        yield return RunAction();
    }

    // This will look at the current state of the game and decide the next step using a frequency table
    public void WeighNextAction()
    {
        CGameManager gameMan = CGameManager.instance;
        CPlayer currentPlayer = CGameManager.instance.activePlayer;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // AI LOGIC
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 1. SOLVE CABAL, IF POSSIBLE
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 2. WEIGH NEXT OPTIONS
        {
            // a. if snail with peptides is unfed, give feed action 3 pts * number of peptides.
            SetValue_FeedUnfedSnailCardWithPeptides();

            // b.   if the AI has a card that allows a guess at a peptide, and AI knows that peptide is hidden, give that USE INSTANT action two points. 
            //      Raise that to five if It’s a peptide the AI doesn’t already have.
            SetValue_UseInstant_ResearchCard();

            //
            // c.   if the AI has a predator, give the USE PREDATOR action 1 point for the every peptide the leading player (if not the AI) has.
            SetValue_UsePredatorCard();

            // d If the AI has a snail hibernated, give the UNHIBERNATE action for that snail 2 points.
            SetValue_UnhibernateSnailCard();

            // e.  If the AI has no snails in play, give it two points to the PLAY A SNAIL action. If it has a snail in play, and it has a prey it 
            //     can kill with a snail in the hand based on type, give it two points to FEED THAT SNAIL.
            SetValue_PlaySnailCard();

            // f. BUYING A MARKET CARD always gets two points if the AI can afford something in the market.
            SetValue_BuyMarketCard();

            // g. If the AI has a snail with no peptides on it that’s unfed, give the FEED action for that snail one point
            SetValue_FeedUnfedSnailCardWithZeroPeptides();

            ////////////////////////////////////////////
            //// Get highest frequency action
            //FinalDecision = GetHighestFrequencyAction();
        }
    }


    #region Set Value Functions

    public void FeedCard()
    {
        //  FEED ACTION
        //  1.  if AI choses to FEED but no prey matches, has Ocean Waves Card, and opponents have compatible card, use Ocean Waves
        //      Order by easiest to kill, harder prey, then no peptides
        //  2.  if no matching prey and has Ocean Waves Card, AI draws new Prey card
        //  3.  if no matching prey and does not have Ocean Waves Card, eat basic prey
        //  4.  otherwise feed on specific prey

        
    }

    // a.
    private void SetValue_FeedUnfedSnailCardWithPeptides()
    {
        CSnailCard snailCardWithPeptides = CGameManager.instance.activePlayer.HasSnailWithPeptides();
        if (snailCardWithPeptides && snailCardWithPeptides.fedState == CardData.FedState.Unfed) {
            snailCardToFeedWithPeptides = snailCardWithPeptides;
            SetFrequencyValue(ACTIONS.FeedSnail_WithPeptides, 3 * snailCardWithPeptides.lstContainedPeptides.Count);
        }
    }

    // b.
    private void SetValue_UseInstant_ResearchCard()
    {
        // Check for Research Card
        bool hasResearchCard = false;
        foreach(CBaseCard handCard in CGameManager.instance.activePlayer.hand)
            if(handCard.cardType == CardData.CardType.Instant)
                if(((CInstantCard)handCard).instantType == CardData.InstantType.Research) {
                    instantCardToPlay = (CInstantCard)handCard;
                    hasResearchCard = true;
                    break;
                }

        if(hasResearchCard)
        {
            // get list of peptides hidden to other players
            List<CPeptide> lstAllHiddenPeptides = CCabalManager.instance.GetHiddenPeptides();
            if(lstAllHiddenPeptides.Count > 0)
                SetFrequencyValue(ACTIONS.UseInstant_Research, 2);

            // Check if any peptides are unknown to AI at the moment, If so, raise value to 5.
            if (lstHiddenPeptides == null) lstHiddenPeptides = new List<CPeptide>();
            lstHiddenPeptides.Clear();

            foreach(CPeptide peptide in lstAllHiddenPeptides)
                if(CGameManager.instance.activePlayer.lstRevealedPeptides.Contains(peptide) == false)
                    lstHiddenPeptides.Add(peptide);

            if (lstHiddenPeptides.Count > 0)
                SetFrequencyValue(ACTIONS.UseInstant_Research, 5);
        }
        else
        {
            SetFrequencyValue(ACTIONS.UseInstant_Research, 0);
        }
    }

    // c.
    private void SetValue_UsePredatorCard()
    {
        CGameManager gameMan = CGameManager.instance;

        CPlayer leadingPlayer = null;
        int leadingPlayerIndex = 0;
        int amtOfPeptides = 0;

        // Get currently winning player
        for(int i = 0; i < gameMan.Players.Count; ++i)
        {
            if (gameMan.Players[i] == gameMan.activePlayer)
                continue;

            int peptides = 0;
            foreach (CSnailCard snailCard in gameMan.Players[i].snails)
                peptides += snailCard.lstContainedPeptides.Count;

            if (amtOfPeptides < peptides) {
                leadingPlayerIndex = i;
                amtOfPeptides = peptides;
            }
        }

        // give the USE PREDATOR action 1 point for the every peptide the leading player (if not the AI) has
        leadingPlayer = gameMan.Players[leadingPlayerIndex];

        if (leadingPlayer == gameMan.activePlayer)
        {
            SetFrequencyValue(ACTIONS.UsePredator, 0);
        }
        else
        {
            foreach(CBaseCard handCard in gameMan.activePlayer.hand) {
                if(handCard.cardType == CardData.CardType.Instant) {
                    CInstantCard instant = (CInstantCard)handCard;

                    // TODO: Need logic to differentiate between Turtle and other predator cards
                    if(instant.predator) {
                        SetFrequencyValue(ACTIONS.UsePredator, amtOfPeptides);
                        break;
                    }
                }
            }
        }
    }

    // d.
    private void SetValue_UnhibernateSnailCard()
    {
        if (CGameManager.instance.activePlayer.hand.Count == 0) {
            SetFrequencyValue(ACTIONS.UnhibernateSnail, 0);
            return;
        }

        snailCardToUnhibernate = CGameManager.instance.activePlayer.GetHibernatingSnail();

        if (snailCardToUnhibernate)
            SetFrequencyValue(ACTIONS.UnhibernateSnail, 2);
        else
            SetFrequencyValue(ACTIONS.UnhibernateSnail, 0);
    }

    // e.
    private void SetValue_PlaySnailCard()
    {
        CPlayer currentPlayer = CGameManager.instance.activePlayer;

        // if current player has no snails in play, play a snail
        if (currentPlayer.snails.Count == 0)
        {
            foreach(CBaseCard baseCard in currentPlayer.hand)
            {
                if(baseCard.cardType == CardData.CardType.Snail) {
                    //snailCardToPlay = (CSnailCard)baseCard;
                    SetFrequencyValue(ACTIONS.PlaySnail, 2);
                    break;
                }
            }
        }
        else
        {
            // else if there are snails in play and a snail in the player's hand can eat a prey, play that snail card.
            foreach (CSnailCard snailCard in currentPlayer.snails)
            {
                foreach (CPreyCard preyCard in currentPlayer.prey) {
                    if (preyCard.preyName != CardData.PreyName.Basic_Prey) {
                        if(snailCard.fedState == CardData.FedState.Unfed) {
                            if (snailCard.preyType == CardData.PreyType.All || snailCard.preyType == preyCard.preyType) {
                                //Debug.Log(currentPlayer.hand.Count);
                                if (snailCard.strength + currentPlayer.hand.Count >= preyCard.resistance) {
                                    //snailCardToFeed = snailCard;
                                    SetFrequencyValue(ACTIONS.PlaySnail, 2);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // f.
    private void SetValue_BuyMarketCard()
    {
        CMarketPanel market = CUIManager.instance.marketPanel;
        int cheapest = 5; // 5 > max cost of card

        foreach (CBaseCard marketCard in market.lstMarketCards)
            if (marketCard.cost < cheapest)
                cheapest = marketCard.cost;

        // Check # of cards in hand with cheapest market card
        if (CGameManager.instance.activePlayer.hand.Count >= cheapest)
            SetFrequencyValue(ACTIONS.BuyFromMarket, 2);
    }

    // g.
    private void SetValue_FeedUnfedSnailCardWithZeroPeptides()
    {
        CSnailCard snailCardWithZeroPeptides = CGameManager.instance.activePlayer.HasSnailWithZeroPeptides();
        if (snailCardWithZeroPeptides && snailCardWithZeroPeptides.fedState == CardData.FedState.Unfed) {
            snailCardToFeedWithoutPeptides = snailCardWithZeroPeptides;
            SetFrequencyValue(ACTIONS.FeedSnail_ZeroPeptides, 1);
        }
    }

    #endregion


    public IEnumerator RunAction()
    {
        yield return new WaitForEndOfFrame();
        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;

        switch(FinalDecision)
        {
            case ACTIONS.UseInstant_Research:
                yield return RunAction_UseInstantResearchCard();
                break;

            case ACTIONS.UsePredator: 
                break;

            case ACTIONS.PlaySnail:
                yield return RunAction_PlaySnail();
                break;

            case ACTIONS.FeedSnail_WithPeptides:
                yield return RunAction_FeedSnail(snailCardToFeedWithPeptides);
                break;

            case ACTIONS.FeedSnail_ZeroPeptides:
                yield return RunAction_FeedSnail(snailCardToFeedWithoutPeptides);
                break;

            case ACTIONS.UnhibernateSnail:
                yield return RunAction_UnhibernateSnail();
                break;

            case ACTIONS.BuyFromMarket:
                yield return RunAction_BuyFromMarket();
                break;
        }
    }

    #region Run Action Functions

    public IEnumerator RunAction_BuyFromMarket()
    {
        yield return new WaitForEndOfFrame();

        // rank from highest attack value, then better snails
        // AI discards snails first, then instants low to high atk value. 
        // OW treated as atk value = 2. Research treated as atk value = 2 until no hidden peptides left

        CMarketPanel market = CUIManager.instance.marketPanel;
        CBaseCard marketCardToBuy = null;

        #region Filter by highest attack value

        foreach (CBaseCard marketCard in market.lstMarketCards)
        {
            if (marketCardToBuy == null) {
                marketCardToBuy = marketCard;
                continue;
            }

            CInstantCard instant = null;
            int attkRating = marketCard.attackRating;
            if(marketCard.cardType == CardData.CardType.Instant)
            {
                instant = (CInstantCard)marketCard;
                if(instant.instantType == CardData.InstantType.Ocean_Waves)
                {
                    attkRating = 2;
                }
                else if(instant.instantType == CardData.InstantType.Research)
                {
                    if (lstHiddenPeptides.Count > 0)
                        attkRating = 2;
                    else
                        attkRating = 0;
                }
            }

            if (attkRating > marketCardToBuy.attackRating)
                marketCardToBuy = marketCard;
        }

        #endregion


        #region Filter by better snails



        #endregion


        // discard
        List<CBaseCard> discards = new List<CBaseCard>();
        List<CBaseCard> snailsInHand = new List<CBaseCard>();
        List<CBaseCard> instantsInHand = new List<CBaseCard>();

        // Get snails and instants in two separate lists
        foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand)
        {
            if (handCard.cardType == CardData.CardType.Instant)
                instantsInHand.Add(handCard);
            else if (handCard.cardType == CardData.CardType.Snail)
                snailsInHand.Add(handCard);
        }

        // Add snails to discards list
        foreach (CBaseCard s in snailsInHand)
        {
            if (discards.Count < marketCardToBuy.cost)
                discards.Add(s);
        }

        // if more cards needed, discard instants
        if(discards.Count < marketCardToBuy.cost)
        {
            foreach (CBaseCard i in instantsInHand)
            {
                if (discards.Count < marketCardToBuy.cost)
                    discards.Add(i);
            }
        }

        // if more cards needed, there are no more.
        // Buy card
        if(discards.Count >= marketCardToBuy.cost)
        {
            market.ViewMarket();

            foreach (CBaseCard discard in discards) {
                yield return new WaitForSeconds(0.75f);
                discard.SelectedCardToSell();
            }

            yield return new WaitForSeconds(1.0f);
            yield return market.BuyCard_CR(marketCardToBuy);
        }
    }

    public IEnumerator RunAction_FeedSnail(CSnailCard snailCardToFeed)
    {
        yield return new WaitForEndOfFrame();

        CPlayer currentPlayer = CGameManager.instance.activePlayer;
        preyCardToEat = null;

        // Check if any prey match our snail
        foreach (CPreyCard preyCard in currentPlayer.prey)
        {
            if (preyCard.preyName != CardData.PreyName.Basic_Prey)
            {
                if (snailCardToFeed.fedState == CardData.FedState.Unfed)
                {
                    if (snailCardToFeed.preyType == CardData.PreyType.All || snailCardToFeed.preyType == preyCard.preyType)
                    {
                        if (snailCardToFeed.strength + currentPlayer.hand.Count >= preyCard.resistance)
                        {
                            preyCardToEat = preyCard;
                            break;
                        }
                    }
                }
            }
        }

        if (preyCardToEat == null)
        {

        }

    }

    public IEnumerator RunAction_FeedSnail_WithPeptides()
    {
        yield return new WaitForEndOfFrame();

    }

    public IEnumerator RunAction_FeedSnail_ZeroPeptides()
    {
        yield return new WaitForEndOfFrame();




        //CPlayer currentPlayer = CGameManager.instance.activePlayer;
        //preyCardToEat = null;

        //foreach (CSnailCard snailCard in currentPlayer.snails)
        //{
        //    foreach (CPreyCard preyCard in currentPlayer.prey) {
        //        if (preyCard.preyName != CardData.PreyName.Basic_Prey) {
        //            if(snailCard.fedState == CardData.FedState.Unfed) {
        //                if (snailCard.preyType == CardData.PreyType.All || snailCard.preyType == preyCard.preyType) {
        //                    if (snailCard.strength + currentPlayer.hand.Count >= preyCard.resistance) {
        //                        preyCardToEat = preyCard;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ////if(preyCardToEat == null)
        ////{

        ////}





        //// Feed Snail Card
        //yield return CUIManager.instance.MoveCardToActionPanel_CR(snailCardToFeedWithoutPeptides);
        //yield return new WaitForSeconds(1f);

        //yield return CUIManager.instance.actionPanel.CR_FeedCard();
        //yield return new WaitForSeconds(1f);

        // Choose Prey

        //yield return CUIManager.instance.actionPanel.ActionEndPrompt.MovePreyCardToPanel
    }

    public IEnumerator RunAction_PlaySnail()
    {
        yield return new WaitForEndOfFrame();
        
        // AI always plays the snail that matches the prey in front with the highest strength first,
        // then lower strength, then snails that dont match. if tie, choose randomly.

        snailCardToPlay = null;
        foreach(CPreyCard preyCard in CGameManager.instance.activePlayer.prey)
        {
            if (preyCard.preyName == CardData.PreyName.Basic_Prey)
                continue;

            bool preyMatch = false;
            foreach (CSnailCard snailCard in CGameManager.instance.activePlayer.hand)
            {
                if (snailCard.preyType == preyCard.preyType || snailCard.preyType == CardData.PreyType.All)
                {
                    preyMatch = true;

                    if (snailCardToPlay == null)    snailCardToPlay = snailCard;
                    else
                    {
                        if (snailCard.strength > snailCardToPlay.strength)
                            snailCardToPlay = snailCard;
                    }
                }
            }

            // if no match, choose strongest snail
            if(preyMatch == false)
            {
                foreach (CSnailCard snailCard in CGameManager.instance.activePlayer.hand)
                {
                    if (snailCardToPlay == null)    snailCardToPlay = snailCard;
                    else
                    {
                        if (snailCard.strength > snailCardToPlay.strength)
                            snailCardToPlay = snailCard;
                    }
                }
            }
        }

        // Play Snail Card
        yield return CUIManager.instance.MoveCardToActionPanel_CR(snailCardToPlay);
        yield return new WaitForSeconds(1f);

        yield return CUIManager.instance.actionPanel.UseCard_CR();
        yield return new WaitForSeconds(1f);
    }


    public IEnumerator RunAction_UnhibernateSnail()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiManager = CUIManager.instance;

        yield return CUIManager.instance.MoveCardToActionPanel_CR(snailCardToUnhibernate);
        yield return new WaitForSeconds(1f);

        uiManager.actionPanel.UnhibernateButton();
        yield return new WaitForSeconds(1f);


        CBaseCard discard = null;

        if (CGameManager.instance.activePlayer.hand.Count > 0)
            discard = CGameManager.instance.activePlayer.hand[0];

        discard.SelectedCardToUnhibernateSnail();
        yield return new WaitForSeconds(1.0f);

        uiManager.actionPanel.UnhibernateSnail();
        yield return new WaitForSeconds(2.0f);
    }

    public IEnumerator RunAction_UseInstantResearchCard()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiManager = CUIManager.instance;

        // Peek at a peptide in any cabal
        yield return CUIManager.instance.MoveCardToActionPanel_CR(instantCardToPlay);
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(uiManager.actionPanel.ExpandActionPanelBackground(false));
        CCabalManager.instance.SetButtonFunctionTargetForResearchCard();

        uiManager.actionPanel.InstructionText.ShowTopHeader("Peek at any peptide in the left-hand panel");

		CGlobals.TweenMove(instantCardToPlay.gameObject, "position", new Vector3(180f, 320f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(instantCardToPlay.gameObject, Vector3.one * 0.8f, 0.5f, iTween.EaseType.easeOutSine, true);

        yield return new WaitForSeconds(0.25f);
		uiManager.cabalPanel.anchorPanelBackground.transform.parent = uiManager.actionPanel.goActionContainer.transform;
        CGlobals.UpdateWidgets();

        yield return new WaitForSeconds(0.25f);
        CCabalManager.instance.EnableButtons();


        yield return new WaitForSeconds(1f);
        CCabalManager.instance.PeekAtPeptide(lstHiddenPeptides[0].GetComponent<UISprite>());
    }

    public IEnumerator RunAction_UsePredator()
    {
        yield return new WaitForEndOfFrame();

    }

    #endregion








}
