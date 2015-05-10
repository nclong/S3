using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Packet
{
    [JsonProperty]
    public PacketType type;
    //need a unique ID represented in bytes
    [JsonProperty]
    public byte ID;
    [JsonProperty]
    public string userName;
}
