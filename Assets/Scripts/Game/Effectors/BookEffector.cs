using UnityEngine;

public class BookEffector : MonoBehaviour
{
    private void OnEnable()
    {
        FindFirstObjectByType<LegendsManager>().AddBook(1);
        FindFirstObjectByType<BookController>().AddBook();
        Destroy(gameObject);
    }
}
