using UnityEngine;
using System.Collections;

public class Clever : MonoBehaviour {

    UIPanel uiPanel;
    public GameObject clever;  //contains all game objects
    public GameObject RHPanel;
    public GameObject RHStudents1;
    public GameObject RHStudents2;  //password
    public GameObject mainMenu;
    public UILabel youUser;
    public UILabel youPass;
    public UILabel studentPass;

    void Start()
    {
        uiPanel = GetComponent<UIPanel>();
    }
    
    public void testButton() {

        print("This is a button");
    }


    public void youPlayButton()   { //StartCoroutine(youPlayButton_CR());

        if (youUser.text == "")
        {
            print("What is your name dear warrior?");
        }
        else if (youPass.text == "")
        {
            print("It's dangerous to go alone, write a password!");
        }
        else
        {
        RHPanel.SetActive(false);
        }
    }

    public  void classRooms() {
        RHStudents1.SetActive(true);
    }

    public void studentSelected() {
        RHStudents1.SetActive(false);
        RHStudents2.SetActive(true);
    }

    public void menu() {

        if (studentPass.text == "")
        {
            print("It's not like I wanted to remind you about your password, Baaaaka!");
        }
        else
            StartCoroutine(youPlayButton_CR());
        }

    public IEnumerator youPlayButton_CR()
    {
        //public static void iTweenValue(GameObject go, int from, int to, float duration, GameObject goTarget, string onFunctionTarget, iTween.EaseType easetype = iTween.EaseType.linear, string onComplete = null)
        //CGlobals.iTweenValue(this.gameObject, 1, 0, 1f, this.gameObject, "UpdatePanelAlphaYou", iTween.EaseType.linear, "CompleteAlphaUpdateYou");
        mainMenu.SetActive(true);
        yield return StartCoroutine(youPlayDesolve());
    }

    private IEnumerator youPlayDesolve()
    {
        yield return new WaitForSeconds(0.5f);
        CGlobals.iTweenValue(this.gameObject, 1, 0, 1f, this.gameObject, "UpdatePanelAlphaYou", iTween.EaseType.linear, "CompleteAlphaUpdateYou");

    }

    public void UpdatePanelAlphaYou(float alpha)
    {
        uiPanel.alpha = alpha;
    }

    public void CompleteAlphaUpdateYou()
    {
        clever.SetActive(false);
    }

}
