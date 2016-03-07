using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HpBar : MonoBehaviour
{
    void Awake()
    {
        rt = gameObject.GetComponent<RectTransform>();
        maxValue = rt.sizeDelta.x;
        t = 1f;
    }

    private void UpdateValue(float t)
    {
        float x = Mathf.Lerp(0f, maxValue, t);
        rt.sizeDelta = new Vector2(x, rt.sizeDelta.y);
    }

    void Update()
    {
        t -= 0.02f;

        UpdateValue(t);

        if (t <= 0f)
        {
            t = 1f;
        }
    }

    private float t;
    private float maxValue;
    private RectTransform rt;
}