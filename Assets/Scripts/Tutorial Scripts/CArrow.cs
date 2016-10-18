using UnityEngine;
using System.Collections;

public class CArrow : MonoBehaviour
{
    UISprite sprite;


    void Awake()
    {

    }

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    public IEnumerator MoveTo(Vector3 v3Position, float duration)
    {
        CGlobals.TweenMove(gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(duration);
    }
    public IEnumerator MoveTo(string axis,float value, float duration)
    {
        CGlobals.TweenMove(gameObject, axis, value, duration, iTween.EaseType.easeInSine, true);
        yield return new WaitForSeconds(duration);
    }




    public IEnumerator ScaleTo(Vector3 v3Scale, float duration)
    {
        CGlobals.TweenScale(gameObject, v3Scale, duration, iTween.EaseType.easeOutSine, true);
        yield return new WaitForSeconds(duration);
    }

    public IEnumerator PointLeft()
    {
        yield return new WaitForEndOfFrame();
        transform.localEulerAngles = Vector3.forward * -180f;
    }
    public IEnumerator PointRight()
    {
        yield return new WaitForEndOfFrame();
        transform.localEulerAngles = Vector3.zero;
    }
    public IEnumerator PointUp()
    {
        yield return new WaitForEndOfFrame();
        transform.localEulerAngles = Vector3.forward * 90f;
    }
    public IEnumerator PointDown()
    {
        yield return new WaitForEndOfFrame();
        transform.localEulerAngles = Vector3.forward * -90f;
    }
}
