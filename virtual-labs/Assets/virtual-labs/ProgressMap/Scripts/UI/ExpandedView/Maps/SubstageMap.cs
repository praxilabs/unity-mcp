using ProgressMap.UI.ExpandedView.Maps.Entries;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressMap.UI.ExpandedView.Maps
{
    public class SubstageMap : MonoBehaviour
    {
        #region fields
        [SerializeField] private SubstageMapEntry _substageEntryPrefab;
        [SerializeField] private Transform _substageEntriesParent;
        [SerializeField] private TMPro.TextMeshProUGUI _substageRatio;
        private Structure.Stage _selectedStage;
        [SerializeField] private ScrollRect _substagesScrollRect;

        [SerializeField, Tooltip("Substage objects to disable when there is only no substage")]
        private GameObject[] _substageGameObjects;
        private SubstageMapEntry[] _substageEntries;
        private UnityEngine.Pool.ObjectPool<SubstageMapEntry> _substagesPool;
        #endregion
        #region properties
        public Structure.Stage SelectedStage
        {
            get => _selectedStage;
            set
            {
                if (_selectedStage != null)
                    _selectedStage.ProgressUpdate -= UpdateUI;
                _selectedStage = value;
                UpdateUI();
                _selectedStage.ProgressUpdate += UpdateUI;
            }
        }
        #endregion
        #region methods
        private void Start()
        {
            Initialize();
        }

        private void PoolSetUp()
        {

            _substagesPool = new UnityEngine.Pool.ObjectPool<SubstageMapEntry>(
                createFunc: () => Instantiate<SubstageMapEntry>(_substageEntryPrefab, _substageEntriesParent),
                actionOnGet: (x) => x.gameObject.SetActive(true),
                actionOnRelease: x => x.gameObject.SetActive(false),
                actionOnDestroy: x => Destroy(x.gameObject),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 1000
                );
        }
        public void Initialize()
        {
            if (_substagesPool == null)
                PoolSetUp();
            ClearSubsStages();

            if (_selectedStage == null || _selectedStage.Substages.Length == 0)
            {
                foreach (var gameObject in _substageGameObjects)
                    gameObject.SetActive(false);
            }
            else
            {
                foreach (var gameObject in _substageGameObjects)
                    gameObject.SetActive(true);
            }

            if (_selectedStage == null)
                return;
            _substageEntries = new SubstageMapEntry[_selectedStage.Substages.Length];
            for (int i = 0; i < _substageEntries.Length; i++)
            {
                _substageEntries[i] = GetSubStageEntry();
            }

        }
        [ContextMenu("UpdateUI")]
        public void UpdateUI()
        {
            //if substages change then stage has changed. initialize for new stage.
            if (_substageEntries == null || _selectedStage.Substages.Length != _substageEntries.Length)
            {
                Initialize();

            }
            int completeStages = 0;
            for (int i = 0; i < _substageEntries.Length; i++)
            {
                _substageEntries[i].transform.SetAsLastSibling();
                if (_selectedStage.LastStepDone >= _selectedStage.Substages[i].StepNumber)
                    completeStages += 1;
                _substageEntries[i].SetData(_selectedStage.Substages[i].SubstageName, i + 1 == _substageEntries.Length);
                _substageEntries[i].Done = _selectedStage.LastStepDone >= _selectedStage.Substages[i].StepNumber;
                _substageRatio.text = $"{completeStages}/{_substageEntries.Length}";
            }
            _substagesScrollRect.verticalNormalizedPosition = 1;

        }

        public void UpdateText()
        {
            for (int i = 0; i < _substageEntries.Length; i++)
            {
                _substageEntries[i].UpdateText(_selectedStage.Substages[i].SubstageName);
            }
        }

        private void ClearSubsStages()
        {
            if (_substageEntries == null || _substageEntries.Length == 0)
                return;
            foreach (SubstageMapEntry substageEntry in _substageEntries)
            {
                _substagesPool.Release(substageEntry);
            }
        }
        private SubstageMapEntry GetSubStageEntry()
        {
            return _substagesPool.Get();
        }
        #endregion
    }
}