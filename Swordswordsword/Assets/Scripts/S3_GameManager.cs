using UnityEngine;
using System.Collections;

public class S3_GameManager : MonoBehaviour {

    public float DeathPauseLength;
    public int PointsToWin;
    private S3_ServerPlayerManager playerManager;
    public int[] PlayerScores;
	// Use this for initialization
	void Start () {
        playerManager = GetComponent<S3_ServerPlayerManager>();
        PlayerScores = new int[4] { 0,0,0,0 };
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < 4; i++) {
			if( PlayerScores[i] >= PointsToWin )
			{
				//Do Game Over stuff
				Debug.Log("Game Over");
			}
		}
	}
}
