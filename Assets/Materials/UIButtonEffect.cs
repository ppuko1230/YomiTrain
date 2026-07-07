using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    private Material material;

    private void Awake()
    {
        Image image = GetComponent<Image>();

        if (image != null)
        {
            // ボタンごとにMaterialを複製
            material = Instantiate(image.material);
            image.material = material;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (material != null)
            material.SetFloat("_Hover", 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (material != null)
        {
            material.SetFloat("_Hover", 0f);
            material.SetFloat("_Pressed", 0f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (material != null)
            material.SetFloat("_Pressed", 1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (material != null)
            material.SetFloat("_Pressed", 0f);
    }
}