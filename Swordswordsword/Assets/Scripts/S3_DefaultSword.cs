﻿using UnityEngine;
using System.Collections;

public class S3_DefaultSword : MonoBehaviour {
    public int Damage;
	public GameObject ServerObject;
	public GameObject PlayerManagerObject;
	private bool IsServer;

	private S3_GameManager gameManager;

	// Use this for initialization
	void Start () {
		ServerObject = GameObject.Find ("ServerObject");
		IsServer = ServerObject != null;
		PlayerManagerObject = GameObject.Find ("ServerPlayerManager");
		if (IsServer) {
			gameManager = PlayerManagerObject.GetComponent<S3_GameManager>();
		}
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
					gameManager.PlayerScores[transform.root.gameObject.GetComponent<S3_PlayerProperties>().PlayerNumber] += 1;
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
