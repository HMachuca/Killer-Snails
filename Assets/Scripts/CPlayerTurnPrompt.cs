using UnityEngine;
using System.Collections;

public class CPlayerTurnPrompt : MonoBehaviour
{
    private bool bActiveUpdate = false;
    public UILabel uiText;
    public UIButton uiButton;
    private UIPanel uiPanel;


    void Start() {
        uiPanel = GetComponent<UIPanel>();
    }

    private void SetText() {
       //  = (int)CGameManager.instance.activePlayer.player + 1;
        PlayerData.PLAYER player = CGlobals.GetNextPlayer(CGameManager.instance.activePlayer.player);
        uiText.text = "Begin Player Turn";// + nPlayer.ToString() + "'s Turn";

        if (CGameManager.instance.Players[(int)player].AIControlled)
            uiText.text = "Begin Opponent Turn";

        //switch(CGameManager.instance.activePlayer.player) {
        //    case PlayerData.PLAYER.ONE:   uiText.color = CGlobals.Player1Color; break;
        //    case PlayerData.PLAYER.TWO:   uiText.color = CGlobals.Player2Color; break;
        //    case PlayerData.PLAYER.THREE: uiText.color = CGlobals.Player3Color; break;
        //    case PlayerData.PLAYER.FOUR:  uiText.color = CGlobals.Player4Color; break;
        //}
    }

    //public void OnContinueButton_Clicked() { StartCoroutine(OnContinueButton_ClickedCR()); }
    //public IEnumerator OnContinueButton_ClickedCR() {
    //    yield return new WaitForEndOfFrame();

    //}

	public IEnumerator ShowPanel() {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(true);
        uiButton.GetComponent<BoxCollider>().enabled = true;
        SetText();
        yield return StartCoroutine(FadeInPanel());
    }
    public IEnumerator ShowPanelInstantly() {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(true);
        uiButton.GetComponent<BoxCollider>().enabled = true;
        SetText();
        yield return StartCoroutine(FadeInPanel(true));
    }

    public void HidePanel() { StartCoroutine(HidePanelCR()); }
    public IEnumerator HidePanelCR() {
        yield return new WaitForEndOfFrame();
        uiButton.GetComponent<BoxCollider>().enabled = false;

        yield return CGameManager.instance.ChangePlayer_SetNextTurn();

        StartCoroutine(CAssessmentPanel.instance.ShowPanel(false));
        yield return CGameManager.instance.BeginNextTurn_CR();
        
        uiButton.GetComponent<BoxCollider>().enabled = true;
        gameObject.SetActive(false);
    }

    public IEnumerator FadeInPanel(bool instantly = false) {
        if(instantly)
            CGlobals.iTweenValue(gameObject, 0, 1, 0f, gameObject, "UpdateButtonUIPanelAlpha", iTween.EaseType.linear);
        else
            CGlobals.iTweenValue(gameObject, 0, 1, 0.5f, gameObject, "UpdateButtonUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator FadeOutPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, 0.5f, gameObject, "UpdateButtonUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(0.5f);
    }

    public void UpdateButtonUIPanelAlpha(float value) {
        uiPanel.alpha = value;
    }
}
