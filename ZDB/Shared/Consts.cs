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
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using ZDB.Database;

namespace ZDB
{
    static class Consts
    {
        public static string DGDefaultStylePath = Properties.Settings.Default.defaultMainGridSetting;
        public const string DGSettingsPath = @".\Styles\MainDataGrid\";
        public const string TemplatePath = @".\templates.bin";
        // public const string DatabasePath = @"D:\dev\ZDB.csv";

        public static readonly IEnumerable<string> StrFields = new HashSet<string>
            { "User", "Obj", "Group", "DocCode", "Subs", "Tasks", "Corrections", "Executor" };

        public static readonly IEnumerable<string> IntFields = new HashSet<string>
            { "Number", "CodeType", "NumberOfOriginals", "NumberOfCopies", "Numeration", "Scan", "Threading",
            "SizeRecievedFromGroup", "SizeFormat", "SizeA4", "SizeA3", "SizeA2", "SizeA1", "SizeA0",
            "SizeCorFormat", "SizeCorA4", "SizeCorA3", "SizeCorA2", "SizeCorA1", "SizeCorA0", "TotalFormats"};

        public static readonly IEnumerable<string> Summable = new HashSet<string>
            { "Numeration", "Scan", "Threading", "TotalFormats",
            "SizeRecievedFromGroup", "SizeFormat", "SizeA4", "SizeA3", "SizeA2", "SizeA1", "SizeA0",
            "SizeCorFormat", "SizeCorA4", "SizeCorA3", "SizeCorA2", "SizeCorA1", "SizeCorA0" };

        public static readonly IEnumerable<string> DateFields = new HashSet<string>
            { "StartDate", "EndDate", "CompleteDate" };

        // Dictionaries for EnumFields
        public static readonly Dictionary<int, string> StatusValues = new Dictionary<int, string>
            { {-2, "???" },
              {-1, "Аннулировано" },
              { 0, "" },
              { 1, "В работе" },
              { 2, "Завершено" } };

        // Dictionaries of values should be defined before
        public static readonly Dictionary<string, Dictionary<int, string>> EnumFields = new Dictionary<string, Dictionary<int, string>>
            { {"Status", StatusValues} };

/*        // !!! Converters in MultiBindFields.cs, change together !!!
        public static readonly Dictionary<string, HashSet<string>> MultiBindFields = new Dictionary<string, HashSet<string>>
            { { "TotalFormats",
                new HashSet<string>{ "NumberOfOriginals", "NumberOfCopies",
                "Numeration", "Scan", "Threading", "SizeFormat" }
            } };*/

        public static readonly IEnumerable<string> OnewayFields = new HashSet<string>
            { "TotalFormats", "Number" };
    }

    //public class FieldsList : List<string>
    //{
    //    public FieldsList()
    //    {
    //        Type t = typeof(Entry);
    //        PropertyInfo[] fields = t.GetProperties();

    //        List<string> exceptions = new List<string>
    //        {
    //            "Item"
    //        };

    //        foreach (string f in fields.Select(x => x.Name))
    //        {
    //            if (exceptions.Contains(f))
    //            {
    //                continue;
    //            }
    //            {
    //                this.Add(f);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Using reflection to get all fields
    /// </summary>

    public class FieldsList : List<string>
    {
        public FieldsList()
        {
            Type t = typeof(Entry);
            PropertyInfo[] fields = t.GetProperties();

            List<string> exceptions = new List<string>
            {
                "Item"
            };

            foreach (string f in fields.Select(x => x.Name))
            {
                if (exceptions.Contains(f))
                {
                    continue;
                }
                {
                    this.Add(f);
                }
            }
        }
    }


    enum FilterOperation
    {
        EQUALS,
        NOTEQUALS,
        LESSTHAN,
        GREATERTHAN,
        LESSEQUAL,
        GREATEREQUAL,
        CONTAINS
    }

    /// Translation

    public class FieldsTranslated : Dictionary<string, string>
    {
        public FieldsTranslated()
        {
            this.Add("Number", "Номер");
            this.Add("User", "Заказчик");
            this.Add("Obj", "Объект");
            this.Add("Group", "Группа");
            this.Add("DocCode", "Шифр документации");
            this.Add("Subs", "Субподрядчик");
            this.Add("Status", "Текущий статус");
            this.Add("Tasks", "Задачи");
            this.Add("Corrections", "Причина корректировки");
            this.Add("Executor", "Исполнитель");
            this.Add("CodeType", "Код");
            this.Add("NumberOfOriginals", "Количество оригиналов");
            this.Add("NumberOfCopies", "Количество копий");
            this.Add("TotalFormats", "Всего форматок");
            this.Add("Numeration", "Нумерация");
            this.Add("Scan", "Сканирование");
            this.Add("SizeRecievedFromGroup", "Форматки от группы");
            this.Add("SizeFormat", "Форматки");
            this.Add("SizeA4", "Стр. A4");
            this.Add("SizeA3", "Стр. A3");
            this.Add("SizeA2", "Стр. A2");
            this.Add("SizeA1", "Стр. A1");
            this.Add("SizeA0", "Стр. A0");
            this.Add("SizeCorFormat", "Корректировка форматки");
            this.Add("SizeCorA4", "Кор. A4");
            this.Add("SizeCorA3", "Кор. A3");
            this.Add("SizeCorA2", "Кор. A2");
            this.Add("SizeCorA1", "Кор. A1");
            this.Add("SizeCorA0", "Кор. A0");
            this.Add("StartDate", "Дата заявки");
            this.Add("EndDate", "Планируемая дата");
            this.Add("CompleteDate", "Фактическая дата");
        }
    }

    class FilterTranslated : Dictionary<FilterOperation, string>
    {
        public FilterTranslated()
        {
            this.Add(FilterOperation.EQUALS, "Равно");
            this.Add(FilterOperation.NOTEQUALS, "Не равно");
            this.Add(FilterOperation.LESSTHAN, "Меньше");
            this.Add(FilterOperation.GREATERTHAN, "Больше");
            this.Add(FilterOperation.LESSEQUAL, "Меньше или равно");
            this.Add(FilterOperation.GREATEREQUAL, "Больше или равно");
            this.Add(FilterOperation.CONTAINS, "Содержит");
        }
    }

    class ColorsList : List<Brush>
    {
        public ColorsList()
        {
            this.Add(Brushes.White);
            this.Add(Brushes.Red);
            this.Add(Brushes.Green);
            this.Add(Brushes.Blue);
        }
    }
}
