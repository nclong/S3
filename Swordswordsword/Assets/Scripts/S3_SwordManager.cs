using UnityEngine;
using System.Collections;

public class S3_SwordManager : MonoBehaviour {

    private MonoBehaviour[] swords;
    [Range(0,1)]
    private int currentSword = 0;
    private int inactiveSword = 1;
    private bool switchPressed = false;
    private bool switchReleased = true;
	// Use this for initialization
	void Start () {
        swords = new MonoBehaviour[2];
        swords[0] = GetComponent<S3_CharacterMovement>();
        swords[1] = null;
	}
	
	// Update is called once per frame
	void Update () {
        if( Input.GetButton("Switch") )
        {
            switchPressed = true;
        }
        else
        {
            switchPressed = false;
            switchReleased = true;
        }

        if( switchPressed && switchReleased)
        {
            if( currentSword == 0 && swords[1] != null)
            {
                currentSword = 1;
                inactiveSword = 0;
            }
            else
            {
                currentSword = 0;
                inactiveSword = 1;
            }

            swords[currentSword].enabled = true;
            if( swords[inactiveSword] != null )
            {
                swords[inactiveSword].enabled = false;
            }
        }
	
	}
}
