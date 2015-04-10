using UnityEngine;
using System.Collections;

public class S3_SwordPickup : MonoBehaviour {

    public string SwordPickup = "dummy";

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject collisionObject = collider.gameObject;
        if( collisionObject.tag == "Player" )
        {
            S3_CombatStateController combatState = collisionObject.GetComponent<S3_CombatStateController>();
            GameObject sword = combatState.GetSword();
            S3_SwordManager swordManager = sword.GetComponent<S3_SwordManager>();
            swordManager.SetSword( SwordPickup );
        }
    }
}
