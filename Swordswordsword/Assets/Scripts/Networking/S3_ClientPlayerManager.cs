using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class S3_ClientPlayerManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    private const int Player_Max = 4;
    private int currentPlayerCount = 0;
    private S3client client;
    public int CurrentPlayers
    {
        get
        {
            return currentPlayerCount;
        }
    }
    public string[] PlayerNames;
    public float[] Latencies;
    public int[] Scores;
    public GameObject[] Players
    {
        get;
        private set;
    }

    public bool HasRoom
    {
        get
        {
            return currentPlayerCount < Player_Max;
        }
    }

    // Use this for initialization
    void Start()
    {
        client = GetComponent<S3client>();
        Players = new GameObject[4];
        Latencies = new float[4]; //{ new S3_LatencyQueue(), new S3_LatencyQueue(), new S3_LatencyQueue(), new S3_LatencyQueue();
        PlayerNames = new string[4];
        Scores = new int[4];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int CreatePlayer(float x, float y)
    {
        if (currentPlayerCount >= Player_Max)
        {
            return -1;
        }

        if( currentPlayerCount == (int)(client.PlayerNum))
        {
            currentPlayerCount++;
        }
        Players[currentPlayerCount] = Instantiate<GameObject>(PlayerPrefab);
        S3_PlayerProperties playerProps = Players[currentPlayerCount].GetComponent<S3_PlayerProperties>();
        if (playerProps != null)
        {
            playerProps.SetPlayerNum(currentPlayerCount);
            playerProps.CheckIfServer();
        }
        Players[currentPlayerCount].transform.position = new Vector3( x, y );
        PlayerNames[currentPlayerCount] = string.Empty;
        Latencies[currentPlayerCount] = new float();
        return currentPlayerCount++;
    }

    public float calculateLatency(int PlayerNum)
    {
        return Latencies[PlayerNum];
    }

    public int GetPing(int PlayerNum)
    {
        return Mathf.RoundToInt(Latencies[PlayerNum]) * 1000;
    }

    public void RemovePlayer(int PlayerToRemove)
    {
        Destroy(Players[PlayerToRemove]);
        bool toRemoveReached = false;
        for (int i = 0; i < currentPlayerCount; ++i)
        {
            if (i == PlayerToRemove)
            {
                toRemoveReached = true;
            }

            if (toRemoveReached && i + 1 < currentPlayerCount)
            {
                if( i + 1 == client.PlayerNum )
                {
                    client.PlayerNum--;
                }
                Latencies[i] = Latencies[i + 1];
                Players[i] = Players[i + 1];
                PlayerNames[i] = PlayerNames[i + 1];
            }
        }

        currentPlayerCount--;
    }
}
