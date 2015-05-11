using UnityEngine;
using System.Collections;

public class S3_GameMessage
{
    float SendTime;
    S3_GameMessageType MessageType;
    IMessageData MessageData;

    public S3_GameMessage(float time, S3_GameMessageType type, IMessageData data)
    {
        SendTime = time;
        MessageType = type;
        MessageData = data;
    }

}
