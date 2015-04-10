using UnityEngine;
using System.Collections;

public class S3_GunSword : MonoBehaviour {
    public int uses = 20;        //number of times it can be used before it disappears (like ammo)
    public int Damage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //shamelessly taken from defaultsword script - hoping it will be temporary
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject collisionObject = collider.gameObject;
        S3_PlayerProperties player = collisionObject.GetComponent<S3_PlayerProperties>();
        if (player != null && collisionObject != transform.parent.gameObject)
        {
            player.TakeDamage(Damage);
            GameObject bloodEmitter = player.GetBloodEmittier();
            Vector3 PlayerToSword = collisionObject.transform.position - transform.parent.position;
            float bloodSplatAngle = Mathf.Atan2(PlayerToSword.y, PlayerToSword.x) * Mathf.Rad2Deg - 180f;
            bloodEmitter.transform.eulerAngles = new Vector3(bloodSplatAngle, bloodEmitter.transform.eulerAngles.y, bloodEmitter.transform.eulerAngles.z);
            ParticleSystem bloodParticles = bloodEmitter.GetComponent<ParticleSystem>();
            bloodParticles.Play();
        }
    }

    //called when GunSword is swung
    void FireBullet()
    {
        //obviously does not fire when out of ammo/uses
        if (uses > 0)
        { 
            //fire bullet, make bullet pre-fab in it at the end of the sword
            uses--; //costs one bullet per swing
        }
    }

    //called when another GunSword is picked up. Should it check to see if the
    //player picked it up first?
    void addAmmo()
    {
        uses += 5;
    }
}
