using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_MiscController : MonoBehaviour {
    public CanvasGroup ScoreBoard;
	public Text[] Names;
	public Text[] Scores;
	public Text[] Lag;
    private S3_ServerPlayerManager playerManager;
    private S3_ClientPlayerManager cPlayerManager;

	private Text[] EmptyStrings;

	// Use this for initialization
	void Start ()
    {
        if (GameObject.Find("ServerPlayerManager"))
        {
            playerManager = GameObject.Find("ServerPlayerManager").GetComponent<S3_ServerPlayerManager>();
        }
        else
        {
            playerManager = null;
            cPlayerManager = GameObject.Find("ClientPlayerManager").GetComponent<S3_ClientPlayerManager>();
        }
        ScoreBoard.alpha = 0.0f;
		for(int i = 0; i < 4; ++i)
		{
			Names[i].text = Scores[i].text = Lag[i].text = string.Empty;
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (playerManager != null)
            {
                if (ScoreBoard.alpha == 1.0f)
                {
                    ScoreBoard.alpha = 0.0f;
                    for (int i = 0; i < 4; ++i)
                    {
                        Names[i].text = Scores[i].text = Lag[i].text = string.Empty;
                    }
                }
                else
                {
                    for (int i = 0; i < playerManager.CurrentPlayers; ++i)
                    {
                        Names[i].text = playerManager.PlayerNames[i];
                        Scores[i].text = playerManager.GetComponent<S3_GameManager>().PlayerScores[i].ToString();
                        Lag[i].text = playerManager.GetPing(i).ToString();
                    }
                    ScoreBoard.alpha = 1.0f;
                }
            }
            else
            {
                if (ScoreBoard.alpha == 1.0f)
                {
                    ScoreBoard.alpha = 0.0f;
                    for (int i = 0; i < 4; ++i)
                    {
                        Names[i].text = Scores[i].text = Lag[i].text = string.Empty;
                    }
                }
                else
                {
                    for (int i = 0; i < cPlayerManager.CurrentPlayers; ++i)
                    {
                        Names[i].text = cPlayerManager.PlayerNames[i];
                        Scores[i].text = cPlayerManager.GetComponent<S3_GameManager>().PlayerScores[i].ToString();
                        Lag[i].text = cPlayerManager.GetPing(i).ToString();
                    }
                    ScoreBoard.alpha = 1.0f;
                }
            }
        }
	}
}
