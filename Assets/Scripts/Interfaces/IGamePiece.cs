using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePiece
{
    // simple game piece operations
    int GetXPos();
    void SetXPos(int XPos);
    int GetYPos();
    void SetYPos(int XPos);

    GamePieceType GetGamePieceType();    

    void Initialize(int X, int Y);

    void SetAudioManager();
    void PlayExplodeSound();

    void Destroy();

    void SetBoard(Board Board);
    
}
