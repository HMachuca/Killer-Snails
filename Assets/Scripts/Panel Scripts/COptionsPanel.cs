using UnityEngine;
using System.Collections;

public class COptionsPanel : CBasePanel
{
    public UISprite musicButton, sfxButton, backButton;
    

    private IEnumerator Initialize()
    {
        // Check UserSettings for Audio
        CGlobals.TweenScale(musicButton.gameObject, Vector3.one, 0.1f, iTween.EaseType.linear, true);
        CGlobals.TweenScale(sfxButton.gameObject, Vector3.one, 0.1f, iTween.EaseType.linear, true);
        string musicSprite = (UserSettings.MusicEnabled) ? "Button_On" : "Button_Off";
        string sfxSprite = (UserSettings.SFXEnabled) ? "Button_On" : "Button_Off";

        musicButton.spriteName = musicSprite;
        musicButton.GetComponent<UIButton>().SetSpriteForAllButtonStates(musicSprite);

        sfxButton.spriteName = sfxSprite;
        sfxButton.GetComponent<UIButton>().SetSpriteForAllButtonStates(sfxSprite);

        yield return new WaitForSeconds(0.1f);
    }

    public void UpdateAnchors()
    {
        //musicButton.leftAnchor.absolute = 0;
        //musicButton.rightAnchor.absolute = 0;
        //sfxButton.leftAnchor.absolute = 0;
        //sfxButton.rightAnchor.absolute = 0;
    }

    public void ShowPanel() { StartCoroutine(ShowPanel_CR()); }
    public IEnumerator ShowPanel_CR()
    {
        StartCoroutine(FadeBG(true, 0.5f));
       /* yield return */StartCoroutine(Initialize());

        iTween.ScaleTo(anchorPanelBackground.gameObject, iTween.Hash("scale", Vector3.one,
                                                            "time", 0.5f,
                                                            "islocal", true,
                                                            "easetype", iTween.EaseType.easeInSine,
                                                            "OnUpdate", "UpdateAnchors",
                                                            "onupdatetarget", this.gameObject));
        yield return new WaitForSeconds(0.5f);
    }

    public void HidePanel() { StartCoroutine(HidePanel_CR()); }
    public IEnumerator HidePanel_CR()
    {
        StartCoroutine(FadeBG(false, 1f));
        CGlobals.TweenScale(anchorPanelBackground.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.5f);
    }
    
	public void ToggleMusicEnabled() { StartCoroutine(ToggleMusicEnabled_CR()); }
    public IEnumerator ToggleMusicEnabled_CR()
    {
        bool toEnable = (UserSettings.MusicEnabled) ? false : true;
        musicButton.GetComponent<BoxCollider>().enabled = false;
        
        if(toEnable)
            yield return StartCoroutine(ToggleButton(musicButton, "Button_On"));
        else
            yield return StartCoroutine(ToggleButton(musicButton, "Button_Off"));
        
        musicButton.GetComponent<BoxCollider>().enabled = true;
        UserSettings.SetMusicEnabled(toEnable);
    }


    public void ToggleSFXEnabled() { StartCoroutine(ToggleSFXEnabled_CR()); }
    public IEnumerator ToggleSFXEnabled_CR()
    {
        bool toEnable = (UserSettings.SFXEnabled) ? false : true;
        sfxButton.GetComponent<BoxCollider>().enabled = false;
        
        if(toEnable)
            yield return StartCoroutine(ToggleButton(sfxButton, "Button_On"));
        else
            yield return StartCoroutine(ToggleButton(sfxButton, "Button_Off"));
        
        sfxButton.GetComponent<BoxCollider>().enabled = true;
        UserSettings.SetSFXEnabled(toEnable);
    }

    public IEnumerator ToggleButton(UISprite sprite, string spriteName)
    {
        CGlobals.TweenScale(sprite.gameObject, Vector3.zero, 0.2f, iTween.EaseType.linear, true);
        yield return new WaitForSeconds(0.2f);

        sprite.spriteName = spriteName;
        sprite.GetComponent<UIButton>().SetSpriteForAllButtonStates(spriteName);

        CGlobals.TweenScale(sprite.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.25f);

        CGlobals.TweenScale(sprite.gameObject, Vector3.one, 0.1f, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator FadeBG(bool fadeIn = true, float duration = 1f)
    {
        uiFadeSprite.enabled = true;
        Color spriteColor = (fadeIn == true ? new Color(0f, 0f, 0f, CGlobals.FADE_BG_ALPHA) : Color.clear);
        CGlobals.iTweenValue(this.gameObject, uiFadeSprite.color, spriteColor, duration, this.gameObject, "OnUpdateFadeBGColor", iTween.EaseType.easeOutCubic, "OnUpdateBGColorComplete");
        yield return new WaitForSeconds(duration);
    }

    public void OnUpdateFadeBGColor(Color colorTo) { uiFadeSprite.color = colorTo; }
    public void OnUpdateBGColorComplete()
    {
        uiFadeSprite.enabled = (uiFadeSprite.alpha <= 0.5f) ? false : true;
    }

    public void OnMainMenuButtonClicked() { StartCoroutine(OnMainMenuButtonClicked_CR()); }
    public IEnumerator OnMainMenuButtonClicked_CR()
    {
        // Fade in Main Menu
        yield return CMainMenu.instance.FadeInMainUIPanel();

        // Hide Options panel
        yield return HidePanel_CR();

        // Delete cards and peptides
        // Clear all lists containing cards
        CCardManager.instance.DeleteDeck();
        CUIManager.instance.marketPanel.lstMarketCards.Clear();

        // Remove created players
        CGameManager.instance.RemoveCreatedPlayers();
        foreach (Transform tr in CCabalManager.instance.gridStars1.transform)
            Destroy(tr.gameObject);
        foreach (Transform tr in CCabalManager.instance.gridStars2.transform)
            Destroy(tr.gameObject);
        foreach (Transform tr in CCabalManager.instance.gridStars3.transform)
            Destroy(tr.gameObject);

        // Reset UI
        CUIManager.instance.ResetUI();
        CMainMenu.instance.EnablePlayerSelectButtons();

        // Fade in Main Buttons
        yield return CMainMenu.instance.FadeInButtonUIPanel();

        // Disable Main Game gameObject
        CGameManager.instance.gameObject.SetActive(false);
    }
}
