using UnityEngine;
using System.Collections;

public class S3_CombatStateController : MonoBehaviour {
    public GameObject SwordObj;
    public AnimationClip SwordSwingClip;
    public GameObject RightShoulder;

    private Animator swordAnimator;
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
        swordAnimator.SetBool("slashing", true);
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
