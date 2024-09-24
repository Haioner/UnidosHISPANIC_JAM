using UnityEngine;
using TMPro;

public class FloatGain : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floatText;
    [SerializeField] private CanvasGroup cg;

    public void SetFloatText(string text)
    {
        floatText.text = text;
    }

    public void DestroyFloat()
    {
        Destroy(gameObject);
    }
}
