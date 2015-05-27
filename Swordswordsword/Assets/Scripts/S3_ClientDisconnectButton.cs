using UnityEngine;
using System.Collections;

public class S3_ClientDisconnectButton : MonoBehaviour {

    private Rigidbody2D PlayerRB;
    private S3client Client;

    void Start()
    {
        Client = GameObject.Find ("ClientObject").GetComponent<S3client>();
    }

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			S3_ClientDisconnectMsg data = new S3_ClientDisconnectMsg();
            S3_GameMessage DCMsg = new S3_GameMessage
            {
                PlayerNum = (byte)(Client.PlayerNum),
                SendTime = S3_ServerTime.ServerTime,
                MessageType = S3_GameMessageType.ClientDisconnectMsg,
				MessageData = data
            };
			Client.SendGameMessage(DCMsg);
        }
	}


}
