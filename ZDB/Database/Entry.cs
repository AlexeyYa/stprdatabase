using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDB.Database
{
    public class Entry : INotifyPropertyChangedExtended
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedExtendedEventHandler PropertyChangedEx;

        private int number;
        [Key]
        public int Number
        {
            get => number;
            set
            {
                if (number != value)
                {
                    string tmp = number.ToString();
                    number = value;
                    OnPropertyChanged("Number", tmp, value.ToString());
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

        private int status;
        public int Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    string tmp = status.ToString();
                    status = value;
                    OnPropertyChanged("Status", tmp, value.ToString());
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

        public Entry()
        {
        }

        public Entry(int ID)
        {
            this.Number = ID;
        }

        /*public Entry(Content c)
        {
            foreach (string _f in new FieldsList())
            {
                if (_f == "Status")
                {
                    switch (c[_f])
                    {
                        case "аннулировано":
                            this[_f] = -1;
                            break;
                        case "в работе":
                            this[_f] = 1;
                            break;
                        case "завершено":
                            this[_f] = 2;
                            break;
                        default:
                            this[_f] = 0;
                            break;
                    }
                }
                else if (_f != "Number")
                {
                    this[_f] = c[_f];
                }
            }
        }*/

        public Entry Copy()
        {
            Entry entry = new Entry();
            foreach (string _f in new FieldsList())
            {
                entry[_f] = this[_f];
            }
            return entry;
        }

        protected void OnPropertyChanged(string name, string oldValue, string newValue)
        {
            PropertyChangedEx?.Invoke(this, new PropertyChangedExtendedEventArgs(name, oldValue, newValue));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
