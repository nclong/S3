using UnityEngine;
using System.Collections;

public class S3_GameManager : MonoBehaviour {

    public float DeathPauseLength;
    public int PointsToWin;
	public GameObject ServerObject;
    private S3_ServerPlayerManager playerManager;
	private S3server server;
    public int[] PlayerScores;
	// Use this for initialization
	void Start () {
        playerManager = GetComponent<S3_ServerPlayerManager>();
		server = ServerObject.GetComponent<S3server> ();
        PlayerScores = new int[4] { 0,0,0,0 };
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < 4; i++) {
			if( PlayerScores[i] >= PointsToWin )
			{
				for(int j = 0; j < playerManager.CurrentPlayers; ++j)
				{
					S3_ServerConnectResponseData responseData = new S3_ServerConnectResponseData
					{
						PosX = playerManager.SpawnPoints[j].transform.position.x,
						PosY = playerManager.SpawnPoints[j].transform.position.y,
						acceptance = false
					};
					S3_GameMessage SpawnMessage = new S3_GameMessage
					{
						SendTime = Time.time,
						PlayerNum = (byte)j,
						MessageData = responseData,
						MessageType = S3_GameMessageType.ServerConnectResponse
					};
					server.SendGameMessage(SpawnMessage);
				}
				PlayerScores = new int[4] { 0,0,0,0 };
				break;
			}
		}
	}
}
