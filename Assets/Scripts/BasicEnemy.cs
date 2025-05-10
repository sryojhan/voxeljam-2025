using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    enum State { Chasing, Leaving, Stunned}

    State state;

    Rigidbody2D rb;
    GameObject player;
    float direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Movement.instance.gameObject;
    }

    void Update()
    {
        if (state == State.Chasing)
        {
            direction = player.transform.position.x < transform.position.x ? -1 : 1;
        }
    }
}
