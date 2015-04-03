using UnityEngine;
using System.Collections;

public class S3_PlayerProperties : MonoBehaviour {
    public int StartingHealth;
    public GameObject BloodEmitter;
    public string PlayerName;
    private int currentHealth;
	// Use this for initialization
	void Start () {
        currentHealth = StartingHealth;
	}
	
	// Update is called once per frame
	void Update () {
	    if( currentHealth <= 0)
        {
            Die();
        }
	}

    public void TakeDamage(int x)
    {
        currentHealth -= x;
    }

    public GameObject GetBloodEmittier()
    {
        return BloodEmitter;
    }

    private void Die()
    {
        Debug.Log(PlayerName + " has died.");
    }
}
