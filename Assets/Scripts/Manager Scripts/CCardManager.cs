using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CCardManager : MonoBehaviour 
{
    #region Singleton Instance
    private static CCardManager _instance;
    public static CCardManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<CCardManager>();
            }
            return _instance;
        }
    }
    #endregion

    private int NUM_STARTER_SNAIL_CARDS = 4;
    private int NUM_STARTER_RESEARCH_CARDS = 2;
    private int NUM_STARTER_OCEAN_WAVES_CARDS = 2;
    private int NUM_STARTER_POTENCY_CARDS = 1;
    private int NUM_STARTER_STINGRAY_CARDS = 1;

    public TextAsset txtCardData;
    public GameObject goDrawPile, goPreyDrawPile, goDiscardPile;
    public List<CBaseCard> lstDeckOfCards;

    public List<CBaseCard> lstDrawPile;
    public List<CBaseCard> lstDrawPile_Prey;
    public List<CBaseCard> lstDiscardPile_Prey;


    void Start()
    {
        lstDeckOfCards = new List<CBaseCard>();
        lstDrawPile = new List<CBaseCard>();
        lstDrawPile_Prey = new List<CBaseCard>();
        lstDiscardPile_Prey = new List<CBaseCard>();
    }


    public void CreateDeck()
    {
        CLogfile.instance.Append("Creating Deck");

        List<string> lines = new List<string>();
		string currentLine = string.Empty;

        for(int i = 0; i < txtCardData.bytes.Length; ++i) {
        	char c = (char)txtCardData.bytes[i];
			currentLine += c;

        	if((int)c == 10) {
        		currentLine = currentLine.Trim();
				lines.Add(currentLine);
        		currentLine = string.Empty;
        	}
        }

        int currLine = 0;
        currentLine = string.Empty;
        do
        {
			currentLine = lines[currLine];
			currLine++;

			if (currentLine != null)
            {
				string[] data = currentLine.Split(',');
                if (data.Length > 0)
                {
                    GameObject prefab = Resources.Load(data[0]) as GameObject;

                    int count = int.Parse(data[1]);
                    for (int i = 0; i < count; ++i)
                    {
                        GameObject go = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
                        go.transform.parent = goDrawPile.transform;
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                        lstDeckOfCards.Add(go.GetComponent<CBaseCard>());
                    }
                }
            }
        }
		while (currLine < lines.Count);

        CGameManager gameMan = CGameManager.instance;
        CUIManager uiMan = CUIManager.instance;
        CTutorialDirector tutDirector = CTutorialDirector.instance;

        foreach (CBaseCard card in lstDeckOfCards)
        {
            if(card.cardType != CardData.CardType.Prey) {
                if(CGlobals.TUTORIAL_ACTIVE)
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, card, card.Button, "MoveCardToSnailsPanel");
                else
                    CGlobals.AssignNewUIButtonOnClickTarget(uiMan, card, card.Button, "MoveCardToActionPanel");
            }
    
            if (card.cardType == CardData.CardType.Prey) {
            	card.transform.parent = goPreyDrawPile.transform;
                lstDrawPile_Prey.Add(card);
            }
            else
                lstDrawPile.Add(card);
        }

        //lstDeckOfCards.Clear();
    }

    public void DeleteDeck()
    {
        for(int i = 0; i < lstDeckOfCards.Count; ++i)
            Destroy(lstDeckOfCards[i].gameObject);

        lstDeckOfCards.Clear();
        lstDrawPile.Clear();
        lstDrawPile_Prey.Clear();
        lstDiscardPile_Prey.Clear();
    }


    public void DealHandCardsToPlayers()
    {
        CLogfile.instance.Append("Dealing hand cards to players");

        if(CGlobals.TUTORIAL_ACTIVE)
        {
            DealHandCardsTutorial();
        }
        else
        {
            DealHandCardsNormal();
        }
    }

    public void DealHandCardsTutorial()
    {
        List<CBaseCard> starterPulicarius = new List<CBaseCard>();
        List<CBaseCard> starterGeographus = new List<CBaseCard>();
        List<CBaseCard> starterCalifornicus = new List<CBaseCard>();
        List<CBaseCard> starterGloriamaris = new List<CBaseCard>();

        //List<CBaseCard> starterSnails = new List<CBaseCard>();
        List<CBaseCard> starterOceanWaves = new List<CBaseCard>();
        List<CBaseCard> starterResearch = new List<CBaseCard>();
        List<CBaseCard> starterPotency = new List<CBaseCard>();
        List<CBaseCard> starterStingray = new List<CBaseCard>();

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Sort into lists
        //////////////////////////////////////////////////////////////////////////////////////////////////
        foreach (CBaseCard card in lstDrawPile)
        {
            if (card.starterCard)
            {
                card.CurrentParentPanel = CardData.ParentPanel.HAND;
                if (card.cardType == CardData.CardType.Snail)
                {
                    CSnailCard snailCard = (CSnailCard)card;
                    switch(snailCard.snailSpecies)
                    {
                        case CardData.SnailSpecies.Conus_Californicus:
                        starterCalifornicus.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Geographus:
                        starterGeographus.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Pulicarius:
                        starterPulicarius.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Gloriamaris:
                        starterGloriamaris.Add(snailCard);
                        break;
                    }
                }
                else if (card.cardType == CardData.CardType.Instant)
                {
                    switch (((CInstantCard)card).instantType) {
                        case CardData.InstantType.Ocean_Waves:
                            starterOceanWaves.Add(card);
                            break;

                        case CardData.InstantType.Research:
                            starterResearch.Add(card);
                            break;

                        case CardData.InstantType.Potency:
                            starterPotency.Add(card);
                            break;

                        case CardData.InstantType.Stingray:
                            starterStingray.Add(card);
                            break;
                    }
                }
            }
        }

        List<CBaseCard> usedCards = new List<CBaseCard>();
        foreach (CPlayer player in CGameManager.instance.Players)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Deal Hand cards from lists
            //////////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterCalifornicus[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterCalifornicus.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterGeographus[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterGeographus.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterPulicarius[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterPulicarius.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterGloriamaris[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterGloriamaris.RemoveAt(i);
            }

            for (int i = 0; i < NUM_STARTER_OCEAN_WAVES_CARDS; ++i) {
                int nRand = Random.Range(0, starterOceanWaves.Count);
                CBaseCard cardToAdd = starterOceanWaves[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterOceanWaves.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_RESEARCH_CARDS; ++i) {
                int nRand = Random.Range(0, starterResearch.Count);
                CBaseCard cardToAdd = starterResearch[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterResearch.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_POTENCY_CARDS; ++i) {
                int nRand = Random.Range(0, starterPotency.Count);
                CBaseCard cardToAdd = starterPotency[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterPotency.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_STINGRAY_CARDS; ++i) {
                int nRand = Random.Range(0, starterStingray.Count);
                CBaseCard cardToAdd = starterStingray[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterStingray.RemoveAt(nRand);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Sort out Hand and Discard cards
            //////////////////////////////////////////////////////////////////////////////////////////////////
            List<CBaseCard> lstTutorialHand = new List<CBaseCard>();
            while(lstTutorialHand.Count < 5)
            {
                CBaseCard cardToAdd = null;

                switch(lstTutorialHand.Count)
                {
                    // Snails
                    case 0:
                    {
                        foreach(CBaseCard handCard in player.hand)
                            if(handCard.cardType == CardData.CardType.Snail) {
                                cardToAdd = handCard;
                                break;
                            }

                        lstTutorialHand.Add(cardToAdd);
                        player.hand.Remove(cardToAdd);
                    }
                    break;

                    // Research Card
                    case 1:
                    {
                        foreach(CBaseCard handCard in player.hand)
                            if(handCard.cardType == CardData.CardType.Instant)
                                if(((CInstantCard)handCard).instantType == CardData.InstantType.Research) {
                                    cardToAdd = handCard;
                                    break;
                                }

                        lstTutorialHand.Add(cardToAdd);
                        player.hand.Remove(cardToAdd);
                    }
                    break;

                    // +1 Atk Rating
                    case 2: {
                        foreach(CBaseCard handCard in player.hand)
                            if(handCard.cardType == CardData.CardType.Instant)
                                if(((CInstantCard)handCard).attackRating >= 1) {
                                    cardToAdd = handCard;
                                    break;
                                }

                        lstTutorialHand.Add(cardToAdd);
                        player.hand.Remove(cardToAdd);
                    }
                    break;

                    // Random card 1
                    case 3:
                    {
                        cardToAdd = player.hand[0];
                        lstTutorialHand.Add(cardToAdd);
                        player.hand.Remove(cardToAdd);
                    }
                    break;

                    // Random card 2
                    case 4:
                    {
                        cardToAdd = player.hand[0];
                        lstTutorialHand.Add(cardToAdd);
                        player.hand.Remove(cardToAdd);
                    }
                    break;
                }
            }

            List<CBaseCard> discards = new List<CBaseCard>();
            foreach (CBaseCard handCard in player.hand)
                discards.Add(handCard);

            foreach (CBaseCard discard in discards) {
                player.hand.Remove(discard);
                player.discard.Add(discard);
            }

            foreach (CBaseCard card in lstTutorialHand)
            {
                card.EnableButton(true);
                card.Texture.enabled = true;
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 2);
                //card.transform.parent = player.goDiscardPile.transform;
                card.transform.parent = player.gridHand.transform;
                card.transform.localPosition = Vector3.zero;
                //card.transform.localScale = Vector3.one * 0.1f;
                //card.transform.localEulerAngles = Vector3.up * 180f;
                card.TextureBack.enabled = false;

                player.hand.Add(card);
            }

            int widgetDepth = 1;
            foreach(CBaseCard card in player.discard)
            {
                card.EnableButton(false);
                card.Texture.enabled = false;
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + widgetDepth++);
                card.transform.parent = player.goDiscardPile.transform;
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.one * 0.1f;
                card.transform.localEulerAngles = Vector3.up * 180f;
                card.TextureBack.enabled = true;
            }






            for (int i = 0; i < player.hand.Count; ++i)
            {
                player.hand[i].IndexInList = i;
                player.discard[i].IndexInList = i;
            }

            foreach (CBaseCard usedCard in usedCards)
                lstDrawPile.Remove(usedCard);

            foreach (CBaseCard card in player.hand) {
                card.transform.localPosition = Vector3.up * -250f;
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 1);
            }

            usedCards.Clear();

            // Turn off Hand Parent Obj if NOT active player
            //if (!player.IsActivePlayer)
                //player.gridHand.transform.parent.gameObject.SetActive(false);

            //player.gridHand.UpdateGrid();
            CActivePlayerPanel.instance.lblHandCount.text = player.hand.Count.ToString();
        }
    }

    public void DealHandCardsNormal()
    {
        List<CBaseCard> starterPulicarius = new List<CBaseCard>();
        List<CBaseCard> starterGeographus = new List<CBaseCard>();
        List<CBaseCard> starterCalifornicus = new List<CBaseCard>();
        List<CBaseCard> starterGloriamaris = new List<CBaseCard>();

        //List<CBaseCard> starterSnails = new List<CBaseCard>();
        List<CBaseCard> starterOceanWaves = new List<CBaseCard>();
        List<CBaseCard> starterResearch = new List<CBaseCard>();
        List<CBaseCard> starterPotency = new List<CBaseCard>();
        List<CBaseCard> starterStingray = new List<CBaseCard>();

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Sort into lists
        //////////////////////////////////////////////////////////////////////////////////////////////////
        foreach (CBaseCard card in lstDrawPile)
        {
            if (card.starterCard)
            {
                card.CurrentParentPanel = CardData.ParentPanel.HAND;
                if (card.cardType == CardData.CardType.Snail)
                {
                    CSnailCard snailCard = (CSnailCard)card;
                    switch(snailCard.snailSpecies)
                    {
                        case CardData.SnailSpecies.Conus_Californicus:
                        starterCalifornicus.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Geographus:
                        starterGeographus.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Pulicarius:
                        starterPulicarius.Add(snailCard);
                        break;
                        case CardData.SnailSpecies.Conus_Gloriamaris:
                        starterGloriamaris.Add(snailCard);
                        break;
                    }
                }
                else if (card.cardType == CardData.CardType.Instant)
                {
                    switch (((CInstantCard)card).instantType) {
                        case CardData.InstantType.Ocean_Waves:
                            starterOceanWaves.Add(card);
                            break;

                        case CardData.InstantType.Research:
                            starterResearch.Add(card);
                            break;

                        case CardData.InstantType.Potency:
                            starterPotency.Add(card);
                            break;

                        case CardData.InstantType.Stingray:
                            starterStingray.Add(card);
                            break;
                    }
                }
            }
        }

        List<CBaseCard> usedCards = new List<CBaseCard>();
        foreach (CPlayer player in CGameManager.instance.Players)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Deal Hand cards from lists
            //////////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterCalifornicus[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterCalifornicus.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterGeographus[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterGeographus.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterPulicarius[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterPulicarius.RemoveAt(i);
            }
            for (int i = 0; i < 1; ++i) {
                CBaseCard cardToAdd = starterGloriamaris[i];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterGloriamaris.RemoveAt(i);
            }

            for (int i = 0; i < NUM_STARTER_OCEAN_WAVES_CARDS; ++i) {
                int nRand = Random.Range(0, starterOceanWaves.Count);
                CBaseCard cardToAdd = starterOceanWaves[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterOceanWaves.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_RESEARCH_CARDS; ++i) {
                int nRand = Random.Range(0, starterResearch.Count);
                CBaseCard cardToAdd = starterResearch[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterResearch.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_POTENCY_CARDS; ++i) {
                int nRand = Random.Range(0, starterPotency.Count);
                CBaseCard cardToAdd = starterPotency[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterPotency.RemoveAt(nRand);
            }

            for (int i = 0; i < NUM_STARTER_STINGRAY_CARDS; ++i) {
                int nRand = Random.Range(0, starterStingray.Count);
                CBaseCard cardToAdd = starterStingray[nRand];
                cardToAdd.owner = player.player;
                player.DealCard(cardToAdd);
                usedCards.Add(cardToAdd);
                starterStingray.RemoveAt(nRand);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Sort out Hand and Discard cards
            //////////////////////////////////////////////////////////////////////////////////////////////////
            int widgetDepth = 1;
            while (player.hand.Count > 5) {
                int rand = Random.Range(0, player.hand.Count);
                CBaseCard card = player.hand[rand];

                card.EnableButton(false);
                card.Texture.enabled = false;
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + widgetDepth++);
                card.transform.parent = player.goDiscardPile.transform;
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.one * 0.1f;
                card.transform.localEulerAngles = Vector3.up * 180f;
                card.TextureBack.enabled = true;

                player.discard.Add(card);
                player.hand.RemoveAt(rand);
            }

            for(int i = 0; i < player.hand.Count; ++i)
            {
                player.hand[i].IndexInList = i;
                player.discard[i].IndexInList = i;
            }

            foreach (CBaseCard usedCard in usedCards)
                lstDrawPile.Remove(usedCard);

            foreach (CBaseCard card in player.hand) {
                card.transform.localPosition = Vector3.up * -250f;
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL + 1);
            }

            usedCards.Clear();

            // Turn off Hand Parent Obj if NOT active player
            //if (!player.IsActivePlayer)
                //player.gridHand.transform.parent.gameObject.SetActive(false);

            //player.gridHand.UpdateGrid();
            CActivePlayerPanel.instance.lblHandCount.text = player.hand.Count.ToString();
        }

        //
        //
        // Remove all starter cards from deck if less than 4 players
        if(CGameManager.instance.Players.Count < 4)
        {
            List<CBaseCard> leftoverStarterCards = new List<CBaseCard>();
            for (int i = 0; i < lstDrawPile.Count; ++i)
                if (lstDrawPile[i].starterCard)
                    leftoverStarterCards.Add(lstDrawPile[i]);

            for(int i = 0; i < leftoverStarterCards.Count; ++i)
            {
                lstDeckOfCards.Remove(leftoverStarterCards[i]);
                lstDrawPile.Remove(leftoverStarterCards[i]);
                Destroy(leftoverStarterCards[i].gameObject);
            }
            leftoverStarterCards.Clear();
        }
    }

    public void DealPreyCardsToPlayers()
    {
        CLogfile.instance.Append("Dealing prey cards to players");

        List<CBaseCard> basicPrey = new List<CBaseCard>();
        List<CBaseCard> otherPrey = new List<CBaseCard>();

        foreach (CBaseCard card in lstDrawPile_Prey) {
            if (((CPreyCard)card).preyName == CardData.PreyName.Basic_Prey)
                basicPrey.Add(card);
            else
                otherPrey.Add(card);
        }

        foreach (CPlayer player in CGameManager.instance.Players) {
            int basicRand = Random.Range(0, basicPrey.Count);
            CBaseCard card = basicPrey[basicRand];
            card.transform.parent = player.gridPrey.transform;
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = Vector3.one * 0.4f;
            card.CurrentParentPanel = CardData.ParentPanel.PREY;
			card.owner = player.player;

            if(player.player == PlayerData.PLAYER.ONE)
                card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 1);
            else
                card.ChangeWidgetDepth(CGlobals.OPPONENT_PREY_BG_DEPTH + 1);

            CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

            if (!player.IsActivePlayer) {
                card.transform.localScale = Vector3.one * 0.3f;
				card.EnableButton(false);
            }
            else
				card.EnableButton(true);

            switch (player.player) {
                case PlayerData.PLAYER.TWO:
                    card.ChangeWidgetDepth(13);
                    break;

                case PlayerData.PLAYER.THREE:
                    card.ChangeWidgetDepth(17);
                    break;

                case PlayerData.PLAYER.FOUR:
                    card.ChangeWidgetDepth(21);
                    break;
            }

            basicPrey.RemoveAt(basicRand);
            player.prey.Add(card);
            lstDrawPile_Prey.Remove(card);
            card.gameObject.SetActive( ((player.player == PlayerData.PLAYER.ONE) ? true : false) );

            // Allow players two extra prey cards
            for (int i = 0; i < 2; ++i) {
                int otherRand = Random.Range(0, otherPrey.Count);
                card = otherPrey[otherRand];
                card.transform.parent = player.gridPrey.transform;
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.one * 0.4f;
                card.CurrentParentPanel = CardData.ParentPanel.PREY;
				card.owner = player.player;

                if(player.player == PlayerData.PLAYER.ONE)
                    card.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 1);
                else
                    card.ChangeWidgetDepth(CGlobals.OPPONENT_PREY_BG_DEPTH + 1);

                CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, card, card.Button, "MoveCardToActionPanel");

                if (!player.IsActivePlayer) {
                    card.transform.localScale = Vector3.one * 0.3f;
					card.EnableButton(false);
                }
                else
					card.EnableButton(true);

                otherPrey.RemoveAt(otherRand);
                player.prey.Add(card);
                lstDrawPile_Prey.Remove(card);
            }

            CGlobals.UpdateSiblingIndexes(player.gridPrey.transform);
            player.gridPrey.UpdateGrid();
        }

        // If less than four players, remove extra BasicPrey cards from Prey draw deck
        if(basicPrey.Count > 0)
        {
            foreach (CBaseCard card in basicPrey) {
                lstDrawPile_Prey.Remove(card);
                lstDiscardPile_Prey.Add(card);
                card.gameObject.SetActive(false);
            }

            basicPrey.Clear();
        }

        // Set aside prey deck
        foreach(CBaseCard card in lstDrawPile_Prey) {
            StartCoroutine(card.FlipCard(180f, 0.5f, false));
        	CGlobals.TweenScale(card.gameObject, Vector3.one * 0.4f, 0.5f, iTween.EaseType.linear, true);
        }

        // shuffle prey cards
        List<CBaseCard> tmpPrey = new List<CBaseCard>();
        while(lstDrawPile_Prey.Count > 0){
        	int rand = Random.Range(0, lstDrawPile_Prey.Count-1);
        	tmpPrey.Add(lstDrawPile_Prey[rand]);
        	lstDrawPile_Prey.RemoveAt(rand);
        }

        foreach(CBaseCard card in tmpPrey)
        	lstDrawPile_Prey.Add(card);

        for(int i = 0; i < lstDrawPile_Prey.Count; ++i)
        	lstDrawPile_Prey[i].transform.SetSiblingIndex(i);
    }

    public void DealCardsToMarket()
    {
        CLogfile.instance.Append("Dealing cards to market");

        CMarketPanel marketPanel = CUIManager.instance.marketPanel;

        CBaseCard[] cardstoAdd = new CBaseCard[5];
        int[] randomCards = new int[5];
        for(int i = 0; i < randomCards.Length; ++i) {
            while(cardstoAdd[i] == null) {
                int rand = Random.Range(0, lstDrawPile.Count);
                CBaseCard cardToAdd = lstDrawPile[rand];

                bool bAddCard = true;
                foreach(CBaseCard card in cardstoAdd)  
                    if(card != null && card == cardToAdd) 
                        bAddCard = false;

                if (bAddCard) {
                    cardToAdd.ChangeWidgetDepth(101);
                    cardstoAdd[i] = cardToAdd;
                    lstDrawPile.RemoveAt(rand);
                }
            }
        }

        foreach(CBaseCard card in cardstoAdd) {
            card.Button.enabled = true;
            marketPanel.lstMarketCards.Add(card);
            StartCoroutine(card.SetNewParentPanel(CardData.ParentPanel.MARKET));
            StartCoroutine(CUIManager.instance.AddCardToMarket(card));
        }

        CUIManager.instance.marketPanel.gridMarketCards.UpdateGrid();
    }

