using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemComponent : MonoBehaviour, IPointerClickHandler
{
    public Item originalItem;
    public Image img;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        originalItem.OnPointerClick(pointerEventData);
    }

    public void SetImage()
    {
        img.color = originalItem.img.color;
    }
}
