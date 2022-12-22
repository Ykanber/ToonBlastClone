using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxExplodeEffect : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(DestroyFX), 0.5f); // after 0.5 seconds destroy itself
    }

    void DestroyFX()
    {
        Destroy(gameObject);
    }
}
