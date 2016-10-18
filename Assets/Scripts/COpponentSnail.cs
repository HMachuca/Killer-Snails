using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class COpponentSnail : MonoBehaviour 
{
	public UISprite uiFed, uiHibernate;
	private UIButton button;

	public Grid gridPeptides;
	public List<tPeptideRow>  lstGridPeptides;

	public CSnailCard snailCard;

    public UIButton Button { get { return button; } }


	void Start()
	{
		button = GetComponent<UIButton>();
		button.enabled = false;
		lstGridPeptides = new List<tPeptideRow>(4);
	}

	public void LowerFedState()
    {
        CAudioManager audioMan = CAudioManager.instance;
        CBaseCard baseCard = CUIManager.instance.SelectedCard;
        CInstantCard instantCard = null;
        if(baseCard.cardType == CardData.CardType.Instant)
            instantCard = (CInstantCard)baseCard;

        if(instantCard.instantType == CardData.InstantType.Lobster ||
            instantCard.instantType == CardData.InstantType.Stingray)
        {
            audioMan.PlaySound(audioMan.acDowngrade);
            StartCoroutine(LowerFedState_WithEndTurn_CR());
        }
        else
            StartCoroutine(LowerFedState_CR());
    }

    public IEnumerator LowerFedState_WithEndTurn_CR()
    {
        // Set Instruction text
        CUIManager uiMan = CUIManager.instance;
        uiMan.actionPanel.InstructionText.HideTopHeader();
        //TweenPosition.Begin(uiMan.actionPanel.trInstructionBG.gameObject, 0.25f, Vector3.up * 525f);
        //CGlobals.TweenMove(uiMan.actionPanel.lblInstructionText.gameObject, "y", 600f, 0.25f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.25f);
        uiMan.actionPanel.lblInstructionText.text = "";
        //uiMan.actionPanel.lblInstructionText.gradientBottom = Color.white;

        yield return StartCoroutine(LowerFedState_CR());
        yield return StartCoroutine(ResetButton());
    }

    public IEnumerator LowerFedState_CR()
	{
        CUIManager.instance.DisableAllCollidersForEndTurn();

		button.enabled = false;
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
        yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());
        float time = 0.5f;

		switch(snailCard.fedState)
		{
		case CardData.FedState.Unfed: // to Hibernate
		{
			uiHibernate.enabled = true;
			CGlobals.iTweenValue(this.gameObject, uiHibernate.color, Color.white, time, this.gameObject, 
							"UpdateFedState", iTween.EaseType.linear, "CompleteUpdate");

			yield return new WaitForSeconds(time);
			snailCard.UpdateFedStateFromOpponentSnail(CardData.FedState.Hibernating);

            //yield return StartCoroutine(ResetButton());
		}
		break;

		case CardData.FedState.Fed:  // to Unfed
		{
			CGlobals.iTweenValue(this.gameObject, uiFed.color, Color.clear, time, this.gameObject, 
							"UpdateFedState", iTween.EaseType.linear, "CompleteUpdate");

            yield return new WaitForSeconds(time);
            snailCard.UpdateFedStateFromOpponentSnail(CardData.FedState.Unfed);
			//yield return StartCoroutine(ResetButton());
		}
		break;

		case CardData.FedState.Hibernating: // Dead
			yield return StartCoroutine(snailCard.KillSnail_CR(0f, 1f));
			break;
		}
	}

    private IEnumerator ResetButton()
    {
		CGameManager gameMan = CGameManager.instance;
		CActivePlayerPanel.instance.gridSnails.transform.parent = CActivePlayerPanel.instance.panelSnails.anchorPanelBackground.transform;
		COpponentsPanel.instance.anchorPanelBackground.transform.parent = COpponentsPanel.instance.transform;
        
        // Enable Opponent Prey cards
        foreach(CPlayer player in CGameManager.instance.Players)
                        if(player.player != CGameManager.instance.activePlayer.player)
                            player.gridPrey.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);
		CGlobals.UpdateWidgets();

		COpponentsPanel.instance.DisableSnailButtons();
		yield return gameMan.EndTurn();
    }


	public void AutomaticSetState(CardData.FedState fedState)
	{
		//currentFedState = fedState;
		switch(fedState)
		{
			case CardData.FedState.Hibernating:
			{
				uiHibernate.color = Color.white;
				uiFed.color = Color.clear;
				uiHibernate.enabled= true;
				uiFed.enabled = false;
			}
			break;
			case CardData.FedState.Unfed:
			{
				uiHibernate.color = Color.clear;
				uiFed.color = Color.clear;
				uiHibernate.enabled= false;
				uiFed.enabled = false;
			}
			break;
			case CardData.FedState.Fed:
			{
				uiHibernate.color = Color.clear;
				uiFed.color = Color.white;
				uiHibernate.enabled= false;
				uiFed.enabled = true;
			}
			break;
			case CardData.FedState.Dead:
			{
				uiHibernate.color = Color.clear;
				uiFed.color = Color.white;
				uiHibernate.enabled= false;
				uiFed.enabled = false;

			}
			break;
		}
	}

	// Tween update color
	public void UpdateFedState(Color color)
	{
		switch(snailCard.fedState)
		{
		case CardData.FedState.Unfed: // to Hibernate
			uiHibernate.color = color;
		break;

		case CardData.FedState.Fed:  // to Unfed
			uiFed.color = color;
			break;

		case CardData.FedState.Hibernating: // Cannot lower
			break;
		}
	}


	// Set currentFedState after tween completion
	public void CompleteUpdate()
	{
//		switch(currentFedState)
//		{
//		case CardData.FedState.Unfed: // to Hibernate
//			currentFedState = CardData.FedState.Hibernating;
//		break;
//
//		case CardData.FedState.Fed:  // to Unfed
//			currentFedState = CardData.FedState.Unfed;
//			break;
//
//		case CardData.FedState.Hibernating: // Cannot lower
//			break;
//		}
	}

	public void EnableButton(bool enable = true)
	{
		button.enabled = enable;
        GetComponent<BoxCollider>().enabled = enable;
	}

	public IEnumerator AddPeptide(CardData.PeptideType peptideType)
	{
		CPeptide newPeptide = CCabalManager.instance.CreatePeptide(peptideType, false, Vector3.up * 1000f);

		int index = 0;
		for (index = 0; index < lstGridPeptides.Count; ++index)
			if (lstGridPeptides[index].peptideType == peptideType)
            {
				newPeptide.transform.parent = lstGridPeptides[index].gridRow.transform;
				newPeptide.transform.localPosition = lstGridPeptides[index].gridRow.transform.localPosition;
            	newPeptide.Enable();
				newPeptide.transform.parent = lstGridPeptides[index].gridRow.transform;
				lstGridPeptides[index].gridRow.transform.parent = gridPeptides.transform;
				yield return new WaitForEndOfFrame();

                CGlobals.UpdateWidgets();
				gridPeptides.UpdateGrid();
				lstGridPeptides[index].gridRow.UpdateGrid();
                break;
            }

		// If already on card, no need to instantiate another grid
		bool bAlreadyOnCard = (index == lstGridPeptides.Count) ? false : true;
        if (bAlreadyOnCard)
            yield break;

		GameObject goPeptideGridRow = Resources.Load("Peptides/PeptideGridRow") as GameObject;
		Grid newPeptideGrid = ((GameObject)Instantiate(goPeptideGridRow, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
        newPeptideGrid.mv2GridDimensions.x = 70;
        newPeptideGrid.mv2CellMaxDimensions.x = 100;
        newPeptideGrid.mnBorder = -70;
        tPeptideRow newRow;
        newRow.gridRow = newPeptideGrid;
		newRow.peptideType = newPeptide.peptide;

		newRow.gridRow.transform.parent = gridPeptides.transform;
		newPeptide.transform.parent = newRow.gridRow.transform;
		newPeptide.Enable();

        newPeptideGrid.UpdateGrid();
		gridPeptides.UpdateGrid();
        lstGridPeptides.Add(newRow);
	}

    public IEnumerator AddPeptide(CPeptide newPeptide)
	{
		int index = 0;
		for (index = 0; index < lstGridPeptides.Count; ++index)
			if (lstGridPeptides[index].peptideType == newPeptide.peptide)
            {
				newPeptide.transform.parent = lstGridPeptides[index].gridRow.transform;
				newPeptide.transform.localPosition = lstGridPeptides[index].gridRow.transform.localPosition;
            	newPeptide.Enable();
				newPeptide.transform.parent = lstGridPeptides[index].gridRow.transform;
				lstGridPeptides[index].gridRow.transform.parent = gridPeptides.transform;
				yield return new WaitForEndOfFrame();

                CGlobals.UpdateWidgets();
				gridPeptides.UpdateGrid();
				lstGridPeptides[index].gridRow.UpdateGrid();
                break;
            }

		// If already on card, no need to instantiate another grid
		bool bAlreadyOnCard = (index == lstGridPeptides.Count) ? false : true;
        if (bAlreadyOnCard)
            yield break;

		GameObject goPeptideGridRow = Resources.Load("Peptides/PeptideGridRow") as GameObject;
		Grid newPeptideGrid = ((GameObject)Instantiate(goPeptideGridRow, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
        newPeptideGrid.mv2GridDimensions.x = 70;
        newPeptideGrid.mv2CellMaxDimensions.x = 100;
        newPeptideGrid.mnBorder = -70;
        tPeptideRow newRow;
        newRow.gridRow = newPeptideGrid;
		newRow.peptideType = newPeptide.peptide;

		newRow.gridRow.transform.parent = gridPeptides.transform;
		newPeptide.transform.parent = newRow.gridRow.transform;
		newPeptide.Enable();

        newPeptideGrid.UpdateGrid();
		gridPeptides.UpdateGrid();
        lstGridPeptides.Add(newRow);
	}

	public IEnumerator RemovePeptidesAndShrink(float duration = 0.5f)
	{
		CUIManager uiManager = CUIManager.instance;

		// Shrink peptides
		List<CPeptide> returnPeptides = new List<CPeptide>();
		foreach(tPeptideRow peptideRow in lstGridPeptides)
			foreach(Transform peptideTR in peptideRow.gridRow.transform) {
				returnPeptides.Add(peptideTR.GetComponent<CPeptide>());
				CGlobals.TweenScale(peptideTR.gameObject, Vector3.zero, duration, iTween.EaseType.easeOutSine, true);
			}

		yield return new WaitForSeconds(duration);
		//foreach(tPeptideRow peptide in lstGridPeptides)
			//Destroy(peptide.);
		
		CGlobals.TweenScale(gameObject, transform.localScale * 1.1f, 0.45f, iTween.EaseType.easeOutQuad, true);
		yield return new WaitForSeconds(0.45f);
		CGlobals.TweenScale(gameObject, Vector3.zero, 0.25f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.25f);
	}
}
