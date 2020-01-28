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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ZDB.Database;

namespace ZDB.Exp
{

    static class Export
    {
        //public static void ExportMain(IEnumerable<Content> data, IEnumerable<IFilter> ExportFilters, 
        //            IEnumerable<string> ExportField, string ExportFilePath)
        //{
        //    if (File.Exists(ExportFilePath))
        //    {
        //        File.Delete(ExportFilePath);
        //    }


        //}

        public static void ExportMain(IEnumerable<Entry> data, FilterCollection ExportFilters,
                                      ExportSetting exportSetting, string ExportFolderPath)
        {
            List<string> exportFields = new List<string>();
            foreach (FieldsEntryViewmodel f in exportSetting.Columns)
            {
                exportFields.Add(f.Field);
            }

            List<Entry> filtered = data.Where(x => ExportFilters.Filter(x)).ToList(); //FilterContent(data, ExportFilters);
            //filtered = SortEntries(filtered, exportSetting.SortBy); /// Check if it works properly

            // Grouping
            if (exportSetting.Partitions.Count == 0)
            {
                ExportCVS(filtered, exportFields, ExportFolderPath + "\\output.csv");
            } else
            {
                ExportGroups(filtered, exportFields, exportSetting, ExportFolderPath);
            }

        }

        private static void ExportGroups(IEnumerable<Entry> data, IEnumerable<string> exportFields,
                                      ExportSetting exportSetting, string ExportFolderPath)
        {
            List<string> gFieldList = new List<string>();
            List<string> gFieldListIns = new List<string>();
            foreach (PartitionsEntryViewmodel p in exportSetting.Partitions)
            {
                if (p.Partition == PARTITION_TYPE.SAME_DIRECTORY)
                {
                    gFieldList.Add(p.Field);
                }
                else if (p.Partition == PARTITION_TYPE.SAME_PAGE)
                {
                    gFieldListIns.Add(p.Field);
                }
            }

            // Putting into different files
            if (gFieldList.Count > 0)
            {
                if (gFieldListIns.Count > 0)
                {
                    string gFieldQuery = string.Join(", ", gFieldList.Select(x => "it[\"" + x + "\"] as " + x));
                    var grouped = data.GroupBy("new(" + gFieldQuery + ")", "it")
                                          .Select("new (it.Key as Key, it as Item)");
                    foreach (dynamic g in grouped)
                    {
                        string filename = GroupingToFilename(g.Key, gFieldList);
                        ExportGroupsSameFile(g.Item, exportFields, gFieldListIns, ExportFolderPath + "\\" + filename + ".csv");
                    }
                }
                else
                {
                    string gFieldQuery = string.Join(", ", gFieldList.Select(x => "it[\"" + x + "\"] as " + x));
                    var grouped = data.GroupBy("new(" + gFieldQuery + ")", "it")
                                          .Select("new (it.Key as Key, it as Item)");
                    foreach (dynamic g in grouped)
                    {
                        string filename = GroupingToFilename(g.Key, gFieldList);
                        ExportCVS(g.Item, exportFields, ExportFolderPath + "\\" + filename + ".csv");
                    }
                }
            }
            else
            {
                ExportGroupsSameFile(data, exportFields, gFieldListIns, ExportFolderPath + "\\output.csv");
            }
        }

        private static void ExportGroupsSameFile(IEnumerable<Entry> data, IEnumerable<string> exportFields,
                                      IEnumerable<string> gFieldList, string ExportFile)
        {
            string gFieldQueryIns = string.Join(", ", gFieldList.Select(x => "it[\"" + x + "\"] as " + x));
            var grouped = data.GroupBy("new(" + gFieldQueryIns + ")", "it")
                                  .Select("new (it.Key as Key, it as Item)");
            bool first = true;
            foreach (dynamic g in grouped)
            {
                if (first)
                {
                    first = false;
                    ExportCVS(g.Item, exportFields, ExportFile);
                }
                else
                {
                    AppendCVS(g.Item, exportFields, ExportFile);
                }
            }
        }

        /// <summary>
        /// Progress bar
        /// </summary>
        //public static void ImportViewModel(ExportViewModel eVM)
        //{
        //    exportViewModel = eVM;
        //}
        //private static ExportViewModel exportViewModel;
        //private static int total;
        //private static int progress;
        //private static void UpdateProgress() {
        //    progress++;
        //    if (exportViewModel != null)
        //    {
        //        exportViewModel.CurrentProgress = progress / total * 100;
        //    }
        //}

        /// <summary>
        /// Export settings processing
        /// </summary>
        //private static List<Content> FilterContent(IEnumerable<Content> data, IEnumerable<IFilter> ExportFilters)
        //{
        //    List<Content> rVal = new List<Content>();

