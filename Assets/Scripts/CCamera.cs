using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {

    private float baseAspect = 1640f / 1024f;
    Camera camera;
    
    void Awake()
    {
        camera = GetComponent<Camera>();
    }
 
    void Start() {
         float currAspect = 1.0f * Screen.width / Screen.height;
     Debug.Log(Camera.main.projectionMatrix);
     Debug.Log(baseAspect + ", " + currAspect + ", " + baseAspect / currAspect);
     Camera.main.projectionMatrix = Matrix4x4.Scale(new Vector3(currAspect / baseAspect, 1.0f, 1.0f)) * Camera.main.projectionMatrix;
 }
}
