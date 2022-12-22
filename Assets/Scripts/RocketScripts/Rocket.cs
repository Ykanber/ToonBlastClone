
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour,IGamePiece,IInstantiable,ICanMakeMatch
{
    //rocket prefabs that are instantiated when rocket is clicked

    [SerializeField]
    DestructiveRocket topRocket;

    [SerializeField]
    DestructiveRocket bottomRocket;

    [SerializeField]
    DestructiveRocket leftRocket;

    [SerializeField]
    DestructiveRocket rightRocket;


    int xPos; // X Position
    int yPos;// Y position of the Rocket

    [SerializeField]
    GamePieceType type; // Type of the rocket


    int listIndex = -1; // List index of the Rocket, default is -1

    [SerializeField]
    int adjacentCount = 1; // Adjacent count of the Rocket , if no matches then it is 1

    bool isDestroyed; // used on on trigger 2D to prevent double explosions

    [SerializeField]
    Board board; // reference to board

    AudioManager audioManager; // reference to audio manager to play audio

    Animator animator; // animator to play animation when a Double rocket is Created

    [SerializeField]
    private RocketType rocketType; // topdown or leftright rocket

    public RocketType RocketType { get => rocketType;}

    public void StartCreatingRocketAnimation()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        animator.SetBool("IsConstructingARocket", true); // starts the animation
    }


    // Makes Rockets move towards the position on grid
    private void Update()
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
    public GamePieceType GetGamePieceType() // Getter for game piece type
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
//        audioManager.Play(type); rocket do not have a sound right now
    }

    /// <summary>
    /// Destroys the rocket
    /// </summary>
    public void Destroy()
    {
        board.gamePieces[xPos, yPos] = null;
        Destroy(this.gameObject);
    }

    public void SetBoard(Board Board) // Setter For Board
    {
        board = Board;
    }

    /// <summary>
    /// Based on the rocket type and position it is called it creates 2 destructive rockets to clear rows and colums
    /// </summary>
    public void ExplodeDestructiveRocket(RocketType Type, int XPos, int YPos)
    {
        if(Type == RocketType.LeftRight)
        {
            Instantiate(leftRocket, new Vector2(XPos, YPos), Quaternion.identity);
            Instantiate(rightRocket, new Vector2(XPos, YPos), Quaternion.identity);
        }
        else
        {
            Instantiate(topRocket, new Vector2(XPos, YPos),Quaternion.identity);
            Instantiate(bottomRocket, new Vector2(XPos, YPos), Quaternion.identity);
        }
    }

    /// <summary>
    /// initializes the rocket
    /// </summary>
    public void Initialize(int XPos, int YPos)
    {
        xPos = XPos;
        yPos = YPos;
        type = GamePieceType.Rocket;
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Instantiates the rocket 
    /// </summary>
    public void Instantiate(int XPos, int YPos, Board Board, int FallBoxIndex = 0)
    {
        GameObject instantiatedRocketObject = Instantiate(this.gameObject, new Vector2(XPos, YPos + FallBoxIndex), Quaternion.identity);
        if (instantiatedRocketObject != null)
        {
            instantiatedRocketObject.name = $"Rocket ({XPos},{YPos})";
            instantiatedRocketObject.GetComponent<IGamePiece>().Initialize(XPos, YPos);
            instantiatedRocketObject.GetComponent<IGamePiece>().SetBoard(Board);
            Board.gamePieces[XPos, YPos] = instantiatedRocketObject;
            instantiatedRocketObject.transform.parent = Board.transform;
        }
    }

    /// <summary>
    /// If a rocket is clicked this function works
    /// </summary>
    public void ClickedRocket()
    {
        if (!board.CanTakePlayerInput) // if player input is disabled return 
            return;
        //decrease move count
        board.CanTakePlayerInput = false;
        board.DecreaseMoveLeft(); // decrease move left
        
        if(adjacentCount > 1) // if the rocket has a match
        {
            StartCoroutine(BlackHoleRoutine(xPos,yPos));
        }
        else
        {
            ExplodeRocket(); // if there is only 1 rocket
        }
    }

    /// <summary>
    /// Creates the destructive rockets and starts Rocket routine on board if it is not already started
    /// </summary>
    public void ExplodeRocket()
    {
        board.gamePieces[xPos, yPos] = null;
        ExplodeDestructiveRocket(rocketType, xPos, yPos);
        board.ControlRocketCoroutine(this);
    }

    /// <summary>
    /// Explodes double rockets
    /// </summary>
    public void ExplodeDoubleRocket(int XPos,int YPos)
    {
        ExplodeDestructiveRocket(RocketType.LeftRight, XPos, YPos);
        ExplodeDestructiveRocket(RocketType.TopBottom, XPos, YPos);
    }

    /// <summary>
    /// if rockets has a match this routine is called
    /// first it creates a blackhole,  then it creates double rocket
    /// </summary>
    /// <returns></returns>
    IEnumerator BlackHoleRoutine(int XPos, int YPos)
    {
        // getting boxes for animation like black hole
        List<GameObject> rockets = board.matchesList[board.gamePieces[XPos, YPos].GetComponent<ICanMakeMatch>().GetListIndex()];

        for (int i = 0; i < rockets.Count; i++)
        {
            board.gamePieces[rockets[i].GetComponent<IGamePiece>().GetXPos(), rockets[i].GetComponent<IGamePiece>().GetYPos()] = null; // makes corresponding points null on array
            
            // creates blackhole effect
            IGamePiece IgamePiecebox = rockets[i].GetComponent<IGamePiece>(); 
            IgamePiecebox.SetXPos(XPos);
            IgamePiecebox.SetYPos(YPos);

            rockets[i].GetComponent<Rocket>().StartCreatingRocketAnimation(); // rockets start animating

            
        }
        yield return new WaitForSeconds(1f);
        ExplodeDoubleRocket(XPos,YPos); // create double rocket
        while (rockets.Count != 0) 
        {
            board.ControlRocketCoroutine(rockets[0].GetComponent<Rocket>());
            rockets.RemoveAt(0);
        }
    }

    private void OnMouseDown()
    {
         ClickedRocket();
    }
    
    /// <summary>
    /// If Destructive rockets collides with the normal rockets they explode
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isDestroyed != true)
        {
            isDestroyed = true;
            ExplodeRocket();
        }
    }
    
    /// <summary>
    /// Checks for two rocket matches
    /// In future it can ve used for rocket - bomb or different matches it only returns false when it is making comparison with a box 
    /// </summary>
    /// <returns></returns>
    public bool CheckMatch(GameObject FirstObject, GameObject SecondObject)
    {
        if (FirstObject.GetComponent<IGamePiece>().GetGamePieceType() != GamePieceType.Box &&
        SecondObject.GetComponent<IGamePiece>().GetGamePieceType() != GamePieceType.Box)
        {
            return true;
        }
        return false;
    }


}
