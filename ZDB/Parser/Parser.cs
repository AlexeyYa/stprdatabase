/*

Copyright 2019 Yamborisov Alexey

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Xceed.Words.NET;

using ZDB.Database;

namespace ZDB.Parser
{
    static class Parser
    {
        public static string template;
        public static Dictionary<string, int> subs;
        private static List<DocTemplate> Templates;

        public static void Initialize()
        {
            // Trying to read existing data
            if (File.Exists(Consts.TemplatePath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Consts.TemplatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Templates = (List<DocTemplate>)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                Templates = new List<DocTemplate>();
            }
        }

        /// <summary>
        /// Adding template and saving it to serialized file
        /// </summary>
        /// <param name="filename"></param>
        public static void AddTemplate(string filename)
        {
            DocTemplate docTemplate = Parser.CreateTemplate(filename);
            Templates.Add(docTemplate);
            Stream stream = new FileStream(Consts.TemplatePath, FileMode.Create, FileAccess.Write, FileShare.None);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Templates);
            stream.Close();
        }

        /// <summary>
        /// Parsing entry from request file through regex
        /// </summary>
        /// <param name="path"></param>
        /// <param name="num"></param>
        /// <returns>Entry</returns>
        public static Entry ProcessFromDocx(string path, int num)
        {
            DocX document = DocX.Load(path);
            Entry entry = new Entry(num);
            Regex rx;
            MatchCollection match;
            foreach (DocTemplate dt in Templates)
            {
                rx = new Regex(dt.RegExData);

                match = rx.Matches(document.Text);

                if (match.Count == 0)
                {
                    continue;
                }
                // Parsing temporary variables
                string[] dateformats = { "yyyy-MM-dd HH:mm" };
                DateTime date;
                int tmp;
                List<string> corrections = new List<string>();

                // Processing each field
                foreach (string sub in dt.Substitutions.Keys)
                {
                    switch (sub)
                    {
                        case "#DATE#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value.Contains("Срок"))
                            {
                                string s = match[0].Groups[dt.Substitutions[sub]].Value;
                                int tmpidx = s.IndexOf(" Срок");
                                string s1 = s.Substring(0, tmpidx);
                                string s2 = s.Substring(s.IndexOf(' ', tmpidx + 1) + 1);
                                bool b = DateTime.TryParseExact(s.Substring(0, tmpidx), dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out date);
                                entry.StartDate = date;
                                DateTime.TryParseExact(s.Substring(s.IndexOf(' ', tmpidx + 1) + 1), dateformats,
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out date);
                                entry.EndDate = date;
                            }
                            else
                            {
                                DateTime.TryParseExact(match[0].Groups[dt.Substitutions[sub]].Value, dateformats,
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out date);
                                entry.StartDate = date;
                            }
                            break;
                        case "#ENDDATE#":
                            DateTime.TryParseExact(match[0].Groups[dt.Substitutions[sub]].Value, dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out date);
                            entry.EndDate = date;
                            break;
                        case "#FIO#":
                            entry.User = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#GROUP#":
                            entry.Group = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        //case "#PHONE#":
                        //    content.Phone = match[0].Groups[dt.Substitutions[sub]].Value;
                        //    break;
                        case "#DOCCODE#":
                            entry.DocCode = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#OBJCODE#":
                            entry.Obj = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#SUBS#":
                            entry.Subs = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#ORIG#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.NumberOfOriginals = tmp;
                            break;
                        case "#COPY#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.NumberOfCopies = tmp;
                            break;
                        case "#FGM#":
                            entry.Tasks = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;

                        case "#SF#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeFormat = tmp;
                            break;
                        case "#SA4#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeA4 = tmp;
                            break;
                        case "#SA3#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeA3 = tmp;
                            break;
                        case "#SA2#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeA2 = tmp;
                            break;
                        case "#SA1#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeA1 = tmp;
                            break;
                        case "#SA0#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeA0 = tmp;
                            break;

                        case "#CF#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorFormat = tmp;
                            break;
                        case "#CA4#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorA4 = tmp;
                            break;
                        case "#CA3#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorA3 = tmp;
                            break;
                        case "#CA2#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorA2 = tmp;
                            break;
                        case "#CA1#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorA1 = tmp;
                            break;
                        case "#CA0#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            entry.SizeCorA0 = tmp;
                            break;

                        case "#1-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("1-1");
                            }
                            break;
                        case "#1-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("1-2");
                            }
                            break;
                        case "#1-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("1-3");
                            }
                            break;
                        case "#1-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("1-4");
                            }
                            break;
                        case "#1-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("1-5");
                            }
                            break;
                        case "#2-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("2-1");
                            }
                            break;
                        case "#2-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("2-2");
                            }
                            break;
                        case "#2-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("2-3");
                            }
                            break;
                        case "#2-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("2-4");
                            }
                            break;
                        case "#2-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("2-5");
                            }
                            break;
                        case "#3-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("3-1");
                            }
                            break;
                        case "#3-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("3-2");
                            }
                            break;
                        case "#3-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("3-3");
                            }
                            break;
                        case "#3-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("3-4");
                            }
                            break;
                        case "#3-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("3-5");
                            }
                            break;
                        case "#4-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("4-1");
                            }
                            break;
                        case "#4-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("4-2");
                            }
                            break;
                        case "#4-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("4-3");
                            }
                            break;
                        case "#4-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("4-4");
                            }
                            break;
                        case "#4-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("4-5");
                            }
                            break;
                        case "#5-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("5-1");
                            }
                            break;
                        case "#5-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("5-2");
                            }
                            break;
                        case "#5-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("5-3");
                            }
                            break;
                        case "#5-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("5-4");
                            }
                            break;
                        case "#5-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("5-5");
                            }
                            break;
                        case "#6-1#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("6-1");
                            }
                            break;
                        case "#6-2#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("6-2");
                            }
                            break;
                        case "#6-3#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("6-3");
                            }
                            break;
                        case "#6-4#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("6-4");
                            }
                            break;
                        case "#6-5#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("6-5");
                            }
                            break;
                        case "#7#":
                            if (match[0].Groups[dt.Substitutions[sub]].Value != "")
                            {
                                corrections.Add("7");
                            }
                            break;

                        default:
                            break;
                    }
                }
                entry.Corrections = String.Join(",", corrections);
                break;
            }
            return entry;
        }

        /// <summary>
        /// Reads file on path and turns into regex
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static DocTemplate CreateTemplate(string path)
        {
            DocX document = DocX.Load(path);
            string temp = document.Text;
            // Replace regex characters
            temp = temp.Replace("(", @"\(");
            temp = temp.Replace(")", @"\)");
            temp = temp.Replace("+", @"\+");
            bool flag = false;
            string sub = "";
            int idx = 1;
            // Dictionary of substitutions < Tag , Position in regex >
            Dictionary<string, int> substitutions = new Dictionary<string, int>();
            foreach (char c in temp)
            {
                if (flag)
                {
                    sub += c;
                    if (c == '#')
                    {
                        substitutions.Add(sub, idx++); // idx increase after addition
                        sub = "";
                        flag = false;
                    }
                }
                else
                {
                    if (c == '#')
                    {
                        sub += c;
                        flag = true;
                    }
                }
            }

            foreach (string s in substitutions.Keys)
            {
                temp = temp.Replace(s, @"(.*)");
            }

            subs = substitutions;
            template = temp;
            DocTemplate result = new DocTemplate(template, substitutions);
            return result;
        }
    }

    /// <summary>
    /// Storing all data needed for processing requests
    /// regExData stores string version document with matching pattern
    /// substitutions stores tag and position for matching patter 
    /// </summary>
    [Serializable]
    class DocTemplate
    {
        private string regExData;
        private Dictionary<string, int> substitutions = new Dictionary<string, int>();

        public string RegExData { get => regExData; set => regExData = value; }
        public Dictionary<string, int> Substitutions { get => substitutions; set => substitutions = value; }

        public DocTemplate() { }
        public DocTemplate(string regex, Dictionary<string, int> subsDict)
        {
            RegExData = regex;
            Substitutions = subsDict;
        }
    }
}
