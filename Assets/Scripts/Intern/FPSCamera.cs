using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour {

    public PlayerController m_playerController;

    public float m_normalFOV = 60;
    public float m_accurateFOV = 30;
    public float m_zoomTime = 1f;

    private float m_FOV;

    private bool m_zoomingIn = false;
    private bool m_zoomingOut = false;
   
    private Camera m_camera;

	// Use this for initialization
	void Start () {
        m_FOV = m_normalFOV;
        m_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        if ( m_playerController.isAiming && m_FOV > m_accurateFOV  && !m_zoomingIn)
        {
            m_zoomingIn = true;
            StartCoroutine( "ZoomIn" );
        }

        if ( !m_playerController.isAiming && m_FOV < m_normalFOV && !m_zoomingOut )
        {
            m_zoomingOut = true;
            StartCoroutine( "ZoomOut" );
        }

        m_camera.fieldOfView = m_FOV;
	}

    IEnumerator ZoomIn()
    {
        int i = 0;

        while ( m_FOV > m_accurateFOV )
        {
            if ( !m_playerController.isAiming ) break;
            float t = ( i * 0.01f ) / m_zoomTime;
            m_FOV = Mathf.Lerp( m_FOV, m_accurateFOV, t );
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        m_zoomingIn = false;

    }

    IEnumerator ZoomOut()
    {
        int i = 0;

        while ( m_FOV < m_normalFOV )
        {
            if ( m_playerController.isAiming ) break;

            float t = ( i * 0.01f ) / m_zoomTime;
            m_FOV = Mathf.Lerp( m_FOV, m_normalFOV, t );
            i++;
            yield return new WaitForSeconds( 0.01f );
        }

        m_zoomingOut = false;        

    }


}
