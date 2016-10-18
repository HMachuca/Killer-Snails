using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTS_Intro : CTutorialScene
{
    public CTutorialDirector.TS_INTRO CurrentScene;
    public string txtWelcome, txtExplainGoal;
    


	public override IEnumerator StartScene()
    {
        yield return StartCoroutine(base.StartScene());

        timeForScene = 3f;
        SceneType = CTutorialDirector.TutorialScene.TS_INTRO;
        StartCoroutine(SetupScene_CR(CTutorialDirector.TS_INTRO.SETUP_HAND));
        StartCoroutine(CTutorialDirector.instance.ExpandTextbox());
	}


    public override IEnumerator AdvanceScene()
    {
        yield return StartCoroutine(base.AdvanceScene());

        CurrentScene = CurrentScene + 1;
        yield return SetupScene_CR(CurrentScene);
    }
	

	public override IEnumerator SetupScene_CR(CTutorialDirector.TS_INTRO nextScene)
    {
        yield return StartCoroutine(base.SetupScene_CR(nextScene));

        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CTutorialDirector director = CTutorialDirector.instance;

        switch(nextScene)
        {
            case CTutorialDirector.TS_INTRO.SETUP_HAND:
            {
                nextScene = CTutorialDirector.TS_INTRO.SHOW_WELCOME_TEXT;
                StartCoroutine(SetupScene_CR(nextScene));
            }
            break;

            case CTutorialDirector.TS_INTRO.SHOW_WELCOME_TEXT:
            {
                yield return StartCoroutine(ShowWelcomeText());
            }
            break;

            case CTutorialDirector.TS_INTRO.EXPLAIN_GOAL:
            {
                yield return StartCoroutine(ExplainGoal());
            }
            break;

            case CTutorialDirector.TS_INTRO.ENDSCENE:
            {
                yield return StartCoroutine(director.ArrowManager.DisableArrows());
                CTutorialDirector.instance.StartNextScene();
            }
            break;
        }

        CurrentScene = nextScene;
	}

    public IEnumerator ShowWelcomeText()
    {
        StartCoroutine(CTutorialDirector.instance.DisableButton());
        CTutorialDirector.instance.SetText(txtWelcome);
        CUIManager.instance.DisableAllCollidersForEndTurn();

        yield return new WaitForSeconds(3f);
        StartCoroutine(CTutorialDirector.instance.EnableButton());
    }

    public IEnumerator ExplainGoal()
    {
        CTutorialDirector director = CTutorialDirector.instance;
        director.SetText(txtExplainGoal);
        StartCoroutine(CTutorialDirector.instance.DisableButton());
        //CUIManager.instance.cabalPanel.transform.parent = CTutorialDirector.instance.uiPanel.transform;
        CGlobals.UpdateWidgets();

        #region Arrow Movement

        float leftPos = -330f, rightPos = -380f;
        float duration = 0.5f;


        List<CArrowManager.tArrowData> lstArrowData = new List<CArrowManager.tArrowData>();
        CArrowManager.tArrowData arrowData1 = new CArrowManager.tArrowData(new Vector3(-380f, 125f, 0f), CArrowManager.DIRECTION.LEFT);
        CArrowManager.tArrowData arrowData2 = new CArrowManager.tArrowData(new Vector3(-380f, 50f, 0f), CArrowManager.DIRECTION.LEFT);
        CArrowManager.tArrowData arrowData3 = new CArrowManager.tArrowData(new Vector3(-380f, -35f, 0f), CArrowManager.DIRECTION.LEFT);
        lstArrowData.Add(arrowData1);
        lstArrowData.Add(arrowData2);
        lstArrowData.Add(arrowData3);

        yield return StartCoroutine(director.ArrowManager.EnableArrows(lstArrowData));
        lstArrowData.Clear();

        yield return StartCoroutine(director.ArrowManager.ScaleActiveArrows(Vector3.one, 0.5f));

        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", leftPos, duration));
        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", rightPos, duration));

        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", leftPos, duration));
        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", rightPos, duration));

        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", leftPos, duration));
        yield return StartCoroutine(director.ArrowManager.MoveActiveArrows("x", rightPos, duration));

        #endregion

        yield return new WaitForSeconds(.5f);
        StartCoroutine(CTutorialDirector.instance.EnableButton());
    }

}
