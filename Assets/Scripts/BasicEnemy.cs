using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    [SerializeField] float velocityX;
    [SerializeField] float altitudToLeave;
    enum State { Chasing, Leaving, Stunned, Falling}

    State state;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    GameObject player;
    float direction;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = Movement.instance.gameObject;

        InvokeRepeating("ChooseDirection", 0, 2);
    }

    void Update()
    {
        HandleState();

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocityX));
        animator.SetBool("Falling", Mathf.Abs(rb.linearVelocityY) > 1);

        if (state == State.Leaving)
        {
            rb.linearVelocityX = velocityX * direction;

            spriteRenderer.flipX = rb.linearVelocityX <= 0;
        }

        if (state == State.Falling)
        {
            rb.linearVelocityX = 0;
        }

        if (state == State.Chasing)
        {
            rb.linearVelocityX = velocityX * direction;

            spriteRenderer.flipX = rb.linearVelocityX <= 0;
        }
    }

    void ChooseDirection()
    {
        if (state == State.Chasing)
        {
            direction = player.transform.position.x < transform.position.x ? -1 : 1;
        }
    }

    void HandleState()
    {
        if (state == State.Leaving)
            return;

        if (transform.position.y < altitudToLeave)
        {
            state = State.Leaving;
            direction = Random.value < 0.5f ? -1 : 1;
        }
        else
        {
            if (Mathf.Abs(rb.linearVelocityY) > 1)
                state = State.Falling;
            else
                state = State.Chasing;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Border>())
        {
            if (collision.gameObject.name == "RightBorder" ||
                collision.gameObject.name == "LeftBorder")
            {
                Destroy(gameObject);
            }
        }
    }
}
