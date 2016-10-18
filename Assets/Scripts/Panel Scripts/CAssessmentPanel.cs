using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct tAssessmentQuestion
{
    public string question;
    public List<string> answers;
    public int correctAnswer;
}

[System.Serializable]
public struct tAssessmentAnswer
{
    public Transform parent;
    public UILabel uiText;
    public UIButton button;
    public UISprite sprite;
}



public class CAssessmentPanel : CBasePanel
{
    #region Singleton Instance
    private static CAssessmentPanel _instance;
    public static CAssessmentPanel instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CAssessmentPanel>();
            }
            return _instance;
        }
    }
    #endregion

    [SerializeField]
    public class tQuestionCounters
    {
        public List<int> lstSnailCounters;
        public List<int> lstInstantCounters;
        public List<int> lstPreyCounters;

        public tQuestionCounters()
        {
            lstSnailCounters = new List<int>((int)CardData.SnailSpecies.Conus_Victoriae + 1);
            for (int i = 0; i < (int)CardData.SnailSpecies.Conus_Victoriae; ++i)
                lstSnailCounters.Add(0);

            lstInstantCounters = new List<int>((int)CardData.InstantType.Turtle + 1);
            for(int i = 0; i < (int)CardData.InstantType.Turtle; ++i)
                lstInstantCounters.Add(0);

            lstPreyCounters = new List<int>((int)CardData.PreyName.Venus_Comb_Murex + 1);
            for(int i = 0; i < (int)CardData.PreyName.Venus_Comb_Murex; ++i)
                lstPreyCounters.Add(0);
        }

        public void IncrementSnailCounter(CardData.SnailSpecies snailSpecies) {
            lstSnailCounters[(int)snailSpecies]++;
            if (lstSnailCounters[(int)snailSpecies] >= 5)
                lstSnailCounters[(int)snailSpecies] = 0;
        }
        public void IncrementInstantCounter(CardData.InstantType instantType) {
            lstInstantCounters[(int)instantType]++;
            if (lstInstantCounters[(int)instantType] >= 4)
                lstInstantCounters[(int)instantType] = 0;
        }
        public void IncrementPreyCounter(CardData.PreyName preyName) {
            lstPreyCounters[(int)preyName]++;
            if (lstPreyCounters[(int)preyName] >= 1)
                lstPreyCounters[(int)preyName] = 0;
        }
    }

    public tQuestionCounters QuestionCounters;
    public tAssessmentQuestion CurrentQuestion;
    public GameObject goQuestionParent;
    public UILabel uiQuestionText;
    public CPlayerTurnPrompt PlayerTurnPromptPanel;
    public tAssessmentQuestion firstPeptideExtractedQuestion;
    public List<tAssessmentAnswer> lstAnswers;
    public AudioClip acCorrect, acIncorrect, acWaveIn, acWaveOut, acQuestionIn, acQuestionOut;
    private Color defaultButtonColor;
    private List<Vector3> lstButtonPositions_Three;
    private List<Vector3> lstButtonPositions_Four;
    public bool firstAnswer; // used to score each player's accuracy in Win Screen
    public int totalQuestionsAsked = 0;
    
    // Questions about peptides
    public List<tAssessmentQuestion> lstPeptideQuestions;

    public List<CBaseCard> lstLastPlayedCards;

    void Awake()
    {
        QuestionCounters = new tQuestionCounters();
        defaultButtonColor = new Color(0f,0f,0f,0.6f);

        lstButtonPositions_Four = new List<Vector3>(4);
        lstButtonPositions_Four.Add(new Vector3(-280f, -60f, 0f));
        lstButtonPositions_Four.Add(new Vector3(280f, -60f, 0f));
        lstButtonPositions_Four.Add(new Vector3(-280f, -240f, 0f));
        lstButtonPositions_Four.Add(new Vector3(280f, -240f, 0f));

        lstButtonPositions_Three = new List<Vector3>(3);
        lstButtonPositions_Three.Add(new Vector3(0f, -75f, 0f));
        lstButtonPositions_Three.Add(new Vector3(-280f, -250f, 0f));
        lstButtonPositions_Three.Add(new Vector3(280f, -250f, 0f));

        firstAnswer = false;
    }

	//IEnumerator Start ()
 //   {
 //       base.Start();

 //       yield return new WaitForSeconds(2.0f);
 //       StartCoroutine(ShowPanel());
	//}

    IEnumerator SetQuestionText(string text)
    {
        uiQuestionText.text = text;
        CGlobals.UpdateWidgets();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator SetAnswerText(List<string> answers, int correctAnswer)
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < answers.Count; ++i)
        {
            lstAnswers[i].uiText.text = answers[i];
            lstAnswers[i].sprite.GetComponent<BoxCollider>().enabled = true;
            //lstAnswers[i].button.pressed = color;
            //lstAnswers[i].button.hover = color;
        }

        if (answers.Count == 3) {
            lstAnswers[3].parent.gameObject.SetActive(false);
            for (int i = 0; i < answers.Count; ++i)
                lstAnswers[i].parent.localPosition = lstButtonPositions_Three[i];
        }
        else if (answers.Count == 4) {
            lstAnswers[3].parent.gameObject.SetActive(true);
            for (int i = 0; i < answers.Count; ++i)
                lstAnswers[i].parent.localPosition = lstButtonPositions_Four[i];
        }
    }

    public IEnumerator ShowPanel(bool show = true)
    {
        if(show)
        {
            goQuestionParent.SetActive(true);
            anchorPanelBackground.GetComponent<UITexture>().enabled = true;
            CAudioManager.instance.PlaySound(acWaveIn);
            firstAnswer = true;
            totalQuestionsAsked++;

            CGlobals.TweenMove(anchorPanelBackground.gameObject, "y", 700f, 1.5f, iTween.EaseType.easeInOutSine, true);
            yield return new WaitForSeconds(1.5f);

            // Need int for every card type's current flavor text
            Debug.Log(lstLastPlayedCards[0].name);
            CBaseCard card = lstLastPlayedCards[0];
            int question = 0;
            switch(card.cardType)
            {
                case CardData.CardType.Snail:
                {
                    CSnailCard snailCard = (CSnailCard)card;
                    question = QuestionCounters.lstSnailCounters[(int)snailCard.snailSpecies];
                    //QuestionCounters.IncrementSnailCounter(snailCard.snailSpecies);
                }
                break;

                case CardData.CardType.Instant:
                {
                    CInstantCard instantCard = (CInstantCard)card;
                    question = QuestionCounters.lstInstantCounters[(int)instantCard.instantType];
                    //QuestionCounters.IncrementInstantCounter(instantCard.instantType);
                }
                break;

                case CardData.CardType.Prey:
                {
                    CPreyCard preyCard = (CPreyCard)card;
                    question = QuestionCounters.lstPreyCounters[(int)preyCard.preyName];
                    //QuestionCounters.IncrementPreyCounter(preyCard.preyName);
                }
                break;
            }

            CurrentQuestion = card.lstAssessmentQuestions[question];

            yield return StartCoroutine(SetQuestionText(CurrentQuestion.question));
            yield return StartCoroutine(SetAnswerText(CurrentQuestion.answers, CurrentQuestion.correctAnswer));

            CGlobals.TweenScale(goQuestionParent, Vector3.one, 0.5f, iTween.EaseType.easeInOutSine, true);
            yield return new WaitForSeconds(0.1f);
            CAudioManager.instance.PlaySound(acQuestionIn);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            CAudioManager.instance.PlaySound(acWaveOut);
            yield return new WaitForSeconds(0.5f);

            CGlobals.TweenMove(anchorPanelBackground.gameObject, "y", -500f, 1.5f, iTween.EaseType.easeInOutSine, true);
            yield return new WaitForSeconds(1.5f);

            StartCoroutine(PlayerTurnPromptPanel.FadeOutPanel());
            anchorPanelBackground.GetComponent<UITexture>().enabled = false;
            lstLastPlayedCards.Clear();
        }
	}

    //private void UpdateQuestionBGAlpha(Color color) { uiQuestionBG.color = color; }
    //private void UpdateQuestionTextAlpha(Color color) { uiQuestionText.color = color; }

    public void AnswerQuestion_1() { StartCoroutine(AnswerQuestion_1CR()); }
    public IEnumerator AnswerQuestion_1CR()
    {
        yield return new WaitForEndOfFrame();
        int i = 0;
        lstAnswers[i].button.enabled = false;
        lstAnswers[i].button.GetComponent<BoxCollider>().enabled = false;
        bool correct = (CurrentQuestion.correctAnswer == i);

        if(correct) {
            CAudioManager.instance.PlaySound(acCorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, new Color(0f, 0.85f, 0f, 1f), 0.25f, this.gameObject, "ChangeAnswer1Color");

            if (firstAnswer)
                CGameManager.instance.activePlayer.numberCorrectlyAnsweredQuestions++;
            firstAnswer = false;
        }
        else {
            CAudioManager.instance.PlaySound(acIncorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, Color.red, 0.25f, this.gameObject, "ChangeAnswer1Color");
            firstAnswer = false;
        }

        yield return StartCoroutine(FinishAnswer_CR(correct));
    }

    public void AnswerQuestion_2() { StartCoroutine(AnswerQuestion_2CR()); }
    public IEnumerator AnswerQuestion_2CR()
    {
        yield return new WaitForEndOfFrame();
        int i = 1;
        lstAnswers[i].button.enabled = false;
        lstAnswers[i].button.GetComponent<BoxCollider>().enabled = false;
        bool correct = (CurrentQuestion.correctAnswer == i);

        if(correct) {
            CAudioManager.instance.PlaySound(acCorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, new Color(0f, 0.85f, 0f, 1f), 0.25f, this.gameObject, "ChangeAnswer2Color");

            if (firstAnswer)
                CGameManager.instance.activePlayer.numberCorrectlyAnsweredQuestions++;
            firstAnswer = false;
        }
        else {
            CAudioManager.instance.PlaySound(acIncorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, Color.red, 0.25f, this.gameObject, "ChangeAnswer2Color");
            firstAnswer = false;
        }
        yield return StartCoroutine(FinishAnswer_CR(correct));
    }

    public void AnswerQuestion_3() { StartCoroutine(AnswerQuestion_3CR()); }
    public IEnumerator AnswerQuestion_3CR()
    {
        yield return new WaitForEndOfFrame();
        int i = 2;
        lstAnswers[i].button.enabled = false;
        lstAnswers[i].button.GetComponent<BoxCollider>().enabled = false;
        bool correct = (CurrentQuestion.correctAnswer == i);

        if(correct) {
            CAudioManager.instance.PlaySound(acCorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, new Color(0f, 0.85f, 0f, 1f), 0.25f, this.gameObject, "ChangeAnswer3Color");

            if (firstAnswer)
                CGameManager.instance.activePlayer.numberCorrectlyAnsweredQuestions++;
            firstAnswer = false;
        }
        else {
            CAudioManager.instance.PlaySound(acIncorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, Color.red, 0.25f, this.gameObject, "ChangeAnswer3Color");
            firstAnswer = false;
        }
        yield return StartCoroutine(FinishAnswer_CR(correct));
    }

    public void AnswerQuestion_4() { StartCoroutine(AnswerQuestion_4CR()); }
    public IEnumerator AnswerQuestion_4CR()
    {
        yield return new WaitForEndOfFrame();
        int i = 3;
        lstAnswers[i].button.enabled = false;
        lstAnswers[i].button.GetComponent<BoxCollider>().enabled = false;
        bool correct = (CurrentQuestion.correctAnswer == i);

        if(correct) {
            CAudioManager.instance.PlaySound(acCorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, new Color(0f, 0.85f, 0f, 1f), 0.25f, this.gameObject, "ChangeAnswer4Color");

            if (firstAnswer)
                CGameManager.instance.activePlayer.numberCorrectlyAnsweredQuestions++;
            firstAnswer = false;
        }
        else {
            CAudioManager.instance.PlaySound(acIncorrect);
            CGlobals.iTweenValue(this.gameObject, lstAnswers[i].sprite.color, Color.red, 0.25f, this.gameObject, "ChangeAnswer4Color");
            firstAnswer = false;
        }
        yield return StartCoroutine(FinishAnswer_CR(correct));
    }

    public IEnumerator FinishAnswer_CR(bool correct = false)
    {
        if(correct)
        {
            for (int i = 0; i < lstAnswers.Count; ++i) {
                lstAnswers[i].button.enabled = false;
                lstAnswers[i].button.GetComponent<BoxCollider>().enabled = false;
            }
            yield return new WaitForSeconds(1f);

            CGlobals.TweenScale(goQuestionParent, Vector3.one * 1.1f, 0.35f, iTween.EaseType.easeInSine, true);
            yield return new WaitForSeconds(0.35f);
            CAudioManager.instance.PlaySound(acQuestionOut);
            CGlobals.TweenScale(goQuestionParent, Vector3.zero, 0.25f, iTween.EaseType.easeInSine, true);
            yield return new WaitForSeconds(0.25f);

            // Reset
            for (int i = 0; i < lstAnswers.Count; ++i) {
                lstAnswers[i].sprite.color = defaultButtonColor;
                lstAnswers[i].button.GetComponent<BoxCollider>().enabled = true;
                lstAnswers[i].button.enabled = true;
            }

            goQuestionParent.SetActive(false);

            //yield return StartCoroutine(PlayerTurnPromptPanel.ShowPanelInstantly());
            yield return StartCoroutine(PlayerTurnPromptPanel.ShowPanel());


            //yield return ShowPanel(false);
            //yield return StartCoroutine(CGameManager.instance.BeginNextTurn_CR());
        }
    }



    public void ChangeAnswer1Color(Color color) { lstAnswers[0].sprite.color = color; }
    public void ChangeAnswer2Color(Color color) { lstAnswers[1].sprite.color = color; }
    public void ChangeAnswer3Color(Color color) { lstAnswers[2].sprite.color = color; }
    public void ChangeAnswer4Color(Color color) { lstAnswers[3].sprite.color = color; }
}
