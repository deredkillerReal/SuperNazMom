using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{

    private AudioManager audioManager;

    private Vector2 AxisInput = Vector2.zero;
    private Vector3 moveDirection = Vector3.zero;

   Rigidbody2D rb2D;
    public LayerMask groundLayer;
    bool fire = false;
    [SerializeField]float jumpForce = 50;
    float _currentHorizontalSpeed = 0;
    private bool canJump = true;
    private bool isJumping = false;
    float lastTimeGrounded;
    float distToGround = 0;
    float lastYpos = 0;
    [SerializeField] UnityEngine.Transform top_left;
    [SerializeField] UnityEngine.Transform bottom_right;
    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
        rb2D = GetComponent<Rigidbody2D>();
        lastTimeGrounded = Time.time;
        distToGround = GetComponent<Collider2D>().bounds.extents.y;

    }

    void Update()
    {
        CalculateWalk();
        CalculateJump();
        Move();

    }

    private void CalculateJump()
    {
        bool CheckIfGrounded()
        {
            return Physics2D.OverlapArea(top_left.position, bottom_right.position, groundLayer);
        }

        if(CheckIfGrounded())
        {
            lastTimeGrounded = Time.time;
            isJumping = false;
        }
        if (Time.time - lastTimeGrounded < 0.15 && !isJumping)
        {
            canJump = true;
        }
        else canJump = false;
        lastYpos = transform.position.y;
    }


    #region Walk

    [Header("WALKING")][SerializeField] private float Speed = 100f;
    [SerializeField] private float _moveClamp = 13;

    private void CalculateWalk()
    {
        {
            _currentHorizontalSpeed = AxisInput.x * Speed * Time.deltaTime;
            moveDirection.x = _currentHorizontalSpeed*Speed;
        }
    }
    #endregion
    private void Move()
    {
        
        rb2D.velocity =new Vector2(_currentHorizontalSpeed, rb2D.velocity.y);

    }

    private void Jump()
    {
        if (canJump)
        {
            //rb2D.AddForce(new Vector2(0f, jumpForce));
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            isJumping = true;
            canJump = false;

        }
    }
    public void onMove(InputAction.CallbackContext context)
    {
        AxisInput = context.ReadValue<Vector2>();
    }
    public void onFire(InputAction.CallbackContext context)
    {
        //fire = context.ReadValue<bool>();
        fire = context.action.triggered;
    }
    public void onJump(InputAction.CallbackContext context)
    {
        //isJump = context.ReadValue<bool>();
        Jump();
    }


    void flip()
    {
        transform.localScale = transform.localScale * -1;
    }

}

