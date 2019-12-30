/*
111
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
using System.Text;
using System.Threading.Tasks;

namespace ZDB
{
    static class Consts
    {
        public static readonly IEnumerable<string> StrFields = new HashSet<string>
            { "Number", "User", "Obj", "Group", "DocCode", "Subs", "Status", "Tasks", "Corrections", "Executor" };
        
        public static readonly IEnumerable<string> IntFields = new HashSet<string>
            { "CodeType", "NumberOfCopies", "NumberOfOriginals", "Numeration", "Scan", "Threading",
            "SizeFormat", "SizeA4", "SizeA3", "SizeA2", "SizeA1", "SizeA0",
            "SizeCorFormat", "SizeCorA4", "SizeCorA3", "SizeCorA2", "SizeCorA1", "SizeCorA0"};

        public static readonly IEnumerable<string> DateFields = new HashSet<string>
            { "StartDate", "EndDate", "CompleteDate" };        
    }

    /// <summary>
    /// Using reflection to get all fields
    /// </summary>

    public class FieldsList : List<string>
    {
        public FieldsList()
        {
            Type t = typeof(Content);
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
            this.Add("Numeration", "Нумерация");
            this.Add("Scan", "Сканирование");
            this.Add("SizeFormat", "Форматки");
            this.Add("SizeCorFormat", "Корректировка форматки");
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
}
