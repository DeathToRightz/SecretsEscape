using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform cameraLocation;
    [SerializeField] Transform orientation;
    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCoolDown;
    [SerializeField] float airMovement;
    private bool readyToJump;
    void Start()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        ControlSpeed();

        if(Input.GetKeyDown(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump  = false;
            Jump();
            Invoke(nameof(ResetJump), 2);
        }

        Debug.DrawRay(cameraLocation.position,cameraLocation.forward * 10, Color.blue);
        if(Input.GetMouseButtonDown(0))
        {
            PlayerPOVPointer();
        }
        
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down,  2f, whatIsGround);
        Debug.DrawRay(transform.position,Vector3.down * 2f,Color.red);

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {          
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        }

        else
        {
            //rb.drag = 0f;
            rb.AddForce(moveDirection.normalized * speed * 10f * airMovement, ForceMode.Force);
        }
    }

    private void Jump()
    {
        Debug.Log("Jump");
        //rb.velocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);

        rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);                 
    }

    private void ControlSpeed()
    {
        Vector3 setVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (setVel.magnitude > speed)
        {
            Vector3 limitVel = setVel.normalized * speed;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    private bool ResetJump()
    {
        return readyToJump = true;
    }

    private void PlayerPOVPointer()
    {
        RaycastHit cameraHit;
        if(Physics.Raycast(cameraLocation.position, cameraLocation.forward, out cameraHit, 4f))
        {
            if(cameraHit.transform.GetComponent<InteractableObjects>() != null)
            {
                Debug.Log("Interact with me");
            }
            else
            {
                Debug.Log(cameraHit.transform.name);
            }
            
        }
    }
}
