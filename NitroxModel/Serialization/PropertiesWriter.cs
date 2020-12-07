﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NitroxModel.Logger;

namespace NitroxModel.Serialization
{
    public static class PropertiesWriter
    {
        private static readonly Dictionary<Type, Dictionary<string, MemberInfo>> typeCache = new Dictionary<Type, Dictionary<string, MemberInfo>>();

        public static T Deserialize<T>() where T : IProperties, new()
        {
            T props = new T();
            if (!File.Exists(props.FileName))
            {
                return props;
            }

            Dictionary<string, MemberInfo> typeCachedDict = GetTypeCacheDictionary<T>();
            using StreamReader reader = new StreamReader(new FileStream(props.FileName, FileMode.Open), Encoding.UTF8);

            char[] lineSeparator = { '=' };
            int lineNum = 0;
            string readLine;
            while ((readLine = reader.ReadLine()) != null)
            {
                lineNum++;
                if (readLine.Length < 1 || readLine[0] == '#')
                {
                    continue;
                }

                if (readLine.Contains('='))
                {
                    string[] keyValuePair = readLine.Split(lineSeparator, 2);
                    // Ignore case for property names in file.
                    if (!typeCachedDict.TryGetValue(keyValuePair[0].ToLowerInvariant(), out MemberInfo member))
                    {
                        Log.Warn($"Property or field {keyValuePair[0]} does not exist on type {typeof(T).FullName}!");
                        continue;
                    }

                    if (!SetMemberValue(props, member, keyValuePair[1]))
                    {
                        (Type type, object value) data = member switch
                        {
                            FieldInfo field => (field.FieldType, field.GetValue(props)),
                            PropertyInfo prop => (prop.PropertyType, prop.GetValue(props)),
                            _ => (typeof(string), "")
                        };
                        Log.Warn($@"Property ""({data.type.Name}) {member.Name}"" has an invalid value {StringifyValue(keyValuePair[1])} on line {lineNum}. Using default value: {StringifyValue(data.value)}");
                    }
                }
                else
                {
                    Log.Error($"Incorrect format detected on line {lineNum} in {Path.GetFullPath(props.FileName)}:{Environment.NewLine}{readLine}");
                }
            }

            return props;
        }

        public static void Serialize<T>(T props) where T : IProperties, new()
        {
            Dictionary<string, MemberInfo> typeCachedDict = GetTypeCacheDictionary<T>();

            using StreamWriter stream = new StreamWriter(new FileStream(props.FileName, FileMode.OpenOrCreate), Encoding.UTF8);
            WritePropertyDescription(typeof(T), stream);

            foreach (string name in typeCachedDict.Keys)
            {
                MemberInfo member = typeCachedDict[name];

                FieldInfo field = member as FieldInfo;
                if (field != null)
                {
                    WritePropertyDescription(member, stream);
                    WriteProperty(field, field.GetValue(props), stream);
                }

                PropertyInfo property = member as PropertyInfo;
                if (property != null)
                {
                    WritePropertyDescription(member, stream);
                    WriteProperty(property, property.GetValue(props), stream);
                }
            }
        }

        private static Dictionary<string, MemberInfo> GetTypeCacheDictionary<T>()
        {
            if (!typeCache.TryGetValue(typeof(T), out Dictionary<string, MemberInfo> typeCachedDict))
            {
                IEnumerable<MemberInfo> members = typeof(T).GetFields()
                                                           .Where(f => f.Attributes != FieldAttributes.NotSerialized)
                                                           .Concat(typeof(T).GetProperties()
                                                                            .Where(p => p.CanWrite)
                                                                            .Cast<MemberInfo>());

                try
                {
                    typeCachedDict = new Dictionary<string, MemberInfo>();
                    foreach (MemberInfo member in members)
                    {
                        typeCachedDict.Add(member.Name.ToLowerInvariant(), member);
                    }
                }
                catch (ArgumentException e)
                {
                    Log.Error(e, $"Type {typeof(T).FullName} has properties that require case-sensitivity to be unique which is unsuitable for .properties format.");
                    throw;
                }

                typeCache.Add(typeof(T), typeCachedDict);
            }
            return typeCachedDict;
        }

        private static string StringifyValue(object value)
        {
            return value switch
            {
                string _ => $@"""{value}""",
                null => @"""""",
                _ => value.ToString()
            };
        }

        private static bool SetMemberValue<T>(T instance, MemberInfo member, string valueFromFile)
        {
            object ConvertFromStringOrDefault(Type typeOfValue, out bool isDefault, object defaultValue = default)
            {
                try
                {
                    object newValue = TypeDescriptor.GetConverter(typeOfValue).ConvertFrom(valueFromFile);
                    isDefault = false;
                    return newValue;
                }
                catch (Exception)
                {
                    isDefault = true;
                    return defaultValue;
                }
            }

            bool usedDefault;
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(instance, ConvertFromStringOrDefault(field.FieldType, out usedDefault, field.GetValue(instance)));
                    return !usedDefault;
                case PropertyInfo prop:
                    prop.SetValue(instance, ConvertFromStringOrDefault(prop.PropertyType, out usedDefault, prop.GetValue(instance)));
                    return !usedDefault;
                default:
                    throw new Exception($"Serialized member must be field or property: {member}.");
            }
        }

        private static void WriteProperty<T>(T member, object value, StreamWriter stream) where T : MemberInfo
        {
            stream.Write(member.Name);
            stream.Write("=");
            stream.WriteLine(value);
        }

        private static void WritePropertyDescription(MemberInfo member, StreamWriter stream)
        {
            PropertyDescriptionAttribute attribute = member.GetCustomAttribute<PropertyDescriptionAttribute>();
            if (attribute != null)
            {
                foreach (string line in attribute.Description.Split(Environment.NewLine.ToCharArray()))
                {
                    stream.Write("# ");
                    stream.WriteLine(line);
                }
            }
        }
    }
}
