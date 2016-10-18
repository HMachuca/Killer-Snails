using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPeptideExtractionPanel : MonoBehaviour
{
    public Grid gridVisiblePeptides;
    public List<UISprite> lstPeptideSprites;

    [HideInInspector] public int peptidesToExtract;
    [HideInInspector] public bool gainTwoPeptides_RevealPeptide = false;
    [HideInInspector] public bool peekAt_thenRevealPeptide = false;
    [HideInInspector] public bool stealPreyFromOpponent = false;
    [HideInInspector] public bool pickRandomMarketCard = false;
    [HideInInspector] public bool drawTwoCardsFromDeck = false;
    [HideInInspector] public bool feedOrUnhibernateAnotherSnailInPool = false;


	void Start ()
    {
        peptidesToExtract = 1;
	}


    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    List<CardData.PeptideType> lst = new List<CardData.PeptideType>();
        //    StartCoroutine(ShowPanel(lst));
        //}
    }
	

    void EnableButtons()
    {
        foreach (UISprite sprite in lstPeptideSprites) {
            sprite.transform.parent = gridVisiblePeptides.transform;
            //sprite.transform.localPosition = Vector3.zero;
            sprite.gameObject.SetActive(true);
        }
        CGlobals.UpdateWidgets();
	}

	void DisableButtons()
    {
        foreach (UISprite sprite in lstPeptideSprites) {
            sprite.transform.parent = this.transform;
            sprite.transform.localPosition = Vector3.zero;
            sprite.gameObject.SetActive(false);
        }
        CGlobals.UpdateWidgets();
	}


    public IEnumerator ShowPanel(List<CardData.PeptideType> peptidesToEnable)
    {
        EnableButtons();
        yield return StartCoroutine(SetupPeptides(peptidesToEnable));

    #if UNITY_IOS
	    CGlobals.TweenMove(gameObject, "y", 500f, 0.5f, iTween.EaseType.easeOutSine, true);
    #else
        CGlobals.TweenMove(gameObject, "y", 480f, 0.5f, iTween.EaseType.easeOutSine, true);
    #endif

        yield return new WaitForSeconds(0.5f);
    }
    

    public IEnumerator HidePanel()
    {
        CGlobals.TweenMove(gameObject, "y", 750f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
        DisableButtons();
    }


    public IEnumerator SetupPeptides(List<CardData.PeptideType> peptidesToEnable)
    {
        CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
        if(snailCard.HasPotency)
        {
            snailCard.HasPotency = false;

            List<CardData.PeptideType> types = new List<CardData.PeptideType>();
            for (CardData.PeptideType type = CardData.PeptideType.Alpha; type < CardData.PeptideType.Count; ++type)
                types.Add(type);

            for (int i = 0; i < 4; ++i)
            {
                UISprite sprite = lstPeptideSprites[i];
                UIButton button = sprite.GetComponent<UIButton>();
                CPeptide peptide = sprite.GetComponent<CPeptide>();

                CardData.PeptideType newType = types[0];
                types.RemoveAt(0);

                peptide.peptide = newType;
                sprite.transform.parent = gridVisiblePeptides.transform;
                sprite.spriteName = newType.ToString().ToLower();
                button.normalSprite = newType.ToString().ToLower();;
                CGlobals.AssignNewUIButtonOnClickTarget(this, peptide, button, "AddPeptide");
            }
        }
        else
        {
            if(peptidesToEnable.Count == 0)
            {
                List<CardData.PeptideType> types = new List<CardData.PeptideType>();
                for (CardData.PeptideType type = CardData.PeptideType.Alpha; type < CardData.PeptideType.Count; ++type)
                    types.Add(type);

                for (int i = 0; i < 4; ++i)
                {
                    UISprite sprite = lstPeptideSprites[i];
                    UIButton button = sprite.GetComponent<UIButton>();
                    CPeptide peptide = sprite.GetComponent<CPeptide>();


                    int rand = Random.Range(0, types.Count);
                    CardData.PeptideType randType = types[rand];
                    types.RemoveAt(rand);

                    peptide.peptide = randType;
                    sprite.transform.parent = gridVisiblePeptides.transform;
                    sprite.spriteName = "random";
                    button.normalSprite = "random";
                    CGlobals.AssignNewUIButtonOnClickTarget(this, peptide, button, "AddPeptide");
                }
            }
            else
            {
                List<CardData.PeptideType> types = new List<CardData.PeptideType>();
                foreach(CardData.PeptideType type in peptidesToEnable)
                    types.Add(type);

                for (int i = 0; i < peptidesToEnable.Count; ++i)
                {
                    UISprite sprite = lstPeptideSprites[i];
                    UIButton button = sprite.GetComponent<UIButton>();
                    CPeptide peptide = sprite.GetComponent<CPeptide>();


                    CardData.PeptideType randType = types[0];
                    types.RemoveAt(0);

                    peptide.peptide = randType;
                    sprite.transform.parent = gridVisiblePeptides.transform;
                    sprite.spriteName = randType.ToString().ToLower();
                    button.normalSprite = randType.ToString().ToLower();;
                    CGlobals.AssignNewUIButtonOnClickTarget(this, peptide, button, "AddPeptide");
                }
            }
        }

        gridVisiblePeptides.UpdateGrid();
        yield return new WaitForSeconds(0.5f);
    }





    public void AddPeptide(CPeptide peptide) { StartCoroutine(AddPeptide_CR(peptide)); }
	public IEnumerator AddPeptide_CR(CPeptide peptide)
    {
        yield return new WaitForEndOfFrame();
        CGameManager gameMan = CGameManager.instance;

        if(peptide.peptide == CardData.PeptideType.Unknown)
        {
            CGlobals.TweenScale(peptide.GetComponent<UISprite>().gameObject, Vector3.zero, 0.65f, iTween.EaseType.easeInSine, true);
            yield return new WaitForSeconds(1.0f);
        }

        //CAudioManager.instance.PlaySound(acAddPeptide);
		CUIManager uiMan = CUIManager.instance;


        CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
        //CSnailCard snailCard = ((CSnailCard)gameMan.activePlayer.hand[0]);

		snailCard.HasPotency = false;
		snailCard.HasStarvation = false;
        //snailCard.CurrentUpgrade = CardData.InstantType.None;
        CardData.PeptideType peptideType = peptide.peptide;
        CPeptide newPeptide = CCabalManager.instance.CreatePeptide(peptideType);

		int index = 0;
		for (index = 0; index < snailCard.lstPeptideRows.Count; ++index)
			if (snailCard.lstPeptideRows[index].peptideType == peptideType)
            {
                newPeptide.transform.parent = peptide.transform;
                newPeptide.transform.localScale = Vector3.one;
                newPeptide.transform.localPosition = Vector3.zero;
                yield return new WaitForEndOfFrame();

				newPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
				newPeptide.transform.localPosition = snailCard.lstPeptideRows[index].gridRow.transform.localPosition;
            	newPeptide.Enable();
				newPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
				snailCard.lstPeptideRows[index].gridRow.transform.parent = snailCard.gridPeptides.transform;
				snailCard.lstContainedPeptides.Add(newPeptide.peptide);
				yield return new WaitForEndOfFrame();
                yield return StartCoroutine(snailCard.opponentSnail.AddPeptide(newPeptide.peptide));

                CGlobals.UpdateWidgets();
				snailCard.gridPeptides.UpdateGrid(1f);
				snailCard.lstPeptideRows[index].gridRow.UpdateGrid(1f);
                yield return new WaitForSeconds(1f);
                break;
            }

        // If already on card, no need to instantiate another grid
		bool bAlreadyOnCard = (index == snailCard.lstPeptideRows.Count) ? false : true;
        if (!bAlreadyOnCard)
        {
		    GameObject goPeptideGridRow = Resources.Load("Peptides/PeptideGridRow") as GameObject;
		    Grid newPeptideGrid = ((GameObject)Instantiate(goPeptideGridRow, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
            newPeptideGrid.mv2GridDimensions.x = 100;
            newPeptideGrid.mv2CellMaxDimensions.x = 100;

            tPeptideRow newRow;
            newRow.gridRow = newPeptideGrid;
		    newRow.peptideType = newPeptide.peptide;
		    newRow.gridRow.transform.parent = snailCard.gridPeptides.transform;
            newRow.gridRow.transform.localScale = Vector3.one;

            newPeptide.transform.parent = peptide.transform;
            newPeptide.transform.localScale = Vector3.one;
            newPeptide.transform.localPosition = Vector3.zero;
            yield return new WaitForEndOfFrame();

		    newPeptide.transform.parent = newRow.gridRow.transform;

		    snailCard.lstPeptideRows.Add(newRow);
		    snailCard.lstContainedPeptides.Add(newPeptide.peptide);

            yield return StartCoroutine(snailCard.opponentSnail.AddPeptide(newPeptide.peptide));
		    newPeptide.Enable();
            CGlobals.UpdateWidgets();

		    snailCard.gridPeptides.UpdateGrid(1f);
		    snailCard.lstPeptideRows[index].gridRow.UpdateGrid(1f);
            yield return new WaitForSeconds(1f);
        }

        // value decremented. when certain prey are eaten they allow 2 peptides to be extracted
        peptidesToExtract--;
        if(peptidesToExtract == 0)
        {
            peptidesToExtract = 1;

            // Move Panel up (offscreen)
		    StartCoroutine(HidePanel());
		    StartCoroutine(snailCard.SetFedState(CardData.FedState.Fed));
		    snailCard.ResetStrength();
		    yield return new WaitForSeconds(1f);

            if(gainTwoPeptides_RevealPeptide) {
                gainTwoPeptides_RevealPeptide = false;

                // Set Instruction text
                uiMan.actionPanel.InstructionText.HideBotHeader();
                uiMan.actionPanel.InstructionText.ShowTopHeader("Reveal a peptide in a cabal");

                yield return StartCoroutine(CPreyReward.GainTwoPeptides_RevealPeptide_CR());
            }
            else if(peekAt_thenRevealPeptide) {
                peekAt_thenRevealPeptide = false;

                // Set Instruction text
                uiMan.actionPanel.InstructionText.HideBotHeader();
                uiMan.actionPanel.InstructionText.ShowTopHeader("Peek at a peptide in a cabal");

                yield return StartCoroutine(CPreyReward.SetupForPeekAtThenRevealPeptide_CR());
            }
            else if(pickRandomMarketCard) {
                pickRandomMarketCard = false;
                yield return StartCoroutine(CPreyReward.PickRandomMarketCard());
            }
            else if(drawTwoCardsFromDeck) {
                drawTwoCardsFromDeck = false;
                yield return StartCoroutine(CPreyReward.RunReward_DrawTwoCards_Setup());
            }
            else if(feedOrUnhibernateAnotherSnailInPool) {
                feedOrUnhibernateAnotherSnailInPool = false;

                uiMan.actionPanel.InstructionText.HideBotHeader();
                yield return StartCoroutine(CPreyReward.FeedOrUnhibernateAnotherSnailInPool_Setup());
            }
            else if(stealPreyFromOpponent) {
                stealPreyFromOpponent = false;
                yield return StartCoroutine(CPreyReward.StealPreyFromOpponent());
            }
            else {
                yield return StartCoroutine(uiMan.actionPanel.EndFeedSnail_CR());
            }
        }
        else
        {
            uiMan.actionPanel.lblInstructionText.text = "Select another peptide";
        }
    }


  //  private IEnumerator SetupToRevealPeptide_CR()
  //  {
  //      yield return new WaitForEndOfFrame();
  //      CUIManager uiMan = CUIManager.instance;
  //      CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
  //      CPlayer activePlayer = CGameManager.instance.activePlayer;

  //      //uiMan.DisableAllCollidersForEndTurn();
		////uiMan.SelectedCards.Remove(snailCard);

		////yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());
		//CUIManager.instance.actionPanel.MoveHandPanelToActivePlayerPanel();

  //      //uiMan.SelectedCards.Remove(snailCard);
  //      //yield return StartCoroutine(snailCard.SetNewParentPanel(CardData.ParentPanel.SNAIL));
  //      //CActivePlayerPanel.instance.gridSnails.UpdateGrid();
  //      CGlobals.TweenMove(snailCard.gameObject, "position", new Vector3(140f, 280f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
  //      CGlobals.TweenScale(snailCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
  //      yield return new WaitForSeconds(0.5f);

  //      CCabalManager.instance.SetButtonFunctionTargetForRevealPeptide();
  //      CCabalManager.instance.EnableButtons();
  //      yield return new WaitForSeconds(0.25f);
		//uiMan.cabalPanel.anchorPanelBackground.transform.parent = uiMan.actionPanel.goActionContainer.transform;
  //      CGlobals.UpdateWidgets();

		////yield return StartCoroutine(CUIManager.instance.MoveCardToSnailsPanel_CR(snailCard));
  //  }


  //  private IEnumerator SetupForPeekAtThenRevealPeptide_CR()
  //  {
  //      yield return new WaitForEndOfFrame();

  //      CCabalManager.instance.SetButtonFunctionTargetForResearchCard();

  //      CUIManager uiMan = CUIManager.instance;
  //      CSnailCard snailCard = ((CSnailCard)uiMan.SelectedCard);
  //      CPlayer activePlayer = CGameManager.instance.activePlayer;

		//CUIManager.instance.actionPanel.MoveHandPanelToActivePlayerPanel();

  //      CGlobals.TweenMove(snailCard.gameObject, "position", new Vector3(140f, 280f, 0f), 0.5f, iTween.EaseType.easeOutSine, true);
  //      CGlobals.TweenScale(snailCard.gameObject, Vector3.one * 0.45f, 0.5f, iTween.EaseType.easeOutSine, true);
  //      yield return new WaitForSeconds(0.5f);

  //      CCabalManager.instance.SetButtonFunctionTargetForPeekAt_thenRevealPeptide();
  //      CCabalManager.instance.EnableButtons();
  //      yield return new WaitForSeconds(0.25f);
		//uiMan.cabalPanel.anchorPanelBackground.transform.parent = uiMan.actionPanel.goActionContainer.transform;
  //      CGlobals.UpdateWidgets();
  //  }



}
