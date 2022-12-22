using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanMakeMatch 
{
    // if an object can make a match it needs to have list index and adjacent count and also a function to understand if there is a match
    void SetListIndex(int Index);
    int GetListIndex();

    void SetAdjacentCount();

    public bool CheckMatch(GameObject FirstObject, GameObject SecondObject);
}
