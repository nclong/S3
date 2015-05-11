using UnityEngine;
using System.Collections;

public class S3_ServerTime : MonoBehaviour {

    private static float time;
    public static float ServerTime
    {
        get { return time; }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
	
	}

    public static void SetTime(float newTime)
    {
        time = newTime;
    }

    public static void SetOffset(float offset)
    {
        time += offset;
    }
}
