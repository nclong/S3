using UnityEngine;
using System.Collections;

public class S3_PickupLabel : MonoBehaviour {
    public GameObject pickup;
    public Vector3 Offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.WorldToScreenPoint( pickup.transform.position ) + Offset;
	}
}
