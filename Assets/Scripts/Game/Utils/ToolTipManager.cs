using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipTEXT;
    [SerializeField] private RectTransform tooltipBackground;
    private bool isActive;

    private void Update()
    {
        if (isActive)
        {
            Vector3 mousePosition = Input.mousePosition;
            tooltipBackground.position = mousePosition;
        }
    }

    public void SetToolTip(string text)
    {
        if (!isActive)
        {
            tooltipTEXT.text = text;
            isActive = true;
            tooltipBackground.gameObject.SetActive(true);
        }
    }

    public void DeactiveToolTip()
    {
        tooltipTEXT.text = "";
        isActive = false;
        tooltipBackground.gameObject.SetActive(false);
    }
}
