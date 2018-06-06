using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtensionsPack.Core
{
    public static class ReflectionExtensions
    {
        private static readonly Lazy<Dictionary<string, FieldInfo[]>> ObjFieldsMap = new Lazy<Dictionary<string, FieldInfo[]>>();
        private static readonly Lazy<Dictionary<string, PropertyInfo[]>> ObjPropertiesMap = new Lazy<Dictionary<string, PropertyInfo[]>>();

        /// <summary>
        /// Returns a name of field or property which value is returned by expression
        /// For example this function applied to 'x => x.Id' will return 'Id'
        /// </summary>
        /// <param name="exp">Target expression that returns some value</param>
        /// <returns></returns>
        public static string GetPropertyName(this Expression<Func<object>> exp)
        {
            return (exp.Body as MemberExpression)?.Member.Name ?? (((UnaryExpression)exp.Body).Operand as MemberExpression)?.Member.Name;
        }

        /// <summary>
        /// Checks whether certain class or its member is decorated with an attribute specified
        /// </summary>
        /// <typeparam name="T">A type of target attribute</typeparam>
        /// <param name="classMember">Target object</param>
        /// <returns>Attribute if member is decorated and default is not</returns>
        public static T GetAttributeOrDefault<T>(this MemberInfo classMember) where T : Attribute
        {
            return classMember.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;
        }

        /// <summary>
        /// Finds a property decorated with attrbute Key
        /// </summary>
        /// <param name="type">Type in which we look for a property decorated with Key</param>
        /// <returns>Property information if Key was found otherwise null</returns>
        public static PropertyInfo GetPrimaryKeyProperty(Type type)
        {
            return type.GetProperties().FirstOrDefault(IsDecoratedWithKeyAttribute);
        }

        /// <summary>
        /// Check wether class or its member is decorated with KeyAtrribute
        /// </summary>
        /// <param name="classMember">Target class or it's member</param>
        /// <returns>True if decorated with KeyAttribute otherwise False</returns>
        public static bool IsDecoratedWithKeyAttribute(this MemberInfo classMember)
        {
            return GetAttributeOrDefault<KeyAttribute>(classMember) != null;
        }

        /// <summary>
        /// Checks whether the object contains a property with certain value
        /// </summary>
        /// <param name="obj">Object to perform check upon</param>
        /// <param name="propertyValue">Value of the property</param>
        /// <param name="withPrivate">Whether to check in private properties</param>
        /// <param name="useCaching">Whether to store values in cache to improve performance</param>
        /// <param name="typeSafetyCheck">Whether to validate object type before check</param>
        /// <returns>True if value is found</returns>
        public static bool ContainsPropertyWithValue(this object obj, object propertyValue, bool withPrivate = true, bool useCaching = true, bool typeSafetyCheck = false)
        {
            if (obj == null || propertyValue == null)
            {
                return false;
            }

            PropertyInfo[] properties = null;
            var type = obj.GetType();

            if (typeSafetyCheck && !(type.IsClass && !typeof(Delegate).IsAssignableFrom(type)) && !(type.IsValueType && !type.IsEnum))
            {
                return false;
            }

            if (useCaching && !ObjPropertiesMap.Value.TryGetValue(type.FullName, out properties))
            {
                ObjPropertiesMap.Value.Add(type.FullName, properties = withPrivate ? type.GetProperties(BindingFlags.Instance & BindingFlags.NonPublic) : type.GetProperties());
            }
            properties = properties ?? (withPrivate ? type.GetProperties(BindingFlags.Instance & BindingFlags.NonPublic) : type.GetProperties());
            return properties.Any(p => p.GetValue(obj)?.Equals(propertyValue) == true);
        }

        /// <summary>
        /// Checks whether the object contains a field with certain value
        /// </summary>
        /// <param name="obj">Object to perform check upon</param>
        /// <param name="fieldValue">Value of the field</param>
        /// <param name="withPrivate">Whether to check in private properties</param>
        /// <param name="useCaching">Whether to store values in cache to improve performance</param>
        /// <param name="typeSafetyCheck">Whether to validate object type before check</param>
        /// <returns>True if value is found</returns>
        public static bool ContainsFieldWithValue(this object obj, object fieldValue, bool withPrivate = true, bool useCaching = true, bool typeSafetyCheck = false)
        {
            if (obj == null || fieldValue == null)
            {
                return false;
            }

            FieldInfo[] fields = null;
            var type = obj.GetType();

            if (typeSafetyCheck && !(type.IsClass && !typeof(Delegate).IsAssignableFrom(type)) && !(type.IsValueType && !type.IsEnum))
            {
                return false;
            }

            if (useCaching && !ObjFieldsMap.Value.TryGetValue(type.FullName, out fields))
            {
                ObjFieldsMap.Value.Add(type.FullName, fields = withPrivate ? type.GetFields(BindingFlags.Instance & BindingFlags.NonPublic) : type.GetFields());
            }
            fields = fields ?? (withPrivate ? type.GetFields(BindingFlags.Instance & BindingFlags.NonPublic) : type.GetFields());
            return fields.Any(p => p.GetValue(obj)?.Equals(fieldValue) == true);
        }

        /// <summary>
        /// Checks whether the object contains a field or a property with certain value
        /// </summary>
        /// <param name="obj">Object to perform check upon</param>
        /// <param name="value">Value of the field or property</param>
        /// <param name="withPrivate">Whether to check in private members</param>
        /// <param name="useCaching">Whether to store values in cache to improve performance</param>
        /// <param name="typeSafetyCheck">Whether to validate object type before check</param>
        /// <returns>0 - value not found, 1 - value found in a property, 2 - value found in a field</returns>
        public static int ContainsValue(this object obj, object value, bool withPrivate = true, bool useCaching = true, bool typeSafetyCheck = true)
        {
            if (obj == null || value == null)
            {
                return 0;
            }

            if (ContainsPropertyWithValue(obj, value, withPrivate, useCaching, typeSafetyCheck))
            {
                return 1;
            }
            return ContainsFieldWithValue(obj, value, withPrivate, useCaching, typeSafetyCheck) ? 2 : 0;
        }
    }
}
