using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Box : MonoBehaviour, IGamePiece, IInstantiable, ICanMakeMatch, IHasSortingOrder, IHasColor, ICanBeGoal
{
    int xPos;   // X position of the box
    int yPos;   // Y position of the box

    [SerializeField]
    GamePieceType type; // Type of the box

    int listIndex = -1; // List index of the box, default is -1

    int adjacentCount = 1; // Adjacent count of the box , if no matches then it is 1

    [SerializeField]
    ParticleSystem explodeFX; // explosion effect

    [SerializeField]
    GameObject moveToGoalBoxPrefab; // prefab when a goal is same with the current box, this object moves to the goal position

    public PieceColor color; // color of the box

    Animator animator; // animator to play animation when a rocket is instantiated

    bool isConstructingARocket; // boolean to understand if is Constructing A Rocket

    Board board; // reference to Board

    AudioManager audioManager; // reference to audio manager to play audio

    // for rocket
    bool isDestroyed; // used on on trigger 2D to prevent double explode sounds

    public bool IsDestroyed { get => isDestroyed; set => isDestroyed = value; }


    public void StartCreatingRocketAnimation()
    {
        animator.SetBool("IsConstructingARocket", true); // starts the animation
    }


    private void Update() // Makes boxes move towards the position on grid
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(xPos, yPos), Time.deltaTime * 20);
    }


    public int GetXPos() // Getter for X position
    {
        return xPos;
    }
    public void SetXPos(int XPos) // Setter for X position
    {
        xPos = XPos;
    }

    public int GetYPos() // Getter for Y position
    {
        return yPos;
    }
    public void SetYPos(int YPos) // Setter for Y position
    {
        yPos = YPos;
    }
    public GamePieceType GetGamePieceType()  // Getter for game piece type
    {
        return type;
    }

    public void SetListIndex(int value) // Setter for List index
    {
        listIndex = value;
    }

    public int GetListIndex() // Getter for List index
    {
        return listIndex;
    }

    public PieceColor GetColor() // Getter for Box Color
    {
        return color;
    }

    public void SetAdjacentCount() // Setter for AdjacentCount
    {
        if (listIndex > -1)
        {
            adjacentCount = board.matchesList[listIndex].Count;
        }
    }

    public void SetAudioManager() // Setter for audio Manager
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlayExplodeSound() //Plays explode sound
    {
        audioManager.Play(type);
    }

     /// <summary>
     /// Instantiates a particle effect on it's position, Checks if there is a goal with the same color, Destroys the Box
     /// </summary>
    public void Destroy()
    {
        ParticleSystem Fx = Instantiate(explodeFX, new Vector2(xPos + 0.5f, yPos + 0.5f), Quaternion.identity);
        board.goalPrefabsHandler.CheckExplodedGamePieceForGoal(this.gameObject); 
        board.gamePieces[xPos, yPos] = null;
        Destroy(this.gameObject);
    }

    public SpriteRenderer GetSpriteRenderer() // Getter for Sprite Renderer for sorting order operations
    {
        return GetComponent<SpriteRenderer>();
    }

    
    public void SetBoard(Board Board) // Setter For Board
    {
        board = Board;
    }

    /// <summary>
    /// Instantiates a prefab that goes to goal section to collide with, ICanBeGoalInterface
    /// </summary>
    public void InstantiateMoveToGoalBox(Vector2 GoalPos)
    {
        GameObject InstantiatedMoveToGoalBoxObject = Instantiate(moveToGoalBoxPrefab, transform.position, Quaternion.identity);
        InstantiatedMoveToGoalBoxObject.GetComponent<MovingToGoalBox>().Init(GoalPos);
    }


    /// <summary>
    /// Compares the goal prefab with the current box to understand if the goal is same with the box
    /// </summary>
    public bool CompareGoalWithObject(GameObject Goal, GameObject GameObjectToCheck)
    {
        if(Goal.GetComponent<BoxGoalPrefab>().type == GamePieceType.Box &&
            GameObjectToCheck.GetComponent<IHasColor>().GetColor() == Goal.GetComponent<BoxGoalPrefab>().color)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// This is used when the rocket collides with the box.
    /// It destroys the box
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDestroyed != true)
        {
            IsDestroyed = true;
            PlayExplodeSound();
            Destroy();
        }
    }


    /// <summary>
    /// Instantiates a box at a Y position + Fall offset
    /// Adds to the game pieces array at the X,Y position
    /// Initializes the box
    /// </summary>
    public void Instantiate(int X, int Y, Board Board, int FallBoxIndex = 0)
    {
        GameObject instantiatedBoxObject = Instantiate(this.gameObject, new Vector3(X, Y + FallBoxIndex, 0), Quaternion.identity);
        if (instantiatedBoxObject != null)
        {
            instantiatedBoxObject.name = $"Box ({X},{Y})";
            IGamePiece instantiatedBox = instantiatedBoxObject.GetComponent<IGamePiece>();
            instantiatedBox.Initialize(X, Y);
            instantiatedBox.SetBoard(Board);
            Board.gamePieces[X, Y] = instantiatedBoxObject;
            instantiatedBoxObject.transform.parent = Board.transform;
            instantiatedBoxObject.GetComponent<IGamePiece>().SetAudioManager();
        }
    }
    public void Initialize(int XPos, int YPos)
    {
        xPos = XPos;
        yPos = YPos;
        type = GamePieceType.Box;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = YPos;
        animator = GetComponent<Animator>();
    }



    /// <summary>
    /// This function is called when a breakable box is clicked.
    /// </summary>
    public void ClickedBox()
    {
        if (!board.CanTakePlayerInput)    // if user input is not allowed returns
            return;
        board.CanTakePlayerInput = false; // Blocks the user input
        StartCoroutine(ClickedBoxRoutine());  //Starts coroutine to spread operation between frames
    }

    public IEnumerator ClickedBoxRoutine()
    {
        board.DecreaseMoveLeft();   // decreases the move count

        GamePieceType HasPriority = GamePieceType.Box;  //When a box breaks and a balloon explodes at the same time only ballonn sound will be played
                                                        // I am using Has priority for this situation
        List<GameObject> boxListToDestroy = board.matchesList[listIndex];
        // we get the list the box is in from matches list
        List<GameObject> baloonsToDestroy = board.FindBalloonsToDestroy(boxListToDestroy); // we get the balloons that will explode from this match
        HasPriority = board.FindGamePieceThatHasPriority(baloonsToDestroy); //get the Game Piece type that has higher priority


        yield return null; // wait a frame


        if (adjacentCount > 4) // if a rocket will be instantiated
        {
            board.DestroyGamePieces(baloonsToDestroy, HasPriority); // we destroy balloons first
            yield return null; // wait a frame
            StartCoroutine(BlackHoleRoutine(xPos, yPos)); // instantiate the black hole effect on match's boxes
        }
        else
        {
            boxListToDestroy = boxListToDestroy.Union(baloonsToDestroy).ToList(); // merging lists to destroy at one function call
            board.DestroyGamePieces(boxListToDestroy, HasPriority); // destroy game pieces
            board.FillTheBoardWithCubesAbove(); // fill the board
        }
    }

    // Input Handling
    private void OnMouseDown()
    {
        if (adjacentCount > 1)
        {
            ClickedBox();
        }
    }

    IEnumerator BlackHoleRoutine(int XPos, int YPos)
    {
        // getting boxes for animation like black hole
        List<GameObject> boxes = board.matchesList[board.gamePieces[XPos, YPos].GetComponent<ICanMakeMatch>().GetListIndex()];


        for (int i = 0; i < boxes.Count; i++) 
        {
            board.gamePieces[boxes[i].GetComponent<IGamePiece>().GetXPos(), boxes[i].GetComponent<IGamePiece>().GetYPos()] = null; // make positions null on grid
           
            //move the boxes to the clicked box for black hole effect
            IGamePiece IgamePiecebox = boxes[i].GetComponent<IGamePiece>();
            IgamePiecebox.SetXPos(XPos);
            IgamePiecebox.SetYPos(YPos);

            boxes[i].GetComponent<Box>().StartCreatingRocketAnimation(); // make animation start on boxes
        }
        yield return new WaitForSeconds(1f); // wait for 1 sec for animation
        board.DestroyBoxesThenInstantiateRocketAt(xPos, yPos, boxes); // destroy boxes then instantiate rocket

    }

    /// <summary>
    /// Used in the find matches function, If the box can make a match with the other object it returns true else false
    /// </summary>
    public bool CheckMatch(GameObject FirstObject, GameObject SecondObject)
    {
        if(FirstObject.GetComponent<IGamePiece>().GetGamePieceType() == GamePieceType.Box &&
           SecondObject.GetComponent<IGamePiece>().GetGamePieceType() == GamePieceType.Box &&
           FirstObject.GetComponent<IHasColor>().GetColor() == SecondObject.GetComponent<IHasColor>().GetColor())
        {
            return true;
        }
        return false;
    }


}
