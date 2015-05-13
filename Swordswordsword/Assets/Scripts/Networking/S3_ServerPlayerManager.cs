using UnityEngine;
using System.Collections;

public class S3_ServerPlayerManager : MonoBehaviour {
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
    public GameObject[] SpawnPoints;
    public float[] TimeOffsets;
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
	void Start () {

        TimeOffsets = new float[] { 0f };
        Players = new GameObject[4];
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
