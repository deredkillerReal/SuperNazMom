using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UIElements.Experimental;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    //https://www.youtube.com/watch?v=Er99e0OOBgc


    private Vector2 AxisInput = Vector2.zero;
    private Vector3 moveDirection = Vector3.zero;

    Rigidbody2D rb2D;
    public LayerMask groundLayer;
    private Player player;
    float _currentHorizontalSpeed = 0;
    private bool canJump = true;
    private bool isJumping = false;
    float lastTimeGrounded;
    [SerializeField] UnityEngine.Transform top_left;
    [SerializeField] UnityEngine.Transform bottom_right;
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        lastTimeGrounded = Time.time;

    }

    void Update()
    {
        CalculateWalk();
        CalculateJump();
        Move();

    }

    private void Move()
    {

        rb2D.velocity = new Vector2(_currentHorizontalSpeed, rb2D.velocity.y);
        flip();
    }

    #region Walk

    [Header("WALKING")][SerializeField] private float Speed = 100f;
    //[SerializeField] private float _moveClamp = 13;

    private void CalculateWalk()
    {
        {
            _currentHorizontalSpeed = AxisInput.x * Speed * Time.deltaTime;
            moveDirection.x = _currentHorizontalSpeed * Speed;
        }
    }
    #endregion

    #region Jump
    [SerializeField] public float jumpForce = 5;
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
    private void CalculateJump()
    {
        bool CheckIfGrounded()
        {
            return Physics2D.OverlapArea(top_left.position, bottom_right.position, groundLayer);
        }

        if (CheckIfGrounded())
        {
            lastTimeGrounded = Time.time;
            isJumping = false;
        }
        if (Time.time - lastTimeGrounded < 0.15 && !isJumping)
        {
            canJump = true;
        }
        else canJump = false;
    }
    #endregion

    void flip()
    {
        int x = 0;
        if (rb2D.velocity.x == 0) return;
        else if (rb2D.velocity.x > 0) x = 1;
        else x = -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);

    }
    public void onMove(InputAction.CallbackContext context)
    {
        AxisInput = context.ReadValue<Vector2>();
    }

    public void onJump(InputAction.CallbackContext context)
    {
        //isJump = context.ReadValue<bool>();
        Debug.Log("jjj");
        Jump();
    }



}

