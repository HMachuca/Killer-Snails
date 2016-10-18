using UnityEngine;
using System.Collections;

public class CInstructionText : MonoBehaviour
{
    public GameObject goInstructionBGTop, goInstructionBGBot;
    public UILabel lblInstructionTextTop, lblInstructionTextBot;
    private float duration = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTopInstructionText(string instruction) { lblInstructionTextTop.text = instruction; }
    public void SetBotInstructionText(string instruction) { lblInstructionTextBot.text = instruction; }


    public void ShowTopHeader(string instruction = default(string))
    {
        TweenPosition.Begin(goInstructionBGTop, duration, Vector3.up * 366f);
        lblInstructionTextTop.text = instruction;
    }

    public void ShowBotHeader(string instruction = default(string))
    {
        TweenPosition.Begin(goInstructionBGBot, duration, Vector3.up * -366f);
        lblInstructionTextBot.text = instruction;
    }

    public void HideTopHeader()
    {
        TweenPosition.Begin(goInstructionBGTop, duration, Vector3.up * 525f);
    }

    public void HideBotHeader()
    {
        TweenPosition.Begin(goInstructionBGBot, duration, Vector3.up * -525f);
    }

    
}
