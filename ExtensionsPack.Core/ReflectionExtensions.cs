using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtensionsPack.Core
{
    public static class ReflectionExtensions
    {
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
    }
}
