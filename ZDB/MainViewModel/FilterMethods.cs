using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using ZDB.Database;

namespace ZDB.MainViewModel
{
    partial class MainViewModelClass
    {
        private void FilterHandler(object sender, FilterEventArgs e)
        {
            if (e.Item is Entry c)
            {
                e.Accepted = filters.Filter(c);
            }
        }
        private void FilterRefresh(object sender, PropertyChangedEventArgs e)
        {
            DataViewSource.View.Refresh();
        }

        private RelayCommand addFilterCommand;
        public RelayCommand AddFilterCommand
        {
            get
            {
                return addFilterCommand ??
                    (addFilterCommand = new RelayCommand(obj =>
                    {
                        Filters.Add(new FilterString("Obj", FilterOperation.GREATEREQUAL, ""));
                    },
                    (obj) => Data != null));
            }
        }


    }
}
