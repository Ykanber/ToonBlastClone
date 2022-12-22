using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentCreator
{ // For creating environment 
    void InitializeGrounds(int Width, int Height);
    void InitializeBorders(int Width, int Height);
}
