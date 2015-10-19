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

/// <summary>
/// This Class handles the player's input to make the character move.
/// It also choose which animation to set
/// Your Game Object must have a Character Controller Component.
/// </summary>
public class PlayerController : MonoBehaviour {

    //-------------- Public --------------

	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
    public float mouseSensivity = 5.0f;

    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float minRotX = -360F;
    public float maxRotX = 360F;
    public float minRotY = -60F;
    public float maxRotY = 60F;

    //-------------- Private --------------

    private CharacterState currentState = CharacterState.IDLE;

    private float xSpeed = 0;
    private float ySpeed = 0;
    private float zSpeed = 0;

    private Vector3 moveDirection = Vector3.zero;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
   
    private float rotationY = 0F;
    
    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if ( controller == null )
        {
            throw new MissingComponentException( "No Character controller is attached to your game object" );
        }

        if ( animator == null )
        {
            throw new MissingComponentException( "No animator is attached to your game object" );
        }
    }

	void Update() 
    {
        //Move The character following the inputs
        Move();

        //Change the character's orientation following the mouse
        Rotate();

        ///Set the proper character animation according to his state
        Animate();

	}

    /// <summary>
    /// Move The character following the inputs
    /// </summary>
    void Move()
    {
        //Get Input values
        xSpeed = Input.GetAxis( "Horizontal" );
        zSpeed = Input.GetAxis( "Vertical" );

        //Handle Vertical and Horizontal Direction
        moveDirection.x = xSpeed;
        moveDirection.y = 0;
        moveDirection.z = zSpeed;

        moveDirection = transform.TransformDirection( moveDirection );
        moveDirection *= speed;

        //Handle Jumping/Not Jumping cases
        if ( controller.isGrounded )
        {
            ySpeed = 0;

            if ( Input.GetButton( "Jump" ) )
            {
                ySpeed = jumpSpeed;
            }
        }

        // Apply gravity on vertical speed
        ySpeed -= gravity * Time.deltaTime;
        moveDirection.y = ySpeed;

        // Move the controller
        controller.Move( moveDirection * Time.deltaTime );
    }

    /// <summary>
    /// Change the character's orientation following the mouse
    /// </summary>
    void Rotate()
    {
        if ( axes == RotationAxes.MouseXAndY )
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis( "Mouse X" ) * mouseSensivity;

            rotationY += Input.GetAxis( "Mouse Y" ) * mouseSensivity;
            rotationY = Mathf.Clamp( rotationY, minRotY, maxRotY );

            transform.localEulerAngles = new Vector3( -rotationY, rotationX, 0 );
        }
        else if ( axes == RotationAxes.MouseX )
        {
            transform.Rotate( 0, Input.GetAxis( "Mouse X" ) * mouseSensivity, 0 );
        }
        else
        {
            rotationY += Input.GetAxis( "Mouse Y" ) * mouseSensivity;
            rotationY = Mathf.Clamp( rotationY, minRotY, maxRotY );

            transform.localEulerAngles = new Vector3( -rotationY, transform.localEulerAngles.y, 0 );
        }
    }

    /// <summary>
    /// Set the proper character animation according to his state
    /// </summary>
    void Animate()
    {
        CharacterState state = GetState();

        if(!state.Equals(currentState))
        {
            currentState = state;
            animator.SetInteger( "State", (int)currentState );
        }
    }

    /// <summary>
    /// Check the current state according to the player's input
    /// </summary>
    /// <returns>An enum of the current state</returns>
    CharacterState GetState()
    {
        if ( !controller.isGrounded )
        {
            return CharacterState.JUMPING;          
        }

        if ( Math.Sign( zSpeed ) > 0 ) return CharacterState.RUN;

        if ( Math.Sign( zSpeed ) < 0 ) return CharacterState.RUN_BACK;

        if ( Math.Sign( xSpeed ) < 0 ) return CharacterState.STRATE_LEFT;

        if ( Math.Sign( xSpeed ) > 0 ) return CharacterState.STRATE_RIGHT;

        return CharacterState.IDLE;
    }
}
