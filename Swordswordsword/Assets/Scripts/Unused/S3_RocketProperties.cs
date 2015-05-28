using UnityEngine;
using System.Collections;

public class S3_RocketProperties : MonoBehaviour
{

    public int shotDmg;
    public float velocity;
    public float lifetime;
    public Vector2 direction;
    public GameObject Explosion;
    public GameObject player;

    private float lifeTimer = 0f;
    private Rigidbody2D rb2D;
    //need direction of player facing

    // Use this for initialization
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = direction.normalized * velocity;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;
        if( lifeTimer >= lifetime )
        {
            Destroy( transform.gameObject );
        }

    }

    void OnCollisionEnter2D( Collision2D collision )
    {
        
        if( (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall") && collision.gameObject != player )
        {
            S3_SoundManager.RocketLand.Play();
            rb2D.velocity = Vector2.zero;
            Explosion.SetActive( true );
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider2D>().isTrigger = true; 
        }
    }
}
