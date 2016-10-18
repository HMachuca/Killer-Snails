using UnityEngine;
using System.Collections;

public class CMainMenu : MonoBehaviour 
{
    #region Singleton Instance
    private static CMainMenu _instance;
    public static CMainMenu instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CMainMenu>();
            }
            return _instance;
        }
    }
    #endregion

    public enum MM_PANELS { Main, Options, PlayMode, PlayerSelect, Tutorial }
    public MM_PANELS CurrentPanel;

    public UIPanel uiMainPanel, uiPlayButtonPanel, uiPlayersSelectPanel, uiOptionsPanel;
    public UIPanel uiPlayModePanel, uiTutorialPanel;

    public GameObject goMainMenuObjs;       // Contains all gameobjects
    public UISprite uiPlayBttn, uiEventsBttn, uiOptionsBttn;
    public UIButton play2, play3, play4;

    public float fFadingAlphaDuration;

	bool bGameStarted = false;
	public GameObject goMainGame;
    public GameObject goTutorialDirector;

    public AudioSource audioSrc;
    public AudioClip acMenu1, acMenu2;

    // Option Menu
    public UISprite musicButton, sfxButton;
	

	void Start () 
	{
        CurrentPanel = MM_PANELS.Main;
        CAudioManager.instance.PlayMusic(acMenu1);

        // Check UserSettings for Audio
        CGlobals.TweenScale(musicButton.gameObject, Vector3.one, 0.1f, iTween.EaseType.linear, true);
        CGlobals.TweenScale(sfxButton.gameObject, Vector3.one, 0.1f, iTween.EaseType.linear, true);
        string musicSprite = (UserSettings.MusicEnabled) ? "Button_On" : "Button_Off";
        string sfxSprite = (UserSettings.SFXEnabled) ? "Button_On" : "Button_Off";

        musicButton.spriteName = musicSprite;
        musicButton.GetComponent<UIButton>().SetSpriteForAllButtonStates(musicSprite);

        sfxButton.spriteName = sfxSprite;
        sfxButton.GetComponent<UIButton>().SetSpriteForAllButtonStates(sfxSprite);
	}

	void Update () 
	{
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            audioSrc.clip = acMenu1;
            audioSrc.Play();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            audioSrc.clip = acMenu2;
            audioSrc.Play();
        }

	}

    public void PlayButton() { StartCoroutine(PlayButton_CR()); }
    public IEnumerator PlayButton_CR()
    {
        CGlobals.TweenScale(uiPlayBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.2f);
        CGlobals.TweenScale(uiPlayBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.125f);

        CGlobals.TweenScale(uiEventsBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(uiOptionsBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.2f);

        CGlobals.TweenScale(uiEventsBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(uiOptionsBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.3f);

        bGameStarted = true;
        yield return StartCoroutine(StartGame());
    }

    public void TutorialButton() { StartCoroutine(TutorialButton_CR()); }
    public IEnumerator TutorialButton_CR()
    {
        CGlobals.TweenScale(uiEventsBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.2f);
        CGlobals.TweenScale(uiEventsBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.125f);

        CGlobals.TweenScale(uiPlayBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(uiOptionsBttn.gameObject, Vector3.one * 1.2f, 0.2f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.2f);

        CGlobals.TweenScale(uiPlayBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenScale(uiOptionsBttn.gameObject, Vector3.zero, 0.15f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(StartTutorial_CR());
    }

    public void StartTutorial() { StartCoroutine(StartTutorial_CR()); }
    private IEnumerator StartTutorial_CR()
    {
        //goTutorialDirector.SetActive(true);
        //UserSettings.NumberOfPlayers = 4;
        //CGlobals.TUTORIAL_ACTIVE = true;

        yield return FadeOutButtonUIPanel();
        yield return FadeInTutorialPanel();
        CurrentPanel = MM_PANELS.Tutorial;
    }

    private IEnumerator StartGame()
    {
        yield return StartCoroutine(FadeOutPlayersSelectUIPanel());

        // Activating the gameObject "Main Game" will wake up CGameManager.cs and it will handle the game creation
        goMainGame.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        yield return CGameManager.instance.WakeUp();

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeOutMainUIPanel());
    }

    #region NGUI Button Targets


    public void OnPlayButton_Clicked() { StartCoroutine(OnPlayButton_ClickedCR()); }
    public IEnumerator OnPlayButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutButtonUIPanel());
        yield return StartCoroutine(FadeInPlayModePanel());
        CurrentPanel = MM_PANELS.PlayMode;
    }

    public void OnEventsButton_Clicked()
    {

    }

    public void OnOptionsButton_Clicked() { StartCoroutine(OnOptionsButton_ClickedCR()); }
    public IEnumerator OnOptionsButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutButtonUIPanel());
        yield return StartCoroutine(FadeInOptionsPanel());
        CurrentPanel = MM_PANELS.Options;
    }

    public void OnOptionsBackButton_Clicked() { StartCoroutine(OnOptionsBackButton_ClickedCR()); }
    public IEnumerator OnOptionsBackButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutOptionsPanel());
        yield return StartCoroutine(FadeInButtonUIPanel());
        CurrentPanel = MM_PANELS.Main;
    }


    // Play Mode Select
    public void OnSelectPlayVSComputer_Clicked() { StartCoroutine(OnSelectPlayVSComputer_ClickedCR()); }
    public IEnumerator OnSelectPlayVSComputer_ClickedCR()
    {
        CGlobals.PLAY_VS_COMPUTER = true;

        yield return StartCoroutine(FadeOutPlayModePanel());
        yield return StartCoroutine(FadeInPlayersSelectUIPanel());
        CurrentPanel = MM_PANELS.PlayerSelect;
    }

    public void OnSelectMultiplayerButton_Clicked() { StartCoroutine(OnSelectMultiplayerButton_ClickedCR()); }
    public IEnumerator OnSelectMultiplayerButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutPlayModePanel());
        yield return StartCoroutine(FadeInPlayersSelectUIPanel());
        CurrentPanel = MM_PANELS.PlayerSelect;
    }

    // Tutorial
    public void OnTutorialBackButton_Clicked() { StartCoroutine(OnTutorialBackButton_ClickedCR()); }
    public IEnumerator OnTutorialBackButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutTutorialPanel());
        yield return StartCoroutine(FadeInButtonUIPanel());
        CurrentPanel = MM_PANELS.Main;
    }

    public void OnPlayModeBackButton_Clicked() { StartCoroutine(OnPlayModeBackButton_ClickedCR()); }
    public IEnumerator OnPlayModeBackButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutPlayModePanel());
        yield return StartCoroutine(FadeInButtonUIPanel());
        CurrentPanel = MM_PANELS.Main;
    }

    public void OnPlayerSelectBackButton_Clicked() { StartCoroutine(OnPlayerSelectBackButton_ClickedCR()); }
    public IEnumerator OnPlayerSelectBackButton_ClickedCR()
    {
        yield return StartCoroutine(FadeOutPlayersSelectUIPanel());
        yield return StartCoroutine(FadeInPlayModePanel());
        CurrentPanel = MM_PANELS.Main;
    }


    #endregion


    #region Alpha Coroutines 

    public IEnumerator FadeInMainUIPanel() {
        uiMainPanel.gameObject.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdateMainUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutMainUIPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdateMainUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiMainPanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeInButtonUIPanel() {
        uiPlayButtonPanel.gameObject.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdatePlayButtonUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutButtonUIPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdatePlayButtonUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiPlayButtonPanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeInPlayersSelectUIPanel() {
        uiPlayersSelectPanel.gameObject.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdatePlayersSelectUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutPlayersSelectUIPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdatePlayersSelectUIPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiPlayersSelectPanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeInOptionsPanel() {
        uiOptionsPanel.gameObject.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdateOptionsPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutOptionsPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdateOptionsPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiOptionsPanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeInPlayModePanel() {
        uiOptionsPanel.gameObject.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdatePlayModePanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutPlayModePanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdatePlayModePanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiOptionsPanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeInTutorialPanel() {
        uiTutorialPanel.gameObject.SetActive(true);
        uiTutorialPanel.GetComponent<CTutorial>().TutorialPrompt.SetActive(true);
        CGlobals.iTweenValue(gameObject, 0, 1, fFadingAlphaDuration, gameObject, "UpdateTutorialPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
    }
    public IEnumerator FadeOutTutorialPanel() {
        CGlobals.iTweenValue(gameObject, 1, 0, fFadingAlphaDuration, gameObject, "UpdateTutorialPanelAlpha", iTween.EaseType.linear);
        yield return new WaitForSeconds(fFadingAlphaDuration);
        uiTutorialPanel.GetComponent<CTutorial>().ResetItemsPosition();
        uiTutorialPanel.gameObject.SetActive(false);
    }
    
	public void UpdateMainUIPanelAlpha(float alpha)             { uiMainPanel.alpha = alpha; }
    public void UpdatePlayButtonUIPanelAlpha(float alpha)       { uiPlayButtonPanel.alpha = alpha; }
    public void UpdatePlayersSelectUIPanelAlpha(float alpha)    { uiPlayersSelectPanel.alpha = alpha; }
    public void UpdateOptionsPanelAlpha(float alpha)            { uiOptionsPanel.alpha = alpha; }
    public void UpdatePlayModePanelAlpha(float alpha)           { uiPlayModePanel.alpha = alpha; }
    public void UpdateTutorialPanelAlpha(float alpha)           { uiTutorialPanel.alpha = alpha; }

    #endregion

    public void EnablePlayerSelectButtons()
    {
        play2.enabled = true;
        play3.enabled = true;
        play4.enabled = true;
    }

    public void SetNumberOfPlayers_2() { StartCoroutine(SetNumberOfPlayers_2CR()); }
    public IEnumerator SetNumberOfPlayers_2CR()
    {
        yield return new WaitForEndOfFrame();
        UserSettings.NumberOfPlayers = 2;
        play2.enabled = false;
        StartCoroutine(StartGame());
    }

    public void SetNumberOfPlayers_3() { StartCoroutine(SetNumberOfPlayers_3CR()); }
    public IEnumerator SetNumberOfPlayers_3CR()
    {
        yield return new WaitForEndOfFrame();
        UserSettings.NumberOfPlayers = 3;
        play3.enabled = false;
        StartCoroutine(StartGame());
    }

    public void SetNumberOfPlayers_4() { StartCoroutine(SetNumberOfPlayers_4CR()); }
    public IEnumerator SetNumberOfPlayers_4CR()
    {
        yield return new WaitForEndOfFrame();
        UserSettings.NumberOfPlayers = 4;
        play4.enabled = false;
        StartCoroutine(StartGame());
    }

    public void ToggleMusic()
    {
        AudioSource src = CAudioManager.instance.MusicSource;
        if (src.isPlaying)
            src.Stop();
        else
            src.Play();
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
}
