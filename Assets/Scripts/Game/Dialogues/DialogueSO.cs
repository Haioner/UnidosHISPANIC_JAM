using UnityEngine.Localization.Tables;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AwnserConsequence
{
    public List<LegendStats> ConsequenceStats = new List<LegendStats>();
    public List<DialogueSO> ConsequenceDialogues = new List<DialogueSO>();
    public long SoulsAmount;

    [Header("Effector")]
    public List<GameObject> ConsequenceEffector;
}

[CreateAssetMenu(menuName ="Game/Dialogue", order = 0)]
public class DialogueSO : ScriptableObject
{
    public CharacterSO characterOwner;

    [Header("Souls")]
    public long FreeSouls_END;
    public bool isTaxes;

    [Header("Dialogue")]
    public bool needAwnser = true;
#if UNITY_EDITOR
    public UnityEditor.Localization.StringTableCollection dialogueTable;
#endif
    //public StringTable runtimeDialogueTable;
    public StringTable dialogueEnglish, dialogueBR;

    [Header("Consequences")]
    public AwnserConsequence YES_AwnserConsequences;
    public AwnserConsequence NO_AwnserConsequences;
}
