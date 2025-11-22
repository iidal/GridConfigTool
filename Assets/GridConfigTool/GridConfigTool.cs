using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.IO;

[System.Serializable]
public class ConfigurableField
{
    public string fieldName;
    public string fieldType; // "int", "float", "string", "bool"
    public string fieldValue; // string for json compatibility, for now need to be parsed by user side
    public void SetValue(object value)
    {
        fieldValue = value?.ToString();
    }
}
public class ConfigData
{
    [System.Serializable]
    public class Row
    {
        public uint[] row;
    }
    public Row[] rows;
    public string id;
    public string name;
    public uint rowsCount;
    public uint columnsCount;
    public uint difficulty;
    public ConfigurableField[] customFields;
}
public class gridButtonData
{
    public int x; // row index
    public int y;  // column index
    public uint value;
};

public class GridConfigTool : EditorWindow
{
    // UI components
    private VisualElement m_rightView;
    private VisualElement m_mainView;
    private VisualElement m_buttonContainer;
    private VisualElement m_customFieldsContainer;

    // Config parameters
    uint m_valueCount = 3; // range of values can be assigned to a button, for example 3 = 0,1,2
    uint m_columnCount = 3;
    uint m_rowCount = 4;
    uint m_difficulty = 1;
    string m_configId = "";
    string m_configName = "";
    string m_assetPath = "";

    List<gridButtonData> m_buttonData = new List<gridButtonData>();
    private List<ConfigurableField> m_customFields = new List<ConfigurableField>();
    private string m_customFieldType = "string";

    //=======================================================================================================================================
    [MenuItem("Window/GridConfigTool")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<GridConfigTool>();
        wnd.titleContent = new GUIContent("GridConfigTool");

        // Limit size of the window.
        wnd.minSize = new Vector2(300, 300);
        wnd.maxSize = new Vector2(1920, 720);
    }

