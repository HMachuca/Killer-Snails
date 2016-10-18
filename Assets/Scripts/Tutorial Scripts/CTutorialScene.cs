using UnityEngine;
using System.Collections;

public class CTutorialScene : MonoBehaviour
{
    [HideInInspector] public CTutorialDirector.TutorialScene SceneType;
    [HideInInspector] public bool updateTime;
    [HideInInspector] public float timeForScene;
    [HideInInspector] public float currentTime;

	public virtual IEnumerator StartScene() { yield return new WaitForEndOfFrame(); }
	
	public virtual void SetupScene(CTutorialDirector.TS_INTRO nextScene) { StartCoroutine(SetupScene_CR(nextScene)); }
    public virtual void SetupScene(CTutorialDirector.TS_EXPLAINHAND nextScene) { StartCoroutine(SetupScene_CR(nextScene)); }
    public virtual void SetupScene(CTutorialDirector.TS_PEEKATPEPTIDE nextScene) { StartCoroutine(SetupScene_CR(nextScene)); }

    public virtual IEnumerator SetupScene_CR(CTutorialDirector.TS_INTRO nextScene) { yield return new WaitForEndOfFrame(); }
    public virtual IEnumerator SetupScene_CR(CTutorialDirector.TS_EXPLAINHAND nextScene) { yield return new WaitForEndOfFrame(); }
    public virtual IEnumerator SetupScene_CR(CTutorialDirector.TS_PEEKATPEPTIDE nextScene) { yield return new WaitForEndOfFrame(); }

    public virtual IEnumerator AdvanceScene() { yield return new WaitForEndOfFrame(); }
}
