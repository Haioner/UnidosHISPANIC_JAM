using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private TextMeshProUGUI gameOverInfoTEXT;

    private void OnEnable()
    {
        legendsManager.onLegendDies += CheckLegends;
        LegendsManager.OnPopulationMax += OverPopulation;
    }

    private void OnDisable()
    {
        legendsManager.onLegendDies -= CheckLegends;
        LegendsManager.OnPopulationMax -= OverPopulation;
        
    }

    private void OverPopulation(object sender, System.EventArgs e)
    {
        GameOver("World population has surpassed 20 billion, and all the legends have been hunted...");
    }

    private void CheckLegends(int legendCount)
    {
        if(legendCount <= 0)
        {
            GameOver("You killed all the legends...");
        }
    }

    public void GameOver(string overReason)
    {
        FindFirstObjectByType<TimeController>().enabled = false;
        Time.timeScale = 0;
        gameOverInfoTEXT.text = overReason;
        gameOverObject.SetActive(true);
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
