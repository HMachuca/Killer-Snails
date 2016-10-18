using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTS_PeekAtPeptide : CTutorialScene
{
    public CTutorialDirector.TS_PEEKATPEPTIDE CurrentScene;
    public string txtExplainInstantCard, txtExplainResearchCard, txtPeekedPeptide;

	public override IEnumerator StartScene()
    {
        yield return StartCoroutine(base.StartScene());
        SceneType = CTutorialDirector.TutorialScene.TS_PEEKATPEPTIDE;
        StartCoroutine(SetupScene_CR(CTutorialDirector.TS_PEEKATPEPTIDE.EXPLAIN_INSTANT_CARDS));
	}
	
	public override IEnumerator SetupScene_CR(CTutorialDirector.TS_PEEKATPEPTIDE nextScene)
    {
        yield return StartCoroutine(base.SetupScene_CR(nextScene));
        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CTutorialDirector director = CTutorialDirector.instance;

        switch(nextScene)
        {
            case CTutorialDirector.TS_PEEKATPEPTIDE.EXPLAIN_INSTANT_CARDS:
            {
                uiMan.DisableAllCollidersForEndTurn();
                StartCoroutine(director.DisableButton());
                director.SetText(txtExplainInstantCard);

                yield return new WaitForSeconds(1f);
                // Setup Arrow
                List<CArrowManager.tArrowData> lstArrowData = new List<CArrowManager.tArrowData>();
                CArrowManager.tArrowData arrowData = new CArrowManager.tArrowData(new Vector3(-235f, -100f, 0f), CArrowManager.DIRECTION.BOTTOM);
                lstArrowData.Add(arrowData);
                yield return StartCoroutine(director.ArrowManager.EnableArrows(lstArrowData));
                yield return StartCoroutine(director.ArrowManager.ScaleActiveArrows(Vector3.one, 0.5f));
                lstArrowData.Clear();

                yield return new WaitForSeconds(2f);
                StartCoroutine(SetupScene_CR(CTutorialDirector.TS_PEEKATPEPTIDE.WFI_CLICKRESEARCHCARD));
            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.WFI_CLICKRESEARCHCARD:
            {
                uiMan.EnableAllBoxCollidersForBeginTurn();
            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.EXPLAIN_RESEARCH_CARD:
            {

            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.WFI_CLICKPEPTIDE:
            {
                
            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.EXPLAIN_PEEKEDPEPTIDE:
            {

            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.ENDTURN_HIGHLIGHTOTHERPLAYERS:
            {

            }
            break;

            case CTutorialDirector.TS_PEEKATPEPTIDE.ENDSCENE:
            {

            }
            break;
        }

        CurrentScene = nextScene;
	}
}
