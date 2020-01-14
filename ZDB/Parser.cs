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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace ZDB
{
    class Parser
    {
        public static string template;
        public static Dictionary<string, int> subs;

        public static Content ProcessFromDocx(string path, List<DocTemplate> dtList)
        {
            DocX document = DocX.Load(path);
            Content content = new Content();
            Regex rx;
            MatchCollection match;
            // rx = new Regex(@"Бланк-заказ № ________________на множительные работы и архивациюДата (.+)Фамилия И. О (.+) ПГ/ГРП/отдел (.+) тел. (.+)Шифр комплекта (.+)Сокращенное наименование объекта \(шифр\) (.+)\n\tпечать/ копирование\tскани-рование\tфальцовка\tформа готового материала \n\tКол-во экз.\tКол-во форматов одного экз.\tВ том числе откорректированных форматов\t\tОбщее кол-во сфальцованных листов \(без А4\)\t\n\t1\t2\t3\t4\t5\t6\n\t(.+)\+(.+)\tФ (.+)\tФ (.+)\t\t\t(.+)\n\t\tА4 (.+)\tА4 (.+)\t\t\t\n\t\tА3 (.+)\tА3 (.+)#\t\t\t\n\t\tА2 #SA2#\tА2 #CA2#\t\t\t\n\t\tА1 #SA1#\tА1 #CA1#\t\t\t\n\t\tА0 #SA0#\tА0 #CA0#\t\t\t\n\tФормирование электронной версии\tКорректировка электронной версии\tАктивное содержание документа \n\tФ #EF#\tФ #ECF#\tТребование Заказчика\n\tА4 #EA4#\tА4 #ECA4#\t\n\tА3 #EA3#\tА3 #ECA3#\t\n\tА2 #EA2#\tА2 #ECA2#\tДля ГГЭ\n\tА1 #EA1#\tА1 #ECA1#\t\n\tА0 #EA0#\tА0 #ECA0#\tСсылка на папку с ПСД #LINK#В графе \(1\) указывается количество экземпляров \(копий\)В графе \(2\) указывается количество каждого формата одного экземпляра \(комплекта\)Графа \(3\) заполняется, если комплект \(листы\) откорректированы. Указывается количество каждого формата откорректированных листов.В графе \(4\) указывается необходимо ли сканирование.Графу \(5\) заполняет сотрудник группы выпуска.В графе \(6\) указать форму готового материала \(сшивка, папка, другое\)Также указываются причины корректировки в соответствии с таблицей \(заполняет разработчик\)\n\t№ п/п\tМесто возникновения\tПричины корректировки\n\t\t\tВнутренние\tВнешние\n\t\t\tСобственные ошибки ПГ\tНеверные исходные данные от смежников и ГРП\tНовая инициатива заказчика после получения ПСД\tНовая инициатива подрядчика после получения ПСД\tНеверные исходные данные от внешних субподряд-чиков \(инженер-ные изыска-ния и т. п.\) \n\t1\tИнженерные расчеты\t#1-1#\t#1-2#\t#1-3#\t#1-4#\t#1-5#\n\t2\tКонструктивно-технологическое решение\t#2-1#\t#2-2#\t#2-3#\t#2-4#\t#2-5#\n\t3\tВедомости объемов работ\t#3-1#\t#3-2#\t#3-3#\t#3-4#\t#3-5#\n\t4\tСметная документация\t#4-1#\t#4-2#\t#4-3#\t#4-4#\t#4-5#\n\t5\tОформление ПСД \(нормоконтроль\)\t#5-1#\t#5-2#\t#5-3#\t#5-4#\t#5-5#\n\t6\tКорректировка\t#6-1#\t#6-2#\t#6-3#\t#6-4#\t#6-5#\n\t7\tВыпуск документации для заказчика\t#7#Подпись___________________                  Дата получения заказа «_____» ________________");
            foreach (DocTemplate dt in dtList)
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
                int tmp; // out value for Int32.TryParse
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
                                content.StartDate = date;
                                DateTime.TryParseExact(s.Substring(s.IndexOf(' ', tmpidx + 1) + 1), dateformats,
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out date);
                                content.EndDate = date;
                            }
                            else
                            {
                                DateTime.TryParseExact(match[0].Groups[dt.Substitutions[sub]].Value, dateformats,
                                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                    out date);
                                content.StartDate = date;
                            }
                            break;
                        case "#ENDDATE#":
                            DateTime.TryParseExact(match[0].Groups[dt.Substitutions[sub]].Value, dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out date);
                            content.EndDate = date;
                            break;
                        case "#FIO#":
                            content.User = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#GROUP#":
                            content.Group = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#PHONE#":
                            content.Phone = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#DOCCODE#":
                            content.DocCode = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#OBJCODE#":
                            content.Obj = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;
                        case "#ORIG#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.NumberOfOriginals = tmp;
                            break;
                        case "#COPY#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.NumberOfCopies = tmp;
                            break;
                        case "#FGM#":
                            content.Tasks = match[0].Groups[dt.Substitutions[sub]].Value;
                            break;

                        case "#SF#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeFormat = tmp;
                            break;
                        case "#SA4#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeA4 = tmp;
                            break;
                        case "#SA3#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeA3 = tmp;
                            break;
                        case "#SA2#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeA2 = tmp;
                            break;
                        case "#SA1#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeA1 = tmp;
                            break;
                        case "#SA0#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeA0 = tmp;
                            break;

                        case "#CF#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorFormat = tmp;
                            break;
                        case "#CA4#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorA4 = tmp;
                            break;
                        case "#CA3#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorA3 = tmp;
                            break;
                        case "#CA2#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorA2 = tmp;
                            break;
                        case "#CA1#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorA1 = tmp;
                            break;
                        case "#CA0#":
                            Int32.TryParse(match[0].Groups[dt.Substitutions[sub]].Value, out tmp);
                            content.SizeCorA0 = tmp;
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
                content.Corrections = String.Join(",", corrections);
                break;
            }
            return content;
        }

        public static DocTemplate Template(string path)
        {
            DocX document = DocX.Load(path);
            string temp = document.Text;
            temp = temp.Replace("(", @"\(");
            temp = temp.Replace(")", @"\)");
            temp = temp.Replace("+", @"\+");
            bool flag = false;
            string sub = "";
            int idx = 1;
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

    [Serializable]
    class DocTemplate
    {
        private string regExData;
        private Dictionary<string, int> substitutions = new Dictionary<string, int>();

        public string RegExData { get => regExData; set => regExData = value; }
        public Dictionary<string, int> Substitutions { get => substitutions; set => substitutions = value; }

        public DocTemplate() { }
        public DocTemplate(string r, Dictionary<string,int> d)
        {
            RegExData = r;
            Substitutions = d;
        }
    }
}
