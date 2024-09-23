using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTonEnable : MonoBehaviour
{
    [SerializeField] private List<DOTweenAnimation> dotAnimations;

    private void OnEnable()
    {
        foreach (var dot in dotAnimations)
        {
            dot.DORestart();
        }
    }
}
