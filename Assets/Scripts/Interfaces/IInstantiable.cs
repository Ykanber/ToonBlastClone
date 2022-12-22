using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInstantiable 
{
    void Instantiate(int XPos, int YPos,Board board, int FallBoxIndex = 0); //I think it can be used to decide if a piece can be dropped from above,
                                                                            //I can drop all the game pieces(bot tiles) that inherit instantiable interface from above
                                                                            // I can instantiate them at the beginning too
}
