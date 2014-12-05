using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyphen.Core
{
    public class ModelWrapper<T>
    {
    
        
        public T ModelData { get; set; }
       
        public IQueryable<T> ModelList { get; set; }

        public Dictionary<string, object> Filters { get; set; }

       // public string ModelListJSON() { return Newtonsoft.Json.JsonConvert.SerializeObject(ModelList) ; }

        public ModelWrapper()
        {
        }


        public object GetFilter(string filterType)
        {
            if (Filters.ContainsKey(filterType))
            {
                return Filters[filterType];
            }
            else
            {
                return "";
            }
        }
    }
}
