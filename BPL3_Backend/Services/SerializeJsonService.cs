using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class SerializeJsonService
    {
        public static void SerializeJson(dynamic model, string filePath)
        {
            var jsonUtf8Bytes = "";
            jsonUtf8Bytes = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(filePath, jsonUtf8Bytes);
        }
    }
}
