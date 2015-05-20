using UnityEngine;
using System.Collections;

public class S3_HealthSystem : MonoBehaviour {

    public int health;

    public void damage(int damage)
    {
        health -= damage;
    }
}
