using UnityEngine;
using System.Collections;

public class S3_DeadReckoningInterpolator : MonoBehaviour {
    public float TimeToReckon = 0.5f;
    public float TargetReachedTreshold = 0.1f;
    public float DesyncAdjustmentThreshold = 10f;

    private bool reckoning = false;
    private Vector3 target;
    private Vector2 realVelocity;
    private Vector2 reckoningVelocity;
    private Rigidbody2D rb2D;
	// Use this for initialization
	void Start () {
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if( reckoning )
        {
            if( Vector3.Distance(target, transform.position) <= TargetReachedTreshold )
            {
                rb2D.velocity = realVelocity;
                reckoning = false;
            }
        }
	
	}

    public void SetTarget(Vector3 actualCurrentPos, Vector2 actualCurrentVel)
    {
        reckoning = true;
        realVelocity = actualCurrentVel;
        target = actualCurrentPos + new Vector3(realVelocity.x, realVelocity.y) * TimeToReckon;
        Vector3 reckoningVelocity3 = ( ( target - transform.position ) / TimeToReckon );
        reckoningVelocity = reckoningVelocity3;
        rb2D.velocity = reckoningVelocity;
    }
}
