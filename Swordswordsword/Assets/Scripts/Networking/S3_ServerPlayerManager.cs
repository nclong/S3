using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S3_ServerPlayerManager : MonoBehaviour {
	private const float CLIENT_TIMEOUT = 60000;

	public GameObject PlayerPrefab;
    private const int Player_Max = 4;
    private int currentPlayerCount = 0;
	private int firstAvailableSlot = 0;

    public int CurrentPlayers
    {
        get
        {
            return currentPlayerCount;
        }
    }
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
		Latencies = new S3_LatencyQueue[4];
	}
	
	// Update is called once per frame
	void Update () {
		List<int> playersTimedout = new List<int> ();
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

    public int CreatePlayer()
    {
        if( currentPlayerCount >= Player_Max )
        {
            return -1;
        }
        Players[currentPlayerCount] = Instantiate<GameObject>( PlayerPrefab );
        Players[currentPlayerCount].transform.position = SpawnPoints[currentPlayerCount].transform.position;

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
		Latencies [PlayerNum].GetMeanI ();
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
				Players[i] = Players[i+1];
				firstAvailableSlot = i+1;
			}
			else if (toRemoveReached && i+1 >= currentPlayerCount)
			{
				firstAvailableSlot = i;
			}
		}

		currentPlayerCount--;
	}
}
