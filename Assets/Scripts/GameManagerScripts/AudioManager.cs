using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // audio clips
    [SerializeField]
    AudioClip boxExplodeClip;
    [SerializeField]
    AudioClip BalloonnExplodeClip;
    [SerializeField]
    AudioClip CubeCollectClip;
    [SerializeField]
    AudioClip DuckDropClip;

    AudioSource audioSource; // reference to audio source

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(GamePieceType audioTypeToPlay) //playa explosuion sounds
    {
        switch (audioTypeToPlay)
        {
            case GamePieceType.Box:
                audioSource.PlayOneShot(boxExplodeClip);
                break;

            case GamePieceType.Balloon:
                audioSource.PlayOneShot(BalloonnExplodeClip);
                break;
            case GamePieceType.Duck:
                audioSource.PlayOneShot(DuckDropClip);
                break;
        }
    }

    public void PlayGoalCollectSound() // play collect sound
    {
        audioSource.PlayOneShot(CubeCollectClip);
    }

}
