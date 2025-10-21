using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.IO;
public class Pack {
    public List<string> packs;
}
public class GridConfigTool : EditorWindow
{
    private string path = "";
    //[SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_mainView;
    private VisualElement m_buttonContainer;

    private int m_columnCount = 3;
    private int m_rowCount = 4;

    List<string> buttonData = new List<string>();

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
        path = Path.Combine(Application.persistentDataPath, "config.json"); // TODO get from text field
    }

    public void CreateGUI()
    {
        // TODO actually go with two pane split, grid is on one side, everyhting else on the other.
        m_mainView = new VisualElement();
        var holder = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        rootVisualElement.Add(m_mainView);
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
            m_rowCount = (int)rowsCount.value;
            m_columnCount = (int)columnsCount.value;
            CreateGrid();
        })
        {
            text = "Update Grid"
        };
        m_mainView.Add(updateGridButton);

        // ==== GRID ========================================================================================================

        m_buttonContainer = new VisualElement();
        CreateGrid();
        m_mainView.Add(m_buttonContainer);

        // ==== GENERATING CONFIG===========================================================================================
        // Add another button to retrieve information from all buttons.
        var retrieveButton = new Button(() =>
        {
            Debug.Log("Retrieve button clicked. Processing all button data...");
            foreach (var data in buttonData)
            {
                Debug.Log($"Processing data: {data}");
            }
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

        int buttonCount = m_columnCount * m_rowCount;
        buttonData = Enumerable.Repeat("default", buttonCount).ToList();

        for (int row = 0; row < m_columnCount; row++)
        {
            var rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Column;
            rowContainer.style.justifyContent = Justify.FlexStart; // Align buttons to the start.
            rowContainer.style.marginBottom = 5; // Add spacing between rows.

            for (int i = 0; i < m_rowCount; i++)
            {
                int buttonIndex = row * m_rowCount + i;
                if (buttonIndex >= buttonData.Count) break; // Stop if no more buttons are left.

                var button = new Button(() =>
                {
                    Debug.Log($"Button clicked with data: {buttonData[buttonIndex]}");
                })
                {
                    text = buttonData[buttonIndex]
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
        m_mainView.Add(m_buttonContainer);
    }

    private void Save()
    {
        Pack packList = new Pack();
        packList.packs = buttonData;
        string json = JsonUtility.ToJson(packList, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved to " + path);
    }


}