using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S3_SwordManager : MonoBehaviour {
    public Sprite DefaultSprite;
    public Sprite GunSprite;
    public Sprite LaserSprite;
    public Sprite SwordzookaSprite;

    private MonoBehaviour[] swords;
    [Range(0,1)]
    private string currentSword = "default";
    private string inactiveSword = string.Empty;
    private bool switchPressed = false;
    private bool switchReleased = true;
    private Dictionary<string, MonoBehaviour> swordDict;
    private Dictionary<string, Sprite> spriteDict;
    private S3_GunSword gunSword;
    private SpriteRenderer sr;
	// Use this for initialization
	void Start () {

        swordDict = new Dictionary<string, MonoBehaviour>();
        swordDict["default"] = GetComponent<S3_DefaultSword>();
//        swordDict["dummy"] = GetComponent<S3_DummySword>();
//        swordDict["dummy"].enabled = false;
//        swordDict["gun"] = GetComponent<S3_GunSword>();
//        swordDict["gun"].enabled = false;
//        swordDict["laser"] = GetComponent<S3_SwordLaser>();
//        swordDict["laser"].enabled = false;
//        swordDict["swordzooka"] = GetComponent<S3_SwordZooka>();
//        swordDict["swordzooka"].enabled = false;

        spriteDict = new Dictionary<string, Sprite>();
        spriteDict["default"] = DefaultSprite;
//        spriteDict["gun"] = GunSprite;
//        spriteDict["laser"] = LaserSprite;
//        spriteDict["swordzooka"] = SwordzookaSprite;

        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
//        if( Input.GetButton("Switch") )
//        {
//            switchPressed = true;
//        }
//        else
//        {
//            switchPressed = false;
//            switchReleased = true;
//        }

        if( switchPressed && switchReleased)
        {
            switchReleased = false;
            if( inactiveSword != string.Empty )
            {
                swordDict[currentSword].enabled = false;
                swordDict[inactiveSword].enabled = true;

                string temp = currentSword;
                currentSword = inactiveSword;
                inactiveSword = temp;

                sr.sprite = spriteDict[currentSword];
            }
        }
	
	}

    public void SetSword(string newSword)
    {
        swordDict[currentSword].enabled = false;
        if( inactiveSword != string.Empty )
        {
            swordDict[inactiveSword].enabled = false; 
        }
        currentSword = newSword;
        inactiveSword = "default";
        swordDict[currentSword].enabled = true;

        sr.sprite = spriteDict[newSword];
    }
}
