using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S3_SwordManager : MonoBehaviour {

    private MonoBehaviour[] swords;
    [Range(0,1)]
    private int currentSword = 0;
    private int inactiveSword = 1;
    private bool switchPressed = false;
    private bool switchReleased = true;
    private Dictionary<string, MonoBehaviour> swordDict;
    private S3_GunSword gunSword;
	// Use this for initialization
	void Start () {

        swordDict = new Dictionary<string, MonoBehaviour>();
        swordDict["default"] = GetComponent<S3_DefaultSword>();
        swordDict["dummy"] = GetComponent<S3_DummySword>();
        swordDict["dummy"].enabled = false;
        swords = new MonoBehaviour[2];
        swords[0] = swordDict["default"];
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
            switchReleased = false;
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

    public void SetSword(string newSword)
    {
        swords[1] = swordDict[newSword];
        currentSword = 1;
        inactiveSword = 0;

        swords[currentSword].enabled = true;
        swords[inactiveSword].enabled = false;
    }
}
