using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private float speed = 3;

    private void Update()
    {
        if (Input.GetMouseButton(0))
            Time.timeScale = speed;
        else
            Time.timeScale = 1;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
