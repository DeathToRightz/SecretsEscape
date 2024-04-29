using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    [Header("Player Settings")]
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCoolDown;
    [SerializeField] float airMovement;
    [SerializeField] float speed;
    [SerializeField] float pushForce;

    [Header("Camera Settings")]
    [SerializeField] Transform cameraLocation;
    [SerializeField] Transform orientation;

    [Header("Hand Settings")]
    [SerializeField] Transform hand;

    [Header("Other Settings")]
    [SerializeField] LayerMask whatIsGround;

    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;    
    private bool readyToJump;      
    private RaycastHit itemInHand, cameraPointer;
    private bool handFull;

    void Start()
    {
        handFull = false;
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
      
        if(Input.GetMouseButtonDown(0) && !handFull)
        {
            PlayerPOVPointer();
        }
        if (Input.GetKeyDown(KeyCode.F) && handFull)
        {
            Drop(itemInHand.transform);
            handFull = false;
        }
      
        if(Input.GetKeyDown(KeyCode.E) &&  handFull && itemInHand.transform.GetComponent<ObjectInteractions>() != null)
        {
            ObjectInteractions itemDescription = itemInHand.transform.GetComponent<ObjectInteractions>();
           
            if(Physics.Raycast(cameraLocation.position, cameraLocation.forward, out cameraPointer, 4f))
            {
                if (itemDescription.itemName == ObjectInteractions.items.Shovel && cameraPointer.transform.tag == "Dirt")
                {
                    Debug.Log("Dig");
                }
            }
            
            
            
            
                
            
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
       
        if(Physics.Raycast(cameraLocation.position, cameraLocation.forward, out itemInHand, 4f))
        {                     
                Debug.Log(itemInHand.transform.name);  
            if(itemInHand.transform.tag == "Pick Up" && !handFull)
            {
                PickUp(itemInHand.transform);
                handFull = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Rigidbody>())
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();  

            Vector3 pushDirection = other.gameObject.transform.position - transform.position;
            pushDirection.y = 0f;
            pushDirection.Normalize();

            rigidbody.AddForceAtPosition(pushDirection * pushForce, transform.position, ForceMode.Impulse);
        }
    }

    private void PickUp(Transform item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        item.parent = hand;
        item.position = hand.transform.position;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        item.LookAt(transform.forward);
    }

    private void Drop(Transform item)
    {
        Rigidbody rb= item.GetComponent<Rigidbody>();
        item.parent = null;
        rb.constraints = RigidbodyConstraints.None;
        
    }
}
