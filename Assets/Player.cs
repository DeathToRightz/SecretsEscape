using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
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
    private GameObject itemInHand;

    [Header("Other Settings")]
    [SerializeField] LayerMask whatIsGround;

    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;    
    private bool readyToJump;      
    private RaycastHit itemInHandPointer, cameraPointer;
    public bool handFull;
    public string[] code = new string[4];
    private string correctCode = "1234";
    //private string[] correctCode = new string[] {"1","2","3","4"};
    private int codeIndex = 0;
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
        Debug.DrawRay(cameraLocation.position, cameraLocation.forward * 10, Color.blue);
        Physics.Raycast(cameraLocation.position, cameraLocation.forward, out cameraPointer, 2);

        if (Input.GetKeyDown(KeyCode.B))
        {
            
            Debug.Log(cameraPointer.transform.parent.name);
            Debug.Log(cameraPointer.transform.name);
            Debug.Log(cameraPointer.transform.tag);
            Debug.Log(itemInHand.name);
        }

        if (Input.GetMouseButtonDown(0) && cameraPointer.transform != null)
        {
            if (cameraPointer.transform.tag == "DoorKnob") { DoorInteraction(cameraPointer); }
            if(cameraPointer.transform.parent != null) { if (cameraPointer.transform.parent.name == "Numpad") { EnterCode(cameraPointer); } }
           
            MoveBetweenLevels(cameraPointer); 
            Debug.Log("Past change");
            /*if (itemInHand == null)
            {
                DoorInteraction(cameraPointer);
                EnterCode(cameraPointer);
            }

            else if (itemInHand != null)
            {
                MoveBetweenLevels(cameraPointer);
            }*/


        }          
      
        if(Input.GetKeyDown(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump  = false;
            Jump();
            Invoke(nameof(ResetJump), 2);
        }


      
      
        if(Input.GetKeyDown(KeyCode.E) && !handFull)
        {
            PickUpPOVPointer();
        }
        if (Input.GetKeyDown(KeyCode.F) && handFull)
        {
            Drop(itemInHandPointer.transform);
            
            handFull = false;
        }
      
        if(Input.GetMouseButtonDown(0) && handFull && itemInHandPointer.transform.GetComponent<ObjectInteractions>() != null)
        {
            ObjectInteractions itemDescription = itemInHandPointer.transform.GetComponent<ObjectInteractions>();
            if (Physics.Raycast(cameraLocation.position,cameraLocation.forward,out cameraPointer, 3))
            {              
                if (itemDescription.itemName == ObjectInteractions.items.Shovel && cameraPointer.transform.tag == "Dirt")
                {
                    Dig();
                }
                else if(itemDescription.itemName == ObjectInteractions.items.Chair && cameraPointer.transform.tag == "Guard")
                {
                    Attack(itemInHand.transform.gameObject,cameraPointer.transform.gameObject);                  
                }
                else
                {
                    return;
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

    private void PickUpPOVPointer()
    {
       
        if(Physics.Raycast(cameraLocation.position, cameraLocation.forward, out itemInHandPointer, 4f))
        {                     
               
            if(itemInHandPointer.transform.tag == "Pick Up" && !handFull)
            {
                PickUp(itemInHandPointer.transform);
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
        Collider collider = rb.GetComponent<Collider>();
        item.gameObject.isStatic = false;
        itemInHand = item.gameObject;
        collider.enabled = false;
        item.parent = hand;
        item.position = hand.transform.position;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        item.LookAt(transform.forward);
        
    }

    private void Drop(Transform item)
    {
        Rigidbody rb= item.GetComponent<Rigidbody>();
        Collider collider = rb.GetComponent<Collider>();
        collider.enabled = true;
        item.parent = null;
        rb.constraints = RigidbodyConstraints.None;
        itemInHand = null;
    }

    private void DoorInteraction(RaycastHit cameraPointer)
    {
        bool doorOpen = cameraPointer.transform.GetComponentInParent<Animator>().GetBool("Opened");

        if (cameraPointer.transform.tag == "DoorKnob" && !doorOpen)
        {
            cameraPointer.transform.GetComponentInParent<Animator>().SetBool("Opened", true);           
        }
        else if(cameraPointer.transform.tag == "DoorKnob" && doorOpen)
        {
            cameraPointer.transform.GetComponentInParent<Animator>().SetBool("Opened", false);
        }

        
    }

    private void EnterCode(RaycastHit cameraPointer)
    {
        if (cameraPointer.transform.parent.name == "Numpad" && cameraPointer.transform.name != "Cancel" && cameraPointer.transform.name != "Enter")
        {
            code[codeIndex] = cameraPointer.transform.name;           
            codeIndex++;
        }
        else if (cameraPointer.transform.name == "Cancel")
        {
            codeIndex = 0;
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = null;
            }
        }
        else if (cameraPointer.transform.name == "Enter")
        {
            string codeDisplay = "";
            foreach (string code in code)
            {
                codeDisplay += code;
            }
           
            if(codeDisplay == correctCode)
            {
                cameraPointer.transform.parent.parent.GetComponentInParent<Animator>().Play("Safe Door Open");
            }
        }

    }

    private void Attack(GameObject itemInHand, GameObject guard)
    {
        if (guard)
        {
            Debug.Log("Attack");
            Destroy(guard);           
            Destroy(itemInHand);
            handFull = false;
        }
    }

    private void Dig()
    {
        if (Physics.Raycast(cameraLocation.position, cameraLocation.forward, out cameraPointer, 4f))
        {
            Destroy(cameraPointer.transform.gameObject);
            Debug.Log("Dig");
        }
    }

    private void MoveBetweenLevels(RaycastHit cameraPoint)
    {
        Debug.Log("Start changing");
        if(cameraPointer.transform.tag == "BackDoorKnob")
        {
            SceneManager.LoadScene("Level2.2");
        }
        if(cameraPointer.transform.tag == "FrontDoorKnob" && itemInHand.name == "File" )
        {
            Debug.Log("Out");
            SceneManager.LoadScene("Level3");
        }
        if(cameraPointer.collider.tag == "CarDoorHandle")
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            return;
        }
    }
}
