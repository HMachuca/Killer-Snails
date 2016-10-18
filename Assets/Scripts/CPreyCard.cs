using UnityEngine;
using System.Collections;

public class CPreyCard : CBaseCard 
{
	public CardData.PreyName preyName;
	public CardData.PreyType preyType;
	public int resistance;
    public UISprite uiOutlineOverlay;

	protected override void Start () 
	{
		base.Start();

		cardType = CardData.CardType.Prey;

        uiOutlineOverlay.depth = 54;
	}

	public void EnableBacksideButton(bool enable = true)
	{
        //UIButton bttn = TextureBack.GetComponent<UIButton>();
       // BoxCollider boxCollider = TextureBack.GetComponent<BoxCollider>();

        //bttn.enabled = 
        this.TextureBack.GetComponent<BoxCollider>().enabled = true;

        this.TextureBack.GetComponent<UIButton>().enabled = true;
	}

    public IEnumerator TweenOutlineOverlayColor(PlayerData.PLAYER player, bool clear = false)
    {
        yield return new WaitForEndOfFrame();
        Color newColor = Color.clear;
        uiOutlineOverlay.enabled = true;

        if(!clear)
            switch(player)
            {
                case PlayerData.PLAYER.ONE: newColor = CGlobals.Player1Color; break;
                case PlayerData.PLAYER.TWO: newColor = CGlobals.Player2Color; break;
                case PlayerData.PLAYER.THREE: newColor = CGlobals.Player3Color; break;
                case PlayerData.PLAYER.FOUR: newColor = CGlobals.Player4Color; break;
            }

        CGlobals.iTweenValue(uiOutlineOverlay.gameObject, uiOutlineOverlay.color, newColor, 0.35f, this.gameObject,
                    "UpdateOutlineColor", iTween.EaseType.easeOutSine, "CompleteOutlineColor");
        yield return new WaitForSeconds(0.35f);
    }

    public void UpdateOutlineColor(Color color) { uiOutlineOverlay.color = color; }
    public void CompleteOutlineColor() { if (uiOutlineOverlay.color.a == 0f) uiOutlineOverlay.enabled = false; }


    //public override void OnSelection() { StartCoroutine(OnSelection_CR()); }
    //private IEnumerator OnSelection_CR()
    //{
    //    Destroy(GetComponent<TweenColor>());
    //    CUIManager uiMan = CUIManager.instance;

    //    if(uiMan.SelectedCards == null || uiMan.SelectedCards.Count == 0)
    //    {
    //        uiMan.SelectedCard = this;
    //        uiMan.MoveCardToActionPanel(this);
    //    }
    //    else
    //    {
    //        if(uiMan.SelectedCard.cardType == CardData.CardType.Prey)
    //        {
    //            uiMan.actionPanel.FadeBG(false);
    //            uiMan.SelectedCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH_PREY_PANEL + 1);
    //            uiMan.SelectedCard.SetNewParentPanel(CardData.ParentPanel.PREY);
    //            this.transform.parent = CGameManager.instance.activePlayer.gridPrey.transform;

    //            yield return new WaitForEndOfFrame();
    //            CGlobals.UpdateWidgets();
    //            //CGlobals.TweenMove(uiMan.SelectedCard.gameObject, "y", 0f, 0.5f, iTween.EaseType.easeOutSine, true);

    //            yield return new WaitForSeconds(0.35f);
    //            uiMan.SelectedCard.Button.enabled = true;
    //            uiMan.SelectedCard = null;
    //            CGameManager.instance.activePlayer.gridPrey.UpdateGrid();
    //        }
    //        else
    //        {
    //            CSnailCard selectedCard = ((CSnailCard)uiMan.SelectedCard);
    //            uiMan.EnablePreyButtons(false);

    //            CardData.PeptideType peptideType = CardData.PeptideType.Unknown;
    //            if (selectedCard.lstPeptideRewards.Count == 0)
    //                peptideType = (CardData.PeptideType)Random.Range(0, 4);
    //            else
    //            {
    //                int nRand = Random.Range(0, selectedCard.lstPeptideRewards.Count);
    //                peptideType = selectedCard.lstPeptideRewards[nRand];
    //            }

    //            CPeptide peptide = uiMan.TakePeptide(peptideType);
    //            peptide.transform.parent = selectedCard.gridPeptides.transform;
    //            peptide.transform.position = this.transform.position;
    //            peptide.Enable();
    //            selectedCard.gridPeptides.UpdateGrid();
                    

    //            yield return new WaitForSeconds(1.0f);
    //            uiMan.MoveCardToSnailsPanel(uiMan.SelectedCard);
                    

    //            yield return new WaitForSeconds(1.0f);
    //            uiMan.EnablePreyButtons();
    //            selectedCard.SetFedState(CardData.FedState.Fed);

    //        }
    //     }
    //}
}
