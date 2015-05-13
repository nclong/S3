using UnityEngine;
using System.Collections;

public class S3_GameManager : MonoBehaviour {

    public float DeathPauseLength;
    public int PointsToWin;
    private S3_ServerPlayerManager playerManager;
    private int[] PlayerScores;
    private float[] DeathTimers;
    private bool[] DeathStarted;
	// Use this for initialization
	void Start () {
        playerManager = GetComponent<S3_ServerPlayerManager>();
        PlayerScores = new int[4] { 0,0,0,0 };
        DeathTimers = new float[4] { 0f,0f,0f,0f };
        DeathStarted = new bool[4] { false, false, false, false };
	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < 4; ++i)
        {
            if(DeathStarted[i])
            {
                DeathTimers[i] += Time.deltaTime;
            }

            if( DeathTimers[i] >= DeathPauseLength )
            {
                playerManager.Players[i].transform.position =
                    playerManager.SpawnPoints[i].transform.position;
                DeathTimers[i] = 0f;
                DeathStarted[i] = false;
            }
        }
	
	}

    public void StartDeath(int attackerNum, int victimNum)
    {
        DeathStarted[victimNum] = true;
        PlayerScores[attackerNum] += 1;
        for(int i = 0; i < 4; ++i)
        {
            if( PlayerScores[i] >= PointsToWin )
            {
                //End Game Stuff
            }
        }
    }
}
