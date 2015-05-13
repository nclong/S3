using UnityEngine;
using System.Collections;

public class S3_ClientCombatInputCollector : MonoBehaviour {


    private S3_CombatStateController combatController;
    private S3_CharacterMovement characterMovement;
	// Use this for initialization
	void Start () {
        combatController = GetComponent<S3_CombatStateController>();
        characterMovement = GetComponent<S3_CharacterMovement>();
    }
	
	// Update is called once per frame
	void Update () {
        AnimatorStateInfo info = combatController.GetAnimatorInfo( 0 );

        if( Input.GetButton( "Attack" ) && !info.IsName("ShoulderSlash") && !characterMovement.dashing )
        {
            combatController.SwingSword();
        }
        else
        {
            combatController.StopSwing();
        }
	}
}
