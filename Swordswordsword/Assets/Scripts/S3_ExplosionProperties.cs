using UnityEngine;
using System.Collections;

public class S3_ExplosionProperties : MonoBehaviour {
    public int Damage;
    public float lifetime;

    private float timer = 0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if( timer >= lifetime )
        {
            Destroy( transform.parent.gameObject );
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject colliderObject = collider.gameObject;
        if( colliderObject.tag == "Player" )
        {
            S3_PlayerProperties properties = colliderObject.GetComponent<S3_PlayerProperties>();
            properties.TakeDamage( Damage );
        }
    }


}
