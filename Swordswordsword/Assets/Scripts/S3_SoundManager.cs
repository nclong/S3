using UnityEngine;
using System.Collections;

public class S3_SoundManager : MonoBehaviour {
    public static AudioSource DashSound;
    public static AudioSource SlashSound;
    public static AudioSource SlashHitSound;

    public AudioSource dashSound;
    public AudioSource slashSound;
    public AudioSource slashHitSound;

    void Start()
    {
        DashSound = dashSound;
        SlashSound = slashSound;
        SlashHitSound = slashHitSound;
    }
}
