using UnityEngine;
using System.Collections;

public class S3_SwordZooka : MonoBehaviour
{

    public int uses = 5;        //number of times it can be used before it disappears (like ammo)
    public float rarity = 2;
    public int Damage;
    public GameObject RocketObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    //called when the SwordZooka is swung.
    public void fireRocket()
    {
        //obviously does not fire when out of ammo/uses
        if (uses > 0)
        {
            GameObject clone = Instantiate<GameObject>( RocketObject );
            clone.transform.position = transform.position;
            clone.transform.rotation = transform.parent.parent.parent.parent.rotation;
            clone.GetComponent<S3_RocketProperties>().direction = Camera.main.ScreenToWorldPoint( Input.mousePosition ) - transform.position;
            clone.GetComponent<S3_RocketProperties>().player = transform.parent.parent.parent.parent.gameObject;
            //fire a rocket (make a pre-fab)
            uses--; //costs one use per swing
        }
    }

    //called when another SwordSplosion is picked up. Should it check to see if the
    //player picked it up first?
    void addAmmo()
    {
        uses += 2;
    }
}
