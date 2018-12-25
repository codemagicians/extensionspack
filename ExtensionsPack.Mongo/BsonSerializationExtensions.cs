using System;
using ExtensionsPack.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

namespace ExtensionsPack.Mongo
{
    public static class BsonSerializationExtensions
    {
        /// <summary>
        /// Deserializes Bson to entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="document">Bson document to deserialize</param>
        /// <param name="convertFunction">Custom mapping function that will be invoked after deserialization</param>
        /// <param name="checkType">Whether to perform safeety check on type parameter</param>
        /// <returns>Deserialized object of the type specified</returns>
        public static TEntity DeserializeTo<TEntity>(this BsonDocument document, Action<TEntity, BsonDocument> convertFunction = null, bool checkType = false)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (checkType && !typeof(TEntity).IsClassOrStruct())
            {
                throw new InvalidOperationException($"Type {typeof(TEntity).Name} is not supported. Type must be either a class or a struct");
            }
            var json = document.ToJson(new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Strict
            });
            TEntity entity = Newtonsoft.Json.JsonConvert.DeserializeObject<TEntity>(json);
            convertFunction?.Invoke(entity, document);
            return entity;
        }

        /// <summary>
        /// Deserializes Bson to JObject
        /// </summary>
        /// <param name="document">Bson document to deserialize</param>
        /// <returns>Deserialized javascript object</returns>
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
