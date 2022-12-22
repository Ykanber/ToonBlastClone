using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingToGoalBox : MonoBehaviour
{
    AudioManager audioManager; // reference to audio manager
    Vector2 goalPos;            // goal position
    GoalPrefabsHandler goalPrefabsHandler; // reference to goalPrefabsHandler

    [SerializeField]
    ParticleSystem cubeCollectFX; // effect that will apper when it collides with the goal

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        goalPrefabsHandler = FindObjectOfType<GoalPrefabsHandler>();
    }

    private void Update() // moves to the goal
    {
        transform.position = Vector2.MoveTowards(transform.position, goalPos, Time.deltaTime * 20);
    }

    public void Init(Vector2 GoalPos) // goal is given to the object
    {
        goalPos = GoalPos;
    }

    private void OnTriggerStay2D(Collider2D Collision) // this is not used in my observations but it is still thereif the ontriggerenter fails to detect the collision
    {
        if (Collision.tag == "Goal")
        {
            Instantiate(cubeCollectFX, new Vector2(goalPos.x + 0.5f, goalPos.y + 0.5f), Quaternion.identity);
            audioManager.PlayGoalCollectSound();
            goalPrefabsHandler.DecreaseCount(Collision.gameObject);
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D Collision) //when collision happens destroy
    {
        if (Collision.tag == "Goal")
        {
            Destroy(Collision);
        }
    }

    void Destroy(Collider2D Collision) 
    {
        Instantiate(cubeCollectFX, new Vector2(goalPos.x + 0.5f, goalPos.y + 0.5f), Quaternion.identity); // plays the particle effect
        audioManager.PlayGoalCollectSound(); // Plays the audio
        goalPrefabsHandler.DecreaseCount(Collision.gameObject); // Decreases the goal count
        Destroy(this.gameObject); // Destroys itself
    }
}