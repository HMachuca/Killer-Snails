using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct tPeptideRow
{
    public CardData.PeptideType peptideType;
    public Grid gridRow;
}

public class CSnailCard : CBaseCard 
{
	public CardData.SnailSpecies snailSpecies;
	public CardData.PreyType preyType;
	public CardData.FedState fedState;

    public UISprite uiFedSprite, uiHibernateSprite, uiCardOutline;
    public Grid gridPeptides;

    [SerializeField]
    public List<tPeptideRow> lstPeptideRows;
    public List<CardData.PeptideType> lstContainedPeptides;

	public int strength;
	private int originalStrengthValue;
    public UISprite uiStrength;

	public bool hasPotency;
	public bool hasStarvation;

    public List<CardData.PeptideType> lstPeptideRewards;
    public COpponentSnail opponentSnail;


    public bool HasPotency {
		get { return hasPotency; }
		set { hasPotency = value; } }

	public bool HasStarvation {
		get { return hasStarvation; }
		set { hasStarvation = value; } }
	

	protected override void Start () 
	{
		base.Start();

		cardType = CardData.CardType.Snail;
		name = snailSpecies.ToString();
		gameObject.name = name;
        lstPeptideRows = new List<tPeptideRow>(4);
		originalStrengthValue = strength;
	}


	public void ResetStrength()
	{
		strength = originalStrengthValue;
        uiStrength.spriteName = "Strength" + strength.ToString();
		//lblStrengthText.text = strength.ToString();
	}

    public IEnumerator ResetStrengthAnimation()
	{
		CGlobals.TweenScale(uiStrength.gameObject, Vector3.zero, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		strength = originalStrengthValue;
        uiStrength.spriteName = "Strength" + strength.ToString();

		CGlobals.TweenScale(uiStrength.gameObject, Vector3.one * 1.35f, 0.4f, iTween.EaseType.easeOutSine, true);
		yield return new WaitForSeconds(0.4f);

		CGlobals.TweenScale(uiStrength.gameObject, Vector3.one, 0.2f, iTween.EaseType.easeOutSine, true);
	}

    public override IEnumerator FlipCard(float yRot, float duration, bool enableButton)
    {
        yield return new WaitForEndOfFrame();
        bool toFaceUp = (yRot == 0f) ? true : false;

        CGlobals.TweenRotate(gameObject, Vector3.up * yRot, duration, iTween.EaseType.linear, true);
        yield return new WaitForSeconds(duration * 0.5f);

        if(toFaceUp) {
            Texture.enabled = true;
            uiStrength.enabled = true;
            TextureBack.enabled = false;
        }
        else {
            Texture.enabled = false;
            uiStrength.enabled = false;
            TextureBack.enabled = true;
        }
    }

    public IEnumerator AddPeptide(CPeptide peptide)
    {
//        int index = 0;
//        for (index = 0; index < lstPeptideRows.Count; ++index)
//            if (lstPeptideRows[index].peptideType == peptide.peptide)
//            {
//                peptide.transform.parent = lstPeptideRows[index].gridRow.transform;
//                peptide.transform.localPosition = lstPeptideRows[index].gridRow.transform.localPosition;
//                peptide.Enable();
//                peptide.transform.parent = lstPeptideRows[index].gridRow.transform;
//                lstPeptideRows[index].gridRow.transform.parent = gridPeptides.transform;
//
//                CGlobals.UpdateWidgets();
//                gridPeptides.UpdateGrid();
//                lstPeptideRows[index].gridRow.UpdateGrid();
//                break;
//            }
//
//        // If already on card, no need to instantiate another grid
//        bool bAlreadyOnCard = (index == lstPeptideRows.Count) ? false : true;
//        if (bAlreadyOnCard)
//            yield break;
//
//        GameObject go = Resources.Load("Peptides/PeptideGridRow") as GameObject;
//        Grid newPeptideGrid = ((GameObject)Instantiate(go, Vector3.zero, Quaternion.identity)).GetComponent<Grid>();
//        newPeptideGrid.mv2GridDimensions.x = 100;
//        newPeptideGrid.mv2CellMaxDimensions.x = 100;
//        tPeptideRow newRow;
//        newRow.gridRow = newPeptideGrid;
//        newRow.peptideType = peptide.peptide;
//
//        peptide.transform.parent = newRow.gridRow.transform;
//        newRow.gridRow.transform.parent = gridPeptides.transform;
//        newRow.gridRow.transform.localScale = Vector3.one;
//        lstPeptideRows.Add(newRow);
//
//        peptide.Enable();
//        CGlobals.UpdateWidgets();
//        gridPeptides.UpdateGrid();
//        lstPeptideRows[index].gridRow.UpdateGrid();
//
        yield return new WaitForSeconds(1.0f);
    }

	public void SetFedStateToHibernate() {
        if (fedState > CardData.FedState.Hibernating)
            CAudioManager.instance.PlaySound(CAudioManager.instance.acDowngrade);
        StartCoroutine(SetFedState(CardData.FedState.Hibernating));
    }
	public void SetFedStateToUnfed() {
        if (fedState > CardData.FedState.Unfed)
            CAudioManager.instance.PlaySound(CAudioManager.instance.acDowngrade);
        StartCoroutine(SetFedState(CardData.FedState.Unfed));
    }
	public void SetFedStateToFed() {
        CAudioManager.instance.PlaySound(CAudioManager.instance.acUpgrade);
        StartCoroutine(SetFedState(CardData.FedState.Fed));
    }

    public void RaiseFedState() { StartCoroutine(RaiseFedState_CR()); }
    public IEnumerator RaiseFedState_CR()
    {
        switch(fedState)
        {
            case CardData.FedState.Dead:
                break;

            case CardData.FedState.Hibernating:
                yield return StartCoroutine(SetFedState(CardData.FedState.Unfed));
                break;

            case CardData.FedState.Unfed:
                yield return StartCoroutine(SetFedState(CardData.FedState.Fed));
                break;
        }

        yield return new WaitForSeconds(1.5f);

        // Send Snail Grid back to Active Player Panel
        CActivePlayerPanel.instance.gridSnails.transform.parent = CActivePlayerPanel.instance.panelSnails.anchorPanelBackground.transform;
        foreach (CSnailCard snailCard in CGameManager.instance.activePlayer.snails)
            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, snailCard, snailCard.Button, "MoveCardToActionPanel");

        foreach(CPlayer player in CGameManager.instance.Players)
            foreach(CBaseCard handCard in player.hand)
                CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, handCard, handCard.Button, "MoveCardToActionPanel");

