using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Board : MonoBehaviour
{
    int height; // Row count of the grid
    int width;  // Column count of the grid


    [SerializeField] InitialLevelSO initialLevelGridSO;  // I take values from scriptable object like initial boxes, width,height, goals

    // Right now tiles are not useful, I did not remove tiles it has can have box property and maybe in the future it can be used for
    // creating blocks that can not be broken
    public Tile[,] tiles;
    [SerializeField] GameObject tilePrefab;

    // This list holds all the game pieces on the grid boxes,ducks,balloons,rockets
    public GameObject[,] gamePieces;

    // this array holds prefabs that can be randomly fall from above it is taken from the scriptable object
    // If you dont want to have ducks in the episode do not add duck prefab to the scriptable object
    [SerializeField] GameObject[] gamePiecePrefabs;

    // Used to store Rocket Prefabs Horizontal,Vertical
    [SerializeField]
    GameObject[] rocketPrefabs;

    // Audios are played on a manager
    AudioManager audioManager;

    // I hold Matches as lists on a list
    public List<List<GameObject>> matchesList;

    // Goals also taken from scriptable object, These prefabs are added to top level UI for Goals
    public GoalPrefabsHandler goalPrefabsHandler;

    // This text shows how many moves left on the top right
    MoveLeftText moveLeftText;


    // I am using this for rocket coroutines
    // If a rocket hits another rocket another they also added to list so it delays the filling operation
    [HideInInspector]
    public List<DestructiveRocket> rocketList;
    Coroutine rocketCoroutine;

    // this boolean is used to stop user input at some situations, When move count == 0 it is false always
    bool canTakePlayerInput = true;

    public bool CanTakePlayerInput { get => canTakePlayerInput; set => canTakePlayerInput = value; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        TakeInputsFromScriptableObject();
        InitializeEnvironment();
        FindObjectOfTypeMethodsAndInitializations();
        InitializeBoardTiles();
        InitializeBoard();
        SetupCamera();
        FindMatches();
        SetAdjacentCount();
    }



    /// <summary>
    /// This function is used to take height, width and gamePieces that will drop from the top
    /// </summary>
    private void TakeInputsFromScriptableObject()
    {
        height = initialLevelGridSO.Height;
        width = initialLevelGridSO.Width;
        gamePiecePrefabs = initialLevelGridSO.GamePieces;
    }

    /// <summary>
    /// Initializes borders and grounds by using an interface
    /// Grounds are used to understand if the duck hit the bottom
    /// </summary>
    void InitializeEnvironment()
    {
        GetComponent<IEnvironmentCreator>().InitializeBorders(width, height);
        GetComponent<IEnvironmentCreator>().InitializeGrounds(width, height);
    }

    /// <summary>
    /// This function is used to Initialize GamePieces array, RocketList and Matches List and find GoalPrefabsHandler, AudioManager and MoveLefttext
    /// </summary>
    void FindObjectOfTypeMethodsAndInitializations()
    {
        gamePieces = new GameObject[width, height];
        rocketList = new List<DestructiveRocket>();
        matchesList = new List<List<GameObject>>();
        goalPrefabsHandler = FindObjectOfType<GoalPrefabsHandler>();
        audioManager = FindObjectOfType<AudioManager>();
        moveLeftText = FindObjectOfType<MoveLeftText>();
    }



    /// <summary>
    /// This function calls camera functions to setup the camera
    /// </summary>
    void SetupCamera()
    {
        FindObjectOfType<CameraController>().SetupCamera(width, height);
    }


    /// <summary>
    /// Initializes the tiles
    // Right now tiles are not useful, I did not remove tiles it has can have box property and maybe in the future it can be used for
    // creating blocks that can not be broken
    /// </summary>
    void InitializeBoardTiles()
    {
        tiles = new Tile[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                InstantiateGamePiece(tilePrefab, i, j);
            }
        }
    }

    /// <summary>
    /// This function initializes the game pieces on grid by using the scriptable object
    /// </summary>
    void InitializeBoard()
    {
        int count = 0; // I am using count because the grid is a 1d array since the unity inspector can not show 2d arrays I have used 1d array to
                       // hold initial grid I am using count to reach prefabs.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null && tiles[i, j].CanHaveBox)
                {
                    InstantiateGamePiece(initialLevelGridSO.InitialGrid[count], i, j);
                }
                count++;
            }
        }
    }

    /// <summary>
    /// Instantiates a random game piece with X and Y coordinates.
    /// Fall offset is used to make objects fall from above.
    /// </summary>
    void InstantiateRandomGamePiece(int X, int Y, int FallOffset = 0)
    {
        int random = UnityEngine.Random.Range(0, gamePiecePrefabs.Length);
        InstantiateGamePiece(gamePiecePrefabs[random], X, Y, FallOffset);
    }

    /// <summary>
    /// If the prefab inherited IInstantiable this method can instantiates it, 
    /// </summary>
    void InstantiateGamePiece(GameObject prefabToInstantiate, int X, int Y, int FallOffset = 0)
    {
        if (prefabToInstantiate != null)
        {
            if (prefabToInstantiate.GetComponent<IInstantiable>() != null)
            {
                prefabToInstantiate.GetComponent<IInstantiable>().Instantiate(X, Y, this, FallOffset);
            }
        }
    }

    /// <summary>
    /// This method is used when a rockes is instantiated. Box class makes blackhole move and calls this method.
    /// It destroys the boxes and instantiates the rocket
    /// </summary>
    public void DestroyBoxesThenInstantiateRocketAt(int XPos, int YPos, List<GameObject> Boxes)
    {
        DestroyGamePieces(Boxes);
        int random = UnityEngine.Random.Range(0, rocketPrefabs.Length);
        GameObject prefabToInstantiate = rocketPrefabs[random];
        InstantiateGamePiece(prefabToInstantiate, XPos, YPos);
        FillTheBoardWithCubesAbove();
    }


    /// <summary>
    /// Takes a list and for each gamepiece on the list by calling DestroyGamePiece function
    /// It only plays 1 audio clip for all pieces
    /// </summary>
    /// 
    public void DestroyGamePieces(List<GameObject> GamePieceToDestroy, GamePieceType audioToPlay = GamePieceType.Box)
    {
        if (GamePieceToDestroy.Count == 0)
            return;
        while (GamePieceToDestroy.Count != 0)
        {
            DestroyGamePiece(GamePieceToDestroy[0]);
            GamePieceToDestroy.RemoveAt(0);
        }
        audioManager.Play(audioToPlay);
    }

    /// <summary>
    /// Destroys a piece by calling their destroy function, The classes that inherit IGamePiece interface can be destroyed here
    /// </summary>
    public void DestroyGamePiece(GameObject gamePieceToDestroy)
    {
        if (gamePieceToDestroy != null)
        {
            gamePieceToDestroy.GetComponent<IGamePiece>().Destroy();
        }
    }




    /// <summary>
    /// Clears Matches list and Sets list indexes of the objects that can make match to -1.
    /// -1 gains meaning in Find Matches function. I am using it to check if this piece are already in a maatch or not
    /// </summary>
    void ResetMatchesOperation()
    {
        matchesList.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePieces[i, j] != null && gamePieces[i, j].GetComponent<ICanMakeMatch>() != null)
                {
                    gamePieces[i, j].GetComponent<ICanMakeMatch>().SetListIndex(-1);
                }
            }
        }

    }

    /// <summary>
    /// Finding Match algorithm is simple, I am Starting with a point if it can make match and not already checked I am extending to neigbours to find match
    /// Then to their neighbours if there was a match at the first point. Then Adding these points to matches list. I am grouping gamepieces that can make match
    /// </summary>
    void FindMatches()
    {
        ResetMatchesOperation();
        // To hold list index, It is used when creating a new box list 
        int listIndexCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePieces[i, j] != null && gamePieces[i, j].GetComponent<ICanMakeMatch>() != null &&
                    gamePieces[i, j].GetComponent<ICanMakeMatch>().GetListIndex() == -1)
                {
                    FindMatchesAt(i, j, listIndexCount);
                    listIndexCount++;
                }
            }
        }
    }
    /// <summary>
    /// Extends the source game piece and find the game pieces that can make a match with itself,
    /// The single box can find their matches by checking their list index at the matches list
    /// </summary>
    void FindMatchesAt(int X, int Y, int ListIndexCount)
    {
        // List to check duplicate values
        List<GameObject> matchesSubList = new List<GameObject>();
        matchesSubList.Add(gamePieces[X, Y]);    //add the source to the list

        // Tuple to hold grid coordinates
        Stack<Tuple<int, int>> coordinateStack = new Stack<Tuple<int, int>>();
        coordinateStack.Push(Tuple.Create(X, Y));

        // set the list index count of the first box
        gamePieces[X, Y].GetComponent<ICanMakeMatch>().SetListIndex(ListIndexCount);

        while (coordinateStack.Count != 0)
        {
            var tempTuple = coordinateStack.Pop();
            GameObject tempObject = gamePieces[tempTuple.Item1, tempTuple.Item2];
            if (tempObject.GetComponent<ICanMakeMatch>() == null)
            {
                continue;
            }

            // upper cube
            if (FindMatchesAtBooleanFunction(tempTuple.Item1, tempTuple.Item2 + 1, tempObject))
            {
                FindMatchesAtOperation(coordinateStack, matchesSubList, Tuple.Create(tempTuple.Item1, tempTuple.Item2 + 1), ListIndexCount);
            }
            //right cube
            if (FindMatchesAtBooleanFunction(tempTuple.Item1 + 1, tempTuple.Item2, tempObject))
            {
                FindMatchesAtOperation(coordinateStack, matchesSubList, Tuple.Create(tempTuple.Item1 + 1, tempTuple.Item2), ListIndexCount);
            }
            // bottom cube
            if (FindMatchesAtBooleanFunction(tempTuple.Item1, tempTuple.Item2 - 1, tempObject))
            {
                FindMatchesAtOperation(coordinateStack, matchesSubList, Tuple.Create(tempTuple.Item1, tempTuple.Item2 - 1), ListIndexCount);
            }
            //left cube
            if (FindMatchesAtBooleanFunction(tempTuple.Item1 - 1, tempTuple.Item2, tempObject))
            {
                FindMatchesAtOperation(coordinateStack, matchesSubList, Tuple.Create(tempTuple.Item1 - 1, tempTuple.Item2), ListIndexCount);
            }

        }
        matchesList.Add(matchesSubList);
    }

    /// <summary>
    /// If the game object that will be added to the matches is already on the list do nothing, else extend the matches list by adding this object
    /// and also add the value to the stack to check it's neighbours
    /// Setting their list index so they can find each other when any of them is clicked
    /// </summary>
    void FindMatchesAtOperation(Stack<Tuple<int, int>> BoxStack, List<GameObject> MatchesSubList, Tuple<int, int> PosTuple, int ListIndexCount)
    {
        if (!MatchesSubList.Contains(gamePieces[PosTuple.Item1, PosTuple.Item2]))
        {
            BoxStack.Push(Tuple.Create(PosTuple.Item1, PosTuple.Item2));
            MatchesSubList.Add(gamePieces[PosTuple.Item1, PosTuple.Item2]);
            gamePieces[PosTuple.Item1, PosTuple.Item2].GetComponent<ICanMakeMatch>().SetListIndex(ListIndexCount);
        }
    }

    /// <summary>
    /// Sets the adjacent Count of the game pieces that can make match. 
    /// By using Adjacent Count The single game pieces understand the number of tiles it can make match with.
    /// </summary>
    private void SetAdjacentCount()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePieces[i, j] != null && gamePieces[i, j].GetComponent<ICanMakeMatch>() != null)
                {
                    gamePieces[i, j].GetComponent<ICanMakeMatch>().SetAdjacentCount();
                }
            }
        }
    }






    /// <summary>
    /// This is used to make only 1 RocketCoroutine work. If there is one working second rocket do not start.
    /// </summary>
    public void ControlRocketCoroutine(Rocket RocketToExplode)
    {
        RocketToExplode.Destroy();
        if (rocketCoroutine == null)
        {
            rocketCoroutine = StartCoroutine(RocketCoroutine());
        }
    }

    /// <summary>
    /// Checks if there is a flying rocket if all rockets are destroyed fills the boards missing pieces
    /// </summary>
    IEnumerator RocketCoroutine()
    {
        while (rocketList.Count != 0)
        {
            yield return null;
        }

        rocketCoroutine = null;
        FillTheBoardWithCubesAbove();
    }

    /// <summary>
    /// Finds null positions on Grid, Returns a dictionary, dictionary keys are Column numbers, value is number of null positions on that column
    /// </summary>
    private Dictionary<int, int> FindNullPositions()
    {
        var positions = new Dictionary<int, int>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePieces[i, j] == null)
                {
                    if (positions.ContainsKey(i))
                    {
                        positions[i]++;
                    }
                    else
                    {
                        positions.Add(i, 1);
                    }
                }
            }
        }
        return positions;
    }

    /// <summary>
    /// This function shifts the upper pieces to lower positions if there is a null position
    /// </summary>
    public void OrderColumns( Dictionary<int,int> ColumnAndTheirCount)
    {
        foreach(int j in ColumnAndTheirCount.Keys) // travel Columns that has a hole
        {
            int count = 0; // used to track count of the null values in a column
            for (int i = 0; i < height; i++)
            {
                if (gamePieces[j, i] == null)
                {
                    count++; 
                }
                else if (count > 0)
                {
                    gamePieces[j, i - count] = gamePieces[j, i];
                    gamePieces[j, i - count].GetComponent<IGamePiece>().SetYPos(i - count); // update pos

                    if (gamePieces[j, i - count].GetComponent<IHasSortingOrder>() != null) // rockets dont have sorting order
                    {
                        gamePieces[j, i - count].GetComponent<IHasSortingOrder>().GetSpriteRenderer().sortingOrder = i - count; // update sorting order
                    }
                }
            }
            for (int i = height - 1; i >= height - count; i--)  //makes upper positions null,If 1 hole in a column then top position is null
            {
                gamePieces[j, i] = null;
            }
        }
    }


    /// <summary>
    /// func will take columns and the number of boxes correspond
    ///  Key is Column number and value is box count
    /// </summary>
    IEnumerator FallOperation(Dictionary<int, int> columnNumberArray)
    {
        CanTakePlayerInput = false;
        foreach (var element in columnNumberArray)
        {
            for (int i = 0; i < element.Value; i++)
            {
                InstantiateRandomGamePiece(element.Key, height - element.Value + i, element.Value + 3);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        FindMatches();  // For new grid Matches needs to be found
        SetAdjacentCount();

        CanTakePlayerInput = true;  // Starts Player Input
    }

    /// <summary>
    /// For each box in a match that will be destroyed This function finds the Balloons and returns them
    /// </summary>
    public List<GameObject> FindBalloonsToDestroy(List<GameObject> boxThatWillBeDestroyed)
    {
        var baloonList = new List<GameObject>();
        foreach (var box in boxThatWillBeDestroyed)
        {
            baloonList = baloonList.Union(GetAdjacentBaloons(box)).ToList();
        }
        return baloonList;
    }

    /// <summary>
    /// Checks all directions of the box and if finds a balloon that can be destroyed adds them to the list that will be returned
    /// </summary>
    /// <returns></returns>
    List<GameObject> GetAdjacentBaloons(GameObject currentBox)
    {
        var baloons = new List<GameObject>();
        int xPos = currentBox.GetComponent<IGamePiece>().GetXPos();
        int yPos = currentBox.GetComponent<IGamePiece>().GetYPos();

        PieceColor currentColor = currentBox.GetComponent<IHasColor>().GetColor(); // color of thegame piece
        if (GetAdjacentBalloonsBooleanFunction(xPos, yPos + 1, currentBox))
        {
            baloons.Add(gamePieces[xPos, yPos + 1]);
        }
        if (GetAdjacentBalloonsBooleanFunction(xPos, yPos - 1, currentBox))
        {
            baloons.Add(gamePieces[xPos, yPos - 1]);
        }
        if (GetAdjacentBalloonsBooleanFunction(xPos + 1, yPos, currentBox))
        {
            baloons.Add(gamePieces[xPos + 1, yPos]);
        }
        if (GetAdjacentBalloonsBooleanFunction(xPos - 1, yPos, currentBox))
        {
            baloons.Add(gamePieces[xPos - 1, yPos]);
        }
        return baloons;
    }

    /// <summary>
    /// Decreases the MoveLeftText
    /// </summary>
    public void DecreaseMoveLeft()
    {
        moveLeftText.DecreaseMoveCount();
    }

    /// <summary>
    /// Used to group the functions that fills the board
    /// </summary>
    public void FillTheBoardWithCubesAbove()
    {
        var positionsToFill = FindNullPositions();      // this operation finds columns and how many empty spaces it has
        OrderColumns(positionsToFill);
        StartCoroutine(FallOperation(positionsToFill)); // starts filling the empty places with falling pieces from above
    }

    /// <summary>
    /// Right now this function is not useful but because all balloons have general color. When colored balloons are added to the game it will be used for
    /// finding if a balloon can be exploded or not
    /// </summary>
    /// <returns></returns>
    bool CheckBalloonColors(GameObject Box, GameObject Balloon)
    {
        if (Balloon.GetComponent<IHasColor>().GetColor() == PieceColor.General)
            return true;

        if (Balloon.GetComponent<IHasColor>().GetColor() == Box.GetComponent<IHasColor>().GetColor())
            return true;

        return false;
    }

    /// <summary>
    /// When a box and a balloon explode at the same match, the game plays only balloon sound.
    /// So I made a Priority function to find correct audio
    /// </summary>
    /// <returns></returns>
    public GamePieceType FindGamePieceThatHasPriority(List<GameObject> baloonsToDestroy)
    {
        if (baloonsToDestroy.Count > 0)
        {
            return GamePieceType.Balloon;
        }
        else
        {
            return GamePieceType.Box;
        }
    }

    /// <summary>
    /// Is used to make the function more readable
    /// </summary>
    bool FindMatchesAtBooleanFunction(int XPos, int YPos, GameObject SourceObject) 
    {
        if(IsWithinBounds(XPos,YPos) && gamePieces[XPos, YPos] != null && gamePieces[XPos, YPos].GetComponent<ICanMakeMatch>() != null &&
             SourceObject.GetComponent<ICanMakeMatch>().CheckMatch(SourceObject, gamePieces[XPos, YPos]))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Is used to make the function more readable
    /// </summary>
    bool GetAdjacentBalloonsBooleanFunction(int XPos, int YPos, GameObject CurrentBox)
    {
        if(IsWithinBounds(XPos,YPos) && gamePieces[XPos, YPos] != null && 
            gamePieces[XPos,YPos].GetComponent<IGamePiece>().GetGamePieceType() == GamePieceType.Balloon &&
            CheckBalloonColors(CurrentBox, gamePieces[XPos, YPos]))
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Is used in some if cases to understand if the positions are on board
    /// </summary>
    /// <returns></returns>
    bool IsWithinBounds(int X, int Y)
    {
        if (X < width && X >= 0 && Y < height && Y >= 0)
        {
            return true;
        }
        return false;
    }

}
