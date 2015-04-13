using UnityEngine;
using System.Collections;

public class S3_BulletProperties : MonoBehaviour {

    public int shotDmg;
    public float velocity;
    public float lifetime;
    public Vector2 direction;

    private float lifeTimer = 0f;
    private Rigidbody2D rb2D;
    //need direction of player facing

	// Use this for initialization
	void Start () {
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        lifeTimer += Time.deltaTime;
        if( lifeTimer >= lifetime )
        {
            Destroy( transform.gameObject );
        }

        rb2D.velocity = direction.normalized * velocity;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        S3_PlayerProperties player = collision.gameObject.GetComponent<S3_PlayerProperties>();
        if( player != null )
        {
            player.TakeDamage( shotDmg );
            GameObject bloodEmitter = player.GetBloodEmittier();
            Vector3 bulletToPlayer = collision.gameObject.transform.position - transform.position;
            float bloodSplatAngle = Mathf.Atan2( bulletToPlayer.y, bulletToPlayer.x ) * Mathf.Rad2Deg - 180f;
            bloodEmitter.transform.eulerAngles = new Vector3( bloodSplatAngle, bloodEmitter.transform.eulerAngles.y, bloodEmitter.transform.eulerAngles.z );
            ParticleSystem bloodParticles = bloodEmitter.GetComponent<ParticleSystem>();
            bloodParticles.Play();
        }

        Destroy( transform.gameObject );
    }
}
