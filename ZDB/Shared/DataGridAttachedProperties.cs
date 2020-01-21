using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZDB
{
    class DataGridAttachedProperties
    {
        public static bool GetIsCellSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCellSelectedProperty);
        }
        public static void SetIsCellSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCellSelectedProperty, value);
        }

        public static readonly DependencyProperty IsCellSelectedProperty =
            DependencyProperty.RegisterAttached("IsCellSelected", typeof(bool),
                typeof(DataGridAttachedProperties), new UIPropertyMetadata(false, (o, e) =>
                {
                    if (o is DataGridCell)
                    {
                        DataGridRow row = VisualTreeHelperEx.FindVisualParent<DataGridRow>(o as DataGridCell);
                        row.SetValue(DataGridAttachedProperties.IsCellSelectedProperty, e.NewValue);
                    }
                }));
    }

    public class VisualTreeHelperEx
    {
        public static T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }
    }
}
