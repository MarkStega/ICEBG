using System.Text.Json;

namespace ICEBG.Client;
public interface IMessage
{
    string GetMessageCardJson(JsonSerializerOptions jsonSerializerOptions);
}