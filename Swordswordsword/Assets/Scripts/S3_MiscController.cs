using UnityEngine;
using System.Collections;

public class S3_MiscController : MonoBehaviour {

    public CanvasGroup ScoreBoard;

	// Use this for initialization
	void Start ()
    {
        ScoreBoard.alpha = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ScoreBoard.alpha == 1.0f)
                ScoreBoard.alpha = 0.0f;
            else
                ScoreBoard.alpha = 1.0f;
        }
	}
}
