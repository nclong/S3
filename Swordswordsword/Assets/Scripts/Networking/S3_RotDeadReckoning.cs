using UnityEngine;
using System.Collections;

public class S3_RotDeadReckoning : MonoBehaviour {
    public GameObject ClientObject;
    public GameObject Player;
    public float Threshold;
    public float Angle;
    private S3client client;
	// Use this for initialization
	void Start () {
        client = ClientObject.GetComponent<S3client>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
        if( Mathf.Abs(Player.transform.eulerAngles.z - Angle) > Threshold)
        {
            UpdateDR();
        }
	}

    void UpdateDR()
    {
        Angle = Player.transform.eulerAngles.z;
    }
}
