using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class COpponentsPanel : CBasePanel
{
	#region Singleton Instance
	private static COpponentsPanel _instance;
	public static COpponentsPanel instance {
        get {
            if (_instance == null) {
				_instance = FindObjectOfType<COpponentsPanel>();
            }
            return _instance;
        }
    }
    #endregion

    public List<COpponent> lstOpponents;


	protected override void Start()
    {
        base.Start();
    }

    public void RemoveCreatedPlayers()
    {
        for (int i = 0; i < lstOpponents.Count; ++i)
            Destroy(lstOpponents[i].gameObject);

        lstOpponents.Clear();
    }

    public void EnableSnailButtons()
    {
		foreach(COpponent opponent in lstOpponents)
    		foreach(COpponentSnail oppSnail in opponent.lstSnails)
        		oppSnail.EnableButton();
    }

    public void EnablePlayer1Buttons()
    {
        foreach(COpponentSnail oppSnail in lstOpponents[0].lstSnails)
        	oppSnail.EnableButton();
    }
    public void EnablePlayer2Buttons()
    {
        foreach(COpponentSnail oppSnail in lstOpponents[1].lstSnails)
        	oppSnail.EnableButton();
    }

    public void EnablePlayer3Buttons()
    {
        foreach(COpponentSnail oppSnail in lstOpponents[2].lstSnails)
        	oppSnail.EnableButton();
    }

    public void EnablePlayer4Buttons()
    {
        foreach(COpponentSnail oppSnail in lstOpponents[3].lstSnails)
        	oppSnail.EnableButton();
    }

    public void DisableSnailButtons()
    {
		foreach(COpponent opponent in lstOpponents)
    		foreach(COpponentSnail oppSnail in opponent.lstSnails) {
        			oppSnail.EnableButton(false);
        			CGlobals.AssignNewUIButtonOnClickTarget(CUIManager.instance, oppSnail.snailCard, oppSnail.snailCard.Button, "MoveCardToActionPanel");
        	}
    }

	public void SetToFirstPosition(PlayerData.PLAYER player)
    {
		CUIManager uiMan = CUIManager.instance;
    	int i = (int)player;
		lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, uiMan.cabalPanel.anchorPanelBackground.transform, 1f, -4f);
		lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, uiMan.cabalPanel.anchorPanelBackground.transform, 1f, uiMan.headerWidth-1);
		lstOpponents[i].uiHeader.UpdateAnchors();

		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth-1);
		lstOpponents[i].uiPrey.UpdateAnchors();

		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth-1);
		lstOpponents[i].uiInfo.UpdateAnchors();
	}

	public void SetToSecondPosition(PlayerData.PLAYER player)
    {
		CUIManager uiMan = CUIManager.instance;
        CGameManager gameMan = CGameManager.instance;
    	int i = (int)player;
		int prevOpp = (i == 0) ? gameMan.numPlayers-1 : i-1;

        bool lastOpponentPosition = (gameMan.numPlayers == (int)PlayerData.PLAYER.TWO+1) ? true : false;
        if(lastOpponentPosition)
        {
            CGlobals.TweenMove(gameMan.Players[i].gridPrey.gameObject, "y", 120f, 0.1f, iTween.EaseType.linear, true);
            lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, uiMan.uiCamera.transform, 1f, 0f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, uiMan.uiCamera.transform, 1f, uiMan.headerWidth-1);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }
        else
        {
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[prevOpp].uiPrey.Sprite.transform, 1f, -1f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[prevOpp].uiPrey.Sprite.transform, 1f, uiMan.headerWidth-1);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }

		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth-1);
		lstOpponents[i].uiPrey.UpdateAnchors();

		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth-1);
		lstOpponents[i].uiInfo.UpdateAnchors();
	}	

	public void SetToThirdPosition(PlayerData.PLAYER player)
    {
		CUIManager uiMan = CUIManager.instance;
    	CGameManager gameMan = CGameManager.instance;
    	int i = (int)player;
		int prevOpp = (i == 0) ? gameMan.numPlayers-1 : i-1;

        bool lastOpponentPosition = (gameMan.numPlayers == (int)PlayerData.PLAYER.THREE+1) ? true : false;
        if(lastOpponentPosition)
        {
            CGlobals.TweenMove(gameMan.Players[i].gridPrey.gameObject, "y", 120f, 0.1f, iTween.EaseType.linear, true);
            lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, uiMan.uiCamera.transform, 1f, 0f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, uiMan.uiCamera.transform, 1f, uiMan.headerWidth-1);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }
        else
        {
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[prevOpp].uiPrey.Sprite.transform, 1f, -1f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[prevOpp].uiPrey.Sprite.transform, 1f, uiMan.headerWidth-1);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }

    	// Set Prey/Info left & right anchors
		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth);
		lstOpponents[i].uiPrey.UpdateAnchors();

		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth);
		lstOpponents[i].uiInfo.UpdateAnchors();
	}

	// Off-screen
	public void SetToFourthPosition(PlayerData.PLAYER player)
    {
		CUIManager uiMan = CUIManager.instance;
    	CGameManager gameMan = CGameManager.instance;
    	int i = (int)player;
		int prevOpp = (i == 0) ? gameMan.numPlayers-1 : i-1;

        bool lastOpponentPosition = (gameMan.numPlayers == (int)PlayerData.PLAYER.FOUR+1) ? true : false;
        if(lastOpponentPosition)
        {
            CGlobals.TweenMove(gameMan.Players[i].gridPrey.gameObject, "y", 120f, 0.1f, iTween.EaseType.linear, true);
            lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, uiMan.uiCamera.transform, 1f, 0f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, uiMan.uiCamera.transform, 1f, uiMan.headerWidth-1);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }
        else
        {
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Left, uiMan.uiCamera.transform, 1f, 2f);
		    lstOpponents[i].uiHeader.SetAnchor(NGUIAnchorController.Anchor.Right, uiMan.uiCamera.transform, 1f, uiMan.headerWidth);
		    lstOpponents[i].uiHeader.UpdateAnchors();
        }

		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiPrey.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth);
		lstOpponents[i].uiPrey.UpdateAnchors();

		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Left, lstOpponents[i].uiHeader.Sprite.transform, 1f, -1f);
		lstOpponents[i].uiInfo.SetAnchor(NGUIAnchorController.Anchor.Right, lstOpponents[i].uiHeader.Sprite.transform, 1f, uiMan.preyWidth);
		lstOpponents[i].uiInfo.UpdateAnchors();
	}
	

	public IEnumerator ShiftOpponentPanel()
    {
        yield return new WaitForEndOfFrame();
	    CUIManager uiMan = CUIManager.instance;
		Transform trCamera = uiMan.uiCamera.transform;
		float oppPanelWidth = uiMan.opponentPanelWidth;

        // Move hand panels to original position.
        foreach (CPlayer player in CGameManager.instance.Players) {
            player.goHandPanel.transform.localPosition += new Vector3(oppPanelWidth, 0f, 0f);
        }

		PlayerData.PLAYER prevPlayer = CGlobals.GetPreviousPlayer(CGameManager.instance.activePlayer.player);

        // Empty all needed anchors
		PlayerData.PLAYER iterPlayer = PlayerData.PLAYER.ONE;
        foreach(COpponent opp in lstOpponents) {
        	opp.uiHeader.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiInfo.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiPrey.ReleaseAnchors(NGUIAnchorController.Anchor.All);

			float xPos = opp.transform.localPosition.x - (oppPanelWidth);
			float xPosShort = opp.transform.localPosition.x - (oppPanelWidth - (oppPanelWidth * 0.1f));

			if(iterPlayer != CGameManager.instance.activePlayer.player)
				CGlobals.TweenMove(opp.gameObject, "x", xPos, 0.75f, iTween.EaseType.easeOutSine, true);
			else
				CGlobals.TweenMove(opp.gameObject, "x", xPosShort, 0.75f, iTween.EaseType.easeOutSine, true);

			iterPlayer++;
        }
        yield return new WaitForSeconds(0.75f);

        // When movement is complete... do the following
        switch(CGameManager.instance.activePlayer.player) {
            case PlayerData.PLAYER.TWO: {
				SetToFirstPosition(PlayerData.PLAYER.THREE);
				SetToSecondPosition(PlayerData.PLAYER.FOUR);
				SetToThirdPosition(PlayerData.PLAYER.ONE);
				SetToFourthPosition(PlayerData.PLAYER.TWO);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

			case PlayerData.PLAYER.THREE: {
				SetToFirstPosition(PlayerData.PLAYER.FOUR);
				SetToSecondPosition(PlayerData.PLAYER.ONE);
				SetToThirdPosition(PlayerData.PLAYER.TWO);
				SetToFourthPosition(PlayerData.PLAYER.THREE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

			case PlayerData.PLAYER.FOUR: {
				SetToFirstPosition(PlayerData.PLAYER.ONE);
				SetToSecondPosition(PlayerData.PLAYER.TWO);
				SetToThirdPosition(PlayerData.PLAYER.THREE);
				SetToFourthPosition(PlayerData.PLAYER.FOUR);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

		case PlayerData.PLAYER.ONE: {
				SetToFirstPosition(PlayerData.PLAYER.TWO);
				SetToSecondPosition(PlayerData.PLAYER.THREE);
				SetToThirdPosition(PlayerData.PLAYER.FOUR);
				SetToFourthPosition(PlayerData.PLAYER.ONE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;
        }
    }

    public IEnumerator ShiftOpponentPanel_TwoPlayers()
    {
        yield return new WaitForEndOfFrame();
	    CUIManager uiMan = CUIManager.instance;
		Transform trCamera = uiMan.uiCamera.transform;
		float oppPanelWidth = uiMan.opponentPanelWidth;

        // Move hand panels to original position.
        foreach (CPlayer player in CGameManager.instance.Players) {
            player.goHandPanel.transform.localPosition += new Vector3(oppPanelWidth, 0f, 0f);
        }

		PlayerData.PLAYER prevPlayer = CGlobals.GetPreviousPlayer(CGameManager.instance.activePlayer.player);

        // Empty all needed anchors
		PlayerData.PLAYER iterPlayer = PlayerData.PLAYER.ONE;
        foreach(COpponent opp in lstOpponents) {
        	opp.uiHeader.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiInfo.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiPrey.ReleaseAnchors(NGUIAnchorController.Anchor.All);

			float xPos = opp.transform.localPosition.x - (oppPanelWidth);
			float xPosShort = opp.transform.localPosition.x - (oppPanelWidth - (oppPanelWidth * 0.1f));

			if(iterPlayer != CGameManager.instance.activePlayer.player)
				CGlobals.TweenMove(opp.gameObject, "x", xPos, 0.75f, iTween.EaseType.easeOutSine, true);
			else
				CGlobals.TweenMove(opp.gameObject, "x", xPosShort, 0.75f, iTween.EaseType.easeOutSine, true);

			iterPlayer++;
        }
        yield return new WaitForSeconds(0.75f);

        // When movement is complete... do the following
        switch(CGameManager.instance.activePlayer.player)
        {
            case PlayerData.PLAYER.ONE: {
				SetToFirstPosition(PlayerData.PLAYER.TWO);
				SetToSecondPosition(PlayerData.PLAYER.ONE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

            case PlayerData.PLAYER.TWO: {
				SetToFirstPosition(PlayerData.PLAYER.ONE);
				SetToSecondPosition(PlayerData.PLAYER.TWO);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;
        }
    }

    public IEnumerator ShiftOpponentPanel_ThreePlayers()
    {
        yield return new WaitForEndOfFrame();
	    CUIManager uiMan = CUIManager.instance;
		Transform trCamera = uiMan.uiCamera.transform;
		float oppPanelWidth = uiMan.opponentPanelWidth;

        // Move hand panels to original position.
        foreach (CPlayer player in CGameManager.instance.Players) {
            player.goHandPanel.transform.localPosition += new Vector3(oppPanelWidth, 0f, 0f);
        }

		PlayerData.PLAYER prevPlayer = CGlobals.GetPreviousPlayer(CGameManager.instance.activePlayer.player);

        // Empty all needed anchors
		PlayerData.PLAYER iterPlayer = PlayerData.PLAYER.ONE;
        foreach(COpponent opp in lstOpponents) {
        	opp.uiHeader.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiInfo.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiPrey.ReleaseAnchors(NGUIAnchorController.Anchor.All);

			float xPos = opp.transform.localPosition.x - (oppPanelWidth);
			float xPosShort = opp.transform.localPosition.x - (oppPanelWidth - (oppPanelWidth * 0.1f));

			if(iterPlayer != CGameManager.instance.activePlayer.player)
				CGlobals.TweenMove(opp.gameObject, "x", xPos, 0.75f, iTween.EaseType.easeOutSine, true);
			else
				CGlobals.TweenMove(opp.gameObject, "x", xPosShort, 0.75f, iTween.EaseType.easeOutSine, true);

			iterPlayer++;
        }
        yield return new WaitForSeconds(0.75f);



        // When movement is complete... do the following
        switch(CGameManager.instance.activePlayer.player)
        {
            case PlayerData.PLAYER.ONE: {
				SetToFirstPosition(PlayerData.PLAYER.TWO);
				SetToSecondPosition(PlayerData.PLAYER.THREE);
				SetToThirdPosition(PlayerData.PLAYER.ONE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

            case PlayerData.PLAYER.TWO: {
				SetToFirstPosition(PlayerData.PLAYER.THREE);
				SetToSecondPosition(PlayerData.PLAYER.ONE);
				SetToThirdPosition(PlayerData.PLAYER.TWO);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

			case PlayerData.PLAYER.THREE: {
				SetToFirstPosition(PlayerData.PLAYER.ONE);
				SetToSecondPosition(PlayerData.PLAYER.TWO);
				SetToThirdPosition(PlayerData.PLAYER.THREE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;
        }
    }

    public IEnumerator ShiftOpponentPanel_FourPlayers()
    {
        yield return new WaitForEndOfFrame();
	    CUIManager uiMan = CUIManager.instance;
		Transform trCamera = uiMan.uiCamera.transform;
		float oppPanelWidth = uiMan.opponentPanelWidth;

        // Move hand panels to original position.
        foreach (CPlayer player in CGameManager.instance.Players) {
            player.goHandPanel.transform.localPosition += new Vector3(oppPanelWidth, 0f, 0f);
        }

		PlayerData.PLAYER prevPlayer = CGlobals.GetPreviousPlayer(CGameManager.instance.activePlayer.player);

        // Empty all needed anchors
		PlayerData.PLAYER iterPlayer = PlayerData.PLAYER.ONE;
        foreach(COpponent opp in lstOpponents) {
        	opp.uiHeader.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiInfo.ReleaseAnchors(NGUIAnchorController.Anchor.All);
			opp.uiPrey.ReleaseAnchors(NGUIAnchorController.Anchor.All);

			float xPos = opp.transform.localPosition.x - (oppPanelWidth);
			float xPosShort = opp.transform.localPosition.x - (oppPanelWidth - (oppPanelWidth * 0.1f));

			if(iterPlayer != CGameManager.instance.activePlayer.player)
				CGlobals.TweenMove(opp.gameObject, "x", xPos, 0.75f, iTween.EaseType.easeOutSine, true);
			else
				CGlobals.TweenMove(opp.gameObject, "x", xPosShort, 0.75f, iTween.EaseType.easeOutSine, true);

			iterPlayer++;
        }
        yield return new WaitForSeconds(0.75f);

        // When movement is complete... do the following
        switch(CGameManager.instance.activePlayer.player) {
            case PlayerData.PLAYER.TWO: {
				SetToFirstPosition(PlayerData.PLAYER.THREE);
				SetToSecondPosition(PlayerData.PLAYER.FOUR);
				SetToThirdPosition(PlayerData.PLAYER.ONE);
				SetToFourthPosition(PlayerData.PLAYER.TWO);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

			case PlayerData.PLAYER.THREE: {
				SetToFirstPosition(PlayerData.PLAYER.FOUR);
				SetToSecondPosition(PlayerData.PLAYER.ONE);
				SetToThirdPosition(PlayerData.PLAYER.TWO);
				SetToFourthPosition(PlayerData.PLAYER.THREE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

			case PlayerData.PLAYER.FOUR: {
				SetToFirstPosition(PlayerData.PLAYER.ONE);
				SetToSecondPosition(PlayerData.PLAYER.TWO);
				SetToThirdPosition(PlayerData.PLAYER.THREE);
				SetToFourthPosition(PlayerData.PLAYER.FOUR);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;

		case PlayerData.PLAYER.ONE: {
				SetToFirstPosition(PlayerData.PLAYER.TWO);
				SetToSecondPosition(PlayerData.PLAYER.THREE);
				SetToThirdPosition(PlayerData.PLAYER.FOUR);
				SetToFourthPosition(PlayerData.PLAYER.ONE);

                CGlobals.UpdateWidgets();
                yield return new WaitForEndOfFrame();
            }
            break;
        }
    }

























    public void AddSnailToOpponent(CBaseCard card)
    {
		int i = (int)CGameManager.instance.activePlayer.player;

		GameObject go = ((GameObject)Instantiate(Resources.Load("Opponent/OpponentSnail") as GameObject, Vector3.zero, Quaternion.identity));
		go.transform.localScale = Vector3.zero;
		go.transform.localPosition = Vector3.zero;
		go.transform.parent = lstOpponents[i].gridSnails.transform;

		COpponentSnail opponentSnail = go.GetComponent<COpponentSnail>();
		((CSnailCard)card).opponentSnail = opponentSnail;
		opponentSnail.snailCard = (CSnailCard)card;

		lstOpponents[i].lstSnails.Add(opponentSnail);
		lstOpponents[i].gridSnails.UpdateGrid();
    }
}
