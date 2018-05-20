using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessConfigurationManager.WPF.UML.Common
{
    internal class LinkTypeComboBoxItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public LinkTypeComboBoxItem()
        {

        }

        public LinkTypeComboBoxItem(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
