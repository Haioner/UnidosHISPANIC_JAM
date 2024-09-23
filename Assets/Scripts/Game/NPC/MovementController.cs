using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Transform enterPosition, exitPosition;
    [SerializeField] private Animator anim;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform pivotTransform;

    private bool isMoving;
    private Transform targetPosition;
    private bool isFacingRight = true;
    public static event System.EventHandler OnMoveEnter;


    private void OnEnable()
    {
        DialogueManager.OnDialogueEND += MoveExit;
        MoveEnter();
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEND -= MoveExit;
    }

    private void Update()
    {
        if (isMoving)
            MoveCharacter();
    }

    private void MoveExit(object sender, System.EventArgs a)
    {
        targetPosition = exitPosition;
        SetMoving(true);
        Flip();
    }

    private void MoveEnter()
    {
        OnMoveEnter?.Invoke(this, System.EventArgs.Empty);
        targetPosition = enterPosition;
        SetMoving(true);
        Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        if (isFacingRight)
            pivotTransform.localScale = new Vector3(1, 1, 1);
        else
            pivotTransform.localScale = new Vector3(-1, 1, 1);
    }

    private void SetMoving(bool state)
    {
        isMoving = state;
        anim.SetBool("isWalking", state);
    }

    private void MoveCharacter()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            SetMoving(false);

            if (targetPosition == enterPosition)
            {
                dialogueManager.NextDialogue();
            }
            if(targetPosition == exitPosition)
            {
                MoveEnter();
            }
        }
    }
}
