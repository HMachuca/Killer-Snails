using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class CBaseCard : MonoBehaviour 
{
    public PlayerData.PLAYER owner; // used when swapping cards with other players

	public CardData.CardType cardType;
	public bool starterCard;
    public List<Transform> lstPeptideOne;

    [SerializeField]
    public CardData.ParentPanel currentParentPanel, previousParentPanel;
    [SerializeField]
    protected int nIndexInList;

	protected string name;
	public int cost;
    public int attackRating;
    protected bool bSelected;
    protected bool bDragging;

    private UISprite cardTexture;
    private UISprite cardBackTexture;
    private UIButton cardButton;

    public float fPosXWhenSelected;

    public List<tAssessmentQuestion> lstAssessmentQuestions;
    public List<string> lstFlavorTexts;
    //private int currentFlavorText = 0;

    //public int CurrentFlavorText {
    //    get { return currentFlavorText; }
    //    private set { }
    //}

    #region Properties & Accessors

    public CardData.ParentPanel CurrentParentPanel {
        get { return currentParentPanel; }
        set { currentParentPanel = value; }
    }

    public CardData.ParentPanel PreviousParentPanel {
        get { return previousParentPanel; }
        set { previousParentPanel = value; }
    }

    public int IndexInList {
        get { return nIndexInList; }
        set { nIndexInList = value; }
    }

    public UISprite Texture {
        get { return cardTexture; }
        private set { }
    }

    public UISprite TextureBack {
        get { return cardBackTexture; }
        private set { }
    }

    public UIButton Button {
        get { return cardButton; }
        private set { }
    }

    public string GetFlavorText()
    {
        CAssessmentPanel assessment = CAssessmentPanel.instance;
        int currentFlavorText = 0;
        if(cardType == CardData.CardType.Snail) {
            assessment.QuestionCounters.IncrementSnailCounter(((CSnailCard)this).snailSpecies);
            currentFlavorText = assessment.QuestionCounters.lstSnailCounters[(int)((CSnailCard)this).snailSpecies];
        }
        else if(cardType == CardData.CardType.Instant) {
            assessment.QuestionCounters.IncrementInstantCounter(((CInstantCard)this).instantType);
            currentFlavorText = assessment.QuestionCounters.lstInstantCounters[(int)((CInstantCard)this).instantType];
        }
        else if(cardType == CardData.CardType.Prey) {
            //assessment.QuestionCounters.IncrementPreyCounter(((CPreyCard)this).preyName);
            currentFlavorText = assessment.QuestionCounters.lstPreyCounters[(int)((CPreyCard)this).preyName];
        }

        string newText = lstFlavorTexts[currentFlavorText];
        return newText;
    }

    #endregion

    protected virtual void Awake()
    {
        cardTexture = GetComponent<UISprite>();
        cardButton = GetComponent<UIButton>();

        foreach (Transform child in this.transform)
            if (child.name == "BackSide")
                cardBackTexture = child.GetComponent<UISprite>();

        lstPeptideOne = new List<Transform>();
        Transform peptide = null;
        foreach (Transform child in this.transform)
            if (child.name == "Peptides")
                peptide = child;

        if(peptide != null)
        {
            Vector3 v3Pos = peptide.localPosition;
            peptide.localPosition = v3Pos;

            foreach (Transform child in peptide)
            {
                child.GetComponent<UIWidget>().depth = 102;
                child.localPosition = Vector3.up * -200;
                lstPeptideOne.Add(child);
            }
        }

//#if UNITY_EDITOR
    }

    protected virtual void Start() { }

    public void SetButtonSound(AudioClip clip)
    {
        GetComponent<UIPlaySound>().audioClip = clip;
    }

    public virtual IEnumerator SetNewParentPanel(CardData.ParentPanel newParentPanel)
    {
        CTutorialDirector director = CTutorialDirector.instance;
        CUIManager uiMan = CUIManager.instance;
        previousParentPanel = currentParentPanel;
        currentParentPanel = newParentPanel;

        switch(currentParentPanel)
        {
            case CardData.ParentPanel.HAND: {
                CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToActionPanel");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridHand.transform;
            }
            break;

            case CardData.ParentPanel.DISCARD: {
                this.TextureBack.GetComponent<UIButton>().enabled = false;
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridDiscard.transform;
            }
            break;

            case CardData.ParentPanel.SNAIL: {
                CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToActionPanel");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridSnails.transform;
            }
            break;

            case CardData.ParentPanel.PREY: {
                CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToActionPanel");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
            }
            break;

            case CardData.ParentPanel.MARKET: {
                CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToActionPanel");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__MARKET_PANEL + 2);
                transform.parent = uiMan.marketPanel.gridMarketCards.transform;
            }
            break;

            case CardData.ParentPanel.ACTION: {
                if(previousParentPanel == CardData.ParentPanel.HAND) {
                    if(CGlobals.TUTORIAL_ACTIVE)
                    {
                        CGlobals.AssignNewUIButtonOnClickTarget(director.CurrentTutorialScene, this, uiMan.actionPanel.bttnClose, "MoveCardToHandPanel");
                    }
                    else
                    {
                        CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToHandPanel");
                        CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToHandPanel");
                    }
                }
                else if(previousParentPanel == CardData.ParentPanel.SNAIL) {
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToSnailsPanel");
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToSnailsPanel");
                }
                else if(previousParentPanel == CardData.ParentPanel.PREY) {
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToPreyPanel");
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToPreyPanel");
                }
                else if(previousParentPanel == CardData.ParentPanel.MARKET) {
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToMarketPanel");
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToMarketPanel");
                }

                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__ACTION_PANEL + 2);
				transform.parent = uiMan.actionPanel.goActionContainer.transform;
            }
            break;

            case CardData.ParentPanel.CABAL:
            {
                transform.parent = CCabalManager.instance.CabalSolvingPanel.gridCards.transform;
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__CABAL_SOLVING_PANEL + 2);
            }
            break;

        }
    
        // Do not call CGlobals.UpdateWidgets() here as this will update NGUI's depth system.
        // Because we need cards to render over certain panels at specific times during transitions,
        // CGlobals.UpdateWidgets() must be called external of this function to control that.
        yield break;
    }


    public virtual IEnumerator SetNewParentPanel_Tutorial(CardData.ParentPanel newParentPanel)
    {

        CUIManager uiMan = CUIManager.instance;
        previousParentPanel = currentParentPanel;
        currentParentPanel = newParentPanel;

        switch(currentParentPanel)
        {
            case CardData.ParentPanel.HAND: {
                CGlobals.AssignNewUIButtonOnClickTarget(CTutorialDirector.instance, this, Button, "MoveCardTo_Center");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridHand.transform;
            }
            break;

            case CardData.ParentPanel.DISCARD: {
                this.TextureBack.GetComponent<UIButton>().enabled = false;
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridDiscard.transform;
            }
            break;

            case CardData.ParentPanel.SNAIL: {
                CGlobals.AssignNewUIButtonOnClickTarget(CTutorialDirector.instance, this, Button, "MoveCardTo_Center");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridSnails.transform;
            }
            break;

            case CardData.ParentPanel.PREY: {
                CGlobals.AssignNewUIButtonOnClickTarget(CTutorialDirector.instance, this, Button, "MoveCardTo_Center");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 2);
                transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
            }
            break;

            case CardData.ParentPanel.MARKET: {
                CGlobals.AssignNewUIButtonOnClickTarget(CTutorialDirector.instance, this, Button, "MoveCardTo_Center");
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__MARKET_PANEL + 2);
                transform.parent = uiMan.marketPanel.gridMarketCards.transform;
            }
            break;

            case CardData.ParentPanel.ACTION:
            {
                //if(previousParentPanel == CardData.ParentPanel.HAND) {
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToHandPanel");
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToHandPanel");
                //}
                //else if(previousParentPanel == CardData.ParentPanel.SNAIL) {
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToSnailsPanel");
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToSnailsPanel");
                //}
                //else if(previousParentPanel == CardData.ParentPanel.PREY) {
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToPreyPanel");
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToPreyPanel");
                //}
                //else if(previousParentPanel == CardData.ParentPanel.MARKET) {
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, this, Button, "MoveCardToMarketPanel");
                //    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, uiMan.SelectedCard, uiMan.actionPanel.bttnClose, "MoveCardToMarketPanel");
                //}

                ///////////////////////////////////////////////////////////////
                // Implement code here for when parenting card to action panel.
                // This code is reached when setting new parent to ACTION panel
                // so I think this would be a good place to keep track of which
                // scene the tutorial is currently in.
                
                /*
                 * switch(scene)
                 * {
                 *      case Sc_Beginning.Beginning:
                 *      {
                 *          // Do something here.
                 *      }
                 *      break;
                 * 
                 * }
                 */
                ///////////////////////////////////////////////////////////////


                // Sets sprite depth/render order
                ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__ACTION_PANEL + 2);
				transform.parent = uiMan.actionPanel.goActionContainer.transform;
            }
            break;

            //case CardData.ParentPanel.CABAL:
            //{
            //    transform.parent = CCabalManager.instance.CabalSolvingPanel.gridCards.transform;
            //    ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__CABAL_SOLVING_PANEL + 2);
            //}
            //break;

        }
    
        // Do not call CGlobals.UpdateWidgets() here as this will update NGUI's depth system.
        // Because we need cards to render over certain panels at specific times during transitions,
        // CGlobals.UpdateWidgets() must be called external of this function to control that.
        yield break;
    }


    public void EnableButton(bool enable = true)
	{
		// Must be enabled/disabled in different order to avoid NGUI buttons auto change of color
		if(enable) {
			transform.GetComponent<BoxCollider>().enabled = enable;
			Button.enabled = enable;
			cardTexture.color = Color.white;
		}
		else {
			Button.enabled = enable;
			transform.GetComponent<BoxCollider>().enabled = enable;
			cardTexture.color = Color.white;
		}
	}


    public IEnumerator EnableButtonCollider_CR()
    {
        CGlobals.iTweenValue(this.gameObject, CGlobals.DisabledCardColor, Color.white, 0.5f, 
                            this.gameObject, "UpdateCardColor", iTween.EaseType.linear, "Finish_DisableButtonCollider");

        yield return new WaitForSeconds(0.5f);
        Button.GetComponent<BoxCollider>().enabled = true;
    }

    public IEnumerator DisableButtonCollider_CR()
    {
        EnableButton(false);
        CGlobals.iTweenValue(this.gameObject, Color.white, CGlobals.DisabledCardColor, 0.5f, 
                            this.gameObject, "UpdateCardColor", iTween.EaseType.linear, "Finish_DisableButtonCollider");

        yield return new WaitForSeconds(0.5f);
    }

    public void Finish_DisableButtonCollider()
    {
        Button.enabled = true;
        CGlobals.UpdateWidgets();
    }

    public void UpdateCardColor(Color color) { cardTexture.color = color; }

    public void ChangeWidgetDepth(int depth)
    {
        cardTexture.depth = depth;
        cardBackTexture.depth = depth-1;

        if(cardType == CardData.CardType.Snail)
        {
            CSnailCard snailCard = (CSnailCard)this;
            snailCard.uiCardOutline.depth = depth + 1;
            snailCard.uiFedSprite.depth = depth + 1;
            snailCard.uiHibernateSprite.depth = depth + 1;
            snailCard.uiStrength.depth = depth + 1;
        }
    }

    public virtual void SelectedCardToSwap() { StartCoroutine(SelectedCardToSwap_CR()); }
    private IEnumerator SelectedCardToSwap_CR()
    {
        yield return new WaitForEndOfFrame();
        CActionPanel actionPanel = CUIManager.instance.actionPanel;

        if(actionPanel.handCardToSwap == null)
        {
            actionPanel.handCardToSwap = this;

            CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.25f);
            CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.15f);
        }
        else
        {
            CGlobals.TweenMove(actionPanel.handCardToSwap.gameObject, "y", 0f, 0.3f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.05f);

            actionPanel.handCardToSwap = this;
            CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.25f);
            CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public virtual void SelectedCardToUnhibernateSnail() { StartCoroutine(SelectedCardToUnhibernateSnail_CR()); }
    private IEnumerator SelectedCardToUnhibernateSnail_CR()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiManager = CUIManager.instance;
        BoxCollider unhibCollider = uiManager.actionPanel.goUnhibernateButton.GetComponent<BoxCollider>();
        UIButton unhibButton = uiManager.actionPanel.goUnhibernateButton.GetComponent<UIButton>();
        UISprite unhibSprite = uiManager.actionPanel.goUnhibernateButton.GetComponent<UISprite>();

        if(uiManager.SelectedCards.Count == 1)
        {
            CAudioManager.instance.PlaySound(CGameManager.instance.acSelectCard);
            uiManager.SelectedCards.Add(this);
            CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.25f);
            CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.15f);

            unhibCollider.enabled = true;
            uiManager.actionPanel.goUnhibernateButton.GetComponent<UISprite>().color = Color.white;
        }
        else
        {
            if(uiManager.SelectedCards.Contains(this))
            {
                CAudioManager.instance.PlaySound(CGameManager.instance.acCloseMarket);
                uiManager.SelectedCards.Remove(this);
                CGlobals.TweenMove(this.gameObject, "y", 0f, 0.3f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.3f);

                unhibCollider.enabled = false;
                unhibButton.enabled = false;
                yield return new WaitForEndOfFrame();
                unhibButton.enabled = true;
            }
            else
            {
                CBaseCard oldCard = uiManager.SelectedCards[1];
                CAudioManager.instance.PlaySound(CGameManager.instance.acSelectCard);
                uiManager.SelectedCards.Remove(oldCard);
                CGlobals.TweenMove(oldCard.gameObject, "y", 0f, 0.3f, iTween.EaseType.easeOutSine, true);

                uiManager.SelectedCards.Add(this);
                CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.25f);
                CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.15f);

                unhibCollider.enabled = true;
                uiManager.actionPanel.goUnhibernateButton.GetComponent<UISprite>().color = Color.white;
            }
        }
    }

    public virtual void SelectedCardToSell() { StartCoroutine(SelectedCardToSell_CR()); }
    private IEnumerator SelectedCardToSell_CR()
    {
        CUIManager uiManager = CUIManager.instance;

        if(uiManager.SelectedCards.Contains(this))
        {
            CAudioManager.instance.PlaySound(CGameManager.instance.acCloseMarket);
            uiManager.SelectedCards.Remove(this);
            CGlobals.TweenMove(this.gameObject, "y", 0f, 0.3f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.3f);

            uiManager.marketPanel.UpdateMarketCardButtonsByCost();
        }
        else
        {
            CAudioManager.instance.PlaySound(CGameManager.instance.acSelectCard);
            uiManager.SelectedCards.Add(this);
            CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.25f);
            CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);
            yield return new WaitForSeconds(0.15f);

            uiManager.marketPanel.UpdateMarketCardButtonsByCost();
        }
    }


    public virtual void SelectedCardToDiscard(CInstantCard instantCard) { StartCoroutine(SelectedCardToDiscard_CR(instantCard)); }
    private IEnumerator SelectedCardToDiscard_CR(CInstantCard instantCard)
    {
        CUIManager uiManager = CUIManager.instance;

        // Remove from SelectedCards, Lower snail strength
        if(uiManager.SelectedCards.Contains(this))
        {
            CAudioManager.instance.PlaySound(CGameManager.instance.acCloseMarket);
            uiManager.SelectedCards.Remove(this);
            StartCoroutine(uiManager.SelectedCards[0].LowerStrengthAnimation());

            if(instantCard != null) 
                yield return StartCoroutine(uiManager.actionPanel.RemoveUpgradeFromFeedingSnail_CR(instantCard));

            // Disable feeding if no cards are selected to discard
            if (uiManager.SelectedCards.Count == 1)
                uiManager.actionPanel.canPlayerFeed = false;

            CGlobals.TweenMove(this.gameObject, "y", 0f, 0.3f, iTween.EaseType.easeOutSine, true);

            this.EnableButton();
        }
        // Add to SelectedCards, Raise snail strength
        else
        {
            CSnailCard snailCardToFeed = (CSnailCard)uiManager.SelectedCard;

            if(snailCardToFeed.strength < 6)
            {
                CAudioManager.instance.PlaySound(CGameManager.instance.acSelectCard);
                uiManager.SelectedCards.Add(this);
                uiManager.actionPanel.canPlayerFeed = true;
                StartCoroutine(uiManager.SelectedCards[0].RaiseStrengthAnimation());

                if(instantCard != null)
                    StartCoroutine(uiManager.actionPanel.AddUpgradeToFeedingSnail_CR(instantCard));

                CGlobals.TweenMove(this.gameObject, "y", 100f, 0.35f, iTween.EaseType.easeOutSine, true);
                yield return new WaitForSeconds(0.25f);
                CGlobals.TweenMove(this.gameObject, "y", 77f, 0.2f, iTween.EaseType.easeOutSine, true);

                this.EnableButton();
            }
            else
            {
                StartCoroutine(CErrorPrompt.instance.ShowError(CErrorPrompt.ERROR.MAX_STRENGTH));
                CLogfile.instance.Append("Failed feed attempt: Snail has reached max strength!");
            }
        }
    }


	//private IEnumerator AnimateStrengthText()
	//{
	//	CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
	//	UISprite strengthTxt = snailCard.uiStrength;
	//	CGlobals.TweenScale(strengthTxt.gameObject, Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
	//	yield return new WaitForSeconds(0.5f);

	//	//int str = System.Convert.ToInt32(strengthTxt.text);
	//	snailCard.strength = snailCard.strength + 1;
 //       snailCard.uiStrength.spriteName = "Strength" + snailCard.strength.ToString();

	//	CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one * 1.25f, 0.6f, iTween.EaseType.easeOutSine, true);
	//	yield return new WaitForSeconds(0.65f);

	//	CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one, 0.3f, iTween.EaseType.easeOutSine, true);
	//}

    

    private IEnumerator LowerStrengthAnimation()
	{
		CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
		UISprite strengthTxt = snailCard.uiStrength;
		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.zero, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		//int str = System.Convert.ToInt32(strengthTxt.text);
		snailCard.strength = snailCard.strength - 1;
        snailCard.uiStrength.spriteName = "Strength" + snailCard.strength.ToString();

		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one * 1.35f, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one, 0.2f, iTween.EaseType.easeOutSine, true);
	}

    private IEnumerator RaiseStrengthAnimation()
	{
        Debug.Log(CUIManager.instance.SelectedCard.name);
		CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
		UISprite strengthTxt = snailCard.uiStrength;
		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.zero, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		//int str = System.Convert.ToInt32(strengthTxt.text);
		snailCard.strength = snailCard.strength + 1;
        snailCard.uiStrength.spriteName = "Strength" + snailCard.strength.ToString();

		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one * 1.35f, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		CGlobals.TweenScale(strengthTxt.gameObject, Vector3.one, 0.2f, iTween.EaseType.easeOutSine, true);
	}


    public virtual IEnumerator FlipCard(float yRot, float duration, bool enableButton)
    {
        yield return new WaitForEndOfFrame();
        bool toFaceUp = (yRot == 0f) ? true : false;

        CGlobals.TweenRotate(gameObject, Vector3.up * yRot, duration, iTween.EaseType.linear, true);
        yield return new WaitForSeconds(duration * 0.5f);

        if(toFaceUp) {
            Texture.enabled = true;
            TextureBack.enabled = false;
            EnableButton(enableButton);
        }
        else {
            Texture.enabled = false;
            TextureBack.enabled = true;
            EnableButton(false);
        }
    }


    //////////////////////////////////////////////////////////////////////////
    // HANDLE MOUSE CLICKS HERE FOR ALL CARDS
    //////////////////////////////////////////////////////////////////////////
    void OnPress(bool down) { StartCoroutine(OnPress_CR(down)); }
    IEnumerator OnPress_CR(bool down)
    {
        CGameManager gameManager = CGameManager.instance;
        CUIManager uiManager = CUIManager.instance;

        if (down)
        {
            ChangeWidgetDepth(cardTexture.depth + 1);
            yield break;
        }
        else if(!down)
        {
            EndDragging();
			CActivePlayerPanel.instance.panelHand.EnableCollider(false);
			CActivePlayerPanel.instance.panelSnails.EnableCollider(false);
        }
    }

    void OnHover(bool hover) { StartCoroutine(OnHover_CR(hover)); }
    IEnumerator OnHover_CR(bool hover)
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiMan = CUIManager.instance;

        if(uiMan.SelectedCards.Count > 0)
            if(uiMan.SelectedCards.Contains(this))
                Destroy(GetComponent<TweenColor>());
    }


    void OnDrag(Vector2 v2Delta)
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // Disable Dragging
        if (cardType == CardData.CardType.Prey || CurrentParentPanel == CardData.ParentPanel.MARKET || CurrentParentPanel == CardData.ParentPanel.ACTION)
            return;
        // 
        CGameManager gameMan = CGameManager.instance;
        // 
        if (gameMan.CurrentPlayState == GameData.PlayState.VIEWING_MARKET || 
            gameMan.CurrentPlayState == GameData.PlayState.SELECTING_PREY ||
            gameMan.CurrentPlayState == GameData.PlayState.UNHIBERNATING_SNAIL)
            return;
        //
        ///////////////////////////////////////////////////////////////////////////////////////////////////

		CActivePlayerPanel.instance.panelHand.EnableCollider(true);
		CActivePlayerPanel.instance.panelSnails.EnableCollider(true);
        Vector3 v3New = transform.localPosition;

		Vector2 v2Diff = new Vector2(CUIManager.REAL_SCREEN_WIDTH / Screen.width, CUIManager.REAL_SCREEN_HEIGHT / Screen.height);
        v3New.x += v2Delta.x * v2Diff.x;
        v3New.y += v2Delta.y * v2Diff.y;

        CUIManager uiMan = CUIManager.instance;
        CActivePlayerPanel app = CActivePlayerPanel.instance;

		if(gameMan.activePlayer.hand.Count == 1)
			app.discardCollider.boxCollider.enabled = true;
        
		uiMan.DraggedCard = this;
        transform.localPosition = v3New;
        bDragging = true;
		CUIManager.instance.isDragging = true;

        Camera camera = uiMan.uiCamera.GetComponent<Camera>();
        Ray ray1 = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray1);

        foreach (RaycastHit hit in hits)
        {
            switch(hit.collider.name)
            {
                case "Hand Background":
                    uiMan.draggedPanel = CardData.ParentPanel.HAND;
                    app.panelHand.EnableFlashBG(true);
                    app.panelSnails.EnableFlashBG(false);
                    Debug.Log("hand");
                    break;

                case "Snail Background":
                    uiMan.draggedPanel = CardData.ParentPanel.SNAIL;
                    app.panelHand.EnableFlashBG(false);
                    app.panelSnails.EnableFlashBG(true);
                Debug.Log("snail");
                    break;

                case "Prey Background":
                    uiMan.draggedPanel = CardData.ParentPanel.PREY;
                    app.panelHand.EnableFlashBG(false);
                    app.panelSnails.EnableFlashBG(false);
                Debug.Log("prey");
                    break;
            }
        }
    }

    // non-Unity
    private void EndDragging() { StartCoroutine(EndDragging_CR()); }
    private IEnumerator EndDragging_CR()
    {
        CGameManager gameManager = CGameManager.instance;
        CUIManager uiManager = CUIManager.instance;
        CActivePlayerPanel app = CActivePlayerPanel.instance;

        app.panelHand.EnableFlashBG(false);
        app.panelSnails.EnableFlashBG(false);

        if(bDragging)
        {
            bDragging = false;
			CUIManager.instance.isDragging = false;

			if(gameManager.CurrentUIState == GameData.UIState.IDLE)
            {
				if (!Button.enabled)
            		yield break;

				uiManager.DraggedCard = null;

                if(CurrentParentPanel == uiManager.draggedPanel)
                    uiManager.ReinsertCardIntoPanel(this);
                else
                {
                    switch(uiManager.draggedPanel)
                    {
                        case CardData.ParentPanel.SNAIL:
                        {
                            if (this.cardType == CardData.CardType.Snail) {
                                CLogfile.instance.Append("Drag Snail Card: " + ((CSnailCard)this).snailSpecies);
                                uiManager.DisableAllCollidersForEndTurn();
				                COpponentsPanel.instance.AddSnailToOpponent(this);
                                CAssessmentPanel.instance.lstLastPlayedCards.Add(this);
                                
                                yield return StartCoroutine(uiManager.MoveCardToSnailsPanel_CR(this));
                                yield return StartCoroutine(gameManager.EndTurn());
                            }
                            else
                                uiManager.ReinsertCardIntoPanel(this);
                        }
                        break;

                        default:
                        uiManager.ReinsertCardIntoPanel(this);
                        break;
                    }
                    
                }

	            ChangeWidgetDepth(cardTexture.depth - 1);
	            yield break;
            }
			else if(gameManager.CurrentUIState == GameData.UIState.DISCARD_DRAG)
			{
				CUIManager.instance.actionPanel.canPlayerFeed = true;
				yield return StartCoroutine(CActivePlayerPanel.instance.DiscardDraggedCard());
				gameManager.CurrentUIState = GameData.UIState.IDLE;

				CGlobals.UpdateWidgets();
				CActivePlayerPanel.instance.gridHand.UpdateGrid();

				switch(gameManager.CurrentPlayState)
				{
				    case GameData.PlayState.IDLE:
                        yield return StartCoroutine(CGameManager.instance.EndTurn());
				    break;

				    case GameData.PlayState.SELECTING_PREY:
					    //StartCoroutine(AnimateStrengthText());
				    break;

				    case GameData.PlayState.UNHIBERNATING_SNAIL:
					    CSnailCard snailCard = (CSnailCard)CUIManager.instance.SelectedCard;
					    snailCard.SetFedStateToUnfed();

					    yield return new WaitForSeconds(0.5f);
					    yield return StartCoroutine(CUIManager.instance.MoveCardToSnailsPanel_CR(snailCard));
					    yield return StartCoroutine(CGameManager.instance.EndTurn());
				    break;
				}
			}
        }
    }
    
}
