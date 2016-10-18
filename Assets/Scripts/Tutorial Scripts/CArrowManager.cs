using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CArrowManager : MonoBehaviour
{
    public enum DIRECTION { LEFT, RIGHT, BOTTOM, TOP };

    public class tArrowData
    {
        public Vector3 startPosition;
        public DIRECTION direction;

        public tArrowData(Vector3 startPos, DIRECTION dir)
        {
            startPosition = startPos;
            direction = dir;
        }
    }



    public List<CArrow> lstArrows;



    public IEnumerator MoveActiveArrows(Vector3 v3Position, float duration)
    {
        foreach (CArrow arrow in lstArrows)
            if(arrow.gameObject.activeSelf)
                CGlobals.TweenMove(arrow.gameObject, "position", v3Position, duration, iTween.EaseType.easeInSine, true);

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator MoveActiveArrows(string axis, float value, float duration)
    {
        foreach (CArrow arrow in lstArrows)
            if(arrow.gameObject.activeSelf)
                CGlobals.TweenMove(arrow.gameObject, axis, value, duration, iTween.EaseType.easeInSine, true);

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator ScaleActiveArrows(Vector3 v3Scale, float duration)
    {
        foreach (CArrow arrow in lstArrows)
            if(arrow.gameObject.activeSelf)
                CGlobals.TweenScale(arrow.gameObject, v3Scale, duration, iTween.EaseType.easeInSine, true);

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator EnableArrows(int numArrows)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < numArrows; ++i)
            lstArrows[i].gameObject.SetActive(true);
    }

    public IEnumerator EnableArrows(List<tArrowData> lstArrowData)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < lstArrowData.Count; ++i)
        {
            lstArrows[i].gameObject.SetActive(true);
            lstArrows[i].transform.localPosition = lstArrowData[i].startPosition;

            switch(lstArrowData[i].direction)
            {
                case DIRECTION.LEFT:    lstArrows[i].transform.localEulerAngles = Vector3.forward * -180f;
                break;
                case DIRECTION.RIGHT:   lstArrows[i].transform.localEulerAngles = Vector3.zero;
                break;
                case DIRECTION.BOTTOM:  lstArrows[i].transform.localEulerAngles = Vector3.forward * -90f;
                break;
                case DIRECTION.TOP:     lstArrows[i].transform.localEulerAngles = Vector3.forward * 90f;
                break;
            }

        }
    }

    public IEnumerator DisableArrows()
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ScaleActiveArrows(Vector3.zero, 0.5f));
        foreach (CArrow arrow in lstArrows)
            arrow.gameObject.SetActive(false);
    }
}
