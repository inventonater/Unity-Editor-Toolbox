using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Toolbox
{
    /// <summary>
    /// Provides utility methods for working with enums.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Converts an enumeration value to an unsigned long integer (ulong).
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value to convert.</param>
        /// <returns>The enumeration value as an unsigned long integer.</returns>
        private static ulong ToUInt64<T>(this T enumValue) where T : struct, Enum
        {
            return Convert.ToUInt64(enumValue);
        }

        /// <summary>
        /// Converts a 64-bit unsigned integer to an enum value of type T.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The 64-bit unsigned integer value to convert.</param>
        /// <returns>The enum value corresponding to the unsigned integer value.</returns>
        public static T FromUInt64<T>(ulong value) where T : struct, Enum
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        // Flag Operations
        private static readonly Dictionary<Type, IReadOnlyCollection<string>> _enumNamesCache = new Dictionary<Type, IReadOnlyCollection<string>>();

        /// <summary>
        /// Gets the flags contained in the specified enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>An IEnumerable of type T containing the flags.</returns>
        public static IEnumerable<T> GetFlags<T>(this T enumValue) where T : struct, Enum
        {
            if (!_enumNamesCache.TryGetValue(typeof(T), out var enumNames))
            {
                enumNames = Enum.GetNames(typeof(T)).ToList();
                _enumNamesCache[typeof(T)] = enumNames;
            }

            foreach (var name in enumNames)
            {
                if (Enum.TryParse(name, out T value) && enumValue.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Determines whether the enumValue has any of the flagsToCheck set.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="flagsToCheck">The flags to check against the enumValue.</param>
        /// <returns>Returns true if the enumValue has any of the flagsToCheck set; otherwise, false.</returns>
        public static bool HasAnyFlag<T>(this T enumValue, T flagsToCheck) where T : struct, Enum
        {
            return (enumValue.ToUInt64() & flagsToCheck.ToUInt64()) != 0;
        }

        /// <summary>
        /// Checks whether the specified enum value has all the specified flags set.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="flagsToCheck">The flags to check.</param>
        /// <returns>True if the enum value has all the specified flags set; otherwise, false.</returns>
        public static bool HasAllFlags<T>(this T enumValue, T flagsToCheck) where T : struct, Enum
        {
            return (enumValue.ToUInt64() & flagsToCheck.ToUInt64()) == flagsToCheck.ToUInt64();
        }

        /// <summary>
        /// Determines whether the specified flag is set in the enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="flag">The flag to check.</param>
        /// <returns><see langword="true"/> if the flag is set; otherwise, <see langword="false"/>.</returns>
        public static bool HasFlag<T>(this T enumValue, T flag) where T : struct, Enum
        {
            return (enumValue.ToUInt64() & flag.ToUInt64()) != 0;
        }

        /// <summary>
        /// Sets the specified flags on the enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to set the flags for.</param>
        /// <param name="flagsToSet">The flags to set on the enum value.</param>
        /// <returns>The enum value with the specified flags set.</returns>
        public static T SetFlags<T>(this T enumValue, T flagsToSet) where T : struct, Enum
        {
            return FromUInt64<T>(enumValue.ToUInt64() | flagsToSet.ToUInt64());
        }

        /// <summary>
        /// Clears the specified flags from the enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to clear the flags from.</param>
        /// <param name="flagsToClear">The flags to clear.</param>
        /// <returns>The enum value with the specified flags cleared.</returns>
        public static T ClearFlags<T>(this T enumValue, T flagsToClear) where T : struct, Enum
        {
            return FromUInt64<T>(enumValue.ToUInt64() & ~flagsToClear.ToUInt64());
        }

        /// <summary>
        /// Toggles the specified flags in an enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="flagsToToggle">The flags to toggle.</param>
        /// <returns>The enum value with the toggled flags.</returns>
        public static T ToggleFlags<T>(this T enumValue, T flagsToToggle) where T : struct, Enum
        {
            return FromUInt64<T>(enumValue.ToUInt64() ^ flagsToToggle.ToUInt64());
        }

        /// <summary>
        /// Combines multiple flags into a single value.
        /// </summary>
        /// <typeparam name="T">The type of the enum flags.</typeparam>
        /// <param name="flags">The flags to combine.</param>
        /// <returns>The combined flags as a single value of type T.</returns>
        public static T CombineFlags<T>(params T[] flags) where T : struct, Enum
        {
            ulong combinedValue = 0;
            foreach (T flag in flags)
            {
                combinedValue |= flag.ToUInt64();
            }
            return FromUInt64<T>(combinedValue);
        }

        /// <summary>
        /// Combines multiple flags of an enumeration using bitwise OR operation and returns the result.
        /// If no flags are provided, the default value is returned.
        /// </summary>
        /// <typeparam name="T">The enumeration type</typeparam>
        /// <param name="defaultValue">The default value to return if no flags are provided</param>
        /// <param name="flags">The flags to combine</param>
        /// <returns>The result of combining the flags</returns>
        public static T CombineFlagsOrDefault<T>(T defaultValue, params T[] flags) where T : struct, Enum
        {
            return flags?.Length > 0 ? CombineFlags(flags) : defaultValue;
        }

        // Enum Information

        /// <summary>
        /// Gets an IEnumerable of type T containing the names of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of type string containing the names of the enumeration values.</returns>
        public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Gets an IEnumerable of type T containing all the values of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of type T containing all the values of the enumeration.</returns>
        public static IEnumerable<T> GetValues<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Parses the specified string value into an enumeration value of type T.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed enumeration value of type T.</returns>
        public static T Parse<T>(string value) where T : struct, Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Tries to parse the specified value as an enum of type T.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">When this method returns, contains the enum value.
        /// If the parsing succeeded, this parameter is set to the parsed value;
        /// otherwise, it is set to the default value of the enum type.</param>
        /// <returns>true if the parsing succeeded; otherwise, false.</returns>
        public static bool TryParse<T>(string value, out T result) where T : struct, Enum
        {
            return Enum.TryParse(value, out result);
        }

        /// <summary>
        /// Converts the specified enumeration value to a string representation using an Unsafe Fast method.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>A string representation of the enumeration value.</returns>
        public static string ToStringFast<T>(this T enumValue) where T : struct, Enum
        {
            return Unsafe.As<T, int>(ref enumValue).ToString();
        }

        // Enum Navigation
        /// <summary>
        /// Returns the next value in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The current value of the enumeration.</param>
        /// <returns>The next value in the enumeration.</returns>
        public static T Next<T>(this T enumValue) where T : struct, Enum
        {
            var values = GetEnumValues<T>();
            int index = Array.IndexOf(values, enumValue) + 1;
            return index >= values.Length ? values[0] : values[index];
        }

        /// <summary>
        /// Returns the previous value of an enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The current enumeration value.</param>
        /// <returns>The previous value of the enumeration.</returns>
        public static T Previous<T>(this T enumValue) where T : struct, Enum
        {
            var values = GetEnumValues<T>();
            int index = Array.IndexOf(values, enumValue) - 1;
            return index < 0 ? values[^1] : values[index];
        }

        /// <summary>
        /// Returns an IEnumerable of type T containing the values within the specified range.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="startInclusive">The starting value of the range (inclusive).</param>
        /// <param name="endExclusive">The ending value of the range (exclusive).</param>
        /// <returns>An IEnumerable of type T containing the values within the specified range.</returns>
        public static IEnumerable<T> Range<T>(T startInclusive, T endExclusive) where T : struct, Enum
        {
            var values = GetEnumValues<T>();
            int startIndex = Array.IndexOf(values, startInclusive);
            int endIndex = Array.IndexOf(values, endExclusive);

            if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
            {
                throw new ArgumentException("Invalid range specified.");
            }

            return values[startIndex..endIndex];
        }

        private static readonly Dictionary<Type, Array> _enumValuesCache = new Dictionary<Type, Array>();

        private static T[] GetEnumValues<T>() where T : Enum
        {
            if (!_enumValuesCache.TryGetValue(typeof(T), out var values))
            {
                values = Enum.GetValues(typeof(T));
                _enumValuesCache[typeof(T)] = values;
            }
            return (T[])values;
        }

        // Enum Dictionaries

        /// <summary>
        /// Converts an enumeration to a dictionary where the keys are the enumeration names as strings and the values are the enumeration values.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>A dictionary with the enumeration names as keys and the enumeration values as values.</returns>
        public static IReadOnlyDictionary<string, T> ToDictionary<T>() where T : struct, Enum
        {
            return GetValues<T>().ToDictionary(e => e.ToString(), e => e);
        }

        /// <summary>
        /// Converts an enumeration to a dictionary where the keys are the enumeration values and the values are their string representations.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>A dictionary where the keys are the enumeration values and the values are their string representations.</returns>
        public static IReadOnlyDictionary<T, string> ToValueDictionary<T>() where T : struct, Enum
        {
            return GetValues<T>().ToDictionary(e => e, e => e.ToString());
        }

        // Enum Min/Max/Count

        /// <summary>
        /// Gets the maximum value of the specified enumeration type.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>The maximum value of type T.</returns>
        public static T MaxValue<T>() where T : struct, Enum
        {
            return GetValues<T>().Max();
        }

        /// <summary>
        /// Gets the minimum value of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>The minimum value of type T.</returns>
        public static T MinValue<T>() where T : struct, Enum
        {
            return GetValues<T>().Min();
        }

        /// <summary>
        /// Returns the count of values in the specified enumeration type.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>The count of values in the specified enumeration type.</returns>
        public static int Count<T>() where T : struct, Enum
        {
            return GetValues<T>().Count();
        }
        // Enum Validation

        /// <summary>
        /// Determines whether the specified value is a defined member of the specified enumeration type.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>true if the specified value is a defined member of the enumeration type; otherwise, false.</returns>
        public static bool IsDefined<T>(T enumValue) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }

        /// <summary>
        /// Checks if the specified name is a valid name for the given enumeration type.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="name">The name to check.</param>
        /// <returns>True if the name is a valid name for the enumeration type, otherwise false.</returns>
        public static bool IsValidName<T>(string name) where T : struct, Enum
        {
            return Enum.GetNames(typeof(T)).Contains(name);
        }

        /// <summary>
        /// Checks if the specified value is a valid value of the given enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is a valid value of the enumeration; otherwise, false.</returns>
        public static bool IsValidValue<T>(int value) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }

        // Enum Display Names and Descriptions

        /// <summary>
        /// A cache that stores display names for enums.
        /// </summary>
        private static readonly Dictionary<Type, string[]> _displayNamesCache = new Dictionary<Type, string[]>();

        /// <summary>
        /// Gets the display names of the enumeration values.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of strings containing the display names of the enumeration values.</returns>
        public static IEnumerable<string> GetDisplayNames<T>() where T : struct, Enum
        {
            if (_displayNamesCache.TryGetValue(typeof(T), out var displayNames)) return displayNames;

            displayNames = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.Name ?? f.Name)
                .ToArray();
            _displayNamesCache[typeof(T)] = displayNames;
            return displayNames;
        }

        /// <summary>
        /// Gets the display name of the specified enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>The display name of the specified enumeration value.</returns>
        public static string GetDisplayName<T>(this T enumValue) where T : struct, Enum
        {
            return typeof(T).GetField(enumValue.ToString())
                ?.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.Name ?? enumValue.ToString();
        }

        /// <summary>
        /// Gets the descriptions of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>
        /// An IEnumerable of type string containing the descriptions of the enumeration values.
        /// </returns>
        public static IEnumerable<string> GetDescriptions<T>() where T : struct, Enum
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? f.Name);
        }

        /// <summary>
        /// Gets the description attribute of the specified enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>The description attribute of the enumeration value, or the string representation of the enumeration value if no description attribute is found.</returns>
        public static string GetDescription<T>(this T enumValue) where T : struct, Enum
        {
            return typeof(T).GetField(enumValue.ToString())
                ?.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? enumValue.ToString();
        }

        /// <summary>
        /// Gets the display pairs of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of KeyValuePair<T, string> representing the display pairs, where the key is an enumeration value and the value is its display name.</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDisplayPairs<T>() where T : struct, Enum
        {
            return GetValues<T>().Select(e => new KeyValuePair<T, string>(e, e.GetDisplayName()));
        }

        /// <summary>
        /// Gets an IEnumerable of KeyValuePair<T, string> that contains the enumeration value and its corresponding description for all values of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of KeyValuePair<T, string> containing the enumeration value and its corresponding description.</returns>
        public static IEnumerable<KeyValuePair<T, string>> GetDescriptionPairs<T>() where T : struct, Enum
        {
            return GetValues<T>().Select(e => new KeyValuePair<T, string>(e, e.GetDescription()));
        }
    }
}