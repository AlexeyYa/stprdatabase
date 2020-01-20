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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Collections.Specialized;

namespace ZDB
{/*
    public class Contents : ObservableCollection<Content>
    {
        //public new void Add(Content c)
        //{
        //    SubscribeLogger(c);
        //    base.Add(c);
        //}

        public Content FindByID(string _s)
        {
            return Items.First(c => c.Number == _s);
        }

        public int IndexOfNum(string _s)
        {
            return Items.IndexOf(Items.First(c => c.Number == _s));
        }

        private void SubscribeItem(Content c)
        {
            c.PropertyChangedEx += Logger.ItemChanged;
        }

        private void UnsubscribeItem(Content c)
        {
            c.PropertyChangedEx -= Logger.ItemChanged;
        }

        protected override void RemoveItem(int index)
        {
            CheckReentrancy();
            UnsubscribeItem(Items[index]);
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, Content item)
        {
            SubscribeItem(item);
            base.InsertItem(index, item);
        }

        public void Set(int index, Content item)
        {
            SubscribeItem(item);
            base.SetItem(index, item);
        }

        public string GenerateID()
        {
            int i = 0;
            string id;
            do
            {
                i++;
                id = String.Format("{0:yyyyMMdd}-{1}", DateTime.Now, i);
            }
            while (CheckNumber(id));
            return id;
        }

        private bool CheckNumber(string number)
        {
            if (Items.Where(i => i.Number == number).FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }

        //public new void Remove(Content c)
        //{
        //    base.Remove(c);
        //}

        //public new void RemoveAt(int index)
        //{
        //    base.RemoveAt(index);
        //}

        //public new void RemoveItem(int index)
        //{
        //    base.RemoveItem(index);
        //}
    }

    

    public class Content : INotifyPropertyChangedExtended
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedExtendedEventHandler PropertyChangedEx;

        private string number;
        public string Number { get => number;
            set {
                if (number != value)
                {
                    string tmp = number;
                    number = value;
                    OnPropertyChanged("Number", tmp, value);
                }
            }
        }

        private int codeType;
        public int CodeType
        {
            get => codeType;
            set
            {
                if (codeType != value)
                {
                    string tmp = codeType.ToString();
                    codeType = value;
                    OnPropertyChanged("CodeType", tmp, value.ToString());
                }
            }
        }


        private string status;
        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    string tmp = status;
                    status = value;
                    OnPropertyChanged("Status", tmp, value);
                }
            }
        }

        private string user;
        public string User
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    string tmp = user;
                    user = value;
                    OnPropertyChanged("User", tmp, value);
                }
            }
        }

        private string group;
        public string Group
        {
            get { return group; }
            set
            {
                if (group != value)
                {
                    string tmp = group;
                    group = value;
                    OnPropertyChanged("Group", tmp, value);
                }
            }
        }

        private string obj;
        public string Obj
        {
            get { return obj; }
            set
            {
                if (obj != value)
                {
                    string tmp = obj;
                    obj = value;
                    OnPropertyChanged("Object", tmp, value);
                }
            }
        }

        private string docCode;
        public string DocCode
        {
            get { return docCode; }
            set
            {
                if (docCode != value)
                {
                    string tmp = docCode;
                    docCode = value;
                    OnPropertyChanged("DocCode", tmp, value);
                }
            }
        }

        private string subs;
        public string Subs
        {
            get { return subs; }
            set
            {
                if (subs != value)
                {
                    string tmp = subs;
                    subs = value;
                    OnPropertyChanged("Subs", tmp, value);
                }
            }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set
            {
                if (link != value)
                {
                    string tmp = link;
                    link = value;
                    OnPropertyChanged("Link", tmp, value);
                }
            }
        }

        private string phone;
        public string Phone
        {
            get { return phone; }
            set
            {
                if (phone != value)
                {
                    string tmp = phone;
                    phone = value;
                    OnPropertyChanged("Phone", tmp, value);
                }
            }
        }

        private string tasks;
        public string Tasks
        {
            get { return tasks; }
            set
            {
                if (tasks != value)
                {
                    string tmp = tasks;
                    tasks = value;
                    OnPropertyChanged("Tasks", tmp, value);
                }
            }
        }

        private string corrections;
        public string Corrections
        {
            get { return corrections; }
            set
            {
                if (corrections != value)
                {
                    string tmp = corrections;
                    corrections = value;
                    OnPropertyChanged("Corrections", tmp, value);
                }
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (startDate != value)
                {
                    string tmp = startDate.ToString();
                    startDate = value;
                    OnPropertyChanged("StartDate", tmp, value.ToString());
                }
            }
        }

        private DateTime completeDate;
        public DateTime CompleteDate
        {
            get { return completeDate; }
            set
            {
                if (completeDate != value)
                {
                    string tmp = completeDate.ToString();
                    completeDate = value;
                    OnPropertyChanged("CompleteDate", tmp, value.ToString());
                }
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (endDate != value)
                {
                    string tmp = endDate.ToString();
                    endDate = value;
                    OnPropertyChanged("EndDate", tmp, value.ToString());
                }
            }
        }

        private int sizeFormat;
        public int SizeFormat
        {
            get { return sizeFormat; }
            set
            {
                if (sizeFormat != value)
                {
                    string tmp = sizeFormat.ToString();
                    sizeFormat = value;
                    OnPropertyChanged("SizeFormat", tmp, value.ToString());
                }
            }
        }

        private int sizeA4;
        public int SizeA4
        {
            get { return sizeA4; }
            set
            {
                if (sizeA4 != value)
                {
                    string tmp = sizeA4.ToString();
                    sizeA4 = value;
                    OnPropertyChanged("SizeA4", tmp, value.ToString());
                }
            }
        }

        private int sizeA3;
        public int SizeA3
        {
            get { return sizeA3; }
            set
            {
                if (sizeA3 != value)
                {
                    string tmp = sizeA3.ToString();
                    sizeA3 = value;
                    OnPropertyChanged("SizeA3", tmp, value.ToString());
                }
            }
        }

        private int sizeA2;
        public int SizeA2
        {
            get { return sizeA2; }
            set
            {
                if (sizeA2 != value)
                {
                    string tmp = sizeA2.ToString();
                    sizeA2 = value;
                    OnPropertyChanged("SizeA2", tmp, value.ToString());
                }
            }
        }

        private int sizeA1;
        public int SizeA1
        {
            get { return sizeA1; }
            set
            {
                if (sizeA1 != value)
                {
                    string tmp = sizeA1.ToString();
                    sizeA1 = value;
                    OnPropertyChanged("SizeA1", tmp, value.ToString());
                }
            }
        }

        private int sizeA0;
        public int SizeA0
        {
            get { return sizeA0; }
            set
            {
                if (sizeA0 != value)
                {
                    string tmp = sizeA0.ToString();
                    sizeA0 = value;
                    OnPropertyChanged("SizeA0", tmp, value.ToString());
                }
            }
        }

        private int sizeCorFormat;
        public int SizeCorFormat
        {
            get { return sizeCorFormat; }
            set
            {
                if (sizeCorFormat != value)
                {
                    string tmp = sizeCorFormat.ToString();
                    sizeCorFormat = value;
                    OnPropertyChanged("SizeCorFormat", tmp, value.ToString());
                }
            }
        }

        private int sizeCorA4;
        public int SizeCorA4
        {
            get { return sizeCorA4; }
            set
            {
                if (sizeCorA4 != value)
                {
                    string tmp = sizeCorA4.ToString();
                    sizeCorA4 = value;
                    OnPropertyChanged("SizeCorA4", tmp, value.ToString());
                }
            }
        }

        private int sizeCorA3;
        public int SizeCorA3
        {
            get { return sizeCorA3; }
            set
            {
                if (sizeCorA3 != value)
                {
                    string tmp = sizeCorA3.ToString();
                    sizeCorA3 = value;
                    OnPropertyChanged("SizeCorA3", tmp, value.ToString());
                }
            }
        }

        private int sizeCorA2;
        public int SizeCorA2
        {
            get { return sizeCorA2; }
            set
            {
                if (sizeCorA2 != value)
                {
                    string tmp = sizeCorA2.ToString();
                    sizeCorA2 = value;
                    OnPropertyChanged("SizeCorA2", tmp, value.ToString());
                }
            }
        }

        private int sizeCorA1;
        public int SizeCorA1
        {
            get { return sizeCorA1; }
            set
            {
                if (sizeCorA1 != value)
                {
                    string tmp = sizeCorA1.ToString();
                    sizeCorA1 = value;
                    OnPropertyChanged("SizeCorA1", tmp, value.ToString());
                }
            }
        }

        private int sizeCorA0;
        public int SizeCorA0
        {
            get { return sizeCorA0; }
            set
            {
                if (sizeCorA0 != value)
                {
                    string tmp = sizeCorA0.ToString();
                    sizeCorA0 = value;
                    OnPropertyChanged("SizeCorA0", tmp, value.ToString());
                }
            }
        }

        private int numberOfOriginals;
        public int NumberOfOriginals
        {
            get { return numberOfOriginals; }
            set
            {
                if (numberOfOriginals != value)
                {
                    string tmp = numberOfOriginals.ToString();
                    numberOfOriginals = value;
                    OnPropertyChanged("NumberOfOriginals", tmp, value.ToString());
                }
            }
        }

        private int numberOfCopies;
        public int NumberOfCopies
        {
            get { return numberOfCopies; }
            set
            {
                if (numberOfCopies != value)
                {
                    string tmp = numberOfCopies.ToString();
                    numberOfCopies = value;
                    OnPropertyChanged("NumberOfCopies", tmp, value.ToString());
                }
            }
        }

        private int numeration;
        public int Numeration
        {
            get { return numeration; }
            set
            {
                if (numeration != value)
                {
                    string tmp = numeration.ToString();
                    numeration = value;
                    OnPropertyChanged("Numeration", tmp, value.ToString());
                }
            }
        }

        private int scan;
        public int Scan
        {
            get { return scan; }
            set
            {
                if (scan != value)
                {
                    string tmp = scan.ToString();
                    scan = value;
                    OnPropertyChanged("Scan", tmp, value.ToString());
                }
            }
        }

        private int threading;
        public int Threading
        {
            get { return threading; }
            set
            {
                if (threading != value)
                {
                    string tmp = threading.ToString();
                    threading = value;
                    OnPropertyChanged("Threading", tmp, value.ToString());
                }
            }
        }

        private string executor;
        public string Executor
        {
            get { return executor; }
            set
            {
                if (executor != value)
                {
                    string tmp = executor;
                    executor = value;
                    OnPropertyChanged("Executor", tmp, value);
                }
            }
        }

        public object this[string propertyName]
        {
            get
            {
                return this.GetType().GetProperty(propertyName).GetValue(this, null);
            }
            set
            {
                this.GetType().GetProperty(propertyName).SetValue(this, value, null);
            }
        }
        
        public Content()
        {
        }

        public Content(string ID)
        {
            this.Number = ID;
        }

        public Content Copy()
        {
            Content c = new Content();
            foreach (string _f in new FieldsList())
            {
                c[_f] = this[_f];
            }
            return c;
        }

        protected void OnPropertyChanged(string name, string oldValue, string newValue)
        {
            PropertyChangedEx?.Invoke(this, new PropertyChangedExtendedEventArgs(name, oldValue, newValue));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }*/
}
