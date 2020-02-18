using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZDB.MainViewModel
{
    // Grouping class
    public struct Grouping
    {
        public Grouping(string propertyName, string converterType)
        {
            field = propertyName;
            converter = converterType;
        }
        public string field;
        public string converter;
    }

    // CollectionViewSource view settings
    public class CVSStyle
    {
        public List<Grouping> groups;

        //public CVSStyle() { }

        public CVSStyle(CollectionViewSource collectionViewSource)
        {
            groups = new List<Grouping>();
            foreach (var g in collectionViewSource.GroupDescriptions)
            {
                var group = (g as PropertyGroupDescription);
                if (group.Converter == null)
                {
                    groups.Add(
                        new Grouping(group.PropertyName, "null"));
                }
                else
                {
                    groups.Add(
                        new Grouping(group.PropertyName, group.Converter.GetType().ToString()));
                }
            }
        }

        public CVSStyle(List<Grouping> groupingList)
        {
            groups = groupingList;
        }

        public void Apply(CollectionViewSource collectionViewSource)
        {
            foreach (var group in groups)
            {
                PropertyGroupDescription propertyGroupDescription;
                if (group.converter == "null")
                {
                    propertyGroupDescription = new PropertyGroupDescription(group.field);
                } else
                {
                    Type converterType = Type.GetType(group.converter, true);
                    IValueConverter converterInstance = Activator.CreateInstance(converterType) as IValueConverter;
                    propertyGroupDescription = new PropertyGroupDescription(group.field, converterInstance);
                }
                collectionViewSource.GroupDescriptions.Add(propertyGroupDescription);
            }
        }
    }
}
