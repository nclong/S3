using UnityEngine;
using System.Collections;

public class S3_ClientCombatInputCollector : MonoBehaviour {
	public GameObject ClientObject;
	private S3client Client;
	public bool dead = false;

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
        if (!dead) {
			AnimatorStateInfo info = combatController.GetAnimatorInfo (0);
        	
			if (Input.GetButton ("Attack") && !info.IsName ("ShoulderSlash") && !characterMovement.dashing) {
                float currentLatency = Client.playerManager.Latencies[Client.PlayerNum];
                //1.5 is because that is the preferred speed, did not want to rekey animation
                combatController.SwingSword( currentLatency );
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
        	
				Client.SendGameMessage (swingMessage);
			} else {
				combatController.StopSwing ();
			}
		} else {
			if( Input.GetButton("Attack") )
			{
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
				
				Client.SendGameMessage (swingMessage);
			}
		}
	}
}
