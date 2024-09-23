using UnityEngine;
using TMPro;

public class FloatNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floatNumberText;

    [Header("Destroy")]
    [SerializeField] private float delayToDestroy = 0.5f;
    [SerializeField] private float destroySpeed = 3f;
    [SerializeField] private CanvasGroup cg;
    private bool canFadeOut;

    private void Update()
    {
        FadeOutCanvas();
    }

    public void InitFloatNumber(float numberValue, Color textColor)
    {
        floatNumberText.text = numberValue.ToString();
        floatNumberText.color = textColor;
        Invoke(nameof(EnableFadeOut), delayToDestroy);
    }

    private void EnableFadeOut()
    {
        canFadeOut = true;
    }

    private void FadeOutCanvas()
    {
        if (!canFadeOut) return;

        cg.alpha = Mathf.MoveTowards(cg.alpha, 0, destroySpeed * Time.deltaTime);
        if (cg.alpha <= 0)
            Destroy(gameObject);
    }
}
