using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour 
{
    [SerializeField]
    private int m_rightLimit = 10;

    [SerializeField]
    private int m_leftLimit = 10;

    [SerializeField]
    private int m_topLimit = 10;

    [SerializeField]
    private int m_bottomLimit = 10;

    [SerializeField]
    private float m_velocity = 1.0F;

    private Vector2 m_direction;

    private Camera m_thisCamera;

    // Use this for initialization
    void Awake () 
    {
        m_thisCamera = GetComponent<Camera>();
	}

    //apply the result of direction computation to move the camera
    void updatePosition()
    {
        transform.position += new Vector3(m_direction.x, 0, m_direction.y);
    }
	
	// Update is called once per frame
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

        //move the camera
        updatePosition();
    }

}
