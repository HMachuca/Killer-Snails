using UnityEngine;
using System.Collections;

public class CDiscardCollider : MonoBehaviour
{
	private BoxCollider _boxCollider;

	public BoxCollider boxCollider { get { return _boxCollider; } }

	void Start () {
		_boxCollider = GetComponent<BoxCollider>();
	}
	
	void Update () {
	
	}


	void OnTriggerEnter(Collider other)
	{
		CBaseCard card = other.GetComponent<CBaseCard>();
        if (!card || card.CurrentParentPanel != CardData.ParentPanel.HAND)
			return;

		Debug.Log("Enter: " + other.name);
		CGameManager.instance.CurrentUIState = GameData.UIState.DISCARD_DRAG;
		card.EnableButton(false);
	}


	void OnTriggerExit(Collider other)
	{
		Debug.Log("Exit: " + other.name);
		CGameManager.instance.CurrentUIState = GameData.UIState.IDLE; 
		CBaseCard card = other.GetComponent<CBaseCard>();
		if(!card)
			return;

		if(!CUIManager.instance.isDragging)
			card.EnableButton();
	}
}
