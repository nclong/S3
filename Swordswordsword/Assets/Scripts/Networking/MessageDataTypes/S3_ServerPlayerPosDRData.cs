using UnityEngine;
using System.Collections;

public class S3_ServerPlayerPosDRData : IMessageData {

    public byte PlayerNum;                     
    public float DRPosX;                       
    public float DRPosY;                       
    public float DRAngle;                      
    public float DRVelX;                       
    public float DRVelY;                       
    public float InitialTime;                  
}
