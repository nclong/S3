using UnityEngine;
using System.Collections;

public class S3_CombatStateController : MonoBehaviour {
    public GameObject SwordObj;
    public AnimationClip SwordSwingClip;

    private bool slashing = false;
    private Animator swordAnimator;
    private float timer = 0f;
    private S3_CharacterMovement characterMovement;
	// Use this for initialization
	void Start () {
        swordAnimator = SwordObj.GetComponent<Animator>();
        characterMovement = GetComponent<S3_CharacterMovement>();
	}
	
	// Update is called once per frame
	void Update () {
        if( Input.GetButton( "Attack" ) && !slashing && !characterMovement.dashing)
        {
            slashing = true;
            swordAnimator.SetBool("slashing", slashing);
        }

        if( slashing )
        {
            timer += Time.deltaTime;
            if( timer >= SwordSwingClip.length )
            {
                timer = 0f;
                slashing = false;
                swordAnimator.SetBool( "slashing", slashing );
            }
        }  
	}

    public GameObject GetSword()
    {
        return SwordObj;
    }
}
