using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour,IGamePiece, IHasSortingOrder, IInstantiable, IHasColor
{
    // fields and interface functions are same with box and rocket
    int xPos;
    int yPos;

    [SerializeField]
    GamePieceType type;


    public PieceColor color; // balloon has color

    Board board;

    AudioManager audioManager;

    bool isDestroyed;

    public bool IsDestroyed { get => isDestroyed; set => isDestroyed = value; }



    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(xPos, yPos), Time.deltaTime * 20);
    }


    public int GetXPos()
    {
        return xPos;
    }
    public void SetXPos(int XPos)
    {
        xPos = XPos;
    }

    public int GetYPos()
    {
        return yPos;
    }
    public void SetYPos(int YPos)
    {
        yPos = YPos;
    }
    public GamePieceType GetGamePieceType()
    {
        return type;
    }

    public PieceColor GetColor()
    {
        return color;
    }

    public void SetAudioManager()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlayExplodeSound()
    {
        audioManager.Play(type);
    }

    public void Destroy()
    {
        PlayExplodeSound();
        board.gamePieces[xPos, yPos] = null;
        Destroy(this.gameObject);
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void SetBoard(Board Board)
    {
        board = Board;
    }


    /// <summary>
    /// Instantiates a Balloon at a Y position + Fall offset
    /// Adds to the game pieces array at the X,Y position
    /// Initializes the box
    /// </summary>
    public void Instantiate(int X, int Y, Board Board, int FallBoxIndex = 0)
    {
        GameObject instantiatedBalloonObject = Instantiate(this.gameObject, new Vector3(X, Y + FallBoxIndex, 0), Quaternion.identity);
        if (instantiatedBalloonObject != null)
        {
            instantiatedBalloonObject.name = $"Balloon ({X},{Y})";
            IGamePiece instantiatedBalloon = instantiatedBalloonObject.GetComponent<IGamePiece>();
            instantiatedBalloon.Initialize(X, Y);
            instantiatedBalloon.SetBoard(Board);
            Board.gamePieces[X, Y] = instantiatedBalloonObject;
            instantiatedBalloonObject.transform.parent = Board.transform;
            instantiatedBalloonObject.GetComponent<IGamePiece>().SetAudioManager();
        }
    }
    public void Initialize(int XPos, int YPos)
    {
        xPos = XPos;
        yPos = YPos;
        type = GamePieceType.Balloon;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = YPos;
    }


    /// <summary>
    /// When destructive rocket collides with the balloon, balloon destroys itself
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDestroyed != true)
        {
            IsDestroyed = true;
            board.DestroyGamePiece(this.gameObject);
        }
    }
}
