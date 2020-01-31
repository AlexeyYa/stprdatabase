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
using System.Collections.Specialized;
using System.Text;

using ZDB.Database;

namespace ZDB
{
    /*class StoryNode
    {
        private readonly string data;
        private readonly string action;
        private readonly string number;
        private HashSet<StoryNode> childrens;
        private StoryNode parent;
        private StoryNode active;

        public StoryNode(string _data)
        {
            data = _data;
        }

        public string GetNumber()
        {
            return number;
        }

        public string GetAction()
        {
            return data;
        }

        public string GetData()
        {
            return data;
        }

        public void AddChild(StoryNode _children)
        {
            childrens.Add(_children);
        }

        public StoryNode NewBranch(string _data)
        {
            StoryNode child = new StoryNode(_data);
            this.childrens.Add(child);
            return child;
        }

        public StoryNode ChangeBranch(StoryNode storyNode)
        {
            this.active = storyNode;
            return this.active;
        }

        public StoryNode GetParent()
        {
            return parent;
        }

        public StoryNode GetNext()
        {
            StoryNode child = this.active;
            return child;
        }

    }*/

    public static class Logger
    {
        private static List<string> actions = new List<string>();
        private static readonly FieldsList fieldList = new FieldsList();
        private static IEnumerable<Entry> contents;
        // private static StoryNode current;
        private static int currentPos = actions.Count;
        private static bool skipWritting = false; // Prevents writting excessive actions

        public static void Load(IEnumerable<Entry> _contents)
        {
            contents = _contents;
        }

        public static void Save()
        {

        }

        public static void Restore()
        {

        }

        /*public static void Undo()
        {
            skipWritting = true;
            if (currentPos < 1)
            {
                return;
            }
            string action = actions[--currentPos];
            if (action.Substring(0, 3) == "cha")
            {
                // ToDo
                int numPos = action.IndexOf('#');
                int index = contents.IndexOfNum(action.Substring(4, numPos - 4));
                
                int propPos = action.IndexOf('$');
                string _f = action.Substring(numPos + 1, propPos - numPos - 1);
                int sepPos = action.IndexOf('@');
                string value = action.Substring(propPos + 1, sepPos - propPos - 1);
                if (Consts.StrFields.Contains(_f)) { contents[index][_f] = value; }
                else if (Consts.IntFields.Contains(_f) && Int32.TryParse(value, out int _intval)) { contents[index][_f] = _intval; }
                else if (Consts.IntFields.Contains(_f) && DateTime.TryParse(value, out DateTime _dval)) { contents[index][_f] = _dval; }
                
            }
            else if (action.Substring(0, 3) == "new")
            {
                Int32.TryParse(action.Substring(4, action.IndexOf('#') - 4), out int index);
                contents.RemoveAt(index);
            }
            else if (action.Substring(0, 3) == "rem")
            {
                int numPos = action.IndexOf('#');
                Int32.TryParse(action.Substring(4, numPos - 4), out int index);
                contents.Insert(index, RestoreContent(action.Substring(numPos + 1)));
            }
            skipWritting = false;
        }

        public static void Redo()
        {
            skipWritting = true;
            if (currentPos >= actions.Count)
            {
                return;
            }
            string action = actions[currentPos++];
            if (action.Substring(0, 3) == "cha")
            {
                int numPos = action.IndexOf('#');
                int index = contents.IndexOfNum(action.Substring(4, numPos - 4));
                int propPos = action.IndexOf('$');
                string _f = action.Substring(numPos + 1, propPos - numPos - 1);
                int sepPos = action.IndexOf('@');
                string value = action.Substring(sepPos + 1);
                if (Consts.StrFields.Contains(_f)) { contents[index][_f] = value; }
                else if (Consts.IntFields.Contains(_f) && Int32.TryParse(value, out int _intval)) { contents[index][_f] = _intval; }
                else if (Consts.IntFields.Contains(_f) && DateTime.TryParse(value, out DateTime _dval)) { contents[index][_f] = _dval; }

            }
            else if (action.Substring(0, 3) == "new")
            {
                int numPos = action.IndexOf('#');
                Int32.TryParse(action.Substring(4, numPos - 4), out int index);
                contents.Insert(index, RestoreContent(action.Substring(numPos + 1)));
            }
            else if (action.Substring(0, 3) == "rem")
            {
                Int32.TryParse(action.Substring(4, action.IndexOf('#') - 4), out int index);
                contents.RemoveAt(index);
            }
            skipWritting = false;
        }*/

        public static void ItemChanged(object sender, PropertyChangedExtendedEventArgs e)
        {
            if (!(skipWritting))
            {
                Entry c = (sender as Entry);
                actions.Add("cha:" + c.Number + "#" + e.PropertyName + "$" + e.OldValue + "@" + e.NewValue);
                currentPos++;
            }
        }

        public static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(skipWritting))
            {
                if (e.NewItems != null)
                {
                    foreach (Entry c in e.NewItems)
                    {
                        actions.Add("new:" + e.NewStartingIndex + "#" + SaveContent(c));
                        currentPos++;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Entry c in e.OldItems)
                    {
                        actions.Add("rem:" + e.OldStartingIndex + "#" + SaveContent(c));
                        currentPos++;
                    }
                }
            }
        }

        public static void LogException(Exception e)
        {
            actions.Add("StartOfException>");
            while (e != null)
            {
                actions.Add(e.Message);
                e = e.InnerException;
            }
            actions.Add("EndOfException<");
        }

        /*public static bool TestSaveRestore(Entry c)
        {
            SaveContent(c);
            Entry other = RestoreContent(actions.Last());
            bool check = (c == other);
            bool memberwise = true;
            foreach (string field in fieldList)
            {
                if ((c[field] != null) && !(c[field].Equals(other[field])))
                {
                    memberwise = false;
                }
            }
            return memberwise;
        }*/

        private static string SaveContent(Entry c)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string _field in fieldList)
            {
                stringBuilder.Append(_field);
                stringBuilder.Append("$");
                stringBuilder.Append(c[_field]);
                stringBuilder.Append("%");
            }
            return stringBuilder.ToString();
        }

        /*private static Entry RestoreContent(string source)
        {
            Entry c = new Entry();
            int pleft = 0;
            int pright = 0;
            string field = String.Empty;
            string val = String.Empty;
            while (pright < source.Length)
            {
                if (source[pright] == '$')
                {
                    field = source.Substring(pleft, pright - pleft);
                    pleft = pright + 1;
                }
                else if (source[pright] == '%')
                {
                    val = source.Substring(pleft, pright - pleft);
                    if (Consts.StrFields.Contains(field))
                    {
                        c[field] = val;
                    }
                    else if (Consts.IntFields.Contains(field) && Int32.TryParse(val, out int i))
                    {
                        c[field] = i;
                    }
                    else if (Consts.DateFields.Contains(field) && DateTime.TryParse(val, out DateTime d))
                    {
                        c[field] = d;
                    }
                    else if (pright != pleft)
                    {
                        throw new Exception("Can't restore data");
                    }
                    pleft = pright + 1;
                }
                pright++;
            }
            return c;
        }

        private static Entry RestoreContentPart(string source, Entry c)
        {
            int pleft = 0;
            int pright = 0;
            string field = String.Empty;
            string val = String.Empty;
            while (pright < source.Length)
            {
                if (source[pright] == '$')
                {
                    field = source.Substring(pleft, pright - pleft);
                    pleft = pright + 1;
                }
                else if (source[pright] == '%')
                {
                    val = source.Substring(pleft, pright - pleft);
                    pleft = pright + 1;
                    if (Consts.StrFields.Contains(field))
                    {
                        c[field] = val;
                    }
                    else if (Consts.IntFields.Contains(field) && Int32.TryParse(val, out int i))
                    {
                        c[field] = i;
                    }
                    else if (Consts.DateFields.Contains(field) && DateTime.TryParse(val, out DateTime d))
                    {
                        c[field] = d;
                    }
                    else
                    {
                        throw new Exception("Can't restore data");
                    }
                }
                pright++;
            }
            return c;
        }*/
    }
}
