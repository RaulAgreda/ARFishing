using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInEffect : MonoBehaviour
{
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeInCoroutine(2));
    }

    IEnumerator FadeInCoroutine(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            image.color = new Color(0, 0, 0, 1 - elapsedTime / time);
            yield return null;
        }
    }
}
