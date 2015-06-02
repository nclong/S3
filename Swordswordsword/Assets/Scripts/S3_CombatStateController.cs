using UnityEngine;
using System.Collections;

public class S3_CombatStateController : MonoBehaviour {
    public GameObject SwordObj;
    public AnimationClip SwordSwingClip;
    public GameObject RightShoulder;

    private bool slashing = false;
    private Animator swordAnimator;
    private float timer = 0f;
    private S3_CharacterMovement characterMovement;
    private S3_SwordManager swordManager;
	// Use this for initialization
	void Start () {
        swordAnimator = RightShoulder.GetComponent<Animator>();
        characterMovement = GetComponent<S3_CharacterMovement>();
        swordManager = SwordObj.GetComponent<S3_SwordManager>();
	}
	
	// Update is called once per frame
	void Update () {
		AnimatorStateInfo info = swordAnimator.GetCurrentAnimatorStateInfo( 0 );
		if (info.IsName("Base.ShoulderSlash"))
		{
			StopSwing();
		}
	}

    public void SwingSword(float offset)
    {
        AnimatorStateInfo info = swordAnimator.GetCurrentAnimatorStateInfo( 0 );
        float ServerTimeToFinish = info.length * 1.5f;
        float localTimeToFinish = ServerTimeToFinish + offset;
        float localSpeed = localTimeToFinish / info.length;
        swordAnimator.speed = localSpeed;
        swordAnimator.SetBool("slashing", true);

        S3_GunSword gun = SwordObj.GetComponent<S3_GunSword>();
        if( gun != null && gun.enabled )
        {
            gun.FireBullet();
        }

        S3_SwordZooka zooka = SwordObj.GetComponent<S3_SwordZooka>();
        if( zooka != null && zooka.enabled )
        {
            zooka.fireRocket();
        }

        Debug.Log( "Slash Start" );
        Debug.Log( info.IsName( "ShoulderSlash" ) );
    }

    public AnimatorStateInfo GetAnimatorInfo(int x)
    {
        return swordAnimator.GetCurrentAnimatorStateInfo( x );
    }
    public void StopSwing()
    {
        swordAnimator.SetBool( "slashing", false );
    }

    public GameObject GetSword()
    {
        return SwordObj;
    }
}
