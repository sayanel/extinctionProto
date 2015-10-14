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

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if ( controller == null ) Debug.LogWarning( "WARNING ! No Character controller is attached to your game object" );
    }

	void Update() 
    {
        if ( controller == null ) return;

        //Handle Vertical and Horizontal Direction
        moveDirection = new Vector3( Input.GetAxis( "Horizontal" ), 0, Input.GetAxis( "Vertical" ) );
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
	}

}