        //    foreach (Content c in data)
        //    {
        //        bool check = true;
        //        foreach (IFilter f in ExportFilters)
        //        {
        //            if (!(f.Check(c)))
        //            {
        //                check = false;
        //            }
        //        }
        //        if (check)
        //        {
        //            rVal.Add(c);
        //        }
        //    }
        //    return rVal;
        //}
        private static List<Entry> SortContent(List<Entry> data, IEnumerable<SortingEntryViewmodel> sortingEntries)
        {
            if (sortingEntries.Count() > 0)
            {
                List<Tuple<string, bool>> sortings = new List<Tuple<string, bool>>();
                foreach (SortingEntryViewmodel s in sortingEntries)
                {
                    sortings.Add(new Tuple<string, bool>(s.Field, s.Ascending));
                }
                string orderQuery = String.Empty;
                bool firstOrd = true;
                foreach (Tuple<string, bool> t in sortings)
                {
                    if (firstOrd)
                    {
                        firstOrd = false;
                    }
                    else
                    {
                        orderQuery += ", ";
                    }
                    if (t.Item2)
                    {
                        orderQuery += t.Item1 + " ASC";
                    }
                    else
                    {
                        orderQuery += t.Item1 + " DESC";
                    }
                }
                return data.OrderBy(orderQuery).ToList();
            }
            else
            {
                return data;
            }
        }
        private static string GroupingToFilename(dynamic key, IEnumerable<string> gFieldList)
        {
            string result = String.Empty;
            foreach (var v in gFieldList)
            {
                result += SanitizeInput(key.GetType().GetProperty(v).GetValue(key, null).ToString()) + "_";
            }
            if (result.Length > 64)
            {
                result = result.Substring(0, 63);
            }
            return result;
        }
        private static readonly HashSet<char> replaceChars = new HashSet<char>(" ?&^%$#@!:;<>'/\\\"*");
        private static string SanitizeInput(string s)
        {
            StringBuilder result = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (!replaceChars.Contains(c))
                {
                    result.Append(c);
                }
            }
            //return System.Text.RegularExpressions.Regex.Replace(s, "[\"\n/\\:@*?#$%^&]", "-");
            return result.ToString();
        }
        /// <summary>
        /// Common Functions
        /// </summary>
        private static string GetField(Entry c, string f)
        {
            if (Consts.IntFields.Contains(f)) return ((int)c[f]).ToString();
            if (Consts.DateFields.Contains(f)) return ((DateTime)c[f]).ToString();
            if (Consts.EnumFields.ContainsKey(f)) return Consts.EnumFields[f][(int)c[f]];
            return (string)c[f];
        }
        private static int[] accumulator;
        private static void Accumulate(Entry c, IEnumerable<string> ExportFields)
        {
            if (accumulator == null)
            {
                accumulator = new int[ExportFields.Count()];
            }
            int i = 0;
            foreach (string f in ExportFields)
            {
                if (Consts.IntFields.Contains(f) && Consts.Summable.Contains(f))
                {
                    accumulator[i] += (int)c[f];
                }
                i++;
            }
        }
        private static void AccumReset()
        {
            accumulator = null;
        }
        /// <summary>
        /// CSV output
        /// </summary>
        public static void ExportCVS(IEnumerable<Entry> data, IEnumerable<string> ExportFields, string ExportPath)
        {
            if (File.Exists(ExportPath))
            {
                File.Delete(ExportPath);
            }

            using (StreamWriter sw = new StreamWriter(ExportPath, false, Encoding.Unicode))
            {
                sw.Write(CSVHeader(ExportFields, new FieldsTranslated()));
                foreach (Entry c in data)
                {
                    sw.Write(CSVLine(c, ExportFields));
                    Accumulate(c, ExportFields);
                }
                sw.Write(CSVAccumOut(ExportFields));
                AccumReset();
            }
        }

        public static void AppendCVS(IEnumerable<Entry> data, IEnumerable<string> ExportFields, string ExportPath)
        {
            if (File.Exists(ExportPath))
            {
                using (StreamWriter sw = new StreamWriter(ExportPath, true, Encoding.Unicode)) {
                    sw.Write(CSVHeader(ExportFields, new FieldsTranslated()));
                    foreach (Entry c in data)
                    {
                        sw.Write(CSVLine(c, ExportFields));
                        Accumulate(c, ExportFields);
                    }
                    sw.Write(CSVAccumOut(ExportFields));
                    AccumReset();
                }
            }
        }

        /// <summary>
        /// CSV items functions
        /// </summary>
        private static readonly char delimiter = '\t';
        private static string CSVLine(Entry c, IEnumerable<string> ExportFields)
        {
            string result = String.Empty;
            bool first = true;
            foreach (string field in ExportFields)
            {
                string tmpfield = GetField(c, field);
                if (tmpfield.Contains('\n'))
                {
                    tmpfield += '\"' + tmpfield + '\"';
                }
                if (first)
                {
                    result += tmpfield;
                    first = false;
                }
                else
                {
                    result += delimiter + tmpfield;
                }
            }
            result += "\n";
            return result;
        }
        private static string CSVAccumOut(IEnumerable<string> ExportFields)
        {
            string result = String.Empty;
            int i = 0;
            bool first = true;
            foreach (string field in ExportFields)
            {
                
                if (first)
                {
                    first = false;
                }
                else
                {
                    result += delimiter;
                }
                if (Consts.IntFields.Contains(field) && Consts.Summable.Contains(field))
                {
                    result += accumulator[i].ToString();
                }
                i++;
            }
            result += '\n';
            return result;
        }
        private static string CSVHeader(IEnumerable<string> ExportFields, FieldsTranslated Translated)
        {
            string result = String.Empty;
            bool first = true;
            foreach (string field in ExportFields)
            {
                string tmpfield = Translated[field];
                if (tmpfield.Contains('\n'))
                {
                    tmpfield += '\"' + tmpfield + '\"';
                }
                if (first)
                {
                    result += tmpfield;
                    first = false;
                }
                else
                {
                    result += delimiter + tmpfield;
                }
            }
            result += '\n';
            return result;
        }
    }
}
