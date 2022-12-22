using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPrefabsHandler : MonoBehaviour
{
    [SerializeField]
    InitialLevelSO initialLevelSO; 

    List<GameObject> goalPrefabList;

    [SerializeField]
    List<int> countList;

    Board board;
    private void Start()
    {
        InitializeGoals();
        board = FindObjectOfType<Board>();
    }

    /// <summary>
    /// Take goals and their counts from the scriptable object
    /// </summary>
    private void InitializeGoals()
    {
        countList = new List<int>();
        foreach (var item in initialLevelSO.GoalCount)
        {
            countList.Add(item);
        }

        goalPrefabList = new List<GameObject>();
        for (int i = 0; i < countList.Count; i++)
        {
            GameObject instantiatedObject = Instantiate(initialLevelSO.GoalPrefabs[i], transform.position, Quaternion.identity);
            instantiatedObject.transform.SetParent(transform, false);
            goalPrefabList.Add(instantiatedObject);
            SetCount(instantiatedObject, countList[i]);
        }
    }

    void SetCount(GameObject GoalObjectToChangeCount, int Count) // set goal gount
    {
        GoalCountText goalCountText = GoalObjectToChangeCount.GetComponentInChildren<GoalCountText>();
        goalCountText.SetCountText(Count);

    }

    /// <summary>
    /// 
    /// </summary>
    public void DecreaseCount(GameObject GoalObjectToChangeCount) // decrease goal count
    {
        if (GoalObjectToChangeCount != null)
        {
            GoalCountText goalCountText = GoalObjectToChangeCount.GetComponentInChildren<GoalCountText>();
            if (goalCountText != null)
            {
                goalCountText.DecreaseCountText(1);
            }
        }
    }

    /// <summary>
    /// if the exploded game piece is one of the goal ones this function detects it, and calls the instantiate function for the moveToGoalBoxes
    /// If a goal's count reaches the 0 it removes the goal from the lists so the explosions on this piecee will not trigger instantiation of moveToGoalBoxes
    /// </summary>
    public void CheckExplodedGamePieceForGoal(GameObject objectToCheck)
    {
        for (int i = 0; i < goalPrefabList.Count; i++)
        {
            if (goalPrefabList[i] != null && objectToCheck.GetComponent<ICanBeGoal>() != null &&
                objectToCheck.GetComponent<ICanBeGoal>().CompareGoalWithObject(goalPrefabList[i], objectToCheck))
            {

                Vector2 Pos = goalPrefabList[i].transform.position;
                Pos = new Vector2(Pos.x - 0.5f, Pos.y - 0.5f);
                objectToCheck.GetComponent<ICanBeGoal>().InstantiateMoveToGoalBox(Pos);
                countList[i]--;
                if (countList[i] <= 0)
                {
                    countList.RemoveAt(i);
                    goalPrefabList.RemoveAt(i);
                }
                break;
            }
        }
    }
}

