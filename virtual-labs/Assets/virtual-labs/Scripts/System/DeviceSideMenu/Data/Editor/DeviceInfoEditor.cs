using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [CustomEditor(typeof(DeviceInfo))]
    public class DeviceInfoEditor : Editor
    {
        // Editor state tracking
        private bool _isInitialized = false;
        
        // Component creation fields
        private bool _showNewControlsComponentFields = false;
        private bool _showNewReadingsComponentFields = false;

        // New Controls Component Fields
        private ControlsComponentType _selectedControlsComponentType = ControlsComponentType.Slider;
        private string _newControlsComponentName = "";

        // New Readings Component Fields
        private ReadingsComponentType _selectedReadingsComponentType = ReadingsComponentType.DisplayField;
        private string _newReadingsComponentName = "";

        // Fields for Slider Component
        private float _newSliderMin = 0;
        private float _newSliderMax = 100;
        private string _newSliderSymbol = "";
        private bool _newShouldDispayScreenValue = false;

        // Fields for Dropdown Component
        private List<string> _dropdownOptions = new List<string>();
        private string _newOptionText = "";
        private bool _newShouldConvertExponential = false;

        // Fields for Checkbox Component
        private bool _newIsCheckboxTrue = false;

        // Fields for DisplayField Component
        private string _newDisplayFieldLabel = "";
        private string _newDisplayTextSign = "";

        // GUI Styles
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _sectionHeaderStyle;

        // Serialized Properties
        SerializedProperty _readingsComponentsProp;
        SerializedProperty _controlsComponentsProp;

        private void InitializeStyles()
        {
            // Header style
            _headerStyle = new GUIStyle(EditorStyles.boldLabel);
            _headerStyle.fontSize = 14;
            _headerStyle.alignment = TextAnchor.MiddleCenter;
            _headerStyle.margin = new RectOffset(0, 0, 10, 10);
            
            // Button style
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.padding = new RectOffset(15, 15, 5, 5);
            
            // Section header style
            _sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            _sectionHeaderStyle.fontSize = 12;
            _sectionHeaderStyle.margin = new RectOffset(0, 0, 8, 4);
        }

        public override void OnInspectorGUI()
        {
            // Initialize styles if needed
            if (_headerStyle == null)
                InitializeStyles();

            serializedObject.Update();
            DeviceInfo deviceInfo = (DeviceInfo)target;

            // Check if the DeviceMenuBuilder component is available
            DeviceMenuBuilder menuBuilder = deviceInfo.GetComponent<DeviceMenuBuilder>();
            bool hasMenuBuilder = menuBuilder != null;
            
            EditorGUILayout.Space(5);

            // Display informative header
            EditorGUILayout.LabelField("Device Menu Configuration", _headerStyle);
            
            if (!_isInitialized)
            {
                // Initial state - just show Initialize button
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Initialize to configure your device menu settings", UnityEditor.MessageType.Info);
                EditorGUILayout.Space(10);
                
                if (GUILayout.Button("Initialize Device Menu", GUILayout.Height(30)))
                {
                    _isInitialized = true;
                }
            }
            else
            {
                // Editing state - show the full editor
                EditorGUILayout.Space(5);
                
                // Use a ScrollView for the content
                EditorGUILayout.BeginVertical("box");
                
                // Basic device info section
                EditorGUILayout.LabelField("Basic Settings", _sectionHeaderStyle);
                
                // Use SerializedProperty with correct property names (check for 'field:' prefix)
                SerializedProperty nameProp = serializedObject.FindProperty("_name");
                if (nameProp == null) nameProp = serializedObject.FindProperty("<Name>k__BackingField");
                if (nameProp != null) EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
                else deviceInfo.Name = EditorGUILayout.TextField("Name", deviceInfo.Name);
                
                SerializedProperty tabTypesProp = serializedObject.FindProperty("_tabTypes");
                if (tabTypesProp == null) tabTypesProp = serializedObject.FindProperty("<TabTypes>k__BackingField");
                if (tabTypesProp != null) EditorGUILayout.PropertyField(tabTypesProp, new GUIContent("Tab Types"));
                else deviceInfo.TabTypes = (TabType)EditorGUILayout.EnumFlagsField("Tab Types", deviceInfo.TabTypes);
                
                EditorGUILayout.Space(10);
                
                // Use the TabTypes flag enum to determine visibility
                bool showReadings = (deviceInfo.TabTypes & TabType.Readings) != 0;
                bool showControls = (deviceInfo.TabTypes & TabType.Controls) != 0;

                // Readings Section
                if (showReadings)
                {
                    DrawReadingsSection(deviceInfo);
                }

                // Controls Section
                if (showControls)
                {
                    DrawControlsSection(deviceInfo);
                }

                EditorGUILayout.EndVertical();
                
                // Bottom buttons section
                EditorGUILayout.Space(10);
                
                // Action buttons at the bottom
                // Only show Create button if Name is not empty and has MenuBuilder component
                EditorGUILayout.BeginHorizontal();
                
                using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(deviceInfo.Name) || !hasMenuBuilder))
                {
                    if (GUILayout.Button("Create Menu", _buttonStyle))
                    {
                        if (menuBuilder != null)
                        {
                            SaveModifications();
                            menuBuilder.BuildDeviceMenu(deviceInfo);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Missing Component", 
                                "DeviceMenuBuilder component is required on this GameObject.", "OK");
                        }
                    }
                }
                
                if (GUILayout.Button("Clear Fields", _buttonStyle))
                {
                    ClearFields(deviceInfo);
                }
                
                if (GUILayout.Button("Close", _buttonStyle))
                {
                    _isInitialized = false;
                }
                
                EditorGUILayout.EndHorizontal();
                
                // Show warning if needed
                if (string.IsNullOrEmpty(deviceInfo.Name))
                {
                    EditorGUILayout.HelpBox("Device Name is required to create a menu.", UnityEditor.MessageType.Warning);
                }
                else if (!hasMenuBuilder)
                {
                    EditorGUILayout.HelpBox("DeviceMenuBuilder component is missing on this GameObject.", UnityEditor.MessageType.Warning);
                }
                
                if(serializedObject.hasModifiedProperties)
                    SaveModifications();
            }
        }

        private void DrawReadingsSection(DeviceInfo deviceInfo)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Readings Components", _sectionHeaderStyle);

            if(_readingsComponentsProp == null) _readingsComponentsProp = serializedObject.FindProperty("_readingsComponents");
            if (_readingsComponentsProp == null) _readingsComponentsProp = serializedObject.FindProperty("<ReadingsComponents>k__BackingField");
            
            if (_readingsComponentsProp != null)
            {
                EditorGUILayout.PropertyField(_readingsComponentsProp, new GUIContent("Readings Components"), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Couldn't find serialized property for ReadingsComponents", UnityEditor.MessageType.Warning);
            }

            if (!_showNewReadingsComponentFields)
            {
                if (GUILayout.Button("Add New Readings Component"))
                {
                    _showNewReadingsComponentFields = true;
                }
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("New Readings Component", EditorStyles.boldLabel);
                
                _selectedReadingsComponentType = (ReadingsComponentType)EditorGUILayout.EnumPopup("Component Type", _selectedReadingsComponentType);
                _newReadingsComponentName = EditorGUILayout.TextField("Name", _newReadingsComponentName);

                switch (_selectedReadingsComponentType)
                {
                    case ReadingsComponentType.DisplayField:
                        _newDisplayFieldLabel = EditorGUILayout.TextField("Display Field Label", _newDisplayFieldLabel);
                        _newDisplayTextSign = EditorGUILayout.TextField("Display Text Sign", _newDisplayTextSign);
                        break;
                    case ReadingsComponentType.DeviceScreen:
                        // Add specific fields for DeviceScreen if needed
                        break;
                    case ReadingsComponentType.CameraView:
                        // Add specific fields for CameraView if needed
                        break;
                    // case ReadingsComponentType.Graph:
                    //     // Add specific fields for Graph if needed
                    //     break;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Component"))
                {
                    if(AddNewReadingsComponent(deviceInfo)) // Proceed only if the name is not a duplicate
                    {
                        _showNewReadingsComponentFields = false;  // Hide the fields after adding
                        ResetNewReadingsComponentFields();       // Reset input fields
                    }
                }

                if (GUILayout.Button("Cancel"))
                {
                    _showNewReadingsComponentFields = false;  // Hide the fields without adding
                    ResetNewReadingsComponentFields();       // Reset input fields
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawControlsSection(DeviceInfo deviceInfo)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Controls Components", _sectionHeaderStyle);

            if (_controlsComponentsProp == null) _controlsComponentsProp = serializedObject.FindProperty("_controlsComponents");
            if (_controlsComponentsProp == null) _controlsComponentsProp = serializedObject.FindProperty("<ControlsComponents>k__BackingField");
            
            if (_controlsComponentsProp != null)
            {
                EditorGUILayout.PropertyField(_controlsComponentsProp, new GUIContent("Controls Components"), true);
            }
            else
            {
                EditorGUILayout.HelpBox("Couldn't find serialized property for ControlsComponents", UnityEditor.MessageType.Warning);
            }

            if (!_showNewControlsComponentFields)
            {
                if (GUILayout.Button("Add New Controls Component"))
                {
                    _showNewControlsComponentFields = true;
                }
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("New Controls Component", EditorStyles.boldLabel);
                
                _selectedControlsComponentType = (ControlsComponentType)EditorGUILayout.EnumPopup("Component Type", _selectedControlsComponentType);
                _newControlsComponentName = EditorGUILayout.TextField("Name", _newControlsComponentName);

                switch (_selectedControlsComponentType)
                {
                    case ControlsComponentType.Slider:
                        _newSliderSymbol = EditorGUILayout.TextField("Symbol", _newSliderSymbol);
                        _newSliderMin = EditorGUILayout.FloatField("Min", _newSliderMin);
                        _newSliderMax = EditorGUILayout.FloatField("Max", _newSliderMax);
                        _newShouldDispayScreenValue = EditorGUILayout.Toggle("Should Display Screen Value", _newShouldDispayScreenValue);
                        break;

                    case ControlsComponentType.Dropdown:
                        // Dropdown options management section
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Dropdown Options", EditorStyles.boldLabel);
                        
                        // Display current options list
                        if (_dropdownOptions.Count > 0)
                        {
                            for (int i = 0; i < _dropdownOptions.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                _dropdownOptions[i] = EditorGUILayout.TextField($"Option {i + 1}", _dropdownOptions[i]);
                                
                                if (GUILayout.Button("×", GUILayout.Width(25)))
                                {
                                    _dropdownOptions.RemoveAt(i);
                                    GUI.FocusControl(null);
                                    break;
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No options added. Add at least one option.", UnityEditor.MessageType.Warning);
                        }
                        
                        // Add new option field
                        EditorGUILayout.BeginHorizontal();
                        _newOptionText = EditorGUILayout.TextField("New Option", _newOptionText);
                        if (GUILayout.Button("Add", GUILayout.Width(60)) && !string.IsNullOrEmpty(_newOptionText))
                        {
                            _dropdownOptions.Add(_newOptionText);
                            _newOptionText = "";
                            GUI.FocusControl(null);
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.EndVertical();
                        
                        _newShouldConvertExponential = EditorGUILayout.Toggle("Convert Exponential", _newShouldConvertExponential);
                        break;
                        
                    case ControlsComponentType.TextInputField:
                        break;
                        
                    case ControlsComponentType.Checkbox:
                        _newIsCheckboxTrue = EditorGUILayout.Toggle("Checkbox Bool", _newIsCheckboxTrue);
                        break;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Component"))
                {
                    if(AddNewControlsComponent(deviceInfo)) // Proceed only if the name is not a duplicate
                    {
                        _showNewControlsComponentFields = false;  // Hide the fields after adding
                        ResetNewControlsComponentFields();       // Reset input fields
                    }
                }

                if (GUILayout.Button("Cancel"))
                {
                    _showNewControlsComponentFields = false;  // Hide the fields without adding
                    ResetNewControlsComponentFields();       // Reset input fields
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private bool AddNewReadingsComponent(DeviceInfo deviceInfo)
        {
            // Check if the component name already exists in the ReadingsComponents list
            bool isDuplicateName = deviceInfo.ReadingsComponents.Exists(component => component.Name == _newReadingsComponentName);

            if (string.IsNullOrEmpty(_newReadingsComponentName))
            {
                EditorUtility.DisplayDialog("Invalid Name", "Component name cannot be empty.", "OK");
                return false;
            }

            if (isDuplicateName)
            {
                EditorUtility.DisplayDialog("Duplicate Component Name", $"A component with the name '{_newReadingsComponentName}' already exists.", "OK");
                return false; // Prevent proceeding if the name is duplicate
            }

            ReadingsComponent newComponent = null;

            switch (_selectedReadingsComponentType)
            {
                case ReadingsComponentType.DisplayField:
                    newComponent = new DisplayFieldReadingsComponent
                    {
                        Name = _newReadingsComponentName,
                        Label = _newDisplayFieldLabel,
                        DisplayTextSign = _newDisplayTextSign
                    };
                    break;
                case ReadingsComponentType.DeviceScreen:
                    newComponent = new DeviceScreenReadingsComponent
                    {
                        Name = _newReadingsComponentName,
                    };
                    break;
                case ReadingsComponentType.CameraView:
                    newComponent = new CameraViewReadingsComponent
                    {
                        Name = _newReadingsComponentName,
                    };
                    break;
            }

            if (newComponent != null)
            {
                deviceInfo.ReadingsComponents.Add(newComponent);
                EditorUtility.SetDirty(deviceInfo);
                SaveModifications();
            }

            return true; // Successfully added the new component
        }

        private bool AddNewControlsComponent(DeviceInfo deviceInfo)
        {
            // Check if the component name already exists in the ControlsComponents list
            bool isDuplicateName = deviceInfo.ControlsComponents.Exists(component => component.Name == _newControlsComponentName);

            if (string.IsNullOrEmpty(_newControlsComponentName))
            {
                EditorUtility.DisplayDialog("Invalid Name", "Component name cannot be empty.", "OK");
                return false;
            }

            if (isDuplicateName)
            {
                EditorUtility.DisplayDialog("Duplicate Component Name", $"A component with the name '{_newControlsComponentName}' already exists.", "OK");
                return false; // Prevent proceeding if the name is duplicate
            }

            ControlsComponent newComponent = null;

            switch (_selectedControlsComponentType)
            {
                case ControlsComponentType.Slider:
                    newComponent = new SliderControlsComponent
                    {
                        Name = _newControlsComponentName,
                        Symbol = _newSliderSymbol,
                        MinValue = _newSliderMin,
                        MaxValue = _newSliderMax,
                        ShouldDispayScreenValue = _newShouldDispayScreenValue
                    };
                    break;

                case ControlsComponentType.Dropdown:
                    newComponent = new DropdownControlsComponent
                    {
                        Name = _newControlsComponentName,
                        Options = new List<string>(_dropdownOptions),
                        ConvertExponential = _newShouldConvertExponential
                    };
                    break;
                    
                case ControlsComponentType.TextInputField:
                    newComponent = new TextInputFieldControlsComponent
                    {
                        Name = _newControlsComponentName,
                    };
                    break;
                    
                case ControlsComponentType.Checkbox:
                    newComponent = new CheckboxControlsComponent
                    {
                        Name = _newControlsComponentName,
                        IsTrue = _newIsCheckboxTrue
                    };
                    break;
            }

            if (newComponent != null)
            {
                deviceInfo.ControlsComponents.Add(newComponent);
                EditorUtility.SetDirty(deviceInfo);
                SaveModifications();
            }

            return true; // Successfully added the new component
        }

        private void ResetNewControlsComponentFields()
        {
            _newControlsComponentName = "";
            _newSliderMin = 0;
            _newSliderMax = 100;
            _newShouldDispayScreenValue = false;
            _newShouldConvertExponential = false;
            _newSliderSymbol = "";
            _newIsCheckboxTrue = false;
            _dropdownOptions.Clear();
            _newOptionText = "";
        }

        private void ResetNewReadingsComponentFields()
        {
            _newReadingsComponentName = "";
            _newDisplayFieldLabel = "";
            _newDisplayTextSign = "";
        }
        
        private void ClearFields(DeviceInfo deviceInfo)
        {
            // Basic fields
            deviceInfo.TabTypes = TabType.None;
            deviceInfo.Name = "";
            
            // Clear component lists
            deviceInfo.ReadingsComponents.Clear();
            deviceInfo.ControlsComponents.Clear();
            
            // Reset fields
            ResetNewControlsComponentFields();
            ResetNewReadingsComponentFields();
            
            EditorUtility.SetDirty(deviceInfo);
        }

        private void SaveModifications()
        {
            // Mark object as dirty to ensure Unity saves it
            EditorUtility.SetDirty(target);
            // Force serialization of the object
            serializedObject.ApplyModifiedProperties();
            // Force Unity to save the asset
            AssetDatabase.SaveAssets();
        }
    }
}