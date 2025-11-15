using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.IO;
public class ConfigData
{
    // add also enum/something array so not restricted to two values

    [System.Serializable]
    public class Row
    {
        public bool[] row;
        public string[] notBoolRow; //???
    }
    public Row[] rows;
    public string id;
    public string name;
    public uint rowsCount;
    public uint columnsCount;
    public uint difficulty;
}
public class gridButtonData
{
    public int x; // row index
    public int y;  // column index
    public string value; // not going to be string forever
};
public class GridConfigTool : EditorWindow
{
    private VisualElement m_rightView;
    private VisualElement m_mainView;
    private VisualElement m_buttonContainer;

    uint m_columnCount = 3;
    uint m_rowCount = 4;
    uint m_difficulty = 1;
    string m_configId = "";
    string m_configName = "";
    string m_assetPath = "";

    List<gridButtonData> m_buttonData = new List<gridButtonData>();

    [MenuItem("Window/GridConfigTool")]
    public static void ShowMyEditor()
    {

        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<GridConfigTool>();
        wnd.titleContent = new GUIContent("GridConfigTool");

        // Limit size of the window.
        wnd.minSize = new Vector2(300, 300);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void OnEnable()
    {
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
        // ==== GRID SIZE ===================================================================================================
        var rowsCount = new UnsignedIntegerField("Rows:");
        rowsCount.value = (uint)m_rowCount;
        var columnsCount = new UnsignedIntegerField("Columns:");
        columnsCount.value = (uint)m_columnCount;

        m_mainView.Add(rowsCount);
        m_mainView.Add(columnsCount);

        var updateGridButton = new Button(() =>
        {
            Debug.Log("Update Grid button clicked.");
            m_rowCount = rowsCount.value;
            m_columnCount = columnsCount.value;
            CreateGrid();
        })
        {
            text = "Update Grid"
        };
        m_mainView.Add(updateGridButton);

        // ==== GRID ========================================================================================================

        m_buttonContainer = new VisualElement();
        CreateGrid();

        // ==== GENERATING CONFIG===========================================================================================
        // Add another button to retrieve information from all buttons.
        var retrieveButton = new Button(() =>
        {
            Debug.Log("Retrieve button clicked. Processing all button data...");
            foreach (var data in m_buttonData)
            {
                Debug.Log($"Processing data: {data.value} at ({data.x}, {data.y})");
            }
            m_configId = configId.value;
            m_configName = configName.value;
            m_assetPath = pathField.value;
            m_difficulty = difficulty.value;
            Save();
        })
        {
            text = "Retrieve Data"
        };
        m_mainView.Add(retrieveButton);

    }


    private void CreateGrid()
    {
        // this doesnt work correctly yet (adds the grid as last elemet), but do the two split pane update first
        m_buttonContainer.Clear();

        m_buttonContainer = new VisualElement();
        m_buttonContainer.style.flexDirection = FlexDirection.Row;
        m_buttonContainer.style.justifyContent = Justify.Center;
        m_buttonContainer.style.marginTop = 10;

        //var buttonCount = m_columnCount * m_rowCount;
        m_buttonData = new List<gridButtonData>();
        for (int col = 0; col < m_columnCount; col++)
        {
            for (int row = 0; row < m_rowCount; row++)
            {
                m_buttonData.Add(new gridButtonData
                {
                    x = col,
                    y = row,
                    value = "default"
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


                // Create the button.
                var button = new Button();
                button.text = m_buttonData[buttonIndex].value;

                // Set button size and spacing.
                // button.style.width = 100;
                // button.style.height = 50;
                // button.style.marginRight = 5;

                // Assign the click event after the button is created.
                button.clicked += () =>
                {
                    // Access and modify the corresponding gridButtonData.
                    var buttonData = m_buttonData[buttonIndex];
                    buttonData.value = buttonData.value == "default" ? "clicked" : "default";
                    m_buttonData[buttonIndex] = buttonData; // Update the list.
                    button.text = buttonData.value; // Update the button text.
                    Debug.Log($"Button at ({buttonData.x}, {buttonData.y}) clicked. New value: {buttonData.value}");
                };

                // Set button size and spacing.
                button.style.width = 100;
                button.style.height = 50;
                button.style.marginRight = 5;

                rowContainer.Add(button);
            }

            m_buttonContainer.Add(rowContainer); // Add the row to the main container.
        }

        // Add the button container to the view
        m_rightView.Add(m_buttonContainer);
    }

    private void Save()
    {
        ConfigData configData = new ConfigData
        {
            rows = new ConfigData.Row[m_rowCount],
            id = m_configId,
            name = m_configName,
            rowsCount = m_rowCount,
            columnsCount = m_columnCount,
            difficulty = m_difficulty
        };

        // Initialize each row with a bool array of size 4.
        for (int i = 0; i < configData.rows.Length; i++)
        {
            configData.rows[i] = new ConfigData.Row
            {
                row = new bool[m_columnCount]
            };
        }
        for (int index = 0; index < m_buttonData.Count; index++)
        {
            bool clicked = m_buttonData[index].value == "clicked" ? true : false;
            configData.rows[m_buttonData[index].y].row[m_buttonData[index].x] = clicked;
        }
        string json = JsonUtility.ToJson(configData, true);
        string folderPath = $"Assets/Resources/{m_assetPath}";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText($"{folderPath}/{m_configName}.json", json);
        AssetDatabase.Refresh();
    }


}