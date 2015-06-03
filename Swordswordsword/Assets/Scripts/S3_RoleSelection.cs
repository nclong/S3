using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_RoleSelection : MonoBehaviour {
    public InputField HostName;


	public void SelectJoin()
    {
        Application.LoadLevel( "lobby" );
    }

    public void SelectHost()
    {
        PlayerPrefs.SetString( "ServerName", HostName.text );
        Application.LoadLevel( "tussle_server" );
    }
}
