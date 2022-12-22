using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasSortingOrder
{
    public SpriteRenderer GetSpriteRenderer(); // sorting order is important to order the rows correctly upper ones needs to have higher sorting orders, rockets are excluded because it's
  //  childs have the sprite renderer
}
