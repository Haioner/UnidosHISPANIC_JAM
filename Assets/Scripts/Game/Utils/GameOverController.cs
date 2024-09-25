using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private LegendsManager legendsManager;
    [SerializeField] private TextMeshProUGUI gameOverInfoTEXT;

    [Header("String Localized")]
    [SerializeField] private LocalizedString overPopulationLocale;
    [SerializeField] private LocalizedString killLegendsLocale;
    private string overPopulationString, killLegendsString;

    private void OnEnable()
    {
        legendsManager.onLegendDies += CheckLegends;
        LegendsManager.OnPopulationMax += OverPopulation;

        overPopulationLocale.StringChanged += (localizedText) => { overPopulationString = localizedText; };
        killLegendsLocale.StringChanged += (localizedText) => { killLegendsString = localizedText; };
    }

    private void OnDisable()
    {
        legendsManager.onLegendDies -= CheckLegends;
        LegendsManager.OnPopulationMax -= OverPopulation;
        
    }

    private void OverPopulation(object sender, System.EventArgs e)
    {
        GameOver(overPopulationString);
    }

    private void CheckLegends(int legendCount)
    {
        if(legendCount <= 0)
        {
            GameOver(killLegendsString);
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
