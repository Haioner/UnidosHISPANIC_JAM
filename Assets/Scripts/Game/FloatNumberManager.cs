using UnityEngine;

public class FloatNumberManager : MonoBehaviour
{
    [Header("Gain Float")]
    [SerializeField] private Transform gainHolder;
    [SerializeField] private FloatGain floatGainPrefab;

    [Header("Float Number")]
    [SerializeField] private FloatGain floatNumberPrefab;

    public void SpawnGainFloat(string text)
    {
        FloatGain floatGain = Instantiate(floatGainPrefab, gainHolder);
        floatGain.SetFloatText(text);
    }

    public void SpawnFloatNumber(string text, Vector3 spawnPos)
    {
        FloatGain floatNumber = Instantiate(floatNumberPrefab, spawnPos, Quaternion.identity);
        floatNumber.SetFloatText(text);
    }
}
