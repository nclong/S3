using UnityEngine;
using System.Collections;

public class S3_RotDeadReckoning : MonoBehaviour {
    public GameObject ClientObject;
    public GameObject Player;
    public float Threshold;
    public float Angle;
    private S3client client;
	// Use this for initialization
	void Start () {
        client = ClientObject.GetComponent<S3client>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
        if( Mathf.Abs(Player.transform.eulerAngles.z - Angle) > Threshold)
        {
            UpdateDR();
        }
	}

    void UpdateDR()
    {
        Angle = Player.transform.eulerAngles.z;
        if (client.PlayerNum != 255 && client.PlayerNum != -1 )
        {
            S3_ClientRotDRData data = new S3_ClientRotDRData
            {
                Angle = Angle
            };
            S3_GameMessage message = new S3_GameMessage
            {
                PlayerNum = (byte)(client.PlayerNum),
                SendTime = S3_ServerTime.ServerTime,
                MessageType = S3_GameMessageType.ClientRotDR,
                MessageData = data
            };

            client.SendGameMessage(message); 
        }
    }
}
