using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct tCabal
{
    public List<CPeptide> peptides;
    public bool solved;

    public void Reset() {
        peptides = new List<CPeptide>(3);
        solved = false;
    }
}

public class CCabalManager : MonoBehaviour
{

    #region Singleton Instance
    private static CCabalManager _instance;
    public static CCabalManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CCabalManager>();
            }
            return _instance;
        }
    }
    #endregion

    public List<CardData.PeptideType> FirstCabal;
    public List<CardData.PeptideType> SecondCabal;
    public List<CardData.PeptideType> ThirdCabal;
    public List<tCabal> lstCabals;
    public CCabalSolvingPanel CabalSolvingPanel;
    public Grid gridStars1, gridStars2, gridStars3;


    public List<CPeptide> GetAllPeptides()
    {
        List<CPeptide> allPeptides = new List<CPeptide>();

        foreach (tCabal cabals in lstCabals)
            foreach(CPeptide peptide in cabals.peptides)
                allPeptides.Add(peptide);

        return allPeptides;
    }


    public CardData.PeptideType GetPeptideType(CPeptide peptide)
    {
        CPlayer activePlayer = CGameManager.instance.activePlayer;
        List<CPeptide> allPeptides = GetAllPeptides();

        for(int i = 0; i < allPeptides.Count; ++i)
        {
            if(peptide == allPeptides[i])
            {
                // Use index to get peptide type
                CardData.PeptideType peptideType = CardData.PeptideType.Alpha;
                switch(i)
                {
                    case 0: peptideType = FirstCabal[0]; break;
                    case 1: peptideType = FirstCabal[1]; break;
                    case 2: peptideType = FirstCabal[2]; break;

                    case 3: peptideType = SecondCabal[0]; break;
                    case 4: peptideType = SecondCabal[1]; break;
                    case 5: peptideType = SecondCabal[2]; break;

                    case 6: peptideType = ThirdCabal[0]; break;
                    case 7: peptideType = ThirdCabal[1]; break;
                    case 8: peptideType = ThirdCabal[2]; break;
                }

                return peptideType;
            }
        }

        return CardData.PeptideType.Unknown;
    }


    public List<CPeptide> GetHiddenPeptides()
    {
        CGameManager gameMan = CGameManager.instance;
        List<CPeptide>[] lstHidden = new List<CPeptide>[gameMan.Players.Count - 1];
        for(int i = 0; i < gameMan.Players.Count - 1; ++i)  
            lstHidden[i] = new List<CPeptide>();

        // Hold a list of hidden peptides for each player except active player
        int currIndex = 0;
        for (int i = 0; i < gameMan.Players.Count; ++i)
        {
            if (gameMan.Players[i] == gameMan.activePlayer)
                continue;

            List<CPeptide> allPeptides = GetAllPeptides();

            foreach(CPeptide cabalPeptide in allPeptides)
            {
                if(gameMan.Players[i].lstRevealedPeptides.Contains(cabalPeptide))
                {
                    Debug.Log("Contains peptide");
                }
                else
                {
                    lstHidden[currIndex].Add(cabalPeptide);
                }
            }

            currIndex++;
        }

        // Get true list of hidden peptides.
        // have to be hidden for all players
        List<CPeptide> lstUniqueHidden = new List<CPeptide>();

        foreach(List<CPeptide> lstPeptides in lstHidden)
            foreach(CPeptide peptide in lstPeptides)
                if(lstUniqueHidden.Contains(peptide) == false)
                    lstUniqueHidden.Add(peptide);

        // Check all players revealed lists. if not in any players' revealed lists, it is hidden
        List<CPeptide> lstFinal = new List<CPeptide>();
        foreach(CPeptide peptide in lstUniqueHidden)
        {
            bool hidden = true;
            foreach(CPlayer player in gameMan.Players) {
                if (player == gameMan.activePlayer)
                    continue;

                if (player.lstRevealedPeptides.Contains(peptide))
                    hidden = false;
            }

            if (hidden)
                lstFinal.Add(peptide);
        }

        return lstFinal;
    }


    void Start ()
    {
        EnableButtons(false);
	}

    public CPeptide CreatePeptide(CardData.PeptideType type, bool forPublishingCard = false, Vector3 position = default(Vector3))
    {
        GameObject goInstantiate = null;
        switch(type) {
            case CardData.PeptideType.Alpha:    goInstantiate = (GameObject)Resources.Load("Peptides/Alpha");
            break;
            case CardData.PeptideType.Delta:    goInstantiate = (GameObject)Resources.Load("Peptides/Delta");
            break;
            case CardData.PeptideType.Mu:       goInstantiate = (GameObject)Resources.Load("Peptides/Mu");
            break;
            case CardData.PeptideType.Omega:    goInstantiate = (GameObject)Resources.Load("Peptides/Omega");
            break;
            case CardData.PeptideType.Unknown:
            {
                int rand = Random.Range(0, (int)CardData.PeptideType.Count);
                CardData.PeptideType randType = (CardData.PeptideType)rand;
                goInstantiate = (GameObject)Resources.Load("Peptides/" + randType);
            }
            break;
        }

        GameObject goPeptide = (GameObject)Instantiate(goInstantiate, position, Quaternion.identity);
        CPeptide peptide = goPeptide.GetComponent<CPeptide>();
        return peptide;
    }


    public void SetButtonFunctionTargetForResearchCard()
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides) {
            CGlobals.AssignNewUIButtonOnClickTarget_UISprite(this, peptide.PeptideSprite, peptide.GetComponent<UIButton>(),
                                                            "PeekAtPeptide");
        }
    }

    public void SetButtonFunctionTargetForPublishingCard()
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides) {
            CGlobals.AssignNewUIButtonOnClickTarget_UISprite(this, peptide.PeptideSprite, peptide.GetComponent<UIButton>(),
                                                            "PeekAtPeptide");
        }
    }

    public void SetButtonFunctionTargetForPresentationCard()
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides) {
            CGlobals.AssignNewUIButtonOnClickTarget_UISprite(this, peptide.PeptideSprite, peptide.GetComponent<UIButton>(),
                                                            "PeekAtPeptide");
        }
    }

    public void SetButtonFunctionTargetForRevealPeptide()
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides) {
            CGlobals.AssignNewUIButtonOnClickTarget_UISprite(this, peptide.PeptideSprite, peptide.GetComponent<UIButton>(),
                                                            "RevealPeptide");
        }
    }

    public void SetButtonFunctionTargetForPeekAt_thenRevealPeptide()
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides) {
            CGlobals.AssignNewUIButtonOnClickTarget_UISprite(this, peptide.PeptideSprite, peptide.GetComponent<UIButton>(),
                                                            "PeekAt_ThenRevealPeptide");
        }
    }


    public void EnableButtons(bool enable = true)
    {
        List<CPeptide> allPeptides = GetAllPeptides();

        foreach(CPeptide peptide in allPeptides)
        {
            // if peptide is already revealed, dont not enable button or enable TweenScale component
            if (enable == true && peptide.PeptideSprite.spriteName != "random")
                continue;

            peptide.EnableButton(enable);

            TweenScale twnScale = peptide.GetComponent<TweenScale>();
            twnScale.enabled = enable;
            if(!enable)
                CGlobals.TweenScale(twnScale.gameObject, Vector3.one, 0.5f, iTween.EaseType.linear, true);
        }
    }

    public void GenerateCabals()
    {
        CLogfile.instance.Append("Generating random cabals");

        if (FirstCabal == null)
            FirstCabal = new List<CardData.PeptideType>(3);

        if (SecondCabal == null)
            SecondCabal = new List<CardData.PeptideType>(3);

        if (ThirdCabal == null)
            ThirdCabal = new List<CardData.PeptideType>(3);

        FirstCabal.Clear();
        SecondCabal.Clear();
        ThirdCabal.Clear();

        for(int i = 0; i < 3; ++i)
            FirstCabal.Add((CardData.PeptideType)Random.Range((int)CardData.PeptideType.Alpha, (int)CardData.PeptideType.Count));
        for(int i = 0; i < 3; ++i)
            SecondCabal.Add((CardData.PeptideType)Random.Range((int)CardData.PeptideType.Alpha, (int)CardData.PeptideType.Count));
        for(int i = 0; i < 3; ++i)
            ThirdCabal.Add((CardData.PeptideType)Random.Range((int)CardData.PeptideType.Alpha, (int)CardData.PeptideType.Count));
    }

    // Used for end turn transition
	public IEnumerator TransformSprites(bool expand = true)
	{
		Vector3 scale = (expand) ? Vector3.one : Vector3.zero;

		if(expand)
		{
			CPlayer activePlayer = CGameManager.instance.activePlayer;

            // No Peptides to reveal
			if(activePlayer.lstRevealedPeptides.Count == 0)
			{
                List<CPeptide> allPeptides = GetAllPeptides();

                foreach (CPeptide peptide in allPeptides)
                {
                    peptide.PeptideSprite.enabled = true;
                    peptide.SetPeptideAs(CardData.PeptideType.Unknown);
                    peptide.ChangeBorderColor(true);
                    CGlobals.TweenScale(peptide.gameObject, scale, 0.25f, iTween.EaseType.easeOutSine, true);
                }
			}
			else
			{

                // Show all peptides that have been revealed by player
                List<CPeptide> allPeptides = GetAllPeptides();

                foreach (CPeptide peptideA in allPeptides)
                {
                    bool revealed = false;
                    for(int i = 0; i < activePlayer.lstRevealedPeptides.Count; ++i)
                    {
                        CPeptide revealedPeptide = activePlayer.lstRevealedPeptides[i];
                        if(revealedPeptide == peptideA)
                        {
                            revealed = true;
                            revealedPeptide.ChangeBorderColor();
                            revealedPeptide.PeptideSprite.enabled = true;

                            // Get peptide index
                            int index = 0;
                            for (int j = 0; j < allPeptides.Count; ++j)
                                if (allPeptides[j] == peptideA) {
                                    index = j;
                                    break;
                                }

                            // Use index to get peptide type
                            CardData.PeptideType peptideType = CardData.PeptideType.Alpha;
                            switch(index)
                            {
                                case 0: peptideType = FirstCabal[0]; break;
                                case 1: peptideType = FirstCabal[1]; break;
                                case 2: peptideType = FirstCabal[2]; break;

                                case 3: peptideType = SecondCabal[0]; break;
                                case 4: peptideType = SecondCabal[1]; break;
                                case 5: peptideType = SecondCabal[2]; break;

                                case 6: peptideType = ThirdCabal[0]; break;
                                case 7: peptideType = ThirdCabal[1]; break;
                                case 8: peptideType = ThirdCabal[2]; break;
                            }

                            // Set sprite
                            revealedPeptide.SetPeptideAs(peptideType);

                            CGlobals.TweenScale(revealedPeptide.gameObject, scale, 0.25f, iTween.EaseType.easeOutSine, true);
                        }
                    }

                    if(!revealed)
                    {
                        peptideA.PeptideSprite.enabled = true;
                        peptideA.SetPeptideAs(CardData.PeptideType.Unknown);
                        peptideA.ChangeBorderColor(true);
                        CGlobals.TweenScale(peptideA.gameObject, scale, 0.25f, iTween.EaseType.easeOutSine, true);
                    }
                }
			}
		}
		else
		{
            List<CPeptide> allPeptides = GetAllPeptides();
            foreach (CPeptide peptide in allPeptides)
            {
                CGlobals.TweenScale(peptide.gameObject, scale, 0.25f, iTween.EaseType.easeOutSine, true);
                peptide.ChangeBorderColor(true);
            }
		}
		yield break;
	}

    public void RevealPeptide(UISprite current)
    {
        EnableButtons(false);
        //CAudioManager.instance.PlaySound(CGameManager.instance.acSelectCard);

        //CBaseCard card = CUIManager.instance.SelectedCard;
        //if(card == null) {
            StartCoroutine(RevealPeptide_CR(current));
        //    return;
        //}

        //CSnailCard snailCard = null;
        //CInstantCard instantCard = null;

        //if (card.cardType == CardData.CardType.Snail)
        //{
        //    snailCard = (CSnailCard)card;
        //    StartCoroutine(RevealPeptide_CR(current));
        //}
        //else if (card.cardType == CardData.CardType.Instant)
        //{
        //    instantCard = (CInstantCard)card;
        //    StartCoroutine(PeekAtPeptide_CR(current));
        //}
    }

    public IEnumerator RevealPeptide_CR(UISprite current)
    {
        CPlayer activePlayer = CGameManager.instance.activePlayer;
        CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

        CUIManager.instance.DisableAllCollidersForEndTurn();

        yield return new WaitForSeconds(0.75f);
        current.enabled = false;
        CPeptide peptide = current.GetComponent<CPeptide>();//GetSelectedPeptide(current);
        foreach(CPlayer player in CGameManager.instance.Players)
            player.lstRevealedPeptides.Add(peptide);

        UISprite sprite = peptide.GetComponent<UISprite>();
        sprite.enabled = true;
        CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);

        yield return new WaitForSeconds(1f);
        //yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

        yield return StartCoroutine(CUIManager.instance.SelectedCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
        CGlobals.UpdateWidgets();

        // Set Instruction text
        CUIManager.instance.actionPanel.InstructionText.HideTopHeader();
        //TweenPosition.Begin(CUIManager.instance.actionPanel.trInstructionBG.gameObject, 0.25f, Vector3.up * 525f);
        //CGlobals.TweenMove(CUIManager.instance.actionPanel.lblInstructionText.gameObject, "y", 600f, 0.25f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.25f);
        CUIManager.instance.actionPanel.lblInstructionText.text = "";

        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        activePlayer.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

        yield return new WaitForSeconds(1f);
        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
        CGlobals.UpdateWidgets();

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    public void PeekAtPeptide(UISprite current) { StartCoroutine(PeekAtPeptide_CR(current)); }
    public IEnumerator PeekAtPeptide_CR(UISprite current)
    {
        CPlayer activePlayer = CGameManager.instance.activePlayer;
        CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

        CUIManager.instance.DisableAllCollidersForEndTurn();

        yield return new WaitForSeconds(0.75f);
        current.enabled = false;
        CPeptide peptide = current.GetComponent<CPeptide>();//GetSelectedPeptide(current);
        CGameManager.instance.activePlayer.lstRevealedPeptides.Add(peptide);
        UISprite sprite = peptide.GetComponent<UISprite>();
        sprite.enabled = true;

        CardData.PeptideType peptideType = CCabalManager.instance.GetPeptideType(peptide);
        if (peptideType == CardData.PeptideType.Unknown)
            sprite.spriteName = "random";
        else
            sprite.spriteName = peptideType.ToString().ToLower();

        CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);

        yield return new WaitForSeconds(1f);
		yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());
        CUIManager.instance.actionPanel.InstructionText.HideTopHeader();

        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        activePlayer.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

        yield return new WaitForSeconds(1f);
        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
        CGlobals.UpdateWidgets();
        EnableButtons(false);

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }

    public void PeekAt_ThenRevealPeptide(UISprite current) { StartCoroutine(PeekAt_ThenRevealPeptide_CR(current)); }
    public IEnumerator PeekAt_ThenRevealPeptide_CR(UISprite current)
    {
        yield return new WaitForEndOfFrame();

        CPlayer activePlayer = CGameManager.instance.activePlayer;
        CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

        // Set Instruction text
        CUIManager.instance.actionPanel.InstructionText.HideBotHeader();
        CUIManager.instance.actionPanel.InstructionText.ShowTopHeader("Reveal a peptide in a cabal");

        yield return new WaitForSeconds(0.75f);
        current.enabled = false;
        CPeptide peptide = current.GetComponent<CPeptide>();//GetSelectedPeptide(current);
        CGameManager.instance.activePlayer.lstRevealedPeptides.Add(peptide);
        UISprite sprite = peptide.GetComponent<UISprite>();
        sprite.enabled = true;
        CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
        CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);

        SetButtonFunctionTargetForRevealPeptide();
        EnableButtons();
    }



  //  public IEnumerator RevealPeptide_CR(UISprite current)
  //  {
  //      CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

  //      yield return new WaitForSeconds(0.75f);
  //      current.enabled = false;

  //      CPeptide peptide = GetSelectedPeptide(current);
  //      current.spriteName = peptide.GetComponent<UISprite>().spriteName;
  //      CGameManager.instance.activePlayer.lstRevealedPeptides.Add(peptide);
  //      CGlobals.TweenScale(current.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
  //      CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);
  //      yield return new WaitForSeconds(0.75f);
  //  }
  //  public IEnumerator PeekAtPeptide(UISprite current)
  //  {
  //      CPlayer activePlayer = CGameManager.instance.activePlayer;
  //      CGlobals.TweenScale(current.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

  //      CUIManager.instance.DisableAllCollidersForEndTurn();

  //      yield return new WaitForSeconds(0.75f);

  //      CPeptide peptide = GetSelectedPeptide(current);
  //      current.spriteName = peptide.GetComponent<UISprite>().spriteName;
  //      CGameManager.instance.activePlayer.lstRevealedPeptides.Add(peptide);
  //      CGlobals.TweenScale(current.gameObject, Vector3.one, 0.75f, iTween.EaseType.easeOutExpo, true);
  //      CAudioManager.instance.PlaySound(CGameManager.instance.acRevealPeptide);

  //      yield return new WaitForSeconds(1f);
		//yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

  //      StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
  //      activePlayer.gridHand.UpdateGrid();
  //      CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

  //      yield return new WaitForSeconds(1f);
  //      CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
  //      CGlobals.UpdateWidgets();

  //      yield return StartCoroutine(CGameManager.instance.EndTurn());
  //  }


    

    #region Peptide Locator Functions
    //
    public bool IsInFirstCabalFirstPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[0])
                return true;

        return false;
    }
    public bool IsInFirstCabalSecondPeptide(Transform current)
    {
        //foreach (CPeptide peptide in this.lstFirstCabalSecondPeptide)
        //    if (current == peptide.transform)
        //        return true;
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[1])
                return true;

        return false;
    }
    public bool IsInFirstCabalThirdPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[2])
                return true;

        return false;
    }
    //
    public bool IsInSecondCabalFirstPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[3])
                return true;

        return false;
    }
    public bool IsInSecondCabalSecondPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[4])
                return true;

        return false;
    }
    public bool IsInSecondCabalThirdPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[5])
                return true;

        return false;
    }
    //
    public bool IsInThirdCabalFirstPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[6])
                return true;

        return false;
    }
    public bool IsInThirdCabalSecondPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[7])
                return true;

        return false;
    }
    public bool IsInThirdCabalThirdPeptide(Transform current)
    {
        CPeptide currentPeptide = current.GetComponent<CPeptide>();
        List<CPeptide> allPeptides = GetAllPeptides();

        if (currentPeptide == allPeptides[8])
                return true;

        return false;
    }
    //
    #endregion


    //public void ChangeRandomPeptide()
    //{
    //    int rand = Random.Range(0, 8);
    //    int i = -1;

    //    foreach (CCabal cabal in cabals)
    //        foreach (CPeptide peptide in cabal.peptides) {
    //            i++;
    //            if (i == rand)
    //                peptide.GeneratePeptide();
    //        }
    //}
}
