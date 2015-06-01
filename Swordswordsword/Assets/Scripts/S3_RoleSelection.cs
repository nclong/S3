using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_RoleSelection : MonoBehaviour {
    public InputField HostIp;


	public void SelectJoin()
    {
        string HostIpString = HostIp.text;
    }

    public void SelectHost()
    {

        Application.LoadLevel( "tussle_server" );
    }
}
