using UnityEngine;
using System.Collections;

public class S3_GameMessage
{
    public float SendTime;
    public S3_GameMessageType MessageType;
    public IMessageData MessageData;

    public S3_GameMessage(float time, S3_GameMessageType type, IMessageData data)
    {
        SendTime = time;
        MessageType = type;
        MessageData = data;
    }

    public S3_GameMessage()
    {

    }

}
