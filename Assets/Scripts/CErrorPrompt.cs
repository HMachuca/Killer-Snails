using UnityEngine;
using System.Collections;

public class CErrorPrompt : MonoBehaviour
{
    #region Singleton Instance
    private static CErrorPrompt _instance;
    public static CErrorPrompt instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CErrorPrompt>();
            }
            return _instance;
        }
    }
    #endregion

    public enum ERROR { DISCARD_CARD, INCREASE_STRENGTH, DECREASE_STRENGTH, WRONG_TYPE, MAX_STRENGTH };
    public GameObject ErrorBG;
    public UISprite uiFadeSprite;
    public UILabel uiLabelText;
    public string txtDiscardCard, txtDecreaseStrength, txtIncreaseStrength, txtWrongType, txtMaxStrength;


    public IEnumerator ShowError(ERROR error)
    {
        yield return new WaitForEndOfFrame();
        ErrorBG.SetActive(true);
        StartCoroutine(FadeBG(true, 0.5f));

        switch(error)
        {
            case ERROR.DISCARD_CARD: uiLabelText.text = txtDiscardCard; break;
            case ERROR.DECREASE_STRENGTH: uiLabelText.text = txtDecreaseStrength; break;
            case ERROR.INCREASE_STRENGTH: uiLabelText.text = txtIncreaseStrength; break;
            case ERROR.WRONG_TYPE: uiLabelText.text = txtWrongType; break;
            case ERROR.MAX_STRENGTH: uiLabelText.text = txtMaxStrength; break;
        }

        CGlobals.TweenScale(ErrorBG, Vector3.one * 1.15f, 0.25f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.25f);
        CGlobals.TweenScale(ErrorBG, Vector3.one, 0.15f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.15f);
    }
    
    public void HideError() { StartCoroutine(HideError_CR()); }
    public IEnumerator HideError_CR()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(FadeBG(false, 0.5f));

        CGlobals.TweenScale(ErrorBG, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.5f);
        ErrorBG.SetActive(false);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //        StartCoroutine(ShowError(ERROR.DISCARD_CARD));
    //    else if (Input.GetKeyDown(KeyCode.O))
    //        StartCoroutine(HideError_CR());
    //}

    public IEnumerator FadeBG(bool fadeIn = true, float duration = 1f)
    {
        uiFadeSprite.enabled = true;


        Color spriteColor = (fadeIn == true ? new Color(0f, 0f, 0f, CGlobals.FADE_BG_ALPHA) : Color.clear);
        CGlobals.iTweenValue(this.gameObject, uiFadeSprite.color, spriteColor, duration, this.gameObject, "OnUpdateFadeBGColor", iTween.EaseType.easeOutCubic, "OnUpdateBGColorComplete");
        yield return new WaitForSeconds(duration);

        uiFadeSprite.enabled = fadeIn;
    }

    public void OnUpdateFadeBGColor(Color colorTo) { uiFadeSprite.color = colorTo; }
    public void OnUpdateBGColorComplete()
    {
        uiFadeSprite.enabled = (uiFadeSprite.alpha == 0) ? false : true;
    }
}