//    public void RemovePreyCard(CBaseCard cardToRemove)
//    {
//    	
//    }

    public void ReplacePreyForActivePlayer(CBaseCard cardToReplace)
    {
        CPlayer activePlayer = CGameManager.instance.activePlayer;

        cardToReplace.transform.parent = CCardManager.instance.goDiscardPile.transform;
        cardToReplace.transform.localScale = Vector3.one;
        cardToReplace.transform.localPosition = Vector3.zero;

        CBaseCard newCard = lstDrawPile_Prey[Random.Range(0, lstDrawPile_Prey.Count)];
        StartCoroutine(newCard.SetNewParentPanel(CardData.ParentPanel.PREY));
        newCard.transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
        newCard.transform.localScale = Vector3.one * 0.334f;
        newCard.transform.localPosition = Vector3.right * 618f;
        newCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 1);
		newCard.transform.localEulerAngles = Vector3.zero;
		newCard.Texture.enabled = true;
		newCard.TextureBack.enabled = false;
        CGlobals.UpdateWidgets();

        lstDiscardPile_Prey.Add(cardToReplace);
        lstDrawPile_Prey.Remove(newCard);

        activePlayer.prey.Remove(cardToReplace);
        //activePlayer.prey.Insert(cardToReplace.IndexInList, newCard);
        activePlayer.prey.Add(newCard);
        CGlobals.UpdateSiblingIndexes(activePlayer.gridPrey.transform);
		CActivePlayerPanel.instance.gridPrey.UpdateGrid();
    }

    public void ReplacePreyForActivePlayer_Swap(CBaseCard newPreyCard)
    {
        CPlayer activePlayer = CGameManager.instance.activePlayer;
        
        StartCoroutine(newPreyCard.SetNewParentPanel(CardData.ParentPanel.PREY));
        newPreyCard.transform.parent = CActivePlayerPanel.instance.gridPrey.transform;
        newPreyCard.transform.localScale = Vector3.one * 0.334f;
        newPreyCard.transform.localPosition = Vector3.right * 618f;
        newPreyCard.ChangeWidgetDepth(CGlobals.ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL + 1);
		newPreyCard.transform.localEulerAngles = Vector3.zero;
		newPreyCard.Texture.enabled = true;
		newPreyCard.TextureBack.enabled = false;
        CGlobals.UpdateWidgets();

        lstDrawPile_Prey.Remove(newPreyCard);

        //activePlayer.prey.Insert(cardToReplace.IndexInList, newCard);
        activePlayer.prey.Add(newPreyCard);
        CGlobals.UpdateSiblingIndexes(activePlayer.gridPrey.transform);
		CActivePlayerPanel.instance.gridPrey.UpdateGrid();
    }


    public void EnableTopCardInPreyDeck()
    {
        foreach (CBaseCard card in lstDrawPile_Prey)
            card.gameObject.SetActive(false);
            //card.EnableButton(false);

    	CPreyCard topCard = (CPreyCard)lstDrawPile_Prey[0];
        topCard.gameObject.SetActive(true);
		topCard.EnableButton(false);
        topCard.EnableBacksideButton(true);

        CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance.actionPanel, (CPreyCard)topCard, topCard.TextureBack.GetComponent<UIButton>(), "TakeFromTopOPreyDeck");
    }
}
