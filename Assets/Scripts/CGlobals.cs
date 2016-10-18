using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGlobals : MonoBehaviour 
{
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__ACTION_PANEL = 120;
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__MARKET_PANEL = 110;
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__HAND_PANEL = 90;
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__CABAL_SOLVING_PANEL = 100;
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__SNAILS_PANEL = 60;
    public static readonly int ACTIVE_PLAYER_TEXTURE_STARTING_DEPTH__PREY_PANEL = 50;
    public static readonly int OPPONENT_SNAIL_BG_DEPTH = 42;
    public static readonly int OPPONENT_PREY_BG_DEPTH = 40;

    public static readonly int MAX_MARKET_CARD_COUNT = 5;

    public static readonly float FADE_BG_ALPHA = 0.65f;
    public static readonly float FADE_WATER_ALPHA = 0.5f;
    public static readonly float FADE_BG_DURATION = 0.5f;

    public static readonly Color Player1Color = new Color(0.886f, 0f, 0.239f);
    public static readonly Color Player2Color = new Color(0.082f, 0.725f, 0.459f);
    public static readonly Color Player3Color = new Color(1f, 0.722f, 0f);
    public static readonly Color Player4Color = new Color(0.765f, 0f, 0.886f);
    public static readonly Color DisabledCardColor = new Color(0.65f, 0.65f, 0.65f);

    public static bool TUTORIAL_ACTIVE = false;
    public static bool PLAY_VS_COMPUTER = false;

    public static void TweenMove(GameObject target, string posXYZ, Vector3 position, float time, iTween.EaseType easetype, bool isLocal)
    {
        iTween.MoveTo(target, iTween.Hash(posXYZ, position, "time", time, "easetype", easetype, "islocal", isLocal));
    }
    public static void TweenMove(GameObject target, string posXYZ, float position, float time, iTween.EaseType easetype, bool isLocal)
    {
        iTween.MoveTo(target, iTween.Hash(posXYZ, position, "time", time, "easetype", easetype, "islocal", isLocal));
    }
    

    public static void TweenScale(GameObject target, Vector3 scale, float time, iTween.EaseType easetype, bool isLocal)
    {
        iTween.ScaleTo(target, iTween.Hash("scale", scale, "time", time, "easetype", easetype, "islocal", isLocal));
    }
	public static void TweenScale(GameObject target, string scaleXYZ, float scale, float time, iTween.EaseType easetype, bool isLocal)
    {
        iTween.ScaleTo(target, iTween.Hash("scaleXYZ", scale, "time", time, "easetype", easetype, "islocal", isLocal));
    }


    public static void TweenRotate(GameObject target, Vector3 rotation, float time, iTween.EaseType easetype, bool isLocal = true)
    {
        iTween.RotateTo(target, iTween.Hash("rotation", rotation, "time", time, "easetype", easetype, "islocal", isLocal));
    }


    public static void iTweenValue(GameObject go, Vector3 from, Vector3 to, float duration, GameObject goTarget, string onFunctionTarget, iTween.EaseType easetype = iTween.EaseType.linear, string onComplete = null)
    {
        if(onComplete != null)
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype, "oncomplete", onComplete, "oncompletetarget", goTarget));
        else
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype));
    }

    public static void iTweenValue(GameObject go, Color from, Color to, float duration, GameObject goTarget, string onFunctionTarget, iTween.EaseType easetype = iTween.EaseType.linear, string onComplete = null)
    {
        if(onComplete != null)
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype, "oncomplete", onComplete, "oncompletetarget", goTarget));
        else
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype));
    }

    public static void iTweenValue(GameObject go, int from, int to, float duration, GameObject goTarget, string onFunctionTarget, iTween.EaseType easetype = iTween.EaseType.linear, string onComplete = null)
    {
        if(onComplete != null)
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype, "oncomplete", onComplete, "oncompletetarget", goTarget));
        else
            iTween.ValueTo(go, iTween.Hash("from", from, "to", to, "time", duration, "onupdate", onFunctionTarget, "onupdatetarget", goTarget, "easetype", easetype));
    }




 //   public static void EnableWidgetAnchors(bool enable = true) { EnableWidgetAnchorsRecursively(CGameManager.instance.transform, enable); }
 //   private static void EnableWidgetAnchorsRecursively(Transform root, bool enable)
	//{
 //       foreach (Transform child in root) {
 //           UIWidget widget = child.GetComponent<UIWidget>();
 //           if (widget != null) { widget }

 //           EnableWidgetAnchorsRecursively(child, enable);
 //       }
 //   }


    public static PlayerData.PLAYER GetNextPlayer(PlayerData.PLAYER currentPlayer)
    {
        currentPlayer++;
        	if(currentPlayer >= (PlayerData.PLAYER)CGameManager.instance.numPlayers) {
        		currentPlayer = PlayerData.PLAYER.ONE;
        	}
        return currentPlayer;
    }

	public static PlayerData.PLAYER GetPreviousPlayer(PlayerData.PLAYER currentPlayer)
	{
		currentPlayer--;
		if(currentPlayer < PlayerData.PLAYER.ONE) {
            //currentPlayer = PlayerData.PLAYER.FOUR;
            currentPlayer = (PlayerData.PLAYER)CGameManager.instance.numPlayers - 1;
		}
		return currentPlayer;
	}

    public static Color GetPlayerColor(PlayerData.PLAYER currentPlayer)
    {
        switch(currentPlayer)
        {
            case PlayerData.PLAYER.ONE: return Player1Color;
            case PlayerData.PLAYER.TWO: return Player2Color;
            case PlayerData.PLAYER.THREE: return Player3Color;
            case PlayerData.PLAYER.FOUR: return Player4Color;
        }
        return Color.white;
    }




    public static void UpdateWidgets() { UpdateWidgetsRecursively(CGameManager.instance.transform); }
    private static void UpdateWidgetsRecursively(Transform root)
	{
        foreach (Transform child in root) {
            UIWidget widget = child.GetComponent<UIWidget>();
            if (widget != null) { widget.ParentHasChanged(); }

            UpdateWidgetsRecursively(child);
        }
    }

    public static void UpdateSiblingIndexes(Transform parent)
    {
        int i = 0;
        foreach (Transform child in parent) {
            CBaseCard card = child.GetComponent<CBaseCard>();
            card.IndexInList = i;
            child.SetSiblingIndex(i);
            ++i;
        }
    }
    //public static void UpdateSiblingIndexes(Transform parent, int indexOfPrey = -1)
    //{
    //    int i = 0;
    //    foreach (Transform child in parent) {
    //        CBaseCard card = child.GetComponent<CBaseCard>();
    //        card.IndexInList = (i == indexOfPrey) ? i-1 : i;
    //        child.SetSiblingIndex((i == indexOfPrey) ? i-1 : i);
    //        ++i;
    //    }
    //}

    public static void AutoResizeTextureColliders(Transform root) { AutoResizeTextureCollidersRecursively(root); }
    private static void AutoResizeTextureCollidersRecursively(Transform root)
	{
        foreach (Transform child in root) {
            UITexture texture = child.GetComponent<UITexture>();
            if (texture != null) { texture.ResizeCollider(); }

            AutoResizeTextureCollidersRecursively(child);
        }
    }
    
    public static void AssignNewUIButtonOnClickTarget(MonoBehaviour targetMonoBehaviour, MonoBehaviour parameter, UIButton button, string functionName_CaseSensitive)
    {
        if(button.onClick.Count > 0)
            button.onClick.RemoveAt(0);

        if(functionName_CaseSensitive == string.Empty)
        	return;

        EventDelegate del = new EventDelegate(targetMonoBehaviour, functionName_CaseSensitive);
        if(parameter != null)
            del.parameters[0] = new EventDelegate.Parameter(parameter, "CBaseCard");

        EventDelegate.Set(button.onClick, del);
    }

    public static void AssignNewUIButtonOnClickTarget_UISprite(MonoBehaviour targetMonoBehaviour, MonoBehaviour parameter, UIButton button, string functionName_CaseSensitive)
    {
        if(button.onClick.Count > 0)
            button.onClick.RemoveAt(0);

        if(functionName_CaseSensitive == string.Empty)
        	return;

        EventDelegate del = new EventDelegate(targetMonoBehaviour, functionName_CaseSensitive);
        if(parameter != null)
            del.parameters[0] = new EventDelegate.Parameter(parameter, "UISprite");

        EventDelegate.Set(button.onClick, del);
    }
}

