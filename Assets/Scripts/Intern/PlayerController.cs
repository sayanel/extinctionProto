using UnityEngine;
using System.Collections;

/// <summary>
/// This Class handles the player's input to make the character move.
/// Your Game Object must have a Character Component.
/// </summary>
public class PlayerController : MonoBehaviour {

	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

    private float verticalSpeed = 0;

	private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if ( controller == null ) Debug.LogWarning( "WARNING ! No Character controller is attached to your game object" );
    }

	void Update() 
    {
        float verticalMove = Input.GetAxis( "Vertical" );
        float horizontalMove = Input.GetAxis( "Horizontal" );

        //Handle Vertical and Horizontal Direction
        moveDirection.x = horizontalMove;
        moveDirection.y = 0;
        moveDirection.z = verticalMove;

        moveDirection = transform.TransformDirection( moveDirection );
        moveDirection *= speed;

        //Handle Jumping/Not Jumping cases
		if (controller.isGrounded) {
			
            verticalSpeed = 0;

			if (Input.GetButton ("Jump")) {
                verticalSpeed = jumpSpeed;
			}
		}

		// Apply gravity on vertical speed
		verticalSpeed -= gravity * Time.deltaTime;

        moveDirection.y = verticalSpeed;

		// Move the controller
		controller.Move(moveDirection * Time.deltaTime);

        //handle Animation
        bool running = horizontalMove != 0f || verticalMove != 0f;
        animator.SetBool("isRunning", running);

	}

}
