using System;
using Newtonsoft.Json;

namespace WebApplication2.Hubs.Models.Chat;

[Serializable]
public class CreateChatRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
    public long[] UsersIds { get; set; }
}