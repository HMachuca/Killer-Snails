using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class CGameManager : MonoBehaviour 
{
    #region Singleton Instance
    private static CGameManager _instance;
    public static CGameManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CGameManager>();
            }
            return _instance;
        }
    }
    #endregion

    private int NUM_PLAYERS_ALLOWED = 4;
    private List<CPlayer> players;

    public GameData.PlayState CurrentPlayState;
    public GameData.UIState CurrentUIState;

    // GameObjects already in scene - used to attach CPlayer scripts
    public GameObject goPlayer1, goPlayer2, goPlayer3, goPlayer4;
	public int numPlayers;
	public CPlayer activePlayer;
    public PlayerData.PLAYER currentActivePlayer;
	public GameObject goActivePlayer;
    public AudioSource audiosource;
    public AudioClip acCloseMarket, acEatPrey, acLowerFedState, acOpenMarket, acRevealPeptide, acSelectCard, acSuccess;
    public bool dealNewRound;


    //copying parameteers from CMarketPanel
    private float ASSESSMET_TWEEN_TIME = 0.75f;
    private float INITIAL_ASSESSMENT_Y_POS = 0f;





    public List<CPlayer> Players {
        get { return players; }
        private set { }
    }

    public CPlayer GetCurrentPlayer()
    {
        CPlayer currentPlayer = null;
        foreach (CPlayer player in players)
            if (activePlayer == player)
                currentPlayer = player;

        return currentPlayer;
    }

    public void RemoveCreatedPlayers()
    {
        COpponentsPanel.instance.RemoveCreatedPlayers();
        players.Clear();
    }

    void Update()
    {
		if(Input.GetKeyDown(KeyCode.A))
			StartCoroutine(EndTurn());

		if(Input.GetKeyDown(KeyCode.W))
			Time.timeScale = 4f;
		if(Input.GetKeyDown(KeyCode.Q))
			Time.timeScale = 1f;
    }


	//IEnumerator Start()
	//{
 //       numPlayers = UserSettings.NumberOfPlayers;
	//	players = new List<CPlayer>(numPlayers);
		
	//	yield return StartCoroutine(BeginNewGame());
	//	CurrentUIState = GameData.UIState.IDLE;
	//	dealNewRound = true;
	//}


    public IEnumerator WakeUp()
	{
        numPlayers = UserSettings.NumberOfPlayers;
		players = new List<CPlayer>(numPlayers);
		
		yield return StartCoroutine(BeginNewGame());
		CurrentUIState = GameData.UIState.IDLE;
		dealNewRound = true;
	}


	public IEnumerator BeginNewGame()
	{
        yield return new WaitForEndOfFrame();

		CreatePlayers();
		CCardManager.instance.CreateDeck();
		CCardManager.instance.DealHandCardsToPlayers();

        CCardManager.instance.DealPreyCardsToPlayers();
        CCardManager.instance.DealCardsToMarket();
        CCabalManager.instance.GenerateCabals();

        yield return StartCoroutine(CUIManager.instance.UpdateResolution());

        yield return new WaitForSeconds(0.75f);
        StartCoroutine(CUIManager.instance.marketPanel.PositionBuyButtons());
        List<CBaseCard> marketCards = CUIManager.instance.marketPanel.lstMarketCards;
        List<UISprite> buyButtons = CUIManager.instance.marketPanel.lstBuyButtons;

        for (int i = 0; i < CUIManager.instance.marketPanel.lstMarketCards.Count; ++i)
            buyButtons[i].transform.localPosition = marketCards[i].transform.localPosition;

        yield return new WaitForEndOfFrame();
        CGlobals.AutoResizeTextureColliders(this.transform);
        //CUIManager.instance.CenterPlayerOneUI(activePlayer.SnailsPanel.width);

        yield return StartCoroutine(SetNextActivePlayer(null, true));
        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        CActivePlayerPanel.instance.gridPrey.UpdateGrid();
    }


    // Swaps cards from previousActivePlayer to nextActivePlayer
    public IEnumerator SetNextActivePlayer(CPlayer prevPlayer = null, bool gameJustStarted = false)
    {
        CActivePlayerPanel activePlayerPanel = CActivePlayerPanel.instance;

        // if activeplayer, send all of active player cards to correct player panel.
        if (activePlayer != null)
        {
            List<Transform> lstHand = new List<Transform>();
            List<Transform> lstSnail = new List<Transform>();
            List<Transform> lstPrey = new List<Transform>();
            List<Transform> lstDiscard = new List<Transform>();
            activePlayer.IsActivePlayer = false;

            // Put children into temp lists
            foreach (Transform child in activePlayerPanel.gridHand.transform) lstHand.Add(child);
            foreach (Transform child in activePlayerPanel.gridSnails.transform) lstSnail.Add(child);
            foreach (Transform child in activePlayerPanel.gridPrey.transform) lstPrey.Add(child);
            foreach (Transform child in activePlayerPanel.goDiscard.transform) lstDiscard.Add(child);

            // Reset parents to Opponent Panel Player gameobject
			foreach (Transform child in lstHand) child.parent = prevPlayer.gridHand.transform;
			foreach (Transform child in lstSnail) child.parent = prevPlayer.gridSnails.transform;
            foreach (Transform child in lstPrey) {
				child.parent = prevPlayer.gridPrey.transform;
                child.GetComponent<CBaseCard>().ChangeWidgetDepth(CGlobals.OPPONENT_PREY_BG_DEPTH + 1);
                if (child.name.Contains("Basic"))
                    child.gameObject.SetActive(false);
            }

            foreach (Transform child in lstDiscard) {
				child.parent = prevPlayer.goDiscardPile.transform;
                child.transform.localPosition = Vector3.zero;
            }
        }

        if(gameJustStarted)
        	activePlayer = players[(int)PlayerData.PLAYER.ONE];
        
        activePlayer.IsActivePlayer = true;

        // Set card parents to active player panel
        foreach (CBaseCard card in activePlayer.prey) {
            card.transform.parent = activePlayerPanel.gridPrey.transform;
            card.transform.localPosition = new Vector3(1000f, 0f, 0f);
            card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 2);
        }
        foreach (CBaseCard card in activePlayer.snails) {
            card.transform.parent = activePlayerPanel.gridSnails.transform;
            card.transform.localPosition = new Vector3(1000f, 0f, 0f);
            card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL + 2);
        }
        foreach (CBaseCard card in activePlayer.hand) {
            card.transform.parent = activePlayerPanel.gridHand.transform;
            card.transform.localPosition = new Vector3(0f, -250f, 0f);
            card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 4);
        }
        foreach (CBaseCard card in activePlayer.discard) {
            card.transform.parent = activePlayerPanel.goDiscard.transform;
            card.transform.localPosition = new Vector3(0f, 0f, 0f);
            card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
        }

        CGlobals.UpdateWidgets();
        yield return new WaitForEndOfFrame();
    }
	
	public void CreatePlayers()
	{
		//GameObject playerPrefab = Resources.Load("Player") as GameObject;
		for(int i = 0; i < numPlayers; ++i)
		{
			CPlayer player = null;

			if(i == 0) {
                GameObject tmpPlayer = Resources.Load("Opponent/Player1") as GameObject;
                GameObject goOppPlayer = (GameObject)Instantiate(tmpPlayer, Vector3.zero, Quaternion.identity);
                goOppPlayer.name = "Player1";
                goOppPlayer.transform.parent = COpponentsPanel.instance.anchorPanelBackground.transform;
                goOppPlayer.transform.localScale = Vector3.one;
				player = goOppPlayer.AddComponent<CPlayer>();
				player.transform.localEulerAngles = new Vector3(0f,0f,0f);
				player.transform.localPosition = new Vector3(0f,0f,0f);
				player.IsActivePlayer = true;
                COpponentsPanel.instance.lstOpponents.Add(goOppPlayer.GetComponent<COpponent>());
			}
			else if(i == 1) {
				GameObject tmpPlayer = Resources.Load("Opponent/Player2") as GameObject;
                GameObject goOppPlayer = (GameObject)Instantiate(tmpPlayer, Vector3.zero, Quaternion.identity);
                goOppPlayer.name = "Player2";
                goOppPlayer.transform.parent = COpponentsPanel.instance.anchorPanelBackground.transform;
                goOppPlayer.transform.localScale = Vector3.one;
				player = goOppPlayer.AddComponent<CPlayer>();
				player.transform.localEulerAngles = new Vector3(0f,0f,0f);
                player.AIControlled = CGlobals.PLAY_VS_COMPUTER;
                COpponentsPanel.instance.lstOpponents.Add(goOppPlayer.GetComponent<COpponent>());
			}
			else if(i == 2) {
				GameObject tmpPlayer = Resources.Load("Opponent/Player3") as GameObject;
                GameObject goOppPlayer = (GameObject)Instantiate(tmpPlayer, Vector3.zero, Quaternion.identity);
                goOppPlayer.name = "Player3";
                goOppPlayer.transform.parent = COpponentsPanel.instance.anchorPanelBackground.transform;
                goOppPlayer.transform.localScale = Vector3.one;
				player = goOppPlayer.AddComponent<CPlayer>();
				player.transform.localEulerAngles = new Vector3(0f,0f,0f);
                player.AIControlled = CGlobals.PLAY_VS_COMPUTER;
                COpponentsPanel.instance.lstOpponents.Add(goOppPlayer.GetComponent<COpponent>());
			}
			else if(i == 3) {
				GameObject tmpPlayer = Resources.Load("Opponent/Player4") as GameObject;
                GameObject goOppPlayer = (GameObject)Instantiate(tmpPlayer, Vector3.zero, Quaternion.identity);
                goOppPlayer.name = "Player4";
                goOppPlayer.transform.parent = COpponentsPanel.instance.anchorPanelBackground.transform;
                goOppPlayer.transform.localScale = Vector3.one;
				player = goOppPlayer.AddComponent<CPlayer>();
				player.transform.localEulerAngles = new Vector3(0f,0f,0f);
                player.AIControlled = CGlobals.PLAY_VS_COMPUTER;
                COpponentsPanel.instance.lstOpponents.Add(goOppPlayer.GetComponent<COpponent>());
			}

			players.Add(player);
		}

		activePlayer = players[0];
        currentActivePlayer = PlayerData.PLAYER.ONE;
		activePlayer.IsActivePlayer = true;
	}

    public IEnumerator EndTurn()
    {
        yield return new WaitForEndOfFrame();
        CUIManager uiMan = CUIManager.instance;

        uiMan.SelectedCards.Clear();

        // Move all cards out of view
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridHand.gameObject, "y", -250f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridSnails.gameObject, "x", 1400f, 0.75f, iTween.EaseType.easeOutCubic, true);
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridPrey.gameObject, "x", 1400f, 0.75f, iTween.EaseType.easeOutCubic, true);

        StartCoroutine(CCabalManager.instance.TransformSprites(false));
        
        CPlayer playerEndTurn = players[(int)activePlayer.player];

        if(playerEndTurn.AIControlled) {
            yield return ChangePlayer_SetNextTurn();
            yield return BeginNextTurn_CR();
        }
        else
    		yield return uiMan.actionPanel.FadeContinuePrompt();
	}

    public IEnumerator ChangePlayer_SetNextTurn()
    {
        yield return new WaitForEndOfFrame();

        // Change Player
		PlayerData.PLAYER eNumPlayers = (PlayerData.PLAYER)CGameManager.instance.numPlayers;
		CPlayer playerEndTurn = players[(int)activePlayer.player];
		CPlayer playerNextTurn = null;

        // Switch to next player
		dealNewRound = true;
        int player = 0;
        PlayerData.PLAYER lastPlayer = activePlayer.player;
		PlayerData.PLAYER currPlayer = lastPlayer;

		while(player < CGameManager.instance.numPlayers)
        {
            currPlayer = CGlobals.GetNextPlayer(currPlayer);
			//activePlayer = players[(int)currPlayer];

			// If next player has cards, continue setting up cards
			// else, continue to next player
			if(players[(int)currPlayer].hand.Count > 0) {
				playerNextTurn = players[(int)currPlayer];
				activePlayer = playerNextTurn;
				dealNewRound = false;
				break;
			}

			player++;
        }

        // No players have cards. Set next player's turn.
        if(playerNextTurn == null)
        {
			lastPlayer++;
			if(lastPlayer >= eNumPlayers) 
				lastPlayer = PlayerData.PLAYER.ONE;

			playerNextTurn = players[(int)lastPlayer];
			activePlayer = playerNextTurn;
        }

		//foreach(CBaseCard card in playerNextTurn.prey)
		//	card.EnableButton(true);
		//foreach(CBaseCard card in playerNextTurn.snails)
		//	card.EnableButton(true);

		CGlobals.TweenMove(playerNextTurn.gridPrey.gameObject, "y", 1400f, 1f, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(0.75f);

        // Set next active player
        yield return StartCoroutine(SetNextActivePlayer(playerEndTurn));
		playerEndTurn.gridPrey.UpdateGrid();

        // Enable Basic Prey card
		playerNextTurn.EnableBasicPreyCard();

		foreach(CPlayer plyr in Players)
			if(plyr.player != playerNextTurn.player)
				foreach(CPreyCard preyCard in plyr.prey)
					preyCard.EnableButton(false);

        CActivePlayerPanel.instance.lblHandCount.text = playerNextTurn.hand.Count.ToString();

        CActivePlayerPanel.instance.gridHand.UpdateGrid();
        CActivePlayerPanel.instance.gridSnails.UpdateGrid();
        CActivePlayerPanel.instance.gridPrey.UpdateGrid();

        playerEndTurn.gridHand.SetChildrenPositionInstant();
        playerEndTurn.gridSnails.SetChildrenPositionInstant();
        playerEndTurn.gridPrey.SetChildrenPositionInstant();

        playerEndTurn.gridHand.UpdateGrid();
		playerEndTurn.gridSnails.UpdateGrid();
		playerEndTurn.gridPrey.UpdateGrid();

		if(dealNewRound)
		{
			CGlobals.TweenMove(CActivePlayerPanel.instance.gridHand.gameObject, "y", -25f, 0.5f, iTween.EaseType.easeOutSine, true);
			yield return new WaitForSeconds(1f);

			CActivePlayerPanel activePlayerPanel = CActivePlayerPanel.instance;
			CGlobals.TweenMove(activePlayerPanel.gridDiscard.gameObject, "position", Vector3.up * 410f, 0.75f, iTween.EaseType.easeOutSine, true);
			activePlayerPanel.gridDiscard.UpdateGrid(1.25f);
			yield return new WaitForSeconds(1.55f);

			foreach(CPlayer plyer in Players)
			{
				List<CBaseCard> lstChosenCards = new List<CBaseCard>(5);
				List<CBaseCard> lstAvailable = new List<CBaseCard>(plyer.discard.Count);

				for(int i = 0; i < plyer.discard.Count; ++i)
					lstAvailable.Add(plyer.discard[i]);

				while(lstChosenCards.Count < 5) {
					int chosenIndex = Random.Range(0, lstAvailable.Count-1);
					lstChosenCards.Add(lstAvailable[chosenIndex]);
					lstAvailable.RemoveAt(chosenIndex);
				}

				// Send to Hand panel
				foreach(CBaseCard card in lstChosenCards) {
					plyer.hand.Add(card);
					plyer.discard.Remove(card);
					card.transform.parent = (plyer == activePlayer) ? activePlayerPanel.gridHand.transform : plyer.gridHand.transform;
				}
			}
			CGlobals.UpdateWidgets();

			activePlayerPanel.gridHand.UpdateGrid(1f);
			activePlayerPanel.gridDiscard.enabled = false;

			foreach(CBaseCard card in activePlayer.discard) {
				CGlobals.TweenMove(card.gameObject, "position", Vector3.zero, 0.75f, iTween.EaseType.easeOutSine, true);
				CGlobals.TweenScale(card.gameObject, Vector3.one * 0.1f, 0.75f, iTween.EaseType.easeOutSine, true);
			}
			yield return new WaitForSeconds(0.75f);

			CGlobals.TweenMove(activePlayerPanel.gridDiscard.gameObject, "position", new Vector3(602f, -25f, 0f), 1f, iTween.EaseType.easeOutSine, true);
			yield return new WaitForSeconds(1.5f);

            foreach (CPlayer plyr in CGameManager.instance.Players)
                foreach (CSnailCard snailCard in plyr.snails)
                    StartCoroutine(snailCard.SetFedState(snailCard.fedState - 1));
		}
    }

	public void BeginNextTurn() { StartCoroutine(BeginNextTurn_CR()); }
	public IEnumerator BeginNextTurn_CR()
	{
		CUIManager uiMan = CUIManager.instance;
        StartCoroutine(CUIManager.instance.actionPanel.FadeBG(false));
		//StartCoroutine(uiMan.actionPanel.FadeContinuePrompt(false));
		StartCoroutine(CCabalManager.instance.TransformSprites(true));

		// If dealing new round, rotate cards to face up for all players
		if(dealNewRound) {
            foreach (CPlayer player in Players)
                foreach (CBaseCard card in player.hand)
                    StartCoroutine(card.FlipCard(0f, 0.8f, true));
					//CGlobals.TweenRotate(card.gameObject, Vector3.zero, 0.75f, iTween.EaseType.linear, true);
		}

        uiMan.EnableAllBoxCollidersForBeginTurn();

        switch(CGameManager.instance.numPlayers)        {
            case 2: StartCoroutine(COpponentsPanel.instance.ShiftOpponentPanel_TwoPlayers());
                break;

            case 3: StartCoroutine(COpponentsPanel.instance.ShiftOpponentPanel_ThreePlayers());
                break;

            case 4: StartCoroutine(COpponentsPanel.instance.ShiftOpponentPanel_FourPlayers());
                break;
        }

        PlayerData.PLAYER eNumPlayers = (PlayerData.PLAYER)CGameManager.instance.numPlayers;
		CPlayer currentPlayer = players[(int)activePlayer.player];
		CPlayer previousPlayer = null;

        if (activePlayer.player == PlayerData.PLAYER.ONE)
            previousPlayer = players[(int)(eNumPlayers - 1)];
        else
            previousPlayer = players[(int)(currentPlayer.player - 1)];


        foreach (CBaseCard card in previousPlayer.prey)
            card.EnableButton(false);
        foreach (CBaseCard card in currentPlayer.prey)
            card.EnableButton(true);
        foreach (CBaseCard card in currentPlayer.snails)
            card.EnableButton(true);
        foreach (CBaseCard card in currentPlayer.hand)
            card.EnableButton(true);

        // Move cards back in view
        yield return new WaitForSeconds(0.5f);
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridHand.gameObject, "y", -25f, 0.5f, iTween.EaseType.easeOutSine, true);
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridSnails.gameObject, "x", 0f, 0.5f, iTween.EaseType.easeOutCubic, true);
        CGlobals.TweenMove(CActivePlayerPanel.instance.gridPrey.gameObject, "x", 0f, 0.5f, iTween.EaseType.easeOutCubic, true);
		//CGlobals.TweenMove(CGameManager.instance.activePlayer.gridPrey.gameObject, "y", 67f, 0.5f, iTween.EaseType.easeOutSine, true);

		for(int i = 0; i < players.Count; ++i)
			COpponentsPanel.instance.lstOpponents[i].lblHandCount.text = players[i].hand.Count.ToString();

        CUIManager.instance.actionPanel.goFeedButton.GetComponent<BoxCollider>().enabled = true;
		yield return new WaitForSeconds(1.5f);
		foreach(CSnailCard snailCard in currentPlayer.snails)
			snailCard.EnableButton(true);



        if (activePlayer.AIControlled)
        {
            yield return CAIDirector.instance.DecideNextMove();
        }
    }


    public IEnumerator ShowPlayerPrompt()
    {
        yield return new WaitForEndOfFrame();
    }
}
