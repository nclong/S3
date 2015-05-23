using UnityEngine;
using System.Collections;

public class S3_DeadReckoning : MonoBehaviour {
    public GameObject Player;
    public GameObject ClientObject;
    public float Threshold;
    public Vector2 Velocity;

    private Rigidbody2D PlayerRB;
    private S3client Client;

	// Use this for initialization
	void Start () {
        PlayerRB = Player.GetComponent<Rigidbody2D>();
        Client = ClientObject.GetComponent<S3client>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if( Mathf.Abs(PlayerRB.velocity.magnitude - Velocity.magnitude) > Threshold)
        {
            UpdateDR();
        }
	}

    private void UpdateDR()
    {
        Velocity = PlayerRB.velocity;
        transform.position = Player.transform.position;
        transform.eulerAngles = new Vector3( 0f, 0f, Mathf.Atan2( Velocity.y, Velocity.x ) * Mathf.Rad2Deg );
        if ( Client.PlayerNum != 255)
        {
            S3_ClientPosDRData data = new S3_ClientPosDRData
            {
                DRPosX = transform.position.x,
                DRPosY = transform.position.y,
                DRVelX = PlayerRB.velocity.x,
                DRVelY = PlayerRB.velocity.y
            };
            S3_GameMessage message = new S3_GameMessage
            {
                PlayerNum = (byte)(Client.PlayerNum),
                SendTime = S3_ServerTime.ServerTime,
                MessageType = S3_GameMessageType.ClientPosDR,
                MessageData = data
            };
            Client.SendGameMessage(message); 
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if( collider.gameObject == Player )
        {
            UpdateDR();
        }
    }
}
