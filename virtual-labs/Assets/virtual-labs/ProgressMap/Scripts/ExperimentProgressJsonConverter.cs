using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonConverterBase;

namespace ProgressMap.Structure
{
    public class ExperimentProgressJsonConverter : IConvertFromJson<Experiment>, IConvertToJson<Experiment>
    {
        private static ExperimentProgressJsonConverter _instance;
        public static ExperimentProgressJsonConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExperimentProgressJsonConverter();
                return _instance;
            }
        }

        public Experiment FromJson(string experimentJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Experiment>(experimentJson);
            //return JsonUtility.FromJson<Experiment>(experimentJson);
        }

        public string ToJson(Experiment experiment, bool prettyPrint)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(experiment, Newtonsoft.Json.Formatting.Indented);
            //return JsonUtility.ToJson(experiment, prettyPrint);
        }
    }
}