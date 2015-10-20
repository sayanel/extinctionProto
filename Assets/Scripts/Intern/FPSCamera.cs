using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour {

    public PlayerController playerController;

    public float normalFOV = 60;
    public float accurateFOV = 30;
    public float zoomTime = 1f;

    private float FOV;

    private bool zoomingIn = false;
    private bool zoomingOut = false;
   
    private Camera camera;

	// Use this for initialization
	void Start () {
        FOV = normalFOV;
        camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        if ( playerController.isAiming && FOV > accurateFOV  && !zoomingIn)
        {
            zoomingIn = true;
            StartCoroutine( "ZoomIn" );
        }

        if ( !playerController.isAiming && FOV < normalFOV && !zoomingOut )
        {
            zoomingOut = true;
            StartCoroutine( "ZoomOut" );
        }

        camera.fieldOfView = FOV;
	}

    IEnumerator ZoomIn()
    {
        int i = 0;

        while ( FOV > accurateFOV )
        {
            if ( !playerController.isAiming ) break;
            float t = ( i * 0.01f ) / zoomTime;
            FOV = Mathf.Lerp( FOV, accurateFOV, t );
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        zoomingIn = false;

    }

    IEnumerator ZoomOut()
    {
        int i = 0;

        while ( FOV < normalFOV )
        {
            if ( playerController.isAiming ) break;

            float t = ( i * 0.01f ) / zoomTime;
            FOV = Mathf.Lerp( FOV, normalFOV, t );
            i++;
            yield return new WaitForSeconds( 0.01f );
        }

        zoomingOut = false;        

    }


}
