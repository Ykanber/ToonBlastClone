using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCreator : MonoBehaviour, IEnvironmentCreator
{
    // I am instantiating ground blocks to understand if the duck reached the bottom
    [SerializeField]
    GameObject groundPrefab;
    GameObject[] groundArray;

    // Border that wraps the blocks
    [SerializeField]
    GameObject borders;

    /// <summary>
    /// Is used for instantiating Ground prefabs to understand if the duck reached bottom
    /// </summary>
    public void InitializeGrounds(int Width, int Height)
    {
        groundArray = new GameObject[Width];
        for (int i = 0; i < Width; i++)
        {
            GameObject tempGround = Instantiate(groundPrefab, new Vector2(i, 0), Quaternion.identity);
            tempGround.transform.parent = this.gameObject.transform;
            groundArray[i] = tempGround;
        }

    }


    /// <summary>
    /// Used to scale the borders so it wraps the boxes
    /// I found the values while testing
    /// </summary>
    public void InitializeBorders(int Width, int Height)
    {
        borders = Instantiate(borders, this.gameObject.transform);
        borders.GetComponent<Transform>().position = new Vector2(-0.0962f, -0.0862f);
        borders.GetComponent<SpriteRenderer>().size = new Vector2(Width + 0.19128f, Height + 0.37349f);
    }

}
