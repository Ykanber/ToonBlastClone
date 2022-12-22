using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Initial Level", fileName ="NewInitialLevel")]
public class InitialLevelSO : ScriptableObject
{
    [SerializeField] // width of the grid
    int width;

    [SerializeField]  // height of the grid
    int height;

    [SerializeField]
    int moveCount; // move count of the episode

    [SerializeField]
    GameObject[] gamePieces; // gamepieces that will drop from top

    [SerializeField]
    GameObject[] initialGrid; // initial grid, it is in 1D because inspector can not show 2d arrays, first columns then rows are filled, It has to be width*height size

    [SerializeField]
    GameObject[] goalPrefabs; // goals

    [SerializeField]
    int[] goalCount; // goal numbers, size has to be same with goal prefabs

    public int Width { get => width;}
    public int Height { get => height;}
    public int MoveCount { get => moveCount;}
    public GameObject[] GamePieces { get => gamePieces;}
    public GameObject[] InitialGrid { get => initialGrid;}
    public GameObject[] GoalPrefabs { get => goalPrefabs;}
    public int[] GoalCount { get => goalCount;}
}
