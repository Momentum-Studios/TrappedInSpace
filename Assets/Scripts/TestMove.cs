using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    private float horizontal;
    private float speed;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private float boostTimer;
    private bool isBoosting;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        boostTimer = 0;
        speed = 8f;
        isBoosting = false;
    }
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if(isBoosting)
        {
            boostTimer += Time.deltaTime;
            if(boostTimer >= 5)
            {
                speed = 8f;
                boostTimer = 0;
                isBoosting = false;
            }    
        }    

        Flip();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "SpeedBoost")
        {
            isBoosting = true;
            speed *= 2;
            Destroy(other.gameObject);
        }    
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
