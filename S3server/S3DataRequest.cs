using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class S3DataRequest
{
    [JsonProperty]
    public string UserName;
    [JsonProperty]
    public int passwordHash;
    [JsonProperty]
    public string type;
}

