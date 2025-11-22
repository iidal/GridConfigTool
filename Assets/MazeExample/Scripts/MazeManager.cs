using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private MazeCreator m_mazeCreator;

    [SerializeField] private GameObject m_menuUi;
    [SerializeField] private GameObject m_endPopup;
    [SerializeField] private List<GridSO> m_levelConfigs;
    [SerializeField] private GameObject m_levelButtonPrefab;
    [SerializeField] private Transform m_levelButtonParent;
    void Start()
    {
        InitMenu();
    }


    void InitMenu()
    {
        foreach (GridSO config in m_levelConfigs)
        {

            GameObject buttonObj = Instantiate(m_levelButtonPrefab, m_levelButtonParent);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
            levelButton.InitConfig(config, this);

            //example for using and parsing a custom field
            GridSO.ConfigurableField customField = new GridSO.ConfigurableField();
            if (GridConfigHelper.GetCustomField(ref customField, config, "LevelDisabled"))
            {
                if (GridConfigHelper.ParseBool(customField.fieldValue) == true)
                {
                    levelButton.SetDisabled();
                }
            }
        }
    }
    public void StartLevel(GridSO levelConfig)
    {
        m_menuUi.SetActive(false);
        m_mazeCreator.CreateMaze(levelConfig);
    }
    public void LevelCompleted()
    {
        Debug.Log("Level Completed!");
        m_endPopup.SetActive(true);

    }
    public void Restart()
    {
        // Lazy :(
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


}
