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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDB
{
    public interface INotifyProppertyChangedExtended : INotifyPropertyChanged
    {
        event PropertyChangedExtendedEventHandler PropertyChangedEx;
    }

    public delegate void PropertyChangedExtendedEventHandler(object sender, PropertyChangedExtendedEventArgs e);

    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual string OldValue { get; private set; }
        public virtual string NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, string oldValue, string newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
