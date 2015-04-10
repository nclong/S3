using UnityEngine;
using System.Collections;

public class S3_CharacterMovement : MonoBehaviour {
    public float MaxSpeed;
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;

    private float dashTimer = 0;
    private float dashCooldownTimer = 0;
    private Rigidbody2D rb2D;
    private bool dashPressed = false;
    private bool dashReleased = true;
    private bool dashing = false;
    private Vector2 dashDirection;
	// Use this for initialization
	void Start () {
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if( Input.GetButton("Dash") )
        {
            dashPressed = true;
        }
        else
        {
            dashReleased = true;
            dashPressed = false;
        }

        if( dashPressed && dashReleased )
        {
            dashReleased = false;
            dashing = true;
            dashDirection = new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) ).normalized;
            if (dashDirection == Vector2.zero )
            {
                dashDirection = ( Camera.main.ScreenToWorldPoint( Input.mousePosition ) - transform.position ).normalized;
            }
        }

        if( !dashing )
        {
            rb2D.velocity = new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) ).normalized * MaxSpeed;

            Vector3 MouseInWorld = Camera.main.ScreenToWorldPoint( Input.mousePosition );
            Vector3 PlayerToMouse = MouseInWorld - transform.position;
            transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Atan2( PlayerToMouse.y, PlayerToMouse.x ) * Mathf.Rad2Deg ); 
        }
        else
        {
            dashTimer += Time.deltaTime;
            dashCooldownTimer += Time.deltaTime;
            if( dashTimer >= dashTime )
            {
                rb2D.velocity = Vector2.zero;
            }
            else
            {
                rb2D.velocity = dashDirection * dashSpeed;
            }

            if( dashCooldownTimer >= dashCooldown )
            {
                dashTimer = 0f;
                dashing = false;
                dashCooldownTimer = 0f;
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if( collisionObject.tag == "Player" )
        {
            rb2D.velocity = Vector2.zero;
            collisionObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Debug.Log( "Player Collision" );
        }
    }
}
