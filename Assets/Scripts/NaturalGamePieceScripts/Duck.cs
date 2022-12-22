using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour, IHasSortingOrder, IInstantiable, IGamePiece
{
    // fields and interface functions are same with box and rocket

    int xPos;
    int yPos;

    [SerializeField]
    GamePieceType type;


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
    /// Instantiates the duck
    /// </summary>
    public void Instantiate(int X, int Y, Board Board, int FallBoxIndex = 0)
    {
        GameObject instantiatedDuckObject = Instantiate(this.gameObject, new Vector3(X, Y + FallBoxIndex, 0), Quaternion.identity);
        if (instantiatedDuckObject != null)
        {
            instantiatedDuckObject.name = $"Duck ({X},{Y})";
            IGamePiece instantiatedDuck = instantiatedDuckObject.GetComponent<IGamePiece>();
            instantiatedDuck.Initialize(X, Y);
            instantiatedDuck.SetBoard(Board);
            Board.gamePieces[X, Y] = instantiatedDuckObject;
            instantiatedDuckObject.transform.parent = Board.transform;
            instantiatedDuckObject.GetComponent<IGamePiece>().SetAudioManager();
        }
    }
    public void Initialize(int XPos, int YPos)
    {
        xPos = XPos;
        yPos = YPos;
        type = GamePieceType.Duck;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = YPos;
    }

    /// <summary>
    /// When it collides with the ground prefab it flys
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDestroyed == false && collision.gameObject.tag == "Ground")
        {
            IsDestroyed = true;
            FlyTheDuck();
        }
    }
    public void FlyTheDuck()
    {
        StartCoroutine(FlyTheDuckRoutine());
    }
    /// <summary>
    /// It waits for 0.3 seconds then destroys itself and new piece appears on top
    /// </summary>
    IEnumerator FlyTheDuckRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        board.gamePieces[xPos, yPos] = null;
        Destroy();
        board.FillTheBoardWithCubesAbove(); // fills the hole duck made
    }
}
