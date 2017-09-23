using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DotCyToolboxPlugins.Utils {

    public static class ExtensionMethods {

        #region Method: IsEnum (public - static - overloaded - extension method)
        public static bool IsEnum<T>(this string s) {
            return Enum.IsDefined(typeof(T), s);
        }
        public static bool IsEnum<T>(this int i) {
            return Enum.IsDefined(typeof(T), i);
        }
        #endregion

        #region Method: ToEnumSafe (public - static - overloaded - extension method)
        public static T? ToEnumSafe<T>(this string s) where T : struct {
            return (IsEnum<T>(s) ? (T?)Enum.Parse(typeof(T), s) : null);
        }
        public static T? ToEnumSafe<T>(this int i) where T : struct {
            return (IsEnum<T>(i) ? (T?)Enum.Parse(typeof(T), i.ToString()) : null);
        }
        public static T? ToEnumSafe<T>(this Nullable<int> i) where T : struct {
            if (i.HasValue) {
                return (IsEnum<T>(i.Value) ? (T?)Enum.Parse(typeof(T), i.Value.ToString()) : null);
            } else {
                return null;
            }
        }
        #endregion

        #region Method: ToEnum (public - static - extension method)
        public static T ToEnum<T>(this int i) where T : struct {
            return (T)Enum.Parse(typeof(T), i.ToString());
        }
        #endregion

        #region Method: GetFriendlyName (public - static - extension method)
        public static string GetFriendlyName<T>(this Enum enumerationValue) where T : struct {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum) {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0) {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0 && attrs.Where(t => t.GetType() == typeof(DescriptionAttribute)).FirstOrDefault() != null) {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs.Where(t => t.GetType() == typeof(DescriptionAttribute)).FirstOrDefault()).Description;
                }
            }
            //If we have no description attribute, just return the name
            return Enum.GetName(type, enumerationValue);
        }
        #endregion

        #region Method: TryParseGuid (public - static - extension method)
        /// <summary>
        /// extension method to a string to return a nullable guid from the parsing of the string
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static Nullable<Guid> TryParseGuid(this string sourceStr) {
            Guid Val;
            if (string.IsNullOrWhiteSpace(sourceStr) || !Guid.TryParse(sourceStr, out Val))
                return null;
            else
                return Val;
        }
        #endregion

        #region Method: TryParseLong (public - static - extension method)
        /// <summary>
        /// extension method to a string to return a nullable long from the parsing of the string
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static Nullable<long> TryParseLong(this string sourceStr) {
            long Val;
            if (string.IsNullOrWhiteSpace(sourceStr) || !long.TryParse(sourceStr, out Val))
                return null;
            else
                return Val;
        }
        #endregion

        #region Method: TryParseInt (public - static - extension method)
        /// <summary>
        /// extension method to a string to return a nullable integer from the parsing of the string
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static Nullable<int> TryParseInt(this string sourceStr) {
            int Val;
            if (string.IsNullOrWhiteSpace(sourceStr) || !int.TryParse(sourceStr, out Val))
                return null;
            else
                return Val;
        }
        #endregion

        #region Method: ForEach (public - static - overloaded - extension method)
        /// <summary>
        /// Dictionary foreach with linq extension method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (action == null) throw new ArgumentNullException("action");
            foreach (T item in sequence)
                action(item);
        }
        /// <summary>
        /// Dictionary foreach with linq extension method
        /// Return false to stop the loop
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> sequence, Func<T, bool> action) {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in sequence)
                if (!action(item))
                    return;
        }
        #endregion

        #region Method: BreakTextIntoMultipleLines (public - static - extension method)
        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        public static string BreakTextIntoMultipleLines(this string text, int width = 75) {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next) {
                // Find end of line
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos) {
                    do {
                        int len = eol - pos;
                        if (len > width)
                            len = text.BreakLine(pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                } else sb.Append(Environment.NewLine); // Empty line
            }
            return sb.ToString();
        }
        #endregion

        #region Method: BreakLine (public - static - extension method)
        /// <summary>
        /// Locates position to break the given line so as to avoid breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(this string text, int pos, int max) {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }
        #endregion

        #region Method: GetAttributeOfType (public - extension method)
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
        #endregion

        #region Method: StartOfWeek (public - static - extension method)
        /// <summary>
        /// returns the starting date of the week to which the date <param name="dt"></param> belongs to
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0) {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
        #endregion

        #region Method: ToReadableString (public - static - extension method)
        public static string ToReadableString(this TimeSpan span) {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }
        #endregion

        public static string GetBase64(this System.Drawing.Bitmap oBitmap, System.Drawing.Imaging.ImageFormat oFormat) {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
                oBitmap.Save(stream, oFormat);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static string ToSentenceCase(this string str) {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }
    } // Class: ExtensionMethods 

} // namespace: DotCyToolboxPlugins.Utils