using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_MasterServerConnectionManager : MonoBehaviour {
    public string MasterServerIP;
    public Text StatusText;
    public InputField UserNameField;
    public InputField PasswordField;

	// Use this for initialization
	void Start () {
        S3_MasterServerClient.StartClient(MasterServerIP);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnLogin()
    {
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

        S3DataResponse response = S3_MasterServerClient.AttemptLoginOrRegister( request );
        StatusText.text = response.message;

        if( response.responseCode == 1)
        {
            S3_MasterServerClient.StopClient();
            Application.LoadLevel( "firstScene" );
        }
    }

    public void OnRegister()
    {
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

        S3DataResponse response = S3_MasterServerClient.AttemptLoginOrRegister( request );
        StatusText.text = response.message;
    }
}
