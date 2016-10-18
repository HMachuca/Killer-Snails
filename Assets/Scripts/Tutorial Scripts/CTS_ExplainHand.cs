using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTS_ExplainHand : CTutorialScene
{
    public CTutorialDirector.TS_EXPLAINHAND CurrentScene;
    public string txtExplainHand, txtCloseBttn;


	public override IEnumerator StartScene()
    {
        yield return StartCoroutine(base.StartScene());
        SceneType = CTutorialDirector.TutorialScene.TS_EXPLAINHAND;
        StartCoroutine(SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND.EXPLAIN_HAND));
        //StartCoroutine(CTutorialDirector.instance.ExpandTextbox());
	}

    public override IEnumerator AdvanceScene()
    {
        yield return StartCoroutine(base.AdvanceScene());

        CurrentScene = CurrentScene + 1;
        yield return SetupScene_CR(CurrentScene);
    }

	
	public override IEnumerator SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND nextScene)
    {
        yield return StartCoroutine(base.SetupScene_CR(nextScene));
        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CTutorialDirector director = CTutorialDirector.instance;

         switch(nextScene)
        {
            case CTutorialDirector.TS_EXPLAINHAND.EXPLAIN_HAND:
            {
                StartCoroutine(director.DisableButton());
                director.SetText(txtExplainHand);
                uiMan.EnableAllBoxCollidersForBeginTurn();

                List<CArrowManager.tArrowData> lstArrowData = new List<CArrowManager.tArrowData>();
                CArrowManager.tArrowData arrowData1 = new CArrowManager.tArrowData(new Vector3(-445f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                CArrowManager.tArrowData arrowData2 = new CArrowManager.tArrowData(new Vector3(-235f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                CArrowManager.tArrowData arrowData3 = new CArrowManager.tArrowData(new Vector3(-30f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                CArrowManager.tArrowData arrowData4 = new CArrowManager.tArrowData(new Vector3(175f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                CArrowManager.tArrowData arrowData5 = new CArrowManager.tArrowData(new Vector3(380f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                lstArrowData.Add(arrowData1);
                lstArrowData.Add(arrowData2);
                lstArrowData.Add(arrowData3);
                lstArrowData.Add(arrowData4);
                lstArrowData.Add(arrowData5);

                yield return StartCoroutine(director.ArrowManager.EnableArrows(lstArrowData));
                yield return StartCoroutine(director.ArrowManager.ScaleActiveArrows(Vector3.one, 0.5f));
                lstArrowData.Clear();

                foreach (CBaseCard handCard in CGameManager.instance.activePlayer.hand) {
                    //StartCoroutine(handCard.DisableButtonCollider_CR());
                    CGlobals.AssignNewUIButtonOnClickTarget(this, handCard, handCard.Button, "ZoomIntoCard");
                }

                StartCoroutine(SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND.WFI_CLICKHANDCARD));
            }
            break;

            case CTutorialDirector.TS_EXPLAINHAND.WFI_CLICKHANDCARD:
            break;

            case CTutorialDirector.TS_EXPLAINHAND.MOVE_TO_ACTION_PANEL:
            {
                director.SetText(txtCloseBttn);
                uiMan.EnableAllBoxCollidersForBeginTurn();
                StartCoroutine(SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND.WFI_CLICKHANDCARD));
            }
            break;

            case CTutorialDirector.TS_EXPLAINHAND.WFI_CLICKCLOSEBUTTON:
            break;

            case CTutorialDirector.TS_EXPLAINHAND.ENDSCENE:
            {
                yield return StartCoroutine(director.ArrowManager.DisableArrows());
                CTutorialDirector.instance.StartNextScene();
            }
            break;

            
        }

        CurrentScene = nextScene;
	}

    public void ZoomIntoCard(CBaseCard handCard) { StartCoroutine(ZoomIntoCard_CR(handCard)); }
    public IEnumerator ZoomIntoCard_CR(CBaseCard handCard)
    {
        CTutorialDirector director = CTutorialDirector.instance;
        yield return StartCoroutine(director.ArrowManager.DisableArrows());
        StartCoroutine(director.TransformTextbox(new Vector3(0f, 350f, 0f), new Vector3(1.5f, 0.6f, 1f), 0.5f));
        director.SetText(txtCloseBttn);

        //yield return StartCoroutine(SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND.MOVE_TO_ACTION_PANEL));
        CurrentScene = CTutorialDirector.TS_EXPLAINHAND.MOVE_TO_ACTION_PANEL;
        yield return StartCoroutine(MoveCardToActionPanel(handCard));
        //yield return CUIManager.instance.MoveCardToActionPanel_CR(handCard);

        // Setup Arrow
        List<CArrowManager.tArrowData> lstArrowData = new List<CArrowManager.tArrowData>();
        CArrowManager.tArrowData arrowData = new CArrowManager.tArrowData(new Vector3(500f, -215f, 0f), CArrowManager.DIRECTION.LEFT);
        lstArrowData.Add(arrowData);
        yield return StartCoroutine(director.ArrowManager.EnableArrows(lstArrowData));
        yield return StartCoroutine(director.ArrowManager.ScaleActiveArrows(Vector3.one, 0.5f));
        lstArrowData.Clear();

        foreach (CBaseCard card in CGameManager.instance.activePlayer.hand) {
            StartCoroutine(card.EnableButtonCollider_CR());
        }

        CurrentScene = CTutorialDirector.TS_EXPLAINHAND.WFI_CLICKCLOSEBUTTON;
    }


    private IEnumerator MoveCardToActionPanel(CBaseCard handCard)
    {
        yield return new WaitForEndOfFrame();

    	CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        uiMan.marketPanel.gridMarketCards.UpdateGrid();
		uiMan.actionPanel.anchorPanelBackground.gameObject.SetActive(true);
		uiMan.actionPanel.anchorPanelBackground.transform.localPosition = Vector3.zero;
        StartCoroutine(uiMan.actionPanel.FadeBG());

        handCard.ChangeWidgetDepth(100);
        handCard.Button.enabled = false;
        
        yield return new WaitForEndOfFrame();
        uiMan.actionPanel.EnableButton(handCard);
        StartCoroutine(handCard.SetNewParentPanel(CardData.ParentPanel.ACTION));

        CGlobals.UpdateWidgets();
        uiMan.actionPanel.SetFlavorText(handCard.GetFlavorText());

        // Used to set widget depth for all sprites on card gameobject
        if(handCard.cardType == CardData.CardType.Snail)
            ((CSnailCard)handCard).UpdateWidgetDepths();

        if(handCard.cardType == CardData.CardType.Snail)
        {
            CSnailCard snailCard = (CSnailCard)handCard;
            bool canAct = (snailCard.fedState != CardData.FedState.Fed) ? true : false;
			yield return StartCoroutine(uiMan.EnableActionBackground(canAct, handCard));
        }
        else if(handCard.cardType == CardData.CardType.Prey)
        {
            uiMan.actionPanel.EnableButton(handCard);
            yield return StartCoroutine(uiMan.EnableActionBackground(true, handCard));
        }
        else if(handCard.cardType == CardData.CardType.Instant)
        {
            CInstantCard instantCard = (CInstantCard)handCard;
            yield return StartCoroutine(uiMan.EnableActionBackground(true, handCard));
        }
        
        handCard.Button.enabled = true;
    }

    public void MoveCardToHandPanel(CBaseCard handCard) { StartCoroutine(MoveCardToHandPanel_CR(handCard)); }
    public IEnumerator MoveCardToHandPanel_CR(CBaseCard handCard)
    {
        yield return new WaitForEndOfFrame();

        StartCoroutine(CTutorialDirector.instance.ArrowManager.DisableArrows());
        yield return StartCoroutine(CUIManager.instance.MoveCardToHandPanel_CR(handCard));
        yield return StartCoroutine(AdvanceScene());

    }
}
