// Copyright © 2021 Syrup Media
// Licensed under VERSION 3 of the GNU GENERAL PUBLIC LICENSE.
// Check the LICENSE.md file for further information.

using UnityEngine;

namespace SyrupPlayer
{
    public class Player : MonoBehaviour
    {
        #region Variables
    
        [Header("Graphics")]
        public Animator animator;

        [Header("Movement")]	
        public CharacterController controller;
    
        [Header("Camera")]
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
        [Range(0.1f, 0.5f)]
        [Min(0.1f)]
        public float turnSmoothing = 0.1f;
        private float turnVelocity;
    
        #endregion
    
        #region Unity Methods
    
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            cameras = GameObject.FindGameObjectsWithTag("MainCamera");
            mainCamera = cameras[0].GetComponent<Transform>();
        } 
    
        // Start() is called before the first frame update.
        private void Start()
        {
            if(lockedMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // Update() is called once per frame.
        private void Update()
        {
            if(Input.GetButtonDown("Cancel"))
            {
                Cursor.lockState = CursorLockMode.None;
                lockedMouse = false;
            }
        
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            animator.SetFloat("horizontal", horizontal);
            animator.SetFloat("vertical", vertical);
            
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            if(direction.magnitude >= 0.1f)
            {      
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSmoothing);
                
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
                
            }
            else
            {   
                animator.SetBool("isWalking", false);
            }
        }   
    
        #endregion
    }
}