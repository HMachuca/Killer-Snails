using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCabalSolvingPanel : MonoBehaviour
{
    // 1025

    public UISprite uiFadeSprite;
    public UISprite[] uiPeptides;
    public UIButton solveCabalButton;
    public Grid gridCards;
    public int nActiveCabal;

    public List<CardData.PeptideType> lstPeptidesContainedInSnails;

    public List<PlayerData.PLAYER> lstPlayersSolvedCabalOne;
    public List<PlayerData.PLAYER> lstPlayersSolvedCabalTwo;
    public List<PlayerData.PLAYER> lstPlayersSolvedCabalThree;

    public AudioClip acSolveCabal;
    public AudioClip acClosePanel;

	void Start ()
    {
        nActiveCabal = 0;
        lstPlayersSolvedCabalOne = new List<PlayerData.PLAYER>();
        lstPlayersSolvedCabalTwo = new List<PlayerData.PLAYER>();
        lstPlayersSolvedCabalThree = new List<PlayerData.PLAYER>();
        lstPeptidesContainedInSnails = new List<CardData.PeptideType>();
    }
	
    public void SetAsCabalOne() { StartCoroutine(SetAsCabalOne_CR()); }
    public IEnumerator SetAsCabalOne_CR()
    {
        CCabalManager cabalMan = CCabalManager.instance;
        nActiveCabal = 0;
        yield return StartCoroutine(ShrinkSprites());

        for(int i = 0; i < 3; ++i)
        {
            CardData.PeptideType peptideType = cabalMan.FirstCabal[i];
            CPlayer activePlayer = CGameManager.instance.activePlayer;
            uiPeptides[i].spriteName = "random";// peptideType.ToString().ToLower();

            if(i == 0) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInFirstCabalFirstPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 1) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInFirstCabalSecondPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 2) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInFirstCabalThirdPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
        }

        yield return StartCoroutine(ExpandSprites());
    }

    public void SetAsCabalTwo() { StartCoroutine(SetAsCabalTwo_CR()); }
    public IEnumerator SetAsCabalTwo_CR()
    {
        CCabalManager cabalMan = CCabalManager.instance;
        nActiveCabal = 1;
        yield return StartCoroutine(ShrinkSprites());

        for(int i = 0; i < 3; ++i)
        {
            CardData.PeptideType peptideType = cabalMan.SecondCabal[i];
            CPlayer activePlayer = CGameManager.instance.activePlayer;
            uiPeptides[i].spriteName = "random";// peptideType.ToString().ToLower();

            if(i == 0) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInSecondCabalFirstPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 1) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInSecondCabalSecondPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 2) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInSecondCabalThirdPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
        }

        yield return StartCoroutine(ExpandSprites());
    }

    public void SetAsCabalThree() { StartCoroutine(SetAsCabalThree_CR()); }
    public IEnumerator SetAsCabalThree_CR()
    {
        CCabalManager cabalMan = CCabalManager.instance;
        nActiveCabal = 2;
        yield return StartCoroutine(ShrinkSprites());

        for(int i = 0; i < 3; ++i)
        {
            CardData.PeptideType peptideType = cabalMan.ThirdCabal[i];
            CPlayer activePlayer = CGameManager.instance.activePlayer;
            uiPeptides[i].spriteName = "random";// peptideType.ToString().ToLower();

            if(i == 0) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInThirdCabalFirstPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 1) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInThirdCabalSecondPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
            else if(i == 2) {
            foreach (CPeptide peptide in activePlayer.lstRevealedPeptides)
                if (cabalMan.IsInThirdCabalThirdPeptide(peptide.transform))
                    uiPeptides[i].spriteName = peptide.PeptideSprite.spriteName;
            }
        }

        yield return StartCoroutine(ExpandSprites());
    }




    public void AddPeptidesForSolving(CSnailCard snailCard) { StartCoroutine(AddPeptidesForSolving_CR(snailCard)); }
    public IEnumerator AddPeptidesForSolving_CR(CSnailCard snailCard)
    {
        yield return new WaitForEndOfFrame();
        foreach (CardData.PeptideType pepType in snailCard.lstContainedPeptides)
        {
            lstPeptidesContainedInSnails.Add(pepType);
        }
        CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.Button, "RemovePeptidesForSolving");

        snailCard.EnableCardOutline();
    }

    public void RemovePeptidesForSolving(CSnailCard snailCard) { StartCoroutine(RemovePeptidesForSolving_CR(snailCard)); }
    public IEnumerator RemovePeptidesForSolving_CR(CSnailCard snailCard)
    {
        yield return new WaitForEndOfFrame();
        foreach (CardData.PeptideType pepType in snailCard.lstContainedPeptides) {
            if (lstPeptidesContainedInSnails.Contains(pepType)) {
                lstPeptidesContainedInSnails.Remove(pepType);
            }
        }
        CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.Button, "AddPeptidesForSolving");

        snailCard.DisableCardOutline();
    }


    public void SolveCabal() { StartCoroutine(SolveCabal_CR()); }
    public IEnumerator SolveCabal_CR()
    {
        CCabalManager cabalMan = CCabalManager.instance;
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        // Get the cabal the player is trying to solve
        List<CardData.PeptideType> cabalToSolve = null;
        switch(nActiveCabal)
        {
            case 0: cabalToSolve = CCabalManager.instance.FirstCabal;
            break;

            case 1: cabalToSolve = CCabalManager.instance.SecondCabal;
            break;

            case 2: cabalToSolve = CCabalManager.instance.ThirdCabal;
            break;
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Get list of all unique peptides
        bool pep1 = false, pep2 = false, pep3 = false;
        List<CardData.PeptideType> lstUnique = new List<CardData.PeptideType>();
                foreach (CardData.PeptideType pepType in lstPeptidesContainedInSnails)
            lstUnique.Add(pepType);
          
        // Check for matches
        foreach (CardData.PeptideType pepType in lstUnique)
        {
            if (cabalToSolve[0] == pepType && pep1 == false)
                pep1 = true;
            else if (cabalToSolve[1] == pepType && pep2 == false)
                pep2 = true;
            else if (cabalToSolve[2] == pepType && pep3 == false)
                pep3 = true;
        }

        if(pep1 && pep2 && pep3)
        {
            // Discard Cards used to solve cabal
            foreach (Transform child in gridCards.transform)
            {
                CSnailCard snailCard = child.GetComponent<CSnailCard>();
                yield return StartCoroutine(snailCard.KillSnail_CR(0f, 1f));

                snailCard.SetFedStateToUnfed();
                StartCoroutine(CActivePlayerPanel.instance.DiscardCard(snailCard));
            }

            yield return new WaitForSeconds(1f);

            // Load Cabal Star
            GameObject prefab = Resources.Load("Cabals/CabalStar") as GameObject;
            UISprite uiStar = ((GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity)).GetComponent<UISprite>();
            uiStar.transform.parent = this.transform;
            uiStar.transform.localScale = Vector3.zero;
            uiStar.transform.localPosition = new Vector3(140f, -465f, 0f);
            uiStar.depth = 91;
            CGlobals.UpdateWidgets();

            // Set Color
            if (activePlayer.player == PlayerData.PLAYER.ONE) uiStar.color = CGlobals.Player1Color;
            else if (activePlayer.player == PlayerData.PLAYER.TWO) uiStar.color = CGlobals.Player2Color;
            else if (activePlayer.player == PlayerData.PLAYER.THREE) uiStar.color = CGlobals.Player3Color;
            else if (activePlayer.player == PlayerData.PLAYER.FOUR) uiStar.color = CGlobals.Player4Color;

            CAudioManager.instance.PlaySound(acSolveCabal);
            CGlobals.TweenScale(uiStar.gameObject, Vector3.one * 2f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(1f);

            HidePanel();
            yield return new WaitForEndOfFrame();

            switch(nActiveCabal)
            {
                case 0:
                    uiStar.transform.parent = cabalMan.gridStars1.transform;
                    cabalMan.gridStars1.UpdateGrid(0.75f);
                    break;

                case 1:
                    uiStar.transform.parent = cabalMan.gridStars2.transform;
                    cabalMan.gridStars2.UpdateGrid(0.75f);
                    break;

                case 2:
                    uiStar.transform.parent = cabalMan.gridStars3.transform;
                    cabalMan.gridStars3.UpdateGrid(0.75f);
                    break;
            }

            yield return new WaitForSeconds(0.75f);
            uiStar.depth = 84;
            CGlobals.UpdateWidgets();

            yield return new WaitForSeconds(1f);

            // if player has solved all cabals, the game is over!
            activePlayer.SetCabalAsSolved(nActiveCabal);

            if(activePlayer.HasSolvedAllCabals())
            {
                Debug.Log("Player " + ((int)activePlayer.player) + " has won!");
                CUIManager.instance.EnableAllBoxCollidersForBeginTurn();
                yield return CWinScreen.instance.ShowWinScreen(activePlayer.player);

            }
            else
                yield return StartCoroutine(CGameManager.instance.EndTurn());
        }
        //else
        //{
        //    // Play sound
        //    CAudioManager.instance.PlaySound(acClosePanel);
        //}
    }


    private IEnumerator ShrinkSprites()
    {
        foreach(UISprite sprite in uiPeptides)
            CGlobals.TweenScale(sprite.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator ExpandSprites()
    {
        foreach(UISprite sprite in uiPeptides)
            CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.25f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
    }

    public void ShowPanel() { StartCoroutine(ShowPanel_CR()); }
    public IEnumerator ShowPanel_CR()
    {
        CAudioManager.instance.PlaySound(CGameManager.instance.acOpenMarket);

        StartCoroutine(FadeBG(true, 0.5f));

		//#if UNITY_IOS
		//CGlobals.TweenMove(this.gameObject, "y", 500f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#else
		CGlobals.TweenMove(this.gameObject, "y", 480f, 0.5f, iTween.EaseType.easeOutSine, true);
		//#endif
        
		yield return new WaitForSeconds(0.49f);

        foreach(CSnailCard snailCard in CGameManager.instance.activePlayer.snails)
        {
            if (snailCard.lstContainedPeptides.Count > 0)
            {
                CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.Button, "AddPeptidesForSolving");
                StartCoroutine(snailCard.SetNewParentPanel(CardData.ParentPanel.CABAL));
            }
        }

        gridCards.UpdateGrid(.5f);

        foreach (UISprite sprite in uiPeptides)
            sprite.transform.localScale = Vector3.zero;

        SetAsCabalOne();
    }

    public void HidePanel()
    {
        foreach(Transform card in gridCards.transform) {
            CSnailCard snailCard = card.GetComponent<CSnailCard>();
            StartCoroutine(snailCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
            StartCoroutine(snailCard.SetFedState(CardData.FedState.Fed));
        }
        CGlobals.UpdateWidgets();
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        StartCoroutine(FadeBG(false, 0.5f));

        foreach(CBaseCard card in CGameManager.instance.activePlayer.hand)
        {
            if (card.cardType == CardData.CardType.Snail) {
                CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");
                card.EnableButton();
            }
            else
                StartCoroutine(card.EnableButtonCollider_CR());
        }
        
		#if UNITY_IOS
		CGlobals.TweenMove(this.gameObject, "y", 1400f, 0.5f, iTween.EaseType.easeOutSine, true);
		#else
		CGlobals.TweenMove(this.gameObject, "y", 1050f, 0.5f, iTween.EaseType.easeOutSine, true);
		#endif
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


    //public void AddCardToHand(CBaseCard baseCard) { StartCoroutine(AddCardToHand_CR(baseCard)); }
    //public IEnumerator AddCardToHand_CR(CBaseCard baseCard)
    //{
    //    baseCard.transform.parent = CActivePlayerPanel.instance.gridHand.transform;

    //    for(int i = 0; i < CGameManager.instance.activePlayer.hand.Count; ++i)
    //        if(CGameManager.instance.activePlayer.hand[i] == baseCard) {
    //            baseCard.transform.SetSiblingIndex(i);
    //            break;
    //        }

    //    CGlobals.UpdateWidgets();
    //    CActivePlayerPanel.instance.gridHand.UpdateGrid();
    //    yield return new WaitForSeconds(0.5f);

    //    //take cards in hand. change button targets.
    //    CGlobals.AssignNewUIButtonOnClickTarget(this, baseCard, baseCard.Button, "AddCardToGrid");
    //}


    //public void AddCardToGrid(CBaseCard baseCard) { StartCoroutine(AddCardToGrid_CR(baseCard)); }
    //public IEnumerator AddCardToGrid_CR(CBaseCard baseCard)
    //{
    //    baseCard.transform.parent = gridCards.transform;
    //    CGlobals.UpdateWidgets();

    //    foreach (Transform trPeptide in gridCards.transform)
    //    {
    //        CSnailCard snailCard = trPeptide.GetComponent<CSnailCard>();
    //        foreach(CardData.PeptideType pepType in snailCard.lstContainedPeptides)
    //        {
    //            if(peptidesContainedInSnails.Contains(pepType) == false)
    //            {
    //                peptidesContainedInSnails.Add(pepType);
    //            }
    //        }
    //    }

    //    gridCards.UpdateGrid();
    //    yield return new WaitForSeconds(0.5f);

    //    //take cards in hand. change button targets.
    //    CGlobals.AssignNewUIButtonOnClickTarget(this, baseCard, baseCard.Button, "AddCardToHand");
    //    solveCabalButton.enabled = true;
    //}
}
