using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPeptidesForPresentation : MonoBehaviour
{
    public enum STATE { Idle, SelectPeptide, CheckForMatch };
    public STATE state;

    public GameObject goButtons;
    public Grid gridSelectedPeptides;
    public UISprite uiAlpha, uiDelta, uiMu, uiOmega;
    public UILabel uiText;

    public PlayerData.PLAYER currentPlayer;
    private List<CPeptide> lstSelectedPeptides;
    private List<CPlayer> lstPlayersWithSnails;

    // Used when player has more than 1 snail
    private CPlayer playerToAddPeptide;
    private CPeptide peptideToAdd;



    ///////////////////////////////////////////////////////////////////
    // * TODO * TODO * TODO * TODO
    ///////////////////////////////////////////////////////////////////
    //
    // Presentation card currently pics first snail for opponents
    // Implementation needed!
    //
    ///////////////////////////////////////////////////////////////////



	void Start ()
    {
        state = STATE.Idle;
        lstSelectedPeptides = new List<CPeptide>();
        lstPlayersWithSnails = new List<CPlayer>();
	}
    
    
    #region ChoosePeptides

    public void ChooseAlphaForPresentation()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Alpha);
        peptide.transform.parent = uiAlpha.transform;
        peptide.transform.localPosition = Vector3.zero;
        peptide.transform.localScale = Vector3.one;
        peptide.owner = currentPlayer;

        lstSelectedPeptides.Add(peptide);
        StartCoroutine(SendToPosition(peptide));
    }

    public void ChooseDeltaForPresentation()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Delta);
        peptide.transform.parent = uiDelta.transform;
        peptide.transform.localPosition = Vector3.zero;
        peptide.transform.localScale = Vector3.one;
        peptide.owner = currentPlayer;

        lstSelectedPeptides.Add(peptide);
        StartCoroutine(SendToPosition(peptide));
	}

    public void ChooseMuForPresentation()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Mu);
        peptide.transform.parent = uiMu.transform;
        peptide.transform.localPosition = Vector3.zero;
        peptide.transform.localScale = Vector3.one;
        peptide.owner = currentPlayer;

        lstSelectedPeptides.Add(peptide);
        StartCoroutine(SendToPosition(peptide));
    }

    public void ChooseOmegaForPresentation()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Omega);
        peptide.transform.parent = uiOmega.transform;
        peptide.transform.localPosition = Vector3.zero;
        peptide.transform.localScale = Vector3.one;
        peptide.owner = currentPlayer;

        lstSelectedPeptides.Add(peptide);
        StartCoroutine(SendToPosition(peptide));
	}

    #endregion

    // Move peptides to 
    private IEnumerator SendToPosition(CPeptide peptide)
    {
        int selectedPeptides = lstSelectedPeptides.Count;
        peptide.transform.parent = gridSelectedPeptides.transform;
        gridSelectedPeptides.mv2GridDimensions.x = 250f * selectedPeptides;

        if (selectedPeptides == 1)  gridSelectedPeptides.mnBorder = 50;
        else if (selectedPeptides == 2)  gridSelectedPeptides.mnBorder = 67;
        else if (selectedPeptides == 3)  gridSelectedPeptides.mnBorder = 75;
        else if (selectedPeptides == 4)  gridSelectedPeptides.mnBorder = 80;

        gridSelectedPeptides.UpdateGrid();
        yield return new WaitForSeconds(0.5f);

        peptide.ChangeBorderColor(CGlobals.GetPlayerColor(currentPlayer), 0.5f);
        yield return new WaitForSeconds(0.5f);

        if (selectedPeptides >= CGameManager.instance.numPlayers)
        {
            StartCoroutine(MoveCabalPanelToActionPanel());
        }
        else
        {
            lstPlayersWithSnails.Remove(CGameManager.instance.Players[(int)currentPlayer]);

            if (lstPlayersWithSnails.Count > 0)
                currentPlayer = lstPlayersWithSnails[0].player;
            else
                StartCoroutine(MoveCabalPanelToActionPanel());
        }
    }


    public IEnumerator MoveCabalPanelToActionPanel()
    {
        CGlobals.TweenMove(gridSelectedPeptides.gameObject, "y", -580, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return StartCoroutine(HidePanel());
        yield return new WaitForSeconds(0.5f);

        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
        CGlobals.UpdateWidgets();
        yield return new WaitForSeconds(0.25f);

        CCabalManager.instance.SetButtonFunctionTargetForPresentationCard();
        CCabalManager.instance.EnableButtons();
    }

    public IEnumerator MoveCabalPanelBack()
    {
        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
        CGlobals.UpdateWidgets();
        yield return new WaitForSeconds(0.25f);
        CCabalManager.instance.EnableButtons(false);
    }


    public void PeekAtPeptide(UISprite current) { StartCoroutine(PeekAtPeptide_CR(current)); }
    public IEnumerator PeekAtPeptide_CR(UISprite current)
    {
        CCabalManager.instance.EnableButtons(false);
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        current.GetComponent<TweenScale>().enabled = false;
        CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

        CUIManager.instance.DisableAllCollidersForEndTurn();
        yield return new WaitForSeconds(0.75f);

        current.enabled = false;
        CPeptide peekedPeptide = current.GetComponent<CPeptide>();//CCabalManager.instance.GetSelectedPeptide(current);
        UISprite sprite = peekedPeptide.GetComponent<UISprite>();
        sprite.enabled = true;
        CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);
        yield return new WaitForSeconds(1f);

        StartCoroutine(CheckForMatches(peekedPeptide));
    }

    private IEnumerator CheckForMatches(CPeptide peekedPeptide)
    {
        currentPlayer = CGameManager.instance.activePlayer.player;
        //nextPlayer = CGlobals.GetNextPlayer(currentPlayer);

        // if peptides dont match, add them to temp list.
        List<CPeptide> temp = new List<CPeptide>();
        foreach(CPeptide pep in lstSelectedPeptides)
        {
            if(pep.peptide != peekedPeptide.peptide)
            {
                // Scale Animation
                CGlobals.TweenScale(pep.gameObject, pep.transform.localScale + (Vector3.one * 0.1f), 0.3f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.3f);

                CGlobals.TweenScale(pep.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.15f);

                // Add to temp list
                pep.transform.parent = this.transform;
                temp.Add(pep);

                // Setup grid variables
                int selectedPeptides = lstSelectedPeptides.Count;
                int newSelectedPeptides = selectedPeptides - temp.Count;
                gridSelectedPeptides.mv2GridDimensions.x = 250f * newSelectedPeptides;

                if (newSelectedPeptides == 1)  gridSelectedPeptides.mnBorder = 50;
                else if (newSelectedPeptides == 2)  gridSelectedPeptides.mnBorder = 67;
                else if (newSelectedPeptides == 3)  gridSelectedPeptides.mnBorder = 75;
                else if (newSelectedPeptides == 4)  gridSelectedPeptides.mnBorder = 80;

                gridSelectedPeptides.UpdateGrid();
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        // Destroy peptides in temp list
        foreach (CPeptide destroyPep in temp)
        for(int i = 0; i < temp.Count; ++i) {
            lstSelectedPeptides.Remove(temp[i]);
                Destroy(temp[i].gameObject);
        }
        temp.Clear();

        // Use temp list so we can remove from "lstSelectedPeptides" in real time while travesing through list
        foreach(CPeptide pep in lstSelectedPeptides)
        {
            temp.Add(pep);
        }

        bool multipleOpponentSnails = false;
        bool bSelectionsNeeded = false;
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        foreach(CPeptide pep in temp)
        {
            if(pep.owner == activePlayer.player)
            {
                CGlobals.TweenMove(gridSelectedPeptides.gameObject, "y", -580, 0.5f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.5f);

                if(activePlayer.snails.Count == 1)
                {
                    CSnailCard snailCard = (CSnailCard)activePlayer.snails[0];
                    StartCoroutine(HidePanel());
                    yield return StartCoroutine(SendPeptideToSnailCard(snailCard, pep));
		            yield return new WaitForSeconds(1f);
                }
                else
                {
                    bSelectionsNeeded = true;
                    CGlobals.TweenMove(gridSelectedPeptides.gameObject, "y", -500f, 0.5f, iTween.EaseType.easeOutSine, true);
                    CUIManager.instance.activePlayerPanel.gridSnails.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;

                    foreach (CSnailCard snailCard in CGameManager.instance.activePlayer.snails) {
                        snailCard.EnableButton();
                        CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.Button, "ngui_SendPeptideToSnailCard");
                    }

                    playerToAddPeptide = CGameManager.instance.activePlayer;
                    peptideToAdd = pep;
                    CGlobals.UpdateWidgets();
                }
            }
            else
            {
                CPlayer player = CGameManager.instance.Players[(int)pep.owner];

                if(player.snails.Count == 1)
                {
                    CSnailCard snailCard = (CSnailCard)player.snails[0];
                    COpponentSnail oppSnail = snailCard.opponentSnail;
                    yield return StartCoroutine(SendPeptideToOpponentSnail(snailCard, pep));
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    multipleOpponentSnails = true;

                    //CGlobals.TweenMove(gridSelectedPeptides.gameObject, "position", Vector3.up * -800f, 0.5f, iTween.EaseType.easeOutSine, true);
                    
                    //// Setup grid variables
                    //int selectedPeptides = lstSelectedPeptides.Count;
                    //int newSelectedPeptides = selectedPeptides - temp.Count;
                    //gridSelectedPeptides.mv2GridDimensions.x = 250f * newSelectedPeptides;

                    //if (newSelectedPeptides == 1)  gridSelectedPeptides.mnBorder = 50;
                    //else if (newSelectedPeptides == 2)  gridSelectedPeptides.mnBorder = 67;
                    //else if (newSelectedPeptides == 3)  gridSelectedPeptides.mnBorder = 75;
                    //else if (newSelectedPeptides == 4)  gridSelectedPeptides.mnBorder = 80;

                    //gridSelectedPeptides.UpdateGrid();


                    yield return new WaitForSeconds(0.5f);

                    foreach(CSnailCard snailCard in player.snails)
                        CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.opponentSnail.Button, "ngui_SendPeptideToOpponentSnail");

                    //COpponentsPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
                    //playerToAddPeptide = CGameManager.instance.activePlayer;
                    peptideToAdd = pep;
                    CGlobals.UpdateWidgets();
                }
            }
        }

        temp.Clear();

        if (bSelectionsNeeded) {
            yield return new WaitForSeconds(1f);
        }
        else if(!multipleOpponentSnails) {
            StartCoroutine(MoveCabalPanelBack());
            activePlayer.hand.Remove(CUIManager.instance.SelectedCard);
            activePlayer.discard.Add(CUIManager.instance.SelectedCard);
            yield return StartCoroutine(CUIManager.instance.activePlayerPanel.DiscardCard(CUIManager.instance.SelectedCard));
            yield return StartCoroutine(CGameManager.instance.EndTurn());
        }
    }

    public void ngui_SendPeptideToSnailCard(CSnailCard snailCard) { StartCoroutine(ngui_SendPeptideToSnailCard_CR(snailCard)); }
    public IEnumerator ngui_SendPeptideToSnailCard_CR(CSnailCard snailCard)
    {
        StartCoroutine(HidePanel());
        yield return StartCoroutine(SendPeptideToSnailCard(snailCard, peptideToAdd));

        // Reset snail cards
        CUIManager.instance.activePlayerPanel.gridSnails.transform.parent = CActivePlayerPanel.instance.uiPanelSnails.transform;
        foreach (CSnailCard card in CGameManager.instance.activePlayer.snails)
        {
            card.EnableButton(false);
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, snailCard, snailCard.Button, "MoveCardToActionPanel");
        }

        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        CGameManager.instance.activePlayer.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.lblHandCount.text = CGameManager.instance.activePlayer.hand.Count.ToString();

        yield return new WaitForSeconds(1f);

        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
        CGlobals.UpdateWidgets();
        //lstSelectedPeptides.Clear();

        // if there are no more selected peptides, end the turn
        lstSelectedPeptides.Remove(peptideToAdd);
        if(lstSelectedPeptides.Count == 0)
        {
            yield return StartCoroutine(FinishTurn());
        }
        else
        {
            CGlobals.TweenMove(gridSelectedPeptides.gameObject, "position", Vector3.up * -800f, 0.5f, iTween.EaseType.easeOutSine, true);

            // Setup grid variables
            int selectedPeptides = lstSelectedPeptides.Count;
            gridSelectedPeptides.mv2GridDimensions.x = 250f * selectedPeptides;

            if (selectedPeptides == 1) gridSelectedPeptides.mnBorder = 50;
            else if (selectedPeptides == 2) gridSelectedPeptides.mnBorder = 67;
            else if (selectedPeptides == 3) gridSelectedPeptides.mnBorder = 75;
            else if (selectedPeptides == 4) gridSelectedPeptides.mnBorder = 80;

            gridSelectedPeptides.UpdateGrid();

            COpponentsPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
            CGlobals.UpdateWidgets();
        }
    }


    public void ngui_SendPeptideToOpponentSnail(CSnailCard snailCard) { StartCoroutine(ngui_SendPeptideToOpponentSnail_CR(snailCard)); }
    public IEnumerator ngui_SendPeptideToOpponentSnail_CR(CSnailCard snailCard)
    {
        StartCoroutine(HidePanel());
        CPeptide selectedPeptide = lstSelectedPeptides[0];
        yield return StartCoroutine(SendPeptideToOpponentSnail(snailCard, peptideToAdd));

        // Reset snail cards
        CUIManager.instance.activePlayerPanel.gridSnails.transform.parent = CActivePlayerPanel.instance.uiPanelSnails.transform;
        foreach (CSnailCard card in CGameManager.instance.activePlayer.snails)
        {
            card.EnableButton(false);
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, snailCard, snailCard.Button, "MoveCardToActionPanel");
        }

        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        //StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        CGameManager.instance.activePlayer.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.lblHandCount.text = CGameManager.instance.activePlayer.hand.Count.ToString();

        yield return new WaitForSeconds(1f);

        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
        CGlobals.UpdateWidgets();

        // if there are no more selected peptides, end the turn
        lstSelectedPeptides.Remove(selectedPeptide);
        if(lstSelectedPeptides.Count == 0)
        {
            yield return StartCoroutine(FinishTurn());
        }
        else
        {
            foreach(CPlayer player in CGameManager.instance.Players) {
                foreach (CSnailCard snail in player.snails)
                    snail.opponentSnail.EnableButton(false);
            }

            CPlayer nextPlayer = CGameManager.instance.Players[(int)lstSelectedPeptides[0].owner];
            foreach (CSnailCard snail in nextPlayer.snails)
                    snail.opponentSnail.EnableButton(true);


            CGlobals.TweenMove(gridSelectedPeptides.gameObject, "position", Vector3.up * -800f, 0.5f, iTween.EaseType.easeOutSine, true);

            // Setup grid variables
            int selectedPeptides = lstSelectedPeptides.Count;
            gridSelectedPeptides.mv2GridDimensions.x = 250f * selectedPeptides;

            if (selectedPeptides == 1) gridSelectedPeptides.mnBorder = 50;
            else if (selectedPeptides == 2) gridSelectedPeptides.mnBorder = 67;
            else if (selectedPeptides == 3) gridSelectedPeptides.mnBorder = 75;
            else if (selectedPeptides == 4) gridSelectedPeptides.mnBorder = 80;

            gridSelectedPeptides.UpdateGrid();

            COpponentsPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
            CGlobals.UpdateWidgets();
        }
    }

    public IEnumerator SendPeptideToSnailCard(CSnailCard snailCard, CPeptide selectedPeptide)
    {
        int index = 0;
		for (index = 0; index < snailCard.lstPeptideRows.Count; ++index)
			if (snailCard.lstPeptideRows[index].peptideType == selectedPeptide.peptide)
            {
				selectedPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
                selectedPeptide.SetWidgetDepth(63);
            	selectedPeptide.Enable();
				selectedPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;

				snailCard.lstPeptideRows[index].gridRow.transform.parent = snailCard.gridPeptides.transform;
				snailCard.lstContainedPeptides.Add(selectedPeptide.peptide);
				yield return new WaitForEndOfFrame();

                CGlobals.UpdateWidgets();
				snailCard.gridPeptides.UpdateGrid();
				snailCard.lstPeptideRows[index].gridRow.UpdateGrid();
                break;
            }

        // If already on card, no need to instantiate another grid
		bool bAlreadyOnCard = (index == snailCard.lstPeptideRows.Count) ? false : true;
        if (bAlreadyOnCard) {
            yield return StartCoroutine(snailCard.opponentSnail.AddPeptide(selectedPeptide.peptide));
            yield break;
        }

		GameObject goPeptideGridRow = Resources.Load("Peptides/PeptideGridRow") as GameObject;
		Grid newPeptideGrid = ((GameObject)Instantiate(goPeptideGridRow, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
        newPeptideGrid.mv2GridDimensions.x = 100;
        newPeptideGrid.mv2CellMaxDimensions.x = 100;

        tPeptideRow newRow;
        newRow.gridRow = newPeptideGrid;
		newRow.peptideType = selectedPeptide.peptide;
		newRow.gridRow.transform.parent = snailCard.gridPeptides.transform;
		selectedPeptide.transform.parent = newRow.gridRow.transform;
        newRow.gridRow.transform.localScale = Vector3.one * 4f;
                
        selectedPeptide.SetWidgetDepth(63);
		snailCard.lstPeptideRows.Add(newRow);
		snailCard.lstContainedPeptides.Add(selectedPeptide.peptide);

		selectedPeptide.Enable();
        CGlobals.UpdateWidgets();

		snailCard.gridPeptides.UpdateGrid();
		snailCard.lstPeptideRows[index].gridRow.UpdateGrid();

		yield return StartCoroutine(snailCard.opponentSnail.AddPeptide(selectedPeptide.peptide));

        // if there are no more selected peptides, end the turn
        lstSelectedPeptides.Remove(selectedPeptide);
        if(lstSelectedPeptides.Count == 0)
        {
            yield return StartCoroutine(FinishTurn());
        }
        else
        {
            foreach(CPlayer player in CGameManager.instance.Players)
            {
                foreach (CSnailCard snail in player.snails)
                    snail.opponentSnail.EnableButton(false);
            }

            CPlayer nextPlayer = CGameManager.instance.Players[(int)lstSelectedPeptides[0].owner];
            foreach (CSnailCard snail in nextPlayer.snails)
                    snail.opponentSnail.EnableButton(true);


            CGlobals.TweenMove(gridSelectedPeptides.gameObject, "position", Vector3.up * -800f, 0.5f, iTween.EaseType.easeOutSine, true);

            // Setup grid variables
            int selectedPeptides = lstSelectedPeptides.Count;
            gridSelectedPeptides.mv2GridDimensions.x = 250f * selectedPeptides;

            if (selectedPeptides == 1) gridSelectedPeptides.mnBorder = 50;
            else if (selectedPeptides == 2) gridSelectedPeptides.mnBorder = 67;
            else if (selectedPeptides == 3) gridSelectedPeptides.mnBorder = 75;
            else if (selectedPeptides == 4) gridSelectedPeptides.mnBorder = 80;

            gridSelectedPeptides.UpdateGrid();

            COpponentsPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
            CGlobals.UpdateWidgets();
        }

    }

    public IEnumerator SendPeptideToOpponentSnail(CSnailCard snailCard, CPeptide selectedPeptide)
    {
        yield return StartCoroutine(snailCard.opponentSnail.AddPeptide(selectedPeptide));

        CPeptide newPeptide = CCabalManager.instance.CreatePeptide(selectedPeptide.peptide, false, Vector3.up * 1000f);
        int index = 0;
		for (index = 0; index < snailCard.lstPeptideRows.Count; ++index)
			if (snailCard.lstPeptideRows[index].peptideType == newPeptide.peptide)
            {
				newPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
                newPeptide.SetWidgetDepth(63);
            	newPeptide.Enable();
				newPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
				snailCard.lstPeptideRows[index].gridRow.transform.parent = snailCard.gridPeptides.transform;
				snailCard.lstContainedPeptides.Add(newPeptide.peptide);
				yield return new WaitForEndOfFrame();

                CGlobals.UpdateWidgets();
				snailCard.gridPeptides.UpdateGrid();
				snailCard.lstPeptideRows[index].gridRow.UpdateGrid();
                break;
            }

        // If already on card, no need to instantiate another grid
		bool bAlreadyOnCard = (index == snailCard.lstPeptideRows.Count) ? false : true;
        if (bAlreadyOnCard)
            yield break;

		GameObject goPeptideGridRow = Resources.Load("Peptides/PeptideGridRow") as GameObject;
		Grid newPeptideGrid = ((GameObject)Instantiate(goPeptideGridRow, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
        newPeptideGrid.mv2GridDimensions.x = 100;
        newPeptideGrid.mv2CellMaxDimensions.x = 100;

        tPeptideRow newRow;
        newRow.gridRow = newPeptideGrid;
		newRow.peptideType = newPeptide.peptide;
		newRow.gridRow.transform.parent = snailCard.gridPeptides.transform;
		newPeptide.transform.parent = newRow.gridRow.transform;
        newRow.gridRow.transform.localScale = Vector3.one * 4f;
                
        newPeptide.SetWidgetDepth(63);
		snailCard.lstPeptideRows.Add(newRow);
		snailCard.lstContainedPeptides.Add(newPeptide.peptide);

		newPeptide.Enable();
        CGlobals.UpdateWidgets();

		snailCard.gridPeptides.UpdateGrid();
		snailCard.lstPeptideRows[index].gridRow.UpdateGrid();

        // if there are no more selected peptides, end the turn
        lstSelectedPeptides.Remove(selectedPeptide);
        if(lstSelectedPeptides.Count == 0)
        {
            yield return StartCoroutine(FinishTurn());
        }
        else
        {
            foreach(CPlayer player in CGameManager.instance.Players) {
                foreach (CSnailCard snail in player.snails)
                    snail.opponentSnail.EnableButton(false);
            }

            CPlayer nextPlayer = CGameManager.instance.Players[(int)lstSelectedPeptides[0].owner];
            foreach (CSnailCard snail in nextPlayer.snails)
                    snail.opponentSnail.EnableButton(true);


            CGlobals.TweenMove(gridSelectedPeptides.gameObject, "position", Vector3.up * -800f, 0.5f, iTween.EaseType.easeOutSine, true);

            // Setup grid variables
            int selectedPeptides = lstSelectedPeptides.Count;
            gridSelectedPeptides.mv2GridDimensions.x = 250f * selectedPeptides;

            if (selectedPeptides == 1) gridSelectedPeptides.mnBorder = 50;
            else if (selectedPeptides == 2) gridSelectedPeptides.mnBorder = 67;
            else if (selectedPeptides == 3) gridSelectedPeptides.mnBorder = 75;
            else if (selectedPeptides == 4) gridSelectedPeptides.mnBorder = 80;

            gridSelectedPeptides.UpdateGrid();

            COpponentsPanel.instance.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
            CGlobals.UpdateWidgets();
        }
    }

    public IEnumerator ShowPanel() {
        lstSelectedPeptides.Clear();
		#if !UNITY_IOS
        CGlobals.TweenMove(this.gameObject, "y", 400f, 0.5f, iTween.EaseType.easeOutSine, true);
		#else
		CGlobals.TweenMove(this.gameObject, "y", 500f, 0.5f, iTween.EaseType.easeOutSine, true);
		#endif
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator HidePanel() {
        CGlobals.TweenMove(this.gameObject, "y", 800f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(goButtons, "y", 0f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator FinishTurn()
    {
        COpponentsPanel.instance.anchorPanelBackground.transform.parent = COpponentsPanel.instance.transform;
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    public void PopulateListOfPlayersWithSnails()
    {
        CGameManager gameMan = CGameManager.instance;
        lstPlayersWithSnails.Add(gameMan.activePlayer);

        int nPlayers = 0;
        for (PlayerData.PLAYER plyr = currentPlayer; plyr < (PlayerData.PLAYER)gameMan.numPlayers; plyr = CGlobals.GetNextPlayer(plyr))
        {
            int i = (int)plyr;

            if (gameMan.Players[i] != gameMan.activePlayer)
                if(gameMan.Players[i].snails.Count > 0)
                    lstPlayersWithSnails.Add(gameMan.Players[i]);

            nPlayers++;
            if (nPlayers >= gameMan.numPlayers)
                break;
        }
    }

}
