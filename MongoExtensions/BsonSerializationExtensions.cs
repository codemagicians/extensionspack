using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

namespace MongoExtensions
{
    public static class BsonSerializationExtensions
    {
        public static TEntity DeserializeTo<TEntity>(this BsonDocument document, Action<TEntity, BsonDocument> convertFunction = null) where TEntity : class
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            var json = document.ToJson(new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Strict
            });
            TEntity entity = Newtonsoft.Json.JsonConvert.DeserializeObject<TEntity>(json);
            convertFunction?.Invoke(entity, document);
            return entity;
        }

        public static JObject DeserializeToObject(this BsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var json = document.ToJson(new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Strict
            });
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return (JObject)obj;
        }
    }
}
