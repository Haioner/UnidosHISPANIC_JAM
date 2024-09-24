using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private LegendsManager legendsManager;

    private void OnEnable()
    {
        legendsManager.onLegendDies += CheckLegends;
    }

    private void CheckLegends(int legendCount)
    {
        if(legendCount <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOverObject.SetActive(true);
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
