using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveLeftText : MonoBehaviour
{
    int moveCount; // move count

    [SerializeField]
    InitialLevelSO initialGameData; // scriptable object to take initial move count

    TextMeshProUGUI moveLeftText;

    Board board;

    private void Awake()
    {
        moveLeftText = GetComponent<TextMeshProUGUI>();
        MoveCount = initialGameData.MoveCount;
        board = FindObjectOfType<Board>();
    }

    int MoveCount 
    {
        get 
        {
            return moveCount;
        }
        set
        {
            moveCount = value;
            moveLeftText.text = moveCount.ToString();
        }
    }

    public void DecreaseMoveCount() 
    {
        if (MoveCount > 1)
        { 
            MoveCount--;
        }
        else
        {
            MoveCount--;
            StartCoroutine(EndGameRoutine());   // I am using a routine because in the other methods there might be a rocket explosion or something else thaat will turn on the input
        }
    }

    IEnumerator EndGameRoutine()
    {
        int iter = 0;
        while(iter < 180)
        {
            board.CanTakePlayerInput = false;
            yield return null;
            iter++;
        }
    }
}
