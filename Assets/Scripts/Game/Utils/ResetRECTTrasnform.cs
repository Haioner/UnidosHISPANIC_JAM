using UnityEngine;

public class ResetRECTTrasnform : MonoBehaviour
{

    public void ResetRectTransformTo1()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}
