using UnityEngine;

public class PictureItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetPictureItem(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
