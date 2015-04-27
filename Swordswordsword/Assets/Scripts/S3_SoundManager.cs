using UnityEngine;
using System.Collections;

public class S3_SoundManager : MonoBehaviour {
    public static AudioSource DashSound;
    public static AudioSource SlashSound;
    public static AudioSource SlashHitSound;
    public static AudioSource GunshotSound;
    public static AudioSource LaserSound;
    public static AudioSource RocketLaunch;
    public static AudioSource RocketLand;

    public AudioSource dashSound;
    public AudioSource slashSound;
    public AudioSource slashHitSound;
    public AudioSource gunshotSound;
    public AudioSource laserSound;
    public AudioSource rocketLaunch;
    public AudioSource rocketLand;

    void Start()
    {
        DashSound = dashSound;
        SlashSound = slashSound;
        SlashHitSound = slashHitSound;
        GunshotSound = gunshotSound;
        LaserSound = laserSound;
        RocketLaunch = rocketLaunch;
        RocketLand = rocketLand;
    }
}
