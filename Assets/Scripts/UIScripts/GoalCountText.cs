using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalCountText : MonoBehaviour
{
    TextMeshProUGUI countText;

    int count;

    private void Awake()
    {
        countText = GetComponent<TextMeshProUGUI>(); 
    }

    public void SetCountText(int Count) // sets the count then the text
    {
        count = Count;
        countText.text = count.ToString();
    }

    public void DecreaseCountText(int Count) // decreases the count then sets the text
    {
        Count = count - Count;
        if(Count < 0)
        {
            Count = 0;
        }
        SetCountText(Count);
    }

}