namespace GameData
{
    public enum PlayState { IDLE, SELECTING_PREY, VIEWING_MARKET, UNHIBERNATING_SNAIL };

    public enum UIState { IDLE, DISCARD_DRAG };
}

namespace CardData
{
	public enum SnailSpecies {
		Conus_Arentus, Conus_Bullatus, Conus_Californicus, Conus_Ebraeus, Conus_Geographus, Conus_Gloriamaris, 
		Conus_Imperialis, Conus_Magus, Conus_Marmoreous, Conus_Pennaceus, Conus_Princeps, Conus_Pulicarius, 
		Conus_Purpurascens, Conus_Textile, Conus_Tulipa, Conus_Victoriae };

	public enum CardType { Unassigned = -1, Snail, Prey, Instant };

	public enum PreyType { All, Fish, Mollusk, Worm };

	public enum InstantType { None = -1,
		Lobster, Meeting, Ocean_Waves, Potency, Presentation, Publishing, Research, Starvation, 
		Stingray, Tsunami, Turtle };

	public enum PeptideType { Unknown = -1, Alpha, Delta, Mu, Omega, Count };

	public enum FedState { Unfed, Fed, Hibernating, Dead };

	public enum PreyName { Basic_Prey, Arrow_Worm, Bearded_Fireworm, Bloodworm, Blue_Devil_Fish, Bobbit_Worm, Butterfly_Fish, Clownfish,
		Common_Periwinkle, Conus_kinoshitai, Dusky_Frillgoby, Gold_Belly_Damsel_Fish, Goldfish, Half_and_Half, Hebrew_Volute, Lugworm,
		Olive_Shells, Peanut_Worm, Ragworm, Rusty_Scale_Worm, Serpents_Head_Cowry, Stripey, Turbo_Snail, Venus_Comb_Murex };

    public enum ParentPanel { HAND, SNAIL, PREY, DISCARD, ACTION, MARKET, CABAL };

    public enum CardActions { Idle = -1, Feed  };
}

namespace PlayerData 
{
	public enum PLAYER { ONE, TWO, THREE, FOUR };

}

//namespace PeptideData
//{
//    public enum PEPTIDE { UNKNOWN = -1, ALPHA, DELTA, MU, OMEGA, TOTAL };
//}


//namespace UIHelper
//{
//    public void TweenPosition(GameObject go, Vector3 from, Vector3 to, float duration, bool worldspace = false, 
//                    UITweener.Method method = UITweener.Method.Linear, UITweener.Style style = UITweener.Style.Once, bool steeperCurves = false) 
//    {
//        TweenPosition tweenPosSnail = go.AddComponent<TweenPosition>();
//        tweenPosSnail.method = method;
//        tweenPosSnail.style = style;
//        tweenPosSnail.steeperCurves = steeperCurves;
//        tweenPosSnail.duration = duration;
//        tweenPosSnail.worldSpace = worldspace;
//        tweenPosSnail.from = from;
//        tweenPosSnail.to = to;
//    }
//}