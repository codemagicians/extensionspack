using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsPack.Core
{
    public static class ModelValidationExtensions
    {
        /// <summary>
        /// Validates nullable int value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minAllowedValue">The inclusive lower bound of int value</param>
        /// <param name="maxValue">The exclusive upper bound of int value</param>
        /// <returns>True if input has values between 0 and max, otherwise</returns>
        public static bool ValidateInt(int? value, int minAllowedValue, int maxValue)
        {
            return value.HasValue && ValidateInt(value.Value, maxValue, minAllowedValue);
        }

        /// <summary>
        /// Validates nullable int value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue">The exclusive upper bound of int value</param>
        /// <returns>True if input has values between 0 and max, otherwise - false</returns>
        public static bool ValidateInt(int? value, int maxValue)
        {
            return value.HasValue && ValidateInt(value.Value, maxValue);
        }

        /// <summary>
        /// Validates nullable int value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue">The exclusive upper bound of int value</param>
        /// <param name="minAllowedValue">The inclusive lower bound of int value</param>
        /// <returns>True if input has values between 0 and max, otherwise</returns>
        public static bool ValidateInt(int value, int maxValue, int minAllowedValue = 0)
        {
            return value >= minAllowedValue && value < maxValue;
        }
    }
}
