using UnityEngine;
using System.Collections;

public class CActivePlayerPanel : CBasePanel
{
    #region Singleton Instance
    private static CActivePlayerPanel _instance;
    public static CActivePlayerPanel instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CActivePlayerPanel>();
            }
            return _instance;
        }
    }
    #endregion

    public CBasePanel panelHand, panelPrey;
    public CSnailPanel panelSnails;
    public NGUIAnchorController uiPanelHand, uiPanelSnails, uiPanelPrey;
    public Grid gridHand, gridSnails, gridPrey, gridDiscard;
    public GameObject goDiscard;
    public UILabel lblHandCount;

    public CDiscardCollider discardCollider;

    public AudioClip acDiscardCard;

	protected override void Start ()
    {
        base.Start();
	}
	
	public IEnumerator DiscardDraggedCard()
	{
		yield return new WaitForEndOfFrame();

        CAudioManager.instance.PlaySound(acDiscardCard);
		CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        uiMan.DisableAllCollidersForEndTurn();

		CBaseCard draggedCard = uiMan.DraggedCard;
        CSnailCard snailCard = (draggedCard.cardType == CardData.CardType.Snail) ? (CSnailCard)draggedCard : null;
        CInstantCard instantCard = (draggedCard.cardType == CardData.CardType.Instant) ? (CInstantCard)draggedCard : null;

        if(instantCard != null) {
            if(gameMan.CurrentPlayState == GameData.PlayState.SELECTING_PREY) {
                if (instantCard.playStateToBeActive == GameData.PlayState.SELECTING_PREY) {
                    yield return StartCoroutine(uiMan.actionPanel.AddUpgradeToFeedingSnail_CR(instantCard));
                    yield return new WaitForSeconds(1.0f);
                }
            }
        }

		draggedCard.EnableButton(false);
		draggedCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + gameMan.activePlayer.discard.Count + 1);
		draggedCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;

        StartCoroutine(draggedCard.FlipCard(180f, 0.3f, false));
		CGlobals.TweenMove(draggedCard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(draggedCard.gameObject, Vector3.one * 0.1f, 0.4f, iTween.EaseType.easeOutSine, true);
		//CGlobals.TweenRotate(draggedCard.gameObject, new Vector3(0f, 180f, 0f), 0.3f, iTween.EaseType.linear);

		gameMan.activePlayer.hand.Remove(draggedCard);
		gameMan.activePlayer.discard.Add(draggedCard);
        uiMan.SelectedCards.Remove(draggedCard);

		yield return new WaitForSeconds(0.15f);
		uiMan.DraggedCard = null;
        uiMan.EnableAllBoxCollidersForBeginTurn();
	}

    public IEnumerator DiscardSelectedCards()
	{
		yield return new WaitForEndOfFrame();
		CGameManager gameMan = CGameManager.instance;

		foreach(CBaseCard selectedCard in CUIManager.instance.SelectedCards) {
       // for(int i = 0; i < CUIManager.instance.SelectedCards.Count; ++i) {
            //CBaseCard selectedCard = CUIManager.instance.SelectedCards[i];
			selectedCard.EnableButton(false);
			selectedCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + gameMan.activePlayer.discard.Count + 1);
			selectedCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;

            StartCoroutine(selectedCard.FlipCard(180f, 0.3f, false));
			CGlobals.TweenMove(selectedCard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
			CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.1f, 0.4f, iTween.EaseType.easeOutSine, true);
			//CGlobals.TweenRotate(selectedCard.gameObject, new Vector3(0f, 180f, 0f), 0.3f, iTween.EaseType.linear);

			gameMan.activePlayer.hand.Remove(selectedCard);
			gameMan.activePlayer.discard.Add(selectedCard);
			yield return new WaitForSeconds(0.15f);
		}

		CUIManager.instance.SelectedCard = null;
	}

    public IEnumerator DiscardSelectedCardsExceptFeedingSnail()
	{
		yield return new WaitForEndOfFrame();
		CGameManager gameMan = CGameManager.instance;

		//foreach(CBaseCard selectedCard in CUIManager.instance.SelectedCards) {
        for(int i = 1; i < CUIManager.instance.SelectedCards.Count; ++i) {
            CBaseCard selectedCard = CUIManager.instance.SelectedCards[i];
			selectedCard.EnableButton(false);
			selectedCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + gameMan.activePlayer.discard.Count + 1);
			selectedCard.transform.parent = CActivePlayerPanel.instance.goDiscard.transform;

            StartCoroutine(selectedCard.FlipCard(180f, 0.3f, false));
			CGlobals.TweenMove(selectedCard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
			CGlobals.TweenScale(selectedCard.gameObject, Vector3.one * 0.1f, 0.4f, iTween.EaseType.easeOutSine, true);
			//CGlobals.TweenRotate(selectedCard.gameObject, new Vector3(0f, 180f, 0f), 0.3f, iTween.EaseType.linear);

			gameMan.activePlayer.hand.Remove(selectedCard);
			gameMan.activePlayer.discard.Add(selectedCard);
			yield return new WaitForSeconds(0.15f);
		}
	}

	public IEnumerator DiscardCard(CBaseCard cardToDiscard)
	{
		yield return new WaitForEndOfFrame();

        CAudioManager.instance.PlaySound(acDiscardCard);
		cardToDiscard.EnableButton(false);
		cardToDiscard.transform.parent = goDiscard.transform;

        StartCoroutine(cardToDiscard.FlipCard(180f, 0.3f, false));
		CGlobals.TweenMove(cardToDiscard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(cardToDiscard.gameObject, Vector3.one * 0.1f, 0.4f, iTween.EaseType.easeOutSine, true);
		//CGlobals.TweenRotate(cardToDiscard.gameObject, new Vector3(0f, 180f, 0f), 0.3f, iTween.EaseType.linear);

		yield return new WaitForSeconds(0.15f);
	}

	public IEnumerator DiscardSnailCard(CSnailCard cardToDiscard)
	{
		yield return new WaitForEndOfFrame();

		cardToDiscard.EnableButton(false);
		cardToDiscard.transform.parent = goDiscard.transform;
		CGameManager.instance.activePlayer.snails.Remove(cardToDiscard);
		cardToDiscard.opponentSnail.transform.parent = CGameManager.instance.transform;
		cardToDiscard.opponentSnail.transform.localPosition = Vector3.up * 1500f;

        StartCoroutine(cardToDiscard.FlipCard(180f, 0.3f, false));
		CGlobals.TweenMove(cardToDiscard.gameObject, "position", Vector3.zero, 0.5f, iTween.EaseType.easeOutSine, true);
		CGlobals.TweenScale(cardToDiscard.gameObject, Vector3.one * 0.1f, 0.4f, iTween.EaseType.easeOutSine, true);
		//CGlobals.TweenRotate(cardToDiscard.gameObject, new Vector3(0f, 180f, 0f), 0.3f, iTween.EaseType.linear);

		yield return new WaitForSeconds(0.15f);
	}
}
