using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiveRocket : MonoBehaviour
{
    int rocketSpeed= 30; // rocket speed

    [SerializeField]
    DestructiveRocketType destructiveRocketType; // type of the destructive rocket
    
    Board board; // reference to board

    Rigidbody2D rb2D; // reference to rigidbody2D

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        board.rocketList.Add(this); // adding the list for rocket routine on board class
        rb2D = GetComponent<Rigidbody2D>();
        Invoke(nameof(DestroyRocket), 0.6f); // after 0.6 seconds destroy the rocket
    }

    private void Update()
    {
        switch (destructiveRocketType) // switch to decide move direction
        {
            case DestructiveRocketType.Left:
                rb2D.velocity = new Vector2(-rocketSpeed, 0);
                break;
            case DestructiveRocketType.Right:
                rb2D.velocity = new Vector2(rocketSpeed, 0);
                break;
            case DestructiveRocketType.Top:
                rb2D.velocity = new Vector2(0, rocketSpeed);
                break;
            case DestructiveRocketType.Bottom:
                rb2D.velocity = new Vector2(0, -rocketSpeed);
                break;
        }
    }

    void DestroyRocket()
    {
        board.rocketList.Remove(this);
        Destroy(gameObject);
    }
}
