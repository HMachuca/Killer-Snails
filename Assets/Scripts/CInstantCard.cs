using UnityEngine;
using System.Collections;

public class CInstantCard : CBaseCard 
{
	public CardData.InstantType instantType;
    public bool predator;
	public GameData.PlayState playStateToBeActive;
	
	protected override void Start () 
	{
		base.Start();

		cardType = CardData.CardType.Instant;
	}

	void Update () 
	{
		
	}

}
