using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeGoal 
{
    void InstantiateMoveToGoalBox(Vector2 GoalPos); // creates a movingtogoal prefab for goals

    bool CompareGoalWithObject(GameObject Goal, GameObject GameObjectToCheck); // comparison to understand if one of the goals is same with the object

}
