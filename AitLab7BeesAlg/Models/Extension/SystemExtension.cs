using System;
using Newtonsoft.Json;

namespace AitLab7BeesAlg.Models.Extension
{
    public static class SystemExtension
    {
        public static T Clone<T>(this T source)
        {
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings() {
                // Use this option to ignore reference looping option
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                // Use this option when properties use an Interface as the type
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
            };
            var serialized = JsonConvert.SerializeObject(source, jsonSettings);
            return JsonConvert.DeserializeObject<T>(serialized, jsonSettings) ?? throw new InvalidOperationException();
        }
    }
}