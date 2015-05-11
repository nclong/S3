using UnityEngine;
using System.Collections;

public class S3_ServerPlayerPosDRData : IMessageData {

    byte PlayerNum;
    float DRPosX;
    float DRPosY;
    float DRAngle;
    float DRVelX;
    float DRVelY;
    float InitialTime;
}
