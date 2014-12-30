using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VSPerformanceTracker.FSInterface;

namespace VSPerformanceTracker.Logging
{
    /// <summary>
    /// Serialize objects to Comma Separated Value (CSV) format [1].
    ///
    /// [1] http://tools.ietf.org/html/rfc4180
    /// </summary>
    public class CsvSerializer : ISerializer
    {
        private const string CsvSeparator = ",";

        private readonly IWritableFile _outputFile;

        public CsvSerializer(IWritableFile outputFile)
        {
            _outputFile = outputFile;
        }

        public void SerializeRecord<T>(T record)
        {
            var fields = GetFields(typeof(T));

            var needsHeader = !_outputFile.Exists();

            using (var writer = _outputFile.OpenWriter())
            {
                if (needsHeader)
                    WriteHeader(fields, writer);

                writer.WriteLine(QuoteRecord(FormatObject(fields, record)));
            }
        }

        private static void WriteHeader(IEnumerable<MemberInfo> fields, StreamWriter writer)
        {
            writer.WriteLine(QuoteRecord(fields.Select(f => f.Name)));
        }

        private static IEnumerable<MemberInfo> GetFields(Type type)
        {
            return
                from mi in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                where new[] { MemberTypes.Field, MemberTypes.Property }.Contains(mi.MemberType)
                let orderAttr = (ColumnOrderAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnOrderAttribute))
                orderby orderAttr == null ? int.MaxValue : orderAttr.Order, mi.Name
                select mi;
        }

        private static IEnumerable<string> FormatObject<T>(IEnumerable<MemberInfo> fields, T record)
        {
            foreach (var field in fields)
                yield return Convert.ToString(GetValue(field, record));
        }

        private static object GetValue<T>(MemberInfo field, T record)
        {
            if (field is FieldInfo)
                return ((FieldInfo)field).GetValue(record);
            else if (field is PropertyInfo)
                return ((PropertyInfo)field).GetValue(record, null);
            else
                throw new ArgumentException(string.Format("Unhandled field type '{0}'", field.GetType().Name));
        }

        private static string QuoteRecord(IEnumerable<string> record)
        {
            return String.Join(CsvSeparator, record.Select(field => QuoteField(field)).ToArray());
        }

        private static string QuoteField(string field)
        {
            if (String.IsNullOrEmpty(field))
                return "\"\"";
            else if (field.Contains(CsvSeparator) || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
                return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
            else
                return field;
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class ColumnOrderAttribute : Attribute
        {
            public int Order { get; private set; }
            public ColumnOrderAttribute(int order) { Order = order; }
        }
    }
}
