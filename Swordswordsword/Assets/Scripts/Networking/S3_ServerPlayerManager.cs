using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class S3_ServerPlayerManager : MonoBehaviour {
	private const float CLIENT_TIMEOUT = 60000;
	public GameObject ServerObject;
	public GameObject PlayerPrefab;
    private const int Player_Max = 4;
    private int currentPlayerCount = 0;

    public int CurrentPlayers
    {
        get
        {
            return currentPlayerCount;
        }
    }
	public IPEndPoint[] PlayerEndPoints;
	public string[] PlayerNames;
	public bool[] TimeRequestSent;
    public GameObject[] SpawnPoints;
    public float[] TimeOffsets;
	public S3_LatencyQueue[] Latencies;
    public GameObject[] Players
    {
        get;
        private set;
    }

	public float[] TimeoutCounter;

    public bool HasRoom
    {
        get
        {
            return currentPlayerCount < Player_Max;
        }
    }

	// Use this for initialization
	void Start () {

		TimeOffsets = new float[] {0f, 0f, 0f, 0f};
		TimeoutCounter = new float[] {0f, 0f, 0f, 0f};
        Players = new GameObject[4];
		Latencies = new S3_LatencyQueue[4]; //{ new S3_LatencyQueue(), new S3_LatencyQueue(), new S3_LatencyQueue(), new S3_LatencyQueue();
		PlayerEndPoints = new IPEndPoint[4];
		PlayerNames = new string[4];
		TimeRequestSent = new bool[] {true, true, true, true};
	}
	
	// Update is called once per frame
	void Update () {
        List<int> playersTimedout = new List<int>();
		for (int i = 0; i < currentPlayerCount; ++i) {
			TimeoutCounter[i] += Time.deltaTime;
			if (TimeoutCounter[i] >= CLIENT_TIMEOUT) {
				playersTimedout.Add (i);
			}
		}

		foreach (int i in playersTimedout) {
			RemovePlayer(i);
		}
	
	}

    public int CreatePlayer(IPEndPoint PlayerEndPoint)
    {
        if( currentPlayerCount >= Player_Max )
        {
            return -1;
        }
        Players[currentPlayerCount] = Instantiate<GameObject>( PlayerPrefab );
		S3_PlayerProperties playerProps = Players [currentPlayerCount].GetComponent<S3_PlayerProperties> ();
		if (playerProps != null) {
			playerProps.SetPlayerNum(currentPlayerCount);
			playerProps.ServerObject = ServerObject;
			playerProps.CheckIfServer();
		}
        Players[currentPlayerCount].transform.position = SpawnPoints[currentPlayerCount].transform.position;
		TimeOffsets [currentPlayerCount] = 0f;
		TimeoutCounter [currentPlayerCount] = 0f;
		PlayerEndPoints [currentPlayerCount] = PlayerEndPoint;
		PlayerNames [currentPlayerCount] = string.Empty;
		TimeRequestSent [currentPlayerCount] = true;
		Latencies [currentPlayerCount] = new S3_LatencyQueue ();
		return currentPlayerCount++;
    }

    public float calculateLatency(int PlayerNum)
    {
		return Latencies[PlayerNum].GetMeanF();
    }

	public void AddLatency(int PlayerNum, float offset)
	{
		Latencies [PlayerNum].Enqueue (offset);
	}

	public int GetPing(int PlayerNum)
	{
		return Latencies [PlayerNum].GetMeanI ();
	}

	public void RemovePlayer(int PlayerToRemove)
	{
		Destroy (Players [PlayerToRemove]);
		bool toRemoveReached = false;
		for(int i = 0; i < currentPlayerCount; ++i)
		{
			if( i == PlayerToRemove )
			{
				toRemoveReached = true;
			}

			if (toRemoveReached && i+1 < currentPlayerCount)
			{
				TimeOffsets[i] = TimeOffsets[i+1];
				TimeoutCounter[i] = TimeoutCounter[i+1];
				Latencies[i] = Latencies[i+1];
				Players[i] = Players[i+1];
				PlayerEndPoints[i] = PlayerEndPoints[i+1];
				PlayerNames[i] = PlayerNames[i+1];
				TimeRequestSent[i] = TimeRequestSent[i+1];
			}
		}

		currentPlayerCount--;
	}
}
