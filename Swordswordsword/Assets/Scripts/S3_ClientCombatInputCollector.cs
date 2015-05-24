using UnityEngine;
using System.Collections;

public class S3_ClientCombatInputCollector : MonoBehaviour {
	public GameObject ClientObject;
	private S3client Client;

    private S3_CombatStateController combatController;
    private S3_CharacterMovement characterMovement;
	// Use this for initialization
	void Start () {
		Client = ClientObject.GetComponent<S3client> ();
        combatController = GetComponent<S3_CombatStateController>();
        characterMovement = GetComponent<S3_CharacterMovement>();
    }
	
	// Update is called once per frame
	void Update () {
        AnimatorStateInfo info = combatController.GetAnimatorInfo( 0 );

        if( Input.GetButton( "Attack" ) && !info.IsName("ShoulderSlash") && !characterMovement.dashing )
        {
            combatController.SwingSword();
			S3_ClientSwingData swingData = new S3_ClientSwingData
			{
				animateSpeed = 1.0f
			};
			S3_GameMessage swingMessage = new S3_GameMessage
			{
				SendTime = S3_ServerTime.ServerTime,
				PlayerNum = (byte)(Client.PlayerNum),
				MessageData = swingData,
				MessageType = S3_GameMessageType.ClientSwing
			};

			Client.SendGameMessage( swingMessage );
        }
        else
        {
            combatController.StopSwing();
        }
	}
}