    public void CreateGUI()
    {
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);
        m_mainView = new VisualElement();
        m_rightView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_mainView);
        splitView.Add(m_rightView);
        // ==== SAVE PATH ===================================================================================================
        var pathTip = new Label("Save path under Assets/Resources/:");
        var pathField = new TextField("Save path:");
        m_mainView.Add(pathTip);
        m_mainView.Add(pathField);

        // ==== ID, NAMES..==================================================================================================
        var configId = new TextField("Config id:");
        m_mainView.Add(configId);
        var configName = new TextField("Config name:");
        m_mainView.Add(configName);
        var difficulty = new UnsignedIntegerField("Difficulty:");
        m_mainView.Add(difficulty);
        // ==================================================================================================================
        var valuesCount = new UnsignedIntegerField("Values per button:");
        valuesCount.value = (uint)m_valueCount;
        m_mainView.Add(valuesCount);
        var updateValueCount = new Button(() =>
        {
            m_valueCount = valuesCount.value;
        })
        {
            text = "Update value count"
        };
        m_mainView.Add(updateValueCount);
        // ==== GRID SIZE ===================================================================================================
        var rowsCount = new UnsignedIntegerField("Rows:");
        rowsCount.value = (uint)m_rowCount;
        var columnsCount = new UnsignedIntegerField("Columns:");
        columnsCount.value = (uint)m_columnCount;

        m_mainView.Add(rowsCount);
        m_mainView.Add(columnsCount);

        var updateGridButton = new Button(() =>
        {
            m_rowCount = rowsCount.value;
            m_columnCount = columnsCount.value;
            CreateGrid();
        })
        {
            text = "Update Grid"
        };
        m_mainView.Add(updateGridButton);
        // ==== CUSTOM FIELDS ==============================================================================================
        var customFieldsLabel = new Label("Add custom fields:");
        m_mainView.Add(customFieldsLabel);
        var typeDropdown = new PopupField<string>("Type", new List<string> { "string", "int", "float", "bool" }, 0);
        m_mainView.Add(typeDropdown);
        var AddFieldButton = new Button(() =>
        {
            m_customFieldType = typeDropdown.value;
            Debug.Log("Add Field button clicked." + m_customFieldType);
            AddField();
        })
        {
            text = "Add Field"
        };
        m_mainView.Add(AddFieldButton);
        m_mainView.Add(m_customFieldsContainer = new VisualElement());
        // ==== GENERATING CONFIG===========================================================================================
        var retrieveButtonJson = new Button(() =>
        {
            Debug.Log("Retrieve button json clicked. Processing all button data...");
            m_configId = configId.value;
            m_configName = configName.value;
            m_assetPath = pathField.value;
            m_difficulty = difficulty.value;
            Save("json");
        })
        {
            text = "Retrieve as JSON"
        };
        m_mainView.Add(retrieveButtonJson);

        var retrieveButtonSO = new Button(() =>
        {
            Debug.Log("Retrieve button SO clicked. Processing all button data...");
            m_configId = configId.value;
            m_configName = configName.value;
            m_assetPath = pathField.value;
            m_difficulty = difficulty.value;
            Save("so");
        })
        {
            text = "Retrieve as SO"
        };
        m_mainView.Add(retrieveButtonSO);

        // ==== GRID ========================================================================================================

        m_buttonContainer = new VisualElement();
        CreateGrid();
    }

    private void CreateGrid()
    {
        m_rightView.Clear();
        m_buttonContainer.Clear();

        m_buttonContainer = new VisualElement();
        m_buttonContainer.style.flexDirection = FlexDirection.Row;
        m_buttonContainer.style.justifyContent = Justify.Center;
        m_buttonContainer.style.marginTop = 10;

        m_buttonData = new List<gridButtonData>();
        for (int col = 0; col < m_columnCount; col++)
        {
            for (int row = 0; row < m_rowCount; row++)
            {
                m_buttonData.Add(new gridButtonData
                {
                    x = col,
                    y = row,
                    value = 0
                });
            }
        }

        for (int row = 0; row < m_columnCount; row++)
        {
            var rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Column;
            rowContainer.style.justifyContent = Justify.FlexStart; // Align buttons to the start.
            rowContainer.style.marginBottom = 5; // Add spacing between rows.

            for (int i = 0; i < m_rowCount; i++)
            {
                int buttonIndex = (int)(row * m_rowCount + i);
                if (buttonIndex >= m_buttonData.Count) break; // Stop if no more buttons are left.

                var button = new Button();
                button.text = m_buttonData[buttonIndex].value.ToString();

                button.clicked += () =>
                {
                    // Access and modify the corresponding gridButtonData.
                    var buttonData = m_buttonData[buttonIndex];
                    uint nextValue = buttonData.value == (m_valueCount - 1) ? 0 : buttonData.value + 1;
                    buttonData.value = nextValue;
                    m_buttonData[buttonIndex] = buttonData; // Update the list.
                    button.text = buttonData.value.ToString(); // Update the button text.
                    Debug.Log($"Button at ({buttonData.x}, {buttonData.y}) clicked. New value: {buttonData.value}");
                };

                // Set button size and spacing.
                button.style.width = 50;
                button.style.height = 50;
                button.style.marginRight = 1;

                rowContainer.Add(button);
            }

            m_buttonContainer.Add(rowContainer);
        }

        m_rightView.Add(m_buttonContainer);
    }
    // ==== CREATING NEW INPUT FIELDS ========================================================================================================
    private void AddField()
    {
        var newField = new ConfigurableField
        {
            fieldName = "NewField",
            fieldType = m_customFieldType,
            fieldValue = ""
        };

        // Create the UI for the field
        var fieldContainer = new VisualElement();
        fieldContainer.style.flexDirection = FlexDirection.Column;
        fieldContainer.style.marginBottom = 5;

        // Field name input
        var nameField = new TextField("Name")
        {
            value = newField.fieldName
        };
        nameField.RegisterValueChangedCallback(evt =>
        {
            newField.fieldName = evt.newValue;
        });
        fieldContainer.Add(nameField);

        // Field value input
        var valueInput = CreateInputField(newField);
        fieldContainer.Add(valueInput);

        var removeButton = new Button(() =>
        {
            Debug.Log("Remofe field");
            m_customFields.Remove(newField);
            m_customFieldsContainer.Remove(fieldContainer);
        })
        {
            text = "Remove this field"
        };

        fieldContainer.Add(removeButton);
        m_customFields.Add(newField);
        m_customFieldsContainer.Add(fieldContainer);
    }
    private VisualElement CreateInputField(ConfigurableField field)
    {
        VisualElement inputField = null;

        switch (field.fieldType)
        {
            case "string":
                var stringField = new TextField("Value (string)");
                stringField.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(evt.newValue);
                });
                inputField = stringField;
                break;
            case "int":
                var intField = new IntegerField("Value");
                intField.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(evt.newValue);
                });
                inputField = intField;
                break;
            case "float":
                var floatField = new FloatField("Value (float)");
                floatField.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(evt.newValue);
                });
                inputField = floatField;
                break;
            case "bool":
                var boolField = new Toggle("Value (bool)");
                boolField.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(evt.newValue);
                });
                inputField = boolField;
                break;
            default:
                Debug.LogError("Unsupported field type");
                break;
        }
        return inputField;
    }

    // ==== CONFIG CREATION AND SAVING =================================================================================================
    private void Save(string saveType = "so")
    {
        // TODO check for duplicate config IDs, so old ones are not overwritten
        if (m_configId == "")
        {
            Debug.LogError("Config ID is empty. Please provide a valid ID.");
            return;
        }
        ConfigData configData = new ConfigData
        {
            rows = new ConfigData.Row[m_rowCount],
            id = m_configId,
            name = m_configName,
            rowsCount = m_rowCount,
            columnsCount = m_columnCount,
            difficulty = m_difficulty,
            customFields = m_customFields.ToArray()
        };

        for (int i = 0; i < configData.rows.Length; i++)
        {
            configData.rows[i] = new ConfigData.Row
            {
                row = new uint[m_columnCount]
            };
        }
        for (int index = 0; index < m_buttonData.Count; index++)
        {
            configData.rows[m_buttonData[index].y].row[m_buttonData[index].x] = m_buttonData[index].value;
        }

        // Create save folder if it does not exist
        string folderPath = $"Assets/Resources/{m_assetPath}";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (saveType == "so")
        {
            SaveSO(configData, folderPath);
        }
        else if (saveType == "json")
        {
            SaveJson(configData, folderPath);
        }
        else
        {
            Debug.LogError("Unknown save type");
        }
    }

    private void SaveJson(ConfigData configData, string path = "Assets/Resources")
    {
        string json = JsonUtility.ToJson(configData, true);

        File.WriteAllText($"{path}/{m_configId}.json", json);
        AssetDatabase.Refresh();
    }

    private void SaveSO(ConfigData puzzleData, string path = "Assets/Resources")
    {
        GridSO puzzleScriptable = ScriptableObject.CreateInstance<GridSO>();

        puzzleScriptable.puzzleName = puzzleData.name;
        puzzleScriptable.puzzleId = puzzleData.id;
        puzzleScriptable.difficulty = puzzleData.difficulty;
        puzzleScriptable.rowCount = puzzleData.rowsCount;
        puzzleScriptable.columnCount = puzzleData.columnsCount;
        puzzleScriptable.customFields = new GridSO.ConfigurableField[puzzleData.customFields.Length];

        puzzleScriptable.rows = new GridSO.Row[puzzleData.rows.Length];
        for (int i = 0; i < puzzleData.rows.Length; i++)
        {
            puzzleScriptable.rows[i] = new GridSO.Row
            {
                row = puzzleData.rows[i].row
            };
        }
        for(int i=0; i<puzzleData.customFields.Length; i++)
        {
            GridSO.ConfigurableField field = new GridSO.ConfigurableField
            {
                fieldName = puzzleData.customFields[i].fieldName,
                fieldType = puzzleData.customFields[i].fieldType,
                fieldValue = puzzleData.customFields[i].fieldValue
            };
            Debug.Log($"Field: {field.fieldName}, Type: {field.fieldType}, Value: {field.fieldValue}");
            puzzleScriptable.customFields[i] = field;
        }

        string folderPath = $"{path}/{puzzleData.id}.asset";
        UnityEditor.AssetDatabase.CreateAsset(puzzleScriptable, folderPath);
        UnityEditor.AssetDatabase.SaveAssets();
    }

}