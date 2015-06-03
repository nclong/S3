using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S3_ServerButton : MonoBehaviour {

    private Button button;
    private Text ServerNameText;
    private Text PlayerCountText;
    private string IP;

    void Start()
    {
        button = GetComponent<Button>();
    }


    public void OnClick()
    {
        if( PlayerCountText.text != "4 / 4" )
        {
            PlayerPrefs.SetString( "HostIP", IP );
            PlayerPrefs.Save();
            Application.LoadLevel( "tussle_client" );
        }

    }

    public void SetInfo(string ServerName, int players, string ip)
    {
        ServerNameText.text = ServerName;
        PlayerCountText.text = players.ToString() + " / 4";
        IP = ip;
    }
}
