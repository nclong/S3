using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class S3_LobbyMasterServerManager : MonoBehaviour {
    public string MasterServerIp;
    public GameObject ServerButtonPrefab;
    public GameObject UICanvas;
    public InputField inputField;
    public Text ChatText;

    public float HeartbeatTick = 1f;
    public float HeartbeatTimer = 0f;

    private S3_MasterServerClient masterServer;
    private string PlayerName;
	// Use this for initialization
	void Start () {
        PlayerName = PlayerPrefs.GetString( "PlayerName" );
        masterServer = new S3_MasterServerClient();
        masterServer.StartClient( MasterServerIp );
	
	}
	
	// Update is called once per frame
	void Update () {
        HeartbeatTimer += Time.deltaTime;
        if( HeartbeatTimer >= HeartbeatTick)
        {
            ChatText.text = masterServer.SendChatHeartBeat();
            HeartbeatTimer = 0f;
        }
	
	}

    public void OnRefresh()
    {
        masterServer.ClearLobbyList();
        for(int i = 0; i < UICanvas.transform.childCount; ++i)
        {
            Transform t = UICanvas.transform.GetChild( i );
            if( t.gameObject.name.Contains( "ServerButton" ) )
            {
                Object.Destroy( t.gameObject );
            }
        }

        List<S3_LobbyServerInfo> serverList = masterServer.RequestServerList();
        int buttonOffset = 0;
        foreach(S3_LobbyServerInfo server in serverList)
        {
            GameObject newButton = Instantiate<GameObject>( ServerButtonPrefab );
            newButton.transform.SetParent( UICanvas.transform, false );
            newButton.transform.position = new Vector3( newButton.transform.position.x, -19 - buttonOffset * 38 );

            buttonOffset += 1;
        }
    }

    public void OnChatSend()
    {
        string toSend = PlayerPrefs.GetString( "PlayerName" ) + ": " + inputField.text + '\n';
        masterServer.SendChatString( toSend );
        inputField.text = string.Empty;
    }
}
