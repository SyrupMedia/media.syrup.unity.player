// Copyright © 2022, Maeve "Molasses" Garside
// Licensed under the MIT license, making this script copyleft.
// Check the LICENSE.md file for further information.

using UnityEngine;

namespace SyrupPlayer
{
public class Player : MonoBehaviour
{
#region Variables

[Header("Physics")]
public bool applyGravity;
public float gravity = -9.81f;
private Vector3 verticalVelocity;
private bool isGrounded;
public Transform groundCheck;
public float groundDistance = 0.5f;
public LayerMask groundLayer;


[Header("Graphics")]
public Animator animator;
[Tooltip("How far away the player has to be to start their landing animation.")]
public float landingDistance = 1.0f;
private bool isLanding;

[Header("Movement")]
public CharacterController controller;

[Header("Camera")]
[Tooltip("The main camera; automatically selected on awake.")]
public Transform mainCamera;
private GameObject[] cameras;

[Header("Controls")]
[Tooltip("Locks the player's mouse.")]
public bool lockedMouse;

[Header("Locomotion")]
[Tooltip("Determines the player's movement speed.")]
[Range(1f, 10f)]
[Min(1f)]
public float movementSpeed = 5f;
[Min(1f)]
public float jumpHeight = 2.5f;
[Range(0.1f, 0.5f)]
[Min(0.1f)]
public float turnSmoothing = 0.1f;
[Range(0.1f, 0.5f)]
[Min(0.1f)]
private float turnVelocity;
[Range(0.1f, 0.5f)]
[Min(0.0f)]
public float coyoteTime = 0.25f;
private float coyoteTimeCounter;
[Range(0.1f, 0.5f)]
[Min(0.0f)]
public float jumpBufferTime = 0.25f;
private float jumpBufferCounter;

#endregion

#region Unity Methods

// Awake() is called when the script instance is being loaded.
private void Awake() {
    cameras = GameObject.FindGameObjectsWithTag("MainCamera");
    mainCamera = cameras[0].GetComponent<Transform>();
}

// Start() is called before the first frame update.
private void Start() {
    if (lockedMouse) {
        Cursor.lockState = CursorLockMode.Locked;
    } else {
        Cursor.lockState = CursorLockMode.None;
    }
}

// Update() is called once per frame.
private void Update() {
    if (Input.GetButtonDown("Cancel")) {
        Cursor.lockState = CursorLockMode.None;
        lockedMouse = false;
    }

    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    if (Input.GetButtonDown("Jump")) {
        jumpBufferCounter = jumpBufferTime;
    } else {
        jumpBufferCounter -= Time.deltaTime;
    }

    animator.SetFloat("horizontal", horizontal);
    animator.SetFloat("vertical", vertical);

    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    if (direction.magnitude >= 0.1f) {
        animator.SetBool("isWalking", true);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSmoothing);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
    } else {
        animator.SetBool("isWalking", false);
    }

    if (applyGravity == true) {
        // This is the same approach featured in Brackeys' FPS Controller.
        // https://youtu.be/_QajrabyTJc?t=1132
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        isLanding = Physics.CheckSphere(groundCheck.position, landingDistance, groundLayer);

        if (isGrounded) {
            animator.SetBool("isGrounded", true);
        } else {
            animator.SetBool("isGrounded", false);
        }

        if (isLanding) {
            animator.SetBool("isLanding", true);
        } else {
            animator.SetBool("isLanding", false);
        }

        if (isGrounded && verticalVelocity.y < 0) {
            coyoteTimeCounter = coyoteTime;
            verticalVelocity.y = -2.5f;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && isGrounded) {
            
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("jump");
            coyoteTimeCounter = 0f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        animator.SetFloat("verticalVelocity", verticalVelocity.y);

        controller.Move(verticalVelocity * Time.deltaTime);
    }
}

#endregion
}
}