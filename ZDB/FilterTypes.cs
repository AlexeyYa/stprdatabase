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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZDB
{
    interface IFilter : INotifyPropertyChanged
    {
        bool Check(Entry c, bool checkLinked = false);
    }

    /// <summary>
    /// Extensions
    /// </summary>
    
    class FilterCollection : ObservableCollection<FilterBase>
    {
        public void FilterChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Field") {
                FilterBase filter = sender as FilterBase;
                string field = filter.Field;
                int idx = this.IndexOf(filter);
                if (Consts.StrFields.Contains(field))
                {
                    this.SetItem(idx, new FilterString(field, FilterOperation.GREATEREQUAL, ""));
                }
                else if (Consts.IntFields.Contains(field))
                {
                    this.SetItem(idx, new FilterInt(field, FilterOperation.GREATEREQUAL, 0));
                }
                else if (Consts.DateFields.Contains(field))
                {
                    this.SetItem(idx, new FilterDate(field, FilterOperation.GREATEREQUAL, new DateTime(2019, 1, 1)));
                }
            }
        }

        public bool Filter(Entry c)
        {
            var groups = Items.GroupBy(x => x.LinkColor);
            foreach (var g in groups)
            {
                if (g.Key == Brushes.White)
                {
                    if (!g.All(f => f.Check(c)))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!g.Any(f => f.Check(c)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class ColorsList : List<Brush>
    {
        public ColorsList()
        {
            this.Add(Brushes.White);
            this.Add(Brushes.Red);
            this.Add(Brushes.Green);
            this.Add(Brushes.Blue);
        }
    }

    abstract class FilterBase : IFilter
    {
        protected string field;
        public string Field
        {
            get => field;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged("Field");
                }
            }
        }

        protected FilterOperation operation;
        public FilterOperation Operation
        {
            get => operation;
            set
            {
                if (operation != value)
                {
                    operation = value;
                    OnPropertyChanged("Operation");
                }
            }
        }

        private Brush linkColor;
        public Brush LinkColor
        {
            get => linkColor;
                //linkColor;
            set
            {
                if (linkColor != value)
                {
                    linkColor = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public FilterBase(string _field, FilterOperation op)
        {
            Field = _field;
            Operation = op;
            LinkColor = Brushes.White;
        }
        
        public abstract bool Check(Entry c, bool checkLinked = false);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Abstract classes for each data type
    /// </summary>

    class FilterString : FilterBase
    {
        protected string val;
        public string Val
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    OnPropertyChanged("Val");
                }
            }
        }



        private string GetFiltered(Entry c)
        {
            return (string)c[field];
        }
        

        public override bool Check(Entry c, bool checkLinked=false)
        {
            string x = GetFiltered(c);
            switch (Operation)
                {
                    case FilterOperation.EQUALS: return String.Compare(x, Val) == 0;
                    case FilterOperation.NOTEQUALS: return String.Compare(x, Val) != 0;
                    case FilterOperation.LESSTHAN: return String.Compare(x, Val) < 0;
                    case FilterOperation.GREATERTHAN: return String.Compare(x, Val) > 0;
                    case FilterOperation.LESSEQUAL: return !(String.Compare(x, Val) > 0);
                    case FilterOperation.GREATEREQUAL: return !(String.Compare(x, Val) < 0);
                    case FilterOperation.CONTAINS:
                        if (x == null) { return false; }
                        return x.Contains(Val);
                    default: return true;
                } 
        }

        public FilterString(string _field, FilterOperation op, string value)
            : base(_field, op)
        {
            Val = value;
        }
    }

    class FilterInt : FilterBase
    {
        protected int val;
        public int Val
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    OnPropertyChanged("Val");
                }
            }
        }

        private int GetFiltered(Entry c)
        {
            return (int)c[Field];
        }

        public override bool Check(Entry c, bool checkLinked = false)
        {
            int x = GetFiltered(c);
            switch (Operation)
                {
                    case FilterOperation.EQUALS: return x == Val;
                    case FilterOperation.NOTEQUALS: return x != Val;
                    case FilterOperation.LESSTHAN: return x < Val;
                    case FilterOperation.GREATERTHAN: return x > Val;
                    case FilterOperation.LESSEQUAL: return x <= Val;
                    case FilterOperation.GREATEREQUAL: return x >= Val;
                    case FilterOperation.CONTAINS: return x == Val;
                    default: return true;
                }
        }

        public FilterInt(string _field, FilterOperation op, int value)
            : base(_field, op)
        {
            Val = value;
        }

    }

    class FilterDate : FilterBase
    {
        protected DateTime val;
        public DateTime Val
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    OnPropertyChanged("Val");
                }
            }
        }

        private DateTime GetFiltered(Entry c)
        {
            return (DateTime)c[Field];
        }

        public override bool Check(Entry c, bool checkLinked=false)
        {
            DateTime x = GetFiltered(c);
            switch (Operation)
                {
                    case FilterOperation.EQUALS: return x == Val;
                    case FilterOperation.NOTEQUALS: return x != Val;
                    case FilterOperation.LESSTHAN: return x < Val;
                    case FilterOperation.GREATERTHAN: return x > Val;
                    case FilterOperation.LESSEQUAL: return x <= Val;
                    case FilterOperation.GREATEREQUAL: return x >= Val;
                    case FilterOperation.CONTAINS: return x == Val;
                    default: return true;
                }
        }

        public FilterDate(string _field, FilterOperation op, DateTime value)
            : base(_field, op)
        {
            Val = value;
        }
    }

    ///// <summary>
    ///// Concrete fields implementation
    ///// String filters
    ///// </summary>

    //class FilterUser : FilterString
    //{
    //    public FilterUser(string value, FilterOperation op) : base(value, op) { }

    //    public FilterUser(FilterString fs) : base(fs.Val, fs.Operation) { }

    //    //internal override string GetFiltered(Content c)
    //    //{
    //    //    return c.User;
    //    //}
    //}

    ///// <summary>
    ///// Integer filters
    ///// </summary>

    //class FilterFormats : FilterInt
    //{
    //    public FilterFormats(int value, FilterOperation op) : base(value, op) { }

    //    public FilterFormats(FilterInt fs) : base(fs.Val, fs.Operation) { }

    //    internal override int GetFiltered(Content c)
    //    {
    //        return c.Size.Formats;
    //    }
    //}

    ///// <summary>
    ///// Date filters
    ///// </summary>
    
    //class FilterStartDate : FilterDate
    //{
    //    public FilterStartDate(DateTime value, FilterOperation op) : base(value, op) { }

    //    public FilterStartDate(FilterDate fs) : base(fs.Val, fs.Operation) { }

    //    internal override DateTime GetFiltered(Content c)
    //    {
    //        return c.StartDate;
    //    }
    //}
}
