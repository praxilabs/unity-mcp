using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProgressMap.Structure
{
    public class TestExperimentJSON : MonoBehaviour
    {
        [SerializeField]
        private string _experimentPath = System.IO.Path.Combine(Application.dataPath, "ExperimentProgress.json");

        [SerializeField] private Experiment _experiment;
        [SerializeField] private ProgressMap.UI.ProgressMapController _progressMapController;

        [ContextMenu("Experiment to Json")]
        public void ExperimentToJson()
        {
            string json = ExperimentProgressJsonConverter.Instance.ToJson(_experiment, true);
            print($"ExperimentJson: \n{json}");
            System.IO.File.WriteAllText(_experimentPath, json);
        }

        [ContextMenu("Json to Experiment")]
        public void JsonToExperiment()
        {
            string json = System.IO.File.ReadAllText(_experimentPath);
            print($"ExperimentJson: \n{json}");
            //_experiment = Experiment.ExperimentFromJson(json);
            _progressMapController.Initialize(json);
            // ExperimentProgressJsonConverter.Instance.FromJson(json);
        }
    }
}