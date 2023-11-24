using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class playermovement : MonoBehaviour
{
    [Header("Movement")]
     public float moveSpeed;
     public float groundDrag;
     public float walkspeed;
     public float sprintspeed;
     [Header("Jumping")] 
     public float jumpForce;
     public float jumpCooldown;
     public float airMultiplier;
     public int jumptimes;
     public int jumptimeslimit;
     bool readytojump;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatisground;
    bool grounded;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Header("slopeHandler")]
    public float maxslopeangle;
    private RaycastHit slopeHit;
    [Header("equipment")]
    private bool doublejump;
    

   

    public Transform orientaion;
    float horizontalInput;
    float verticalInput;
    bool exitingslope;

    Vector3 moveDirection;
    Rigidbody rb;
    public movementstate state;
    public enum movementstate
    {
        walking,
        sprinting,
        air

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation=true;
        readytojump = true;
        jumptimes=0;
        doublejump=false;
    }
  
    // Update is called once per frame
    void Update()
    {
        MyInput();
        statehandler();
        grounded =Physics.Raycast(transform.position,Vector3.down,playerHeight*0.5f+0.2f,whatisground);
        if (grounded){
        rb.drag = groundDrag;
        jumptimes=0;
        }
        else
        rb.drag = 0;
        SpeedControl();
        
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(jumpKey)&& readytojump && grounded)
        {
            readytojump = false;
            Jump();
            Invoke(nameof(resetjump),jumpCooldown);

        }
        else if(Input.GetKey(jumpKey)&& readytojump && state==movementstate.air&&jumptimes<jumptimeslimit-1&&doublejump==true)
        {  
             jumptimes+=1;
            readytojump=false;
            Jump();
            Invoke(nameof(resetjump),jumpCooldown);
        }
        


    }
    private void   MovePlayer()
    {
        moveDirection = orientaion.forward*verticalInput+orientaion.right*horizontalInput;
        if(grounded)
        rb.AddForce(moveDirection.normalized*moveSpeed*10f,ForceMode.Force);
        else if(!grounded)
        rb.AddForce(moveDirection.normalized*moveSpeed*10f*airMultiplier,ForceMode.Force);
        if (onslope()&&!exitingslope)
        {
            rb.AddForce(getslopemovedirection()*moveSpeed*20f,ForceMode.Force);
            if(rb.velocity.y>0){
                rb.AddForce(Vector3.down*80f,ForceMode.Force);
            }
        }
        rb.useGravity = !onslope();

    }
    private void SpeedControl()
    { 
        if(onslope()&&!exitingslope){
            if(rb.velocity.magnitude>moveSpeed){
                rb.velocity = rb.velocity.normalized*moveSpeed;
            }
        }
        else{
            Vector3 flatVel = new Vector3(rb.velocity.x,0f,rb.velocity.z);
        if(flatVel.magnitude>moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized*moveSpeed;
            rb.velocity = new   Vector3(limitedVel.x,rb.velocity.y,limitedVel.z);
        }
        }
       
    }
    private void Jump()
    {   exitingslope = true;
        rb.velocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);
        rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);
        
    }
    
    private void resetjump()
    {
        readytojump = true;
        exitingslope = false;
    }
    private void statehandler()
    {
        if(grounded&&Input.GetKey(sprintKey))
        {
            state = movementstate.sprinting;
            moveSpeed = sprintspeed;
        }
        else if(grounded)
        {
            state = movementstate.walking;
            moveSpeed = walkspeed;
        }
        else
        {
            state = movementstate.air;
        }
    }
    private bool onslope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit,playerHeight*0.5f+0.3f))
        {
            float angle = Vector3.Angle(Vector3.up,slopeHit.normal);
            return angle<maxslopeangle&&angle !=0;
        }
        return false;
    }
    private Vector3 getslopemovedirection()
    {
        return Vector3.ProjectOnPlane(moveDirection,slopeHit.normal).normalized; 
    }
    void  OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag=="equipment")
        {
            Debug.Log("on collision!");
            Destroy(other.gameObject);
            doublejump=true;
        }
    }
    
}
