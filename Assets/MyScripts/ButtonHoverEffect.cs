using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalColor = new Color(0.48f, 0.30f, 0.07f); // بني غامق
    public Color hoverColor  = new Color(1f, 0.72f, 0.28f);     // ذهبي فاتح
    public float hoverScale  = 1.12f; // كم يكبر الزر (1.0 = عادي، 1.12 = يكبر 12%)

    private Image btnImage;
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Awake()
    {
        btnImage = GetComponent<Image>();
        originalScale = transform.localScale;
        if (btnImage)
            btnImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btnImage)
            btnImage.color = hoverColor;
        StartScale(hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (btnImage)
            btnImage.color = normalColor;
        StartScale(1f);
    }

    void StartScale(float targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(targetScale));
    }

    System.Collections.IEnumerator ScaleTo(float target)
    {
        float time = 0f;
        float duration = 0.12f; // سرعة التكبير/التصغير
        Vector3 start = transform.localScale;
        Vector3 end = originalScale * target;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(start, end, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localScale = end;
    }
}
