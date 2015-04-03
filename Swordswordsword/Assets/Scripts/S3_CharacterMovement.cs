using UnityEngine;
using System.Collections;

public class S3_CharacterMovement : MonoBehaviour {
    public float MaxSpeed;

    private Rigidbody2D rb2D;
	// Use this for initialization
	void Start () {
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb2D.velocity = new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) ).normalized * MaxSpeed;

        Vector3 MouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 PlayerToMouse = MouseInWorld - transform.position;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Atan2( PlayerToMouse.y, PlayerToMouse.x ) * Mathf.Rad2Deg );
	}
}
