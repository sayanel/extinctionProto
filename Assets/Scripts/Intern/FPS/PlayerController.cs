using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Enum which describes the character's state, 
/// mainly for the animator
/// </summary>
public enum CharacterState
{
    IDLE = 0,
    JUMPING = 1,
    RUN = 2,
    RUN_BACK = 3,
    STRATE_LEFT = 4,
    STRATE_RIGHT = 5
}

public enum RotationAxes
{ 
    MouseXAndY = 0, 
    MouseX = 1, 
    MouseY = 2 
} 

/// <summary>
/// This Class handles the player's input to make the character move.
/// It also choose which animation to set
/// Your Game Object must have a Character Controller Component.
/// </summary>
public class PlayerController : MonoBehaviour {

    //-------------- Public --------------

    public CharacterController m_controller;
    public Animator m_animator;

    public Weapon m_weapon;

    public Transform m_cameraTransform;

    public bool m_isControllable = false;

	public float m_speed = 6.0f;
    public float m_accurateSpeedReduction = 0.1f;
	public float m_jumpSpeed = 8.0f;
	public float m_gravity = 20.0f;
    public float m_mouseSensivity = 5.0f;

    public RotationAxes m_axes = RotationAxes.MouseXAndY;

    public float m_minRotX = -360F;
    public float m_maxRotX = 360F;
    public float m_minRotY = -60F;
    public float m_maxRotY = 60F;

    public bool isAiming { get { return m_aiming; } }

    public CharacterState state {
        get { return m_currentState; }
        set { m_currentState = value; }
    }

    //-------------- Private --------------

    private CharacterState m_currentState = CharacterState.IDLE;
    private bool m_aiming;

    private float m_xSpeed = 0;
    private float m_ySpeed = 0;
    private float m_zSpeed = 0;
    private Vector3 m_moveDirection = Vector3.zero;

    private float m_rotationY = 0F;
    private float m_rotationX = 0F;
    
    void Start()
    {
       
    }

	void Update() 
    {
        // Network Setting
        if ( m_isControllable )
        {
            //Handle Specials commands such as aiming, etc.
            SpecialMove();

            //Move The character following the inputs
            Move();

            //Change the character's orientation following the mouse
            Rotate();
        }

        ///Set the proper character animation according to his state
        Animate();

	}

    /// <summary>
    /// Move The character following the inputs
    /// </summary>
    void Move()
    {
        //Get Input values
        m_xSpeed = Input.GetAxis( "Horizontal" );
        m_zSpeed = Input.GetAxis( "Vertical" );

        //Handle Vertical and Horizontal Direction
        m_moveDirection.x = m_xSpeed;
        m_moveDirection.y = 0;
        m_moveDirection.z = m_zSpeed;

        m_moveDirection = transform.TransformDirection( m_moveDirection );
        m_moveDirection *= m_speed;

        if ( m_aiming )
        {
            m_moveDirection *= m_accurateSpeedReduction;
        }

        //Handle Jumping/Not Jumping cases
        if ( m_controller.isGrounded )
        {
            m_ySpeed = 0;

            if ( Input.GetButton( "Jump" ) )
            {
                m_ySpeed = m_jumpSpeed;
            }
        }

        // Apply gravity on vertical speed
        m_ySpeed -= m_gravity * Time.deltaTime;
        m_moveDirection.y = m_ySpeed;

        // Move the controller
        m_controller.Move( m_moveDirection * Time.deltaTime );
    }

    void SpecialMove()
    {
        m_aiming = false;

        if ( Input.GetMouseButton( 1 ) ) m_aiming = true;
        if ( Input.GetMouseButton( 0 ) )
        {
            m_weapon.Fire();
        }
    }

    /// <summary>
    /// Change the character's orientation following the mouse
    /// </summary>
    void Rotate()
    {
        if ( m_axes == RotationAxes.MouseXAndY )
        {
            //m_rotationX += transform.localEulerAngles.y + Input.GetAxis( "Mouse X" ) * m_mouseSensivity;
            m_rotationX += Input.GetAxis( "Mouse X" ) * m_mouseSensivity;

            m_rotationY += Input.GetAxis( "Mouse Y" ) * m_mouseSensivity;
            m_rotationY = Mathf.Clamp( m_rotationY, m_minRotY, m_maxRotY );

            transform.localEulerAngles = new Vector3( 0, m_rotationX, 0 );
            //m_playerTransform.localEulerAngles = new Vector3( 0, m_rotationX, 0 );

            Vector3 cameraAngle = m_cameraTransform.localEulerAngles;
            cameraAngle.x = -m_rotationY;
            m_cameraTransform.localEulerAngles = cameraAngle;

        }
        else if ( m_axes == RotationAxes.MouseX )
        {
            transform.Rotate( 0, Input.GetAxis( "Mouse X" ) * m_mouseSensivity, 0 );
        }
        else
        {
            m_rotationY += Input.GetAxis( "Mouse Y" ) * m_mouseSensivity;
            m_rotationY = Mathf.Clamp( m_rotationY, m_minRotY, m_maxRotY );

            transform.localEulerAngles = new Vector3( -m_rotationY, transform.localEulerAngles.y, 0 );
        }
    }

    /// <summary>
    /// Set the proper character animation according to his state
    /// </summary>
    void Animate()
    {
        m_currentState = m_isControllable ? GetState() : m_currentState;

        m_animator.SetInteger( "State", (int)m_currentState );
        
    }

    /// <summary>
    /// Check the current state according to the player's input
    /// </summary>
    /// <returns>An enum of the current state</returns>
    public CharacterState GetState()
    {
        if ( !m_controller.isGrounded ) return CharacterState.JUMPING;          
      
        if ( Math.Sign( m_zSpeed ) > 0 ) return CharacterState.RUN;

        if ( Math.Sign( m_zSpeed ) < 0 ) return CharacterState.RUN_BACK;

        if ( Math.Sign( m_xSpeed ) < 0 ) return CharacterState.STRATE_LEFT;

        if ( Math.Sign( m_xSpeed ) > 0 ) return CharacterState.STRATE_RIGHT;

        return CharacterState.IDLE;
    }
}
