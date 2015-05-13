using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S3_MessagesQueue {

    public S3_MessagesQueue()
    {
        ReceivedMessages = new Queue<S3_GameMessage>();
    }

    public void AddMessage(S3_GameMessage message)
    {
        ReceivedMessages.Enqueue( message );
    }

    public S3_GameMessage GetMessage()
    {
        if( !IsEmpty )
        {
            return ( ReceivedMessages.Dequeue() ); 
        }
        else
        {
            throw new MissingComponentException();
        }
    }

    public bool IsEmpty
    {
        get
        {
            return ReceivedMessages.Count <= 0;
        }
    }

    private Queue<S3_GameMessage> ReceivedMessages;

}
