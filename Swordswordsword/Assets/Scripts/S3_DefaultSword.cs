using UnityEngine;
using System.Collections;

public class S3_DefaultSword : MonoBehaviour {
    public int Damage;
	public GameObject ServerObject;
	private bool IsServer;

	// Use this for initialization
	void Start () {
		ServerObject = GameObject.Find ("ServerObject");
		IsServer = ServerObject != null;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if( enabled )
        {
            GameObject collisionObject = collider.gameObject;
            S3_PlayerProperties player = collisionObject.GetComponent<S3_PlayerProperties>();
            if( player != null && collisionObject != transform.parent.gameObject )
            {
				if(IsServer)
				{
					player.TakeDamage();
				}
				else {
	                GameObject bloodEmitter = player.GetBloodEmittier();
	                Vector3 PlayerToSword = collisionObject.transform.position - transform.parent.position;
	                float bloodSplatAngle = Mathf.Atan2( PlayerToSword.y, PlayerToSword.x ) * Mathf.Rad2Deg - 180f;
	                bloodEmitter.transform.eulerAngles = new Vector3( bloodSplatAngle, bloodEmitter.transform.eulerAngles.y, bloodEmitter.transform.eulerAngles.z );
	                ParticleSystem bloodParticles = bloodEmitter.GetComponent<ParticleSystem>();
	                bloodParticles.Play();
	                S3_SoundManager.SlashHitSound.Play();
				}
            } 
        }
    }
}
