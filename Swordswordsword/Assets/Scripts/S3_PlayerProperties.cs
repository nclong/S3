using UnityEngine;
using System.Collections;

public class S3_PlayerProperties : MonoBehaviour {
    public int StartingHealth;
    public GameObject BloodEmitter;
    public string PlayerName;
	public GameObject ServerObject;
    private int currentHealth;
	public int PlayerNumber;
	public bool dead;
	private bool IsServer;
	// Use this for initialization
	void Start () {
        currentHealth = StartingHealth;
		dead = false;
		IsServer = ServerObject != null;
	}
	
	// Update is called once per frame
	void Update () {
	}
	public void CheckIfServer()
	{
		IsServer = ServerObject != null;
	}
    public void TakeDamage()
    {
		if (IsServer) {
			dead = true;
			S3_ServerPlayerDiedData data = new S3_ServerPlayerDiedData
			{
				PlayerNum = (byte)PlayerNumber
			};
			for(int i = 0; i < ServerObject.GetComponent<S3server>().playerManager.CurrentPlayers; ++i)
			{
				S3_GameMessage message = new S3_GameMessage
				{
					PlayerNum = (byte)i,
					SendTime = Time.time,
					MessageData = data,
					MessageType = S3_GameMessageType.ServerPlayerDied
				};

				ServerObject.GetComponent<S3server>().SendGameMessage(message);
			}

			Debug.Log ("Dead Player.");
		} 
    }

    public GameObject GetBloodEmittier()
    {
        return BloodEmitter;
    }

    private void Die()
    {
        Debug.Log(PlayerName + " has died.");
    }

	public void SetPlayerNum(int x)
	{
		PlayerNumber = x;
	}
}
