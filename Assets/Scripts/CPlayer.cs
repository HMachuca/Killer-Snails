using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPlayer : MonoBehaviour 
{
    public bool AIControlled;
	public PlayerData.PLAYER player;
	private bool activePlayer;

	public List<CBaseCard> hand;
	public List<CBaseCard> discard;
	public List<CBaseCard> snails;
	public List<CBaseCard> prey;
	public Grid gridHand, gridSnails, gridPrey;
	public GameObject goDiscard;
    public GameObject goHandPanel, goSnailPanel;
    private UISprite uiHandPanel, uiSnailsPanel, uiPreyPanel;
	public GameObject goDiscardPile;

    public List<CPeptide> lstRevealedPeptides;

    //public List<CPeptide> lstPeekedPeptides;
    public List<bool> lstSolvedCabals = new List<bool>(3);
    public int numberCorrectlyAnsweredQuestions = 0;

    

    public CBasePanel panelHand;
    public CBasePanel panelSnails;
    public CBasePanel panelPrey;


	public bool IsActivePlayer {
		get { return activePlayer; }
		set { activePlayer = value; }
    }

    public void SetCabalAsSolved(int cabal) {
        lstSolvedCabals[cabal] = true;
    }

    public bool HasSolvedAllCabals()
    {
        bool solved = true;

        for(int i = 0; i < lstSolvedCabals.Count; ++i) {
            if (lstSolvedCabals[i] == false) {
                solved = false;
                break;
            }
        }
        return solved;
    }
    
    public UISprite HandPanel { get { return uiHandPanel; } }
    public UISprite SnailsPanel { get { return uiSnailsPanel; } }
    public UISprite PreyPanel { get { return uiPreyPanel; } }

	// Use this for initialization
	void Awake() 
	{
		hand = new List<CBaseCard>();
		discard = new List<CBaseCard>();
		snails = new List<CBaseCard>();
		prey = new List<CBaseCard>(3);
        lstRevealedPeptides = new List<CPeptide>();

        for (int i = 0; i < 3; ++i)
            lstSolvedCabals.Add(false);

		string szPlayer = string.Empty;
		if(this.name.Contains("Player1")) {
			szPlayer = "P1 ";
			player = PlayerData.PLAYER.ONE; }
		else if(this.name.Contains("Player2")) {
			szPlayer = "P2 ";
			player = PlayerData.PLAYER.TWO; }
		else if(this.name.Contains("Player3")) {
			szPlayer = "P3 ";
			player = PlayerData.PLAYER.THREE; }
		else if(this.name.Contains("Player4")) {
			szPlayer = "P4 ";
			player = PlayerData.PLAYER.FOUR; }

		gridHand = GameObject.Find(szPlayer + "Hand Cards").GetComponent<Grid>();
		goDiscard = GameObject.Find(szPlayer + "Discard Cards").gameObject;
		gridSnails = GameObject.Find(szPlayer + "Snail Cards").GetComponent<Grid>();
		gridPrey = GameObject.Find(szPlayer + "Prey Cards").GetComponent<Grid>();
		goDiscardPile = GameObject.Find(szPlayer + "Discard Cards");

        uiHandPanel = gridHand.transform.parent.GetComponent<UISprite>();
        uiSnailsPanel = gridSnails.transform.parent.GetComponent<UISprite>();
        uiPreyPanel = gridPrey.transform.parent.GetComponent<UISprite>();

        foreach (Transform child in this.transform) {
            if (child.name.Contains("Hand"))
                goHandPanel = child.gameObject;
            else if (child.name.Contains("Snail"))
                goSnailPanel = child.gameObject;
        }
	}

    void Start()
    {
        if(activePlayer)
        {
            foreach(Transform child in this.transform)
            {
                if (child.name == "Prey Panel")
                    panelPrey = child.GetComponent<CBasePanel>();
                else if (child.name == "Snails Panel")
                    panelSnails = child.GetComponent<CBasePanel>();
                else if (child.name == "Hand Panel")
                    panelHand = child.GetComponent<CBasePanel>();
            }
        }
    }
	
	//// Update is called once per frame
	//void Update () {
 //       if (Input.GetKeyDown(KeyCode.E))
 //           CAIDirector.instance.UpdateFrequencyTable();
	//}

	public void ClearHand() { hand.Clear(); }
	public void DealCard(CBaseCard card) 
	{
		card.gameObject.transform.parent = gridHand.transform;
		card.transform.localPosition = Vector3.zero;

		if(IsActivePlayer)
            card.transform.localScale = Vector3.one * 0.375f;
		else
            card.transform.localScale = Vector3.one * 0.3f;

        hand.Add(card);
	}

	public void DiscardCard(CBaseCard card) {}

	public void SetSnailCardButtonTargetForFedState()
	{
		foreach(CBaseCard card in snails)
		{
			CSnailCard snailCard = (CSnailCard)card;
			UIButton oppButton = snailCard.opponentSnail.GetComponent<UIButton>();
			BoxCollider oppCollider = snailCard.opponentSnail.GetComponent<BoxCollider>();
			oppButton.enabled = true;
			oppCollider.enabled = true;

			switch(snailCard.fedState)
			{
			case CardData.FedState.Hibernating:
				CGlobals.AssignNewUIButtonOnClickTarget(card, null, card.Button, "KillSnail");
				CGlobals.AssignNewUIButtonOnClickTarget(snailCard, null, oppButton, "KillSnailFromOpponent");
				break;
			case CardData.FedState.Unfed:
				CGlobals.AssignNewUIButtonOnClickTarget(card, null, card.Button, "SetFedStateToHibernate");
				CGlobals.AssignNewUIButtonOnClickTarget(snailCard.opponentSnail, null, oppButton, "LowerFedState");
				break;
			case CardData.FedState.Fed:
				CGlobals.AssignNewUIButtonOnClickTarget(card, null, card.Button, "SetFedStateToUnfed");
				CGlobals.AssignNewUIButtonOnClickTarget(snailCard.opponentSnail, null, oppButton, "LowerFedState");
				break;
			}
		}
	}

	public void EnableBasicPreyCard(bool enable = true)
	{
		foreach(CPreyCard preyCard in prey)
			if(preyCard.preyName == CardData.PreyName.Basic_Prey)
				preyCard.gameObject.SetActive(enable);
	}

	public void EnablePreyButtons(bool enable = true)
	{
		foreach(CPreyCard preyCard in prey)
			preyCard.EnableButton(enable);
	}


    #region AI Functions

    public CSnailCard HasSnailWithPeptides()
    {
        foreach (CSnailCard snailCard in snails)
            if (snailCard.lstContainedPeptides.Count > 0)
                return snailCard;

        return null;
    }

    public CSnailCard HasSnailWithZeroPeptides()
    {
        foreach (CSnailCard snailCard in snails)
            if (snailCard.lstContainedPeptides.Count == 0)
                return snailCard;

        return null;
    }

    public CSnailCard GetHibernatingSnail()
    {
        foreach (CSnailCard snailCard in snails)
            if (snailCard.fedState == CardData.FedState.Hibernating)
                return snailCard;

        return null;
    }

    #endregion

}