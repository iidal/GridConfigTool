using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelButton : MonoBehaviour
{
    private MazeManager m_manager;
    [SerializeField] private TextMeshProUGUI m_levelNameText;
    [SerializeField] private GridSO m_levelConfig;

    public void InitConfig(GridSO config, MazeManager manager)
    {
        m_manager = manager;
        m_levelConfig = config;
        m_levelNameText.text = m_levelConfig.puzzleName;
    }
    public void OnClick()
    {
        Debug.Log($"Clicked level: {m_levelConfig.puzzleName}");
        m_manager.StartLevel(m_levelConfig);
    }
    public void SetDisabled()
    {
        GetComponent<Button>().interactable = false;
    }
}
