﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_MasterServerConnectionManager : MonoBehaviour {
    public string MasterServerIP;
    public Text StatusText;
    public InputField UserNameField;
    public InputField PasswordField;
    public S3_MasterServerClient client = new S3_MasterServerClient(); 

	// Use this for initialization
	void Start () {
        //we need to move this elsewhere. we want to start it as we try to login or register as many times as we want.
        
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnLogin()
    {
        client.StartClient( MasterServerIP );
        string userName = UserNameField.text;
        string password = PasswordField.text;
        if( userName == string.Empty || password == string.Empty)
        {
            StatusText.text = "Please enter a userName and password.";
            return;
        }
        int passHash = password.GetHashCode();
        S3DataRequet request = new S3DataRequet()
        {
            UserName = userName,
            passwordHash = passHash,
            type = "Login"
        };

        S3DataResponse response = client.AttemptLoginOrRegister(request);
        StatusText.text = response.message;

        //whether we fail to login or not, we want to close the sockets on BOTH ends.
        if( response.responseCode == 1)
        {
            
            Application.LoadLevel( "firstScene" );
        }
        else if (response.responseCode == -1)
        { 
            
        }

        client.StopClient();
    }

    public void OnRegister()
    {
        client.StartClient( MasterServerIP );
        string userName = UserNameField.text;
        string password = PasswordField.text;
        if( userName == string.Empty || password == string.Empty )
        {
            StatusText.text = "Please enter a userName and password.";
            return;
        }
        int passHash = password.GetHashCode();
        S3DataRequet request = new S3DataRequet()
        {
            UserName = userName,
            passwordHash = passHash,
            type = "Register"
        };

        S3DataResponse response = client.AttemptLoginOrRegister(request);
        StatusText.text = response.message;

        client.StopClient();
    }
}
