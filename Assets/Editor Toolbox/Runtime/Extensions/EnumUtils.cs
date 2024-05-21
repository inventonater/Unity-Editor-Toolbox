using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides utility methods for working with enums.
    /// </summary>
    public static class EnumUtils
    {
        private static int ToInt<T>(this T enumValue) where T : struct, Enum
        {
            return Convert.ToInt32(enumValue);
        }

        public static T FromInt<T>(int value) where T : struct, Enum
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static IEnumerable<TFlag> GetFlags<TFlag>(this TFlag flagsValue) where TFlag : struct, Enum
        {
            if (!typeof(TFlag).IsDefined(typeof(FlagsAttribute), false)) throw new ArgumentException($"The type {nameof(TFlag)} is not a [Flags] enum.");

            foreach (var enumValue in GetValues<TFlag>())
            {
                if (flagsValue.HasFlag(enumValue)) yield return enumValue;
            }
        }

        public static TEnum ToEnum<TEnum>(this string name)
            where TEnum : struct, Enum
        {
            if (!Enum.TryParse(name, ignoreCase: false, out TEnum result))
                throw new ArgumentException($"The value '{name}' is not a valid member of the '{nameof(TEnum)}' enum.");

            return result;
        }

        public static string ToName<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return Enum.GetName(typeof(TEnum), value);
        }

        public static IEnumerable<TEnum> FlagsToEnumValues<TFlag, TEnum>(this TFlag flags)
            where TFlag : struct, Enum
            where TEnum : struct, Enum
        {
            if (!typeof(TFlag).IsDefined(typeof(FlagsAttribute), false)) throw new ArgumentException($"The type {nameof(TFlag)} is not a [Flags] enum.");

            foreach (TFlag value in flags.GetFlags())
            {
                var flagName = PrecomputedEnumValues<TFlag>.ValuesToNames[value];
                if (PrecomputedEnumValues<TEnum>.NamesToValues.TryGetValue(flagName, out TEnum enumValue))
                {
                    yield return enumValue;
                }
                else
                {
                    throw new ArgumentException($"{nameof(TFlag)}.{flagName} could not be converted to {nameof(TEnum)}.");
                }
            }
        }

        public static TFlag EnumValueToFlag<TEnum, TFlag>(this TEnum enumValue) where TEnum : struct, Enum where TFlag : struct, Enum
        {
            var enumName = PrecomputedEnumValues<TEnum>.ValuesToNames[enumValue];
            if (!PrecomputedEnumValues<TFlag>.NamesToValues.TryGetValue(enumName, out TFlag flagValue))
                throw new ArgumentException($"{typeof(TEnum)}.{enumName} could not be converted to {typeof(TFlag)}.");
            return flagValue;
        }

        public static TFlag EnumValuesToFlags<TEnum, TFlag>(this IEnumerable<TEnum> enumValues)
            where TEnum : struct, Enum
            where TFlag : struct, Enum
        {
            if (!typeof(TFlag).IsDefined(typeof(FlagsAttribute), false)) throw new ArgumentException($"The type {typeof(TFlag).Name} is not a [Flags] enum.");

            TFlag result = default;
            foreach (TEnum enumValue in enumValues) result.AddFlags(enumValue.EnumValueToFlag<TEnum, TFlag>());
            return result;
        }

        private static class PrecomputedEnumValues<TEnum>
            where TEnum : struct, Enum
        {
            public static IEnumerable<TEnum> Values => _valuesToNames.Keys;
            public static IEnumerable<string> Names => _namesToValues.Keys;
            public static IReadOnlyDictionary<TEnum, string> ValuesToNames => _valuesToNames;
            public static IReadOnlyDictionary<string, TEnum> NamesToValues => _namesToValues;

            private static readonly Dictionary<TEnum, string> _valuesToNames = new();
            private static readonly Dictionary<string, TEnum> _namesToValues = new();

            static PrecomputedEnumValues()
            {
                Type enumType = typeof(TEnum);
                if (!enumType.IsDefined(typeof(FlagsAttribute), false)) throw new ArgumentException($"The type {enumType.Name} is not a [Flags] enum.");

                foreach (TEnum value in Enum.GetValues(enumType))
                {
                    var name = Enum.GetName(enumType, value);
                    _valuesToNames[value] = name;
                    _namesToValues[name] = value;
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
            return (enumValue.ToInt() & flagsToCheck.ToInt()) != 0;
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
            return (enumValue.ToInt() & flagsToCheck.ToInt()) == flagsToCheck.ToInt();
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
            return (enumValue.ToInt() & flag.ToInt()) != 0;
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
            return FromInt<T>(enumValue.ToInt() & ~flagsToClear.ToInt());
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
            return FromInt<T>(enumValue.ToInt() ^ flagsToToggle.ToInt());
        }

        /// <summary>
        /// Combines multiple flags into a single value.
        /// </summary>
        /// <typeparam name="T">The type of the enum flags.</typeparam>
        /// <param name="flags">The flags to combine.</param>
        /// <returns>The combined flags as a single value of type T.</returns>
        public static T AddFlags<T>(params T[] flags) where T : struct, Enum
        {
            int combinedValue = 0;
            foreach (T flag in flags) combinedValue |= flag.ToInt();
            return FromInt<T>(combinedValue);
        }

        public static T AddFlags<T>(this T source, params T[] flags) where T : struct, Enum
        {
            int combinedValue = source.ToInt();
            foreach (T flag in flags) combinedValue |= flag.ToInt();
            return FromInt<T>(combinedValue);
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
            return flags?.Length > 0 ? AddFlags(flags) : defaultValue;
        }

        // Enum Information

        // Flag Operations

        /// <summary>
        /// Gets an IReadOnlyList of type T containing all the values of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of type T containing all the values of the enumeration.</returns>
        public static IEnumerable<T> GetValues<T>() where T : struct, Enum
        {
            return PrecomputedEnumValues<T>.Values;
        }

        /// <summary>
        /// Gets an IReadOnlyList of type T containing the names of the specified enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of type string containing the names of the enumeration values.</returns>
        public static IEnumerable<string> GetNames<T>() where T : struct, Enum
        {
            return PrecomputedEnumValues<T>.Names;
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
            return PrecomputedEnumValues<T>.Names.Contains(name);
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
