using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConverterBase;

namespace Table.Structure
{
    public class TableJsonConverter : IConvertFromJson<Table>, IConvertToJson<Table>
    {
        private static TableJsonConverter _instance;
        public static TableJsonConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TableJsonConverter();
                return _instance;
            }
        }


        public Table FromJson(string objectJson)
        {
            return JsonUtility.FromJson<Table>(objectJson);
        }

        public string ToJson(Table objectToConvert, bool prettyPrint)
        {
            return JsonUtility.ToJson(objectToConvert, prettyPrint);
        }
    }
}
