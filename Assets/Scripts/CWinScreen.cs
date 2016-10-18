using UnityEngine;
using System.Collections;

public class CWinScreen : MonoBehaviour
{
    #region Singleton Instance
    private static CWinScreen _instance;
    public static CWinScreen instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CWinScreen>();
            }
            return _instance;
        }
    }
    #endregion

    [System.Serializable]
    public struct tStar
    {
        public UISprite star, starBG, starOutline;
    }

    public GameObject ParentObj;
    public UIButton HomeButton;
    public tStar[] Stars;
    public UILabel txtWinner;
    public UILabel txtPeptidesCollected;
    public UILabel txtCabalsSolved;
    public UILabel txtCorrectAnswers;

    public void SetWinningPlayer(PlayerData.PLAYER player) { txtWinner.text = "Player " + (player + 1) + " Won!"; }
    public void SetPeptideCount(int count) { txtPeptidesCollected.text = "Peptides Collected: " + count; }
    public void SetCabalCount(int count) { txtCabalsSolved.text = "Cabals solved: " + count; }
    public void SetCorrectAnswersCount(int count) { txtCorrectAnswers.text = "You answered " + count + " out of " + CAssessmentPanel.instance.totalQuestionsAsked + " questions correctly!"; }


	void Start ()
    {
	
	}
	
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(ShowWinScreen(PlayerData.PLAYER.ONE));
        }
	}

    public IEnumerator HideWinScreen()
    {
        CGlobals.TweenMove(this.gameObject, "y", -500f, 1.0f, iTween.EaseType.easeInSine, true);

        for (int i = 0; i < 3; ++i) {
            CGlobals.iTweenValue(this.gameObject, new Color(1f, 0.85f, 0f, 1f), Color.clear, 1.0f, this.gameObject, "UpdateStar" + (i+1).ToString(), iTween.EaseType.easeInSine, null);
            CGlobals.iTweenValue(this.gameObject, new Color(1f, 0.9f, 0.47f, 1f), Color.clear, 1.0f, this.gameObject, "UpdateStarOutline" + (i+1).ToString(), iTween.EaseType.easeInSine, null);
        }

        ParentObj.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ShowWinScreen(PlayerData.PLAYER Winner)
    {
        ParentObj.gameObject.SetActive(true);
        HomeButton.enabled = false;

        SetWinningPlayer(Winner);
        CPlayer player = CGameManager.instance.activePlayer;

        // Multiplayer (TODO : Rework coming soon)
        SetPeptideCount(player.lstRevealedPeptides.Count);
        SetCabalCount(player.lstSolvedCabals.Count);
        SetCorrectAnswersCount(player.numberCorrectlyAnsweredQuestions);

        CGlobals.TweenMove(this.gameObject, "y", 700f, 1.0f, iTween.EaseType.easeInSine, true);
        
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < player.lstSolvedCabals.Count; ++i)
        {
            CGlobals.iTweenValue(this.gameObject, Color.clear, new Color(1f, 0.85f, 0f, 1f), 1.0f, this.gameObject, "UpdateStar" + (i+1).ToString(), iTween.EaseType.easeInSine, null);
            CGlobals.iTweenValue(this.gameObject, Color.clear, new Color(1f, 0.9f, 0.47f, 1f), 1.0f, this.gameObject, "UpdateStarOutline" + (i+1).ToString(), iTween.EaseType.easeInSine, null);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        HomeButton.enabled = true;
    }

    public void UpdateStar1(Color color)
    {
        Stars[0].star.color = color;
    }
    public void UpdateStarOutline1(Color color)
    {
        Stars[0].starOutline.color = color;
    }

    public void UpdateStar2(Color color)
    {
        Stars[1].star.color = color;
    }
    public void UpdateStarOutline2(Color color)
    {
        Stars[1].starOutline.color = color;
    }

    public void UpdateStar3(Color color)
    {
        Stars[2].star.color = color;
    }
    public void UpdateStarOutline3(Color color)
    {
        Stars[2].starOutline.color = color;
    }

    public void OnHomeButtonClicked() { StartCoroutine(OnHomeButtonClicked_CR()); }
    public IEnumerator OnHomeButtonClicked_CR()
    {
        // Fade in Main Menu
        yield return CMainMenu.instance.FadeInMainUIPanel();

        // Hide Options panel
        yield return HideWinScreen();

        // Delete cards and peptides
        // Clear all lists containing cards
        CCardManager.instance.DeleteDeck();
        CUIManager.instance.marketPanel.lstMarketCards.Clear();

        // Remove created players
        CGameManager.instance.RemoveCreatedPlayers();

        // Reset UI
        CUIManager.instance.ResetUI();
        CMainMenu.instance.EnablePlayerSelectButtons();

        // Fade in Main Buttons
        yield return CMainMenu.instance.FadeInButtonUIPanel();

        // Disable Main Game gameObject
        CGameManager.instance.gameObject.SetActive(false);
    }
}
