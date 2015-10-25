using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour 
{
    //if mouse pointer X position is greater than screenWidth - m_rightLimit, move the camera to the right
    [SerializeField]
    private int m_rightLimit = 10;

    //if mouse pointer X position is less than m_leftLimit, move the camera to the left
    [SerializeField]
    private int m_leftLimit = 10;

    //if mouse pointer Y position is less than m_topLimit, move the camera to the top
    [SerializeField]
    private int m_topLimit = 10;

    //if mouse pointer Y position is more than screenHeight - m_topLimit, move the camera to the bottom
    [SerializeField]
    private int m_bottomLimit = 10;

    //velocity of the camera
    [SerializeField]
    private float m_velocity = 1.0F;

    [SerializeField]
    private float m_zoomMin = 10.0F;

    [SerializeField]
    private float m_zoomMax = 90.0F;

    [SerializeField]
    private float m_zoomStep = 0.1F;

    [SerializeField]
    private float m_currentZoom = 60.0F;

    [SerializeField]
    private bool m_hasChildCamera = true;

    private Camera m_childCamera;

    private Vector2 m_direction;

    private Camera m_thisCamera;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Awake () 
    {
        m_thisCamera = GetComponent<Camera>();

        //seerch into child object to find a child camera.
        //if there is such a camera, store it in m_childCamera
        if(m_hasChildCamera)
        {
            Camera[] cams = GetComponentsInChildren<Camera>();
            foreach(Camera cam in cams)
            {
                if(cam != m_thisCamera)
                {
                    m_childCamera = cam;
                    break;
                }
            }
        }

        m_currentZoom = m_zoomMax;

        zoom( 0 );
	}

    //apply the result of direction computation to move the camera
    void updatePosition()
    {
        transform.position += new Vector3(m_direction.x, 0, m_direction.y);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () 
    {
        //initialisation of the direction
        m_direction = Vector2.zero;

        //update of the direction of the camera, based on the mouse position.
        //No use of esle, because more than on instructions may be valid (diagonal direction)
        if(Input.mousePosition.x > m_thisCamera.pixelWidth - m_rightLimit)
        {
            m_direction += new Vector2(m_velocity, 0);
        }
        if (Input.mousePosition.x < m_leftLimit)
        {
            m_direction += new Vector2(-m_velocity, 0);
        }
        if (Input.mousePosition.y < m_topLimit)
        {
            m_direction += new Vector2(0, -m_velocity);
        }
        if (Input.mousePosition.y > m_thisCamera.pixelHeight - m_bottomLimit)
        {
            m_direction += new Vector2(0, m_velocity);
        }

        if( !Mathf.Approximately(0.0F, Input.GetAxis( "Mouse ScrollWheel" ) ) )
        {
            zoom( Input.GetAxis( "Mouse ScrollWheel" ) );
        }

        //move the camera
        updatePosition();
    }

    /// <summary>
    /// change the zoom of the camera, changing its field of view
    /// </summary>
    /// <param name="delta"></param>
    void zoom(float delta)
    {

        if( delta < 0 )
        {
            m_currentZoom += m_zoomStep;
        }
        else if( delta > 0 )
        {
            m_currentZoom -= m_zoomStep;
        }

        m_currentZoom = Mathf.Clamp( m_currentZoom, m_zoomMin, m_zoomMax );
        m_thisCamera.fieldOfView = m_currentZoom;

        if( m_hasChildCamera )
            m_childCamera.fieldOfView = m_currentZoom;
    }

    /// <summary>
    /// apply a zoom on the camera, changing its field of view
    /// </summary>
    /// <param name="zoomValue"></param>
    public void setZoom( float zoomValue )
    {
        m_currentZoom = zoomValue;

        m_currentZoom = Mathf.Clamp( m_currentZoom, m_zoomMin, m_zoomMax );
        m_thisCamera.fieldOfView = m_currentZoom;

        if( m_hasChildCamera )
            m_childCamera.fieldOfView = m_currentZoom;
    }

}
