using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace Kakhanouskaya.UI.Extensions
{
      public static class SessionExtensions
        {
            // Захаваць аб'ект тыпу T у сесію
            public static void Set<T>(this ISession session, string key, T value)
            {
                var json = JsonSerializer.Serialize(value);
                session.SetString(key, json);
            }

            // Атрымаць аб'ект тыпу T з сесіі
            public static T? Get<T>(this ISession session, string key)
            {
                var json = session.GetString(key);
                return json == null ? default : JsonSerializer.Deserialize<T>(json);
            }
        }
}
