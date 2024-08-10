using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Audio : MonoBehaviour
{
    [Header("---------Audio Source------------)")]
    public AudioSource SFXSource;
    [Header("---------Audio Clip Player------------)")]
    public AudioClip PickUp;
    public AudioClip Get;
    public static Audio Instance => instance;
    private static Audio instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

        if (SFXSource != null)
        {
            SFXSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);

    }


}
