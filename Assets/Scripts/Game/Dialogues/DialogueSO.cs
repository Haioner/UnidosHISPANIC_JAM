using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AwnserConsequence
{
    public List<string> DialogueAfterAwnser;
    public List<LegendStats> ConsequenceStats = new List<LegendStats>();
    public List<DialogueSO> ConsequenceDialogues = new List<DialogueSO>();
    public float SoulsAmount;

    [Header("Effector")]
    public List<GameObject> ConsequenceEffector;
}

[CreateAssetMenu(menuName ="Game/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public CharacterSO characterOwner;

    [Space]
    public bool needAwnser = true;
    public List<string> dialogue;

    [Header("Souls")]
    public float DefaultSouls;

    [Header("Consequences")]
    public AwnserConsequence YES_AwnserConsequences;
    public AwnserConsequence NO_AwnserConsequences;
}