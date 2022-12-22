using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IInstantiable
{
    int posX;
    int posY;

    bool canHaveBox;

    public int PosX { get => posX; }
    public int PosY { get => posY; }
    public bool CanHaveBox { get => canHaveBox; set => canHaveBox = value; }

    /// <summary>
    /// Initialize the tile
    /// </summary>
    public void InitTile(int X, int Y, bool CanHaveBox)
    {
        posX = X;
        posY = Y;
        canHaveBox = CanHaveBox;
    }

    /// <summary>
    /// Instantiate the tile
    /// </summary>
    public void Instantiate(int XPos, int YPos, Board board, int FallBoxIndex = 0)
    {
        GameObject tile = Instantiate(this.gameObject, new Vector3(XPos, YPos, 0), Quaternion.identity);
        if (tile != null)
        {
            tile.name = string.Format("Tile {0},{1}", XPos, YPos);
            tile.transform.parent = board.transform;
            Tile currentTile = tile.GetComponent<Tile>();
            currentTile.InitTile(XPos, YPos, true);
            board.tiles[XPos, YPos] = currentTile;
        }
    }

}
