﻿// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Arctium.Core.Configuration
{
    public abstract class ConfigBase<TDerived>
    {
        static Dictionary<string, object> configEntries;

        public static void Initialize(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException(configFile);

            var fileContent = File.ReadAllText(configFile);

            configEntries = new Dictionary<string, object>();

            // Get key,key/value pair config options.
            foreach (Match m in Regex.Matches(fileContent, @"^([^\W].*)[^\S\r\n]?=\s*\[\s?([^\]]*)\]", RegexOptions.Multiline))
            {
                var key = m.Groups[1].Value.Trim();
                var value = new Dictionary<string, string>();

                foreach (Match m2 in Regex.Matches(m.Captures[0].Value.Trim(',', '\r', '\n'), @"([^\W].*)[^\S\r\n]?:[^\S\r\n]?(.*)$", RegexOptions.Multiline))
                {
                    var subKey = m2.Groups[1].Value.Trim();
                    var subValue = m2.Groups[2].Value.Trim(',', '"', '\r', '\n');

                    if (value.ContainsKey(subKey))
                    {
                        Console.WriteLine($"Replacing value for '{key}.{subKey}'.");

                        value[subKey] = subValue;
                    }
                    else
                        value.Add(subKey, subValue);
                }

                if (configEntries.ContainsKey(key))
                {
                    Console.WriteLine($"Duplicate entry found for '{key}'.");

                    configEntries[key] = value;
                }
                else
                    configEntries.Add(key, value);
            }

            // Get single line config options.
            foreach (Match m in Regex.Matches(fileContent, @"^([^\W].*)[^\S\r\n]?=[^\S\r\n](.*)$", RegexOptions.Multiline))
            {
                var key = m.Groups[1].Value.Trim();
                var value = m.Groups[2].Value.Trim(',', '"', '\r', '\n');

                if (configEntries.ContainsKey(key))
                    configEntries[key] = value;
                else
                    configEntries.Add(key, value);
            }

            AssignValues();
        }

        static void AssignValues()
        {
            var configEntryFields = typeof(TDerived).GetProperties().Where(f => f.GetCustomAttribute<ConfigEntryAttribute>() != null);

            foreach (var field in configEntryFields)
            {
                object fieldValue = null;

                var configEntryAttribute = field.GetCustomAttribute<ConfigEntryAttribute>();

                if (!configEntries.TryGetValue(configEntryAttribute.Name, out var value))
                {
                    Console.WriteLine($"Can't find config entry '{configEntryAttribute.Name}'.");

                    fieldValue = configEntryAttribute.DefaultValue;
                }

                // Assign config options if found.
                if (fieldValue == null)
                    field.AssignValue(null, value);
            }
        }
    }
}
