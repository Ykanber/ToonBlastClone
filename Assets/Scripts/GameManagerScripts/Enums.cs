using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// color enum
public enum PieceColor
{
    Red,
    Blue,
    Green,
    Purple,
    Yellow,
    General
}
// game piece type enum
public enum GamePieceType
{
    Box,
    Rocket,
    Balloon,
    Duck
}
// rocket type enum
public enum RocketType
{
    LeftRight,
    TopBottom
}
// destructive rocket type enum, decides move direction
public enum DestructiveRocketType
{
    Left,
    Right,
    Top,
    Bottom
}