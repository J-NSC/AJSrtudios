using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer; 

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1;
    public float fallMultiplier = 5f;

    [Header("Jump")]
    public float jumpSpeed = 1.0f;
    public float jumDelay = 0.25f; 
    private float jumpTimer;

    [Header("Collison")]
    public bool onGround = false; 
    public float groundLength = 0.6f;

    // Update is called once per frame

    void Update(){

        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer);

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetButtonDown("Jump")){
            jumpTimer = Time.time + jumDelay;
        }
    }

    void FixedUpdate(){
        // teste = System.Math.Round(rb.velocity.x , 1);
        moveCharacter(direction.x);
        if(jumpTimer > Time.time && onGround){
            Jump();
        }
        modifyPhysics();


    }

    // //  Mathf.Round((Mathf.Abs(rb.velocity.x) * 10.0f) * 0.1f)

    void moveCharacter(float horizontal){
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)){
            Flip();
        }
        if(Mathf.Abs(rb.velocity.x) > maxSpeed){
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        animator.SetFloat("x", Mathf.Abs(rb.velocity.x));
    }
    void modifyPhysics(){
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) ||  (direction.x < 0 && rb.velocity.x > 0);
        

        if(onGround){
            if(Mathf.Abs(direction.x) < 0.4f || changingDirections){
                rb.drag = linearDrag;
            }else{
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }else {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if(rb.velocity.y < 0){
                rb.gravityScale = gravity * fallMultiplier;
            }else if(rb.velocity.y  > 0 && !Input.GetButton("Jump")){
                rb.gravityScale = gravity * (fallMultiplier /2);
            }
        }
        
    }
    void Flip(){
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundLength);    
    }
}