        CGameManager.instance.CurrentPlayState = GameData.PlayState.IDLE;
        yield return StartCoroutine(CGameManager.instance.EndTurn());
        CGlobals.UpdateWidgets();
    }

    public IEnumerator SetFedState(CardData.FedState fedState)
    {
        string updateFunc = "UpdateCardOutlineColor";
        string completeFunc = "CompleteCardOutlineColor";
        iTween.EaseType easetype = iTween.EaseType.easeOutSine;
		PlayerData.PLAYER player = CGameManager.instance.activePlayer.player;
        int index = CGameManager.instance.activePlayer.snails.IndexOf(this);
		//COpponentsPanel.instance.lstOpponents[(int)player].lstSnails[index].LowerFedState();
        this.fedState = fedState;

        Debug.Log("FS: " + fedState + "         Name: " + this.name);


        switch(fedState)
        {
            case CardData.FedState.Fed:
                uiFedSprite.enabled = true;
                uiHibernateSprite.enabled = false;
                uiCardOutline.enabled = true;
                
                opponentSnail.AutomaticSetState(CardData.FedState.Fed);
                CGlobals.iTweenValue(uiCardOutline.gameObject, uiCardOutline.color, Color.green, 0.5f, gameObject, updateFunc, easetype);
                break;

            case CardData.FedState.Hibernating:
                uiFedSprite.enabled = false;
                uiHibernateSprite.enabled = true;
                uiCardOutline.enabled = true;
				opponentSnail.AutomaticSetState(CardData.FedState.Hibernating);
                CGlobals.iTweenValue(uiCardOutline.gameObject, uiCardOutline.color, Color.blue, 0.5f, gameObject, updateFunc, easetype);
                break;

            case CardData.FedState.Unfed:
                uiFedSprite.enabled = false;
                //uiHibernateSprite.enabled = false;
                //uiCardOutline.enabled = false;
				opponentSnail.AutomaticSetState(CardData.FedState.Unfed);
                CGlobals.iTweenValue(uiCardOutline.gameObject, uiCardOutline.color, Color.clear, 0.5f, gameObject, updateFunc, easetype, completeFunc);
                break;

            case CardData.FedState.Dead:
            	opponentSnail.AutomaticSetState(CardData.FedState.Dead);
				CGlobals.iTweenValue(uiCardOutline.gameObject, uiCardOutline.color, Color.clear, 0.5f, gameObject, updateFunc, easetype, completeFunc);
				CGlobals.iTweenValue(uiHibernateSprite.gameObject, uiHibernateSprite.color, Color.clear, 0.5f, gameObject, updateFunc, easetype, completeFunc);
				break;
        }

        UpdateWidgetDepths();

        //  if(fedState == CardData.FedState.Dead) {
        //CBaseCard selectedCard = CUIManager.instance.SelectedCard;

        //if(selectedCard.cardType == CardData.CardType.Instant)
        //{
        // yield return StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));

        // CActivePlayerPanel activePlayerPanel = CUIManager.instance.activePlayerPanel;
        // activePlayerPanel.gridSnails.transform.parent = activePlayerPanel.panelSnails.anchorPanelBackground.transform;
        // COpponentsPanel.instance.anchorPanelBackground.transform.parent = COpponentsPanel.instance.transform;

        // yield return StartCoroutine(CUIManager.instance.activePlayerPanel.DiscardSelectedCards());

        // yield return new WaitForSeconds(1f);
        // CGlobals.UpdateWidgets();

        //       yield return CGameManager.instance.EndTurn();
        //      }
        //  }

        yield break;
    }

    public void UpdateFedStateFromOpponentSnail(CardData.FedState fedState)
	{
		this.fedState = fedState;

		switch(fedState)
        {
            case CardData.FedState.Fed:
            {
                uiFedSprite.enabled = true;
                uiHibernateSprite.enabled = false;
                uiCardOutline.enabled = true;
                uiCardOutline.color = Color.green;
                break;
            }

            case CardData.FedState.Hibernating:
            {
                uiFedSprite.enabled = false;
                uiHibernateSprite.enabled = true;
                uiCardOutline.enabled = true;
				uiCardOutline.color = Color.blue;
                break;
            }

            case CardData.FedState.Unfed:
            {
                uiFedSprite.enabled = false;
                uiHibernateSprite.enabled = false;
                uiCardOutline.enabled = false;
				uiCardOutline.color = Color.blue;
                break;
            }
        }

        UpdateWidgetDepths();
    }
		

    public void UpdateWidgetDepths()
    {
        uiFedSprite.depth = Texture.depth + 1;
        uiHibernateSprite.depth = Texture.depth + 1;
        uiCardOutline.depth = Texture.depth + 1;

        // Update peptide depth
        foreach (tPeptideRow peptideRow in lstPeptideRows)
            foreach (Transform child in peptideRow.gridRow.transform) {
                child.GetComponent<UISprite>().depth = Texture.depth + 5;
                foreach (Transform bg in child)
                    bg.GetComponent<UISprite>().depth = Texture.depth + 4;
            }
    }

    public void EnableCardOutline()
    {
        uiCardOutline.enabled = true;

        switch(CGameManager.instance.activePlayer.player)
        {
            case PlayerData.PLAYER.ONE:
                CGlobals.iTweenValue(uiCardOutline.gameObject, Color.clear, CGlobals.Player1Color, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear);
                break;
            case PlayerData.PLAYER.TWO:
                CGlobals.iTweenValue(uiCardOutline.gameObject, Color.clear, CGlobals.Player2Color, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear);
                break;
            case PlayerData.PLAYER.THREE:
                CGlobals.iTweenValue(uiCardOutline.gameObject, Color.clear, CGlobals.Player3Color, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear);
                break;
            case PlayerData.PLAYER.FOUR:
                CGlobals.iTweenValue(uiCardOutline.gameObject, Color.clear, CGlobals.Player4Color, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear);
                break;
        }
    }

    public void DisableCardOutline()
    {
        switch(CGameManager.instance.activePlayer.player)
        {
            case PlayerData.PLAYER.ONE:
                CGlobals.iTweenValue(uiCardOutline.gameObject, CGlobals.Player1Color, Color.clear, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear, "CompleteCardOutlineColor");
                break;
            case PlayerData.PLAYER.TWO:
                CGlobals.iTweenValue(uiCardOutline.gameObject, CGlobals.Player2Color, Color.clear, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear, "CompleteCardOutlineColor");
                break;
            case PlayerData.PLAYER.THREE:
                CGlobals.iTweenValue(uiCardOutline.gameObject, CGlobals.Player3Color, Color.clear, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear, "CompleteCardOutlineColor");
                break;
            case PlayerData.PLAYER.FOUR:
                CGlobals.iTweenValue(uiCardOutline.gameObject, CGlobals.Player4Color, Color.clear, 0.5f, gameObject, "UpdateCardOutlineColor", iTween.EaseType.linear, "CompleteCardOutlineColor");
                break;
        }     
    }

    public void UpdateCardOutlineColor(Color color) { uiCardOutline.color = color; }
	public void UpdateHibernateSprite(Color color) { uiHibernateSprite.color = color; }
    public void CompleteCardOutlineColor() 
    { 
		uiHibernateSprite.enabled = false;
    	uiCardOutline.enabled = false; 
    }

    //public void KillSnailFromOpponent() { StartCoroutine(KillSnailFromOpponent_CR(0f, 1f)); }
    //public IEnumerator KillSnailFromOpponent_CR(float snailDuration = 1f, float oppSnailDuration = 1f)
    //{
    //    yield return StartCoroutine(KillSnail_CR(snailDuration, oppSnailDuration));

    //}
    public void KillSnailFromOpponent() { StartCoroutine(KillSnail_CR(0f, 1f, true)); }
    public void KillSnail() { StartCoroutine(KillSnail_CR(1f, 0f, true)); }
	public IEnumerator KillSnail_CR(float snailDuration = 1f, float oppSnailDuration = 1f, bool opponentSnailClicked = false)
	{
		CGameManager gameManager = CGameManager.instance;
		CUIManager uiManager = CUIManager.instance;

        uiManager.DisableAllCollidersForEndTurn();

        // Shrink peptides
        List<tPeptideRow> toDelete = new List<tPeptideRow>();
		foreach(tPeptideRow peptideRow in lstPeptideRows)
			foreach(Transform peptideTR in peptideRow.gridRow.transform) {
                toDelete.Add(peptideRow);
				CGlobals.TweenScale(peptideTR.gameObject, Vector3.zero, snailDuration, iTween.EaseType.easeOutSine, true);
			}

		yield return new WaitForSeconds(snailDuration);

        //foreach(tPeptideRow peptideRow in toDelete)
        //    Destroy(peptideRow.)

		CGlobals.iTweenValue(uiCardOutline.gameObject, uiCardOutline.color, Color.clear, snailDuration, gameObject, "UpdateCardOutlineColor", iTween.EaseType.easeOutSine, "CompleteCardOutlineColor");
		CGlobals.iTweenValue(uiHibernateSprite.gameObject, uiHibernateSprite.color, Color.clear, snailDuration, gameObject, "UpdateHibernateSprite", iTween.EaseType.easeOutSine, "CompleteCardOutlineColor");

		yield return StartCoroutine(opponentSnail.RemovePeptidesAndShrink(oppSnailDuration));
		yield return new WaitForSeconds(snailDuration);

		CPlayer OwnerOfCard = null;
		foreach(CPlayer player in gameManager.Players)
			foreach(CBaseCard card in player.snails)
				if(card == this){
					OwnerOfCard = player;
					break;
				}

		if(OwnerOfCard == gameManager.activePlayer)	{
			yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSnailCard(this));
			CGlobals.UpdateWidgets();
			yield return new WaitForEndOfFrame();
			CActivePlayerPanel.instance.gridSnails.UpdateGrid();
		}
		else {
			yield return StartCoroutine(uiManager.DiscardSnailCardInstantly(OwnerOfCard, this));
			CGlobals.UpdateWidgets();
			yield return new WaitForEndOfFrame();
			COpponentsPanel.instance.lstOpponents[(int)OwnerOfCard.player].gridSnails.UpdateGrid();
		}

        //if(uiManager.SelectedCards.Count > 0)  
        //yield return StartCoroutine(CActivePlayerPanel.instance.DiscardCard(uiManager.SelectedCard));

        int ia = uiManager.SelectedCards.Count; 

        CGlobals.UpdateWidgets();

        if (opponentSnailClicked)
        {
            yield return StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));

            CGameManager gameMan = CGameManager.instance;
            CActivePlayerPanel.instance.gridSnails.transform.parent = CActivePlayerPanel.instance.panelSnails.anchorPanelBackground.transform;
            COpponentsPanel.instance.anchorPanelBackground.transform.parent = COpponentsPanel.instance.transform;

            yield return StartCoroutine(CActivePlayerPanel.instance.DiscardSelectedCards());
            yield return new WaitForSeconds(1f);
            CGlobals.UpdateWidgets();

            COpponentsPanel.instance.DisableSnailButtons();
            yield return gameMan.EndTurn();
        }

        // Reset card info
        fedState = CardData.FedState.Unfed;
	}
}
