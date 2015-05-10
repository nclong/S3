using UnityEngine;
using System.Collections;

public class S3_ClientCamera : MonoBehaviour {
    public GameObject Player;

	// Use this for initialization
    void LateUpdate()
    {
        transform.position = new Vector3(
            Player.transform.position.x,
            Player.transform.position.y,
            transform.position.z );
    }
}
