﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ZDB
{
    class DataGridCustomInit
    {
        FieldsTranslated fieldsTranslated;


        public DataGridCustomInit(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
            fieldsTranslated = new FieldsTranslated();
            culture = new System.Globalization.CultureInfo("RU-ru");
            dataGrid.MinRowHeight = 40;
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
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
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
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                tColumn.Binding = binding;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = field;
                tColumn.CanUserSort = true;

                return tColumn;
            }
            /*else if (Consts.MultiBindFields.ContainsKey(field))
            {
                DataGridTextColumn tColumn = new DataGridTextColumn();

                MultiBinding multiBinding = new MultiBinding();

                // MultiBinding converter - replace
                multiBinding.Converter = new TotalFormatsConverter();

                foreach (string bindField in Consts.MultiBindFields[field])
                {
                    Binding tmpBind = new Binding(bindField);
                    multiBinding.Bindings.Add(tmpBind);
                }

                tColumn.Binding = multiBinding;
                tColumn.Header = fieldsTranslated[field];
                tColumn.SortMemberPath = "SortParam";
                tColumn.CanUserSort = true;
                tColumn.IsReadOnly = true;

                return tColumn;
            }*/
            else if (Consts.OnewayFields.Contains(field))
            {
                DataGridTextColumn tColumn = new DataGridTextColumn();
                Binding binding = new Binding(field);
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
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
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
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
