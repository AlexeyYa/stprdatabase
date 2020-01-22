using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ZDB
{
    class DatagridCustomInit
    {
        FieldsTranslated fieldsTranslated;


        public DatagridCustomInit(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
            fieldsTranslated = new FieldsTranslated();
            culture = new System.Globalization.CultureInfo("RU-ru");
            foreach (string column in fieldsTranslated.Keys)
            {
                _dataGrid.Columns.Add(ColumnFactory(column));
            }
        }



        private DataGridColumn ColumnFactory(string field)
        {
            if (Consts.EnumFields.ContainsKey(field))
            {
                DataGridTemplateColumn tColumn = new DataGridTemplateColumn();

                FrameworkElementFactory cmbElem = new FrameworkElementFactory(typeof(ComboBox));
                Binding dict = new Binding();
                dict.Source = Consts.EnumFields[field];
                dict.Mode = BindingMode.OneWay;

                cmbElem.SetBinding(ComboBox.ItemsSourceProperty, dict);
                Binding binding = new Binding(field);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                cmbElem.SetBinding(ComboBox.SelectedValueProperty, binding);
                cmbElem.SetValue(ComboBox.SelectedValuePathProperty, "Key");
                cmbElem.SetValue(ComboBox.DisplayMemberPathProperty, "Value");

                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.DataType = typeof(string);
                dataTemplate.VisualTree = cmbElem;
                tColumn.CellTemplate = dataTemplate;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = field;
                tColumn.CanUserSort = true;

                return tColumn;
            }
            else if (Consts.DateFields.Contains(field))
            {
                DataGridTextColumn tColumn = new DataGridTextColumn();
                Binding binding = new Binding(field);
                binding.Mode = BindingMode.TwoWay;
                binding.ConverterCulture = culture;
                binding.StringFormat = "dd-MM-yyyy HH:mm";
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tColumn.Binding = binding;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = field;
                tColumn.CanUserSort = true;

                return tColumn;
            }
            else if (field == "Number")
            {
                DataGridTextColumn tColumn = new DataGridTextColumn();
                Binding binding = new Binding(field);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tColumn.Binding = binding;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = field;
                tColumn.CanUserSort = true;
                tColumn.IsReadOnly = true;

                return tColumn;
            }
            else
            {
                DataGridTextColumn tColumn = new DataGridTextColumn();
                Binding binding = new Binding(field);
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tColumn.Binding = binding;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = field;
                tColumn.CanUserSort = true;

                return tColumn;
            }
        }

        private DataGrid _dataGrid;
        private System.Globalization.CultureInfo culture;
    }
}
