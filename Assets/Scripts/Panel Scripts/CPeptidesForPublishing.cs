using UnityEngine;
using System.Collections;

public class CPeptidesForPublishing : MonoBehaviour
{
    public GameObject goButtons;
    public UISprite uiAlpha, uiDelta, uiMu, uiOmega;
    public UILabel uiText;

    public UISprite uiMatch, uiNoMatch;
    private CPeptide selectedPeptide;

	void Start ()
    {
        selectedPeptide = null;
	}

    #region ChoosePeptides
    public void ChooseAlphaForPublishing()
    {
        CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Alpha);
        peptide.transform.parent = this.transform;
        peptide.transform.localPosition = uiAlpha.transform.localPosition;
        peptide.transform.localScale = uiAlpha.transform.localScale;
        StartCoroutine(FinishSelection(peptide));
	}

    public void ChooseDeltaForPublishing()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Delta);
        peptide.transform.parent = this.transform;
        peptide.transform.localPosition = uiDelta.transform.localPosition;
        peptide.transform.localScale = uiDelta.transform.localScale;
        StartCoroutine(FinishSelection(peptide));
	}

    public void ChooseMuForPublishing()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Mu);
        peptide.transform.parent = this.transform;
        peptide.transform.localPosition = uiMu.transform.localPosition;
        peptide.transform.localScale = uiMu.transform.localScale;
        StartCoroutine(FinishSelection(peptide));
	}

    public void ChooseOmegaForPublishing()
    {
	    CPeptide peptide = CCabalManager.instance.CreatePeptide(CardData.PeptideType.Omega);
        peptide.transform.parent = this.transform;
        peptide.transform.localPosition = uiOmega.transform.localPosition;
        peptide.transform.localScale = uiOmega.transform.localScale;
        StartCoroutine(FinishSelection(peptide));
	}
    #endregion

    private IEnumerator FinishSelection(CPeptide peptide)
    {
        selectedPeptide = peptide;

        // Move All buttons out of screen view.
        // Move and Scale selectedPeptide
        CGlobals.TweenMove(goButtons, "position", Vector3.up * 230f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(selectedPeptide.gameObject, "position", Vector3.up * -135f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(selectedPeptide.gameObject, Vector3.one * 1.5f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);

        CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
        CGlobals.UpdateWidgets();
        yield return new WaitForSeconds(0.25f);

        CCabalManager.instance.SetButtonFunctionTargetForPublishingCard();
        CCabalManager.instance.EnableButtons();
    }


    public void PeekAtPeptide(UISprite current) { StartCoroutine(PeekAtPeptide_CR(current)); }
    public IEnumerator PeekAtPeptide_CR(UISprite current)
    {
        CPeptide peptide = current.GetComponent<CPeptide>();//CCabalManager.instance.GetSelectedPeptide(current);
        CGameManager.instance.activePlayer.lstRevealedPeptides.Add(peptide);

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
        bool showSnailCards = false;

        ///////////////////////////////////////////////////
        // Determine if there is a match
        ///////////////////////////////////////////////////
        if(peekedPeptide.peptide == selectedPeptide.peptide)
        {
            // MATCH
            if(activePlayer.snails.Count == 1)
            {
                CSnailCard snailCard = (CSnailCard)activePlayer.snails[0];
                StartCoroutine(HidePanel());
                yield return StartCoroutine(SendPeptideToSnailCard(snailCard));
		        yield return new WaitForSeconds(1f);
            }
            else
            {
                CUIManager.instance.activePlayerPanel.gridSnails.transform.parent = CUIManager.instance.actionPanel.goActionContainer.transform;
                foreach (CSnailCard snailCard in CGameManager.instance.activePlayer.snails) {
                    snailCard.EnableButton();
                    CGlobals.AssignNewUIButtonOnClickTarget(this, snailCard, snailCard.Button, "ngui_SendPeptideToSnailCard");
                }

                CGlobals.UpdateWidgets();
                showSnailCards = true;
            }
        }
        else
        {
            // NO MATCH
            // Scale down peekedPeptide
            CGlobals.TweenScale(peekedPeptide.gameObject, Vector3.zero, 0.75f, iTween.EaseType.easeOutExpo, true);

            // Peptide Button Shrink Animation. Destroy Object
            CGlobals.TweenScale(selectedPeptide.gameObject, Vector3.one * 1.9f, 0.3f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.25f);
            CGlobals.TweenScale(selectedPeptide.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.15f);

            Destroy(selectedPeptide.gameObject);

            StartCoroutine(HidePanel());
            CGlobals.TweenMove(goButtons, "y", 0f, 0.5f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.4f);

            // Scale up unknown
            current.enabled = true;
            yield return new WaitForEndOfFrame();

        }  

        if(!showSnailCards)
        {
		    yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());

            StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
            activePlayer.gridHand.UpdateGrid();
            CActivePlayerPanel.instance.lblHandCount.text = activePlayer.hand.Count.ToString();

            yield return new WaitForSeconds(1f);
            CUIManager.instance.cabalPanel.anchorPanelBackground.transform.parent = CUIManager.instance.cabalPanel.transform;
            CGlobals.UpdateWidgets();

            yield return StartCoroutine(CGameManager.instance.EndTurn());
        }
    }


    public void ngui_SendPeptideToSnailCard(CSnailCard snailCard) { StartCoroutine(ngui_SendPeptideToSnailCard_CR(snailCard)); }
    public IEnumerator ngui_SendPeptideToSnailCard_CR(CSnailCard snailCard)
    {
        StartCoroutine(HidePanel());
        yield return StartCoroutine(SendPeptideToSnailCard(snailCard));

        // Reset snail cards
        CUIManager.instance.activePlayerPanel.gridSnails.transform.parent = CActivePlayerPanel.instance.uiPanelSnails.transform;
        foreach (CSnailCard card in CGameManager.instance.activePlayer.snails) {
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

        yield return StartCoroutine(CGameManager.instance.EndTurn());
    }


    public IEnumerator SendPeptideToSnailCard(CSnailCard snailCard)
    {
        int index = 0;
		for (index = 0; index < snailCard.lstPeptideRows.Count; ++index)
			if (snailCard.lstPeptideRows[index].peptideType == selectedPeptide.peptide)
            {
				selectedPeptide.transform.parent = snailCard.lstPeptideRows[index].gridRow.transform;
				//selectedPeptide.transform.localPosition = snailCard.lstPeptideRows[index].gridRow.transform.localPosition;
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
        if (bAlreadyOnCard)
            yield break;

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
    }


    public void MoveCardToActionPanel(CBaseCard publishingCard) { StartCoroutine(MoveCardToActionPanel_CR(publishingCard)); }
    public IEnumerator MoveCardToActionPanel_CR(CBaseCard publishingCard)
    {
        StartCoroutine(HidePanel());
        yield return StartCoroutine(CUIManager.instance.MoveCardToActionPanel_CR(publishingCard));
        CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, publishingCard, publishingCard.Button, "MoveCardToHandPanel");
    }


    public IEnumerator ShowPanel() {
		#if !UNITY_IOS
		CGlobals.TweenMove(this.gameObject, "y", 400f, 0.5f, iTween.EaseType.easeOutSine, true);
		#else
		CGlobals.TweenMove(this.gameObject, "y", 500f, 0.5f, iTween.EaseType.easeOutSine, true);
		#endif
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator HidePanel() {
        CGlobals.TweenMove(this.gameObject, "y", 800f, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
		CGlobals.TweenMove(goButtons, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
    }
}
