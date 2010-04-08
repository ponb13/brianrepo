using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DomainModel.Entities
{
    public class ShippingDetails : IDataErrorInfo
    {
        public string Name { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool GiftRap { get; set; }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Name" && String.IsNullOrEmpty(Name))
                    return "Please enter a name.";
                if (columnName == "Line1" && String.IsNullOrEmpty(Line1))
                    return "Please enter address line 1.";
                if (columnName == "Line2" && String.IsNullOrEmpty(Line2))
                    return "Please enter address line 2.";
                if (columnName == "Line3" && String.IsNullOrEmpty(Line3))
                    return "Please enter address line 3.";
                if (columnName == "City" && String.IsNullOrEmpty(City))
                    return "Please enter a city.";
                if (columnName == "Zip" && String.IsNullOrEmpty(Zip))
                    return "Please enter a zip code";
                if (columnName == "Country" && String.IsNullOrEmpty(Country))
                    return "Please enter a Country";

                return null;
            }
        }

        public string Error
        {
            // not required.. 
            get { return null; }
        }
    }
}
