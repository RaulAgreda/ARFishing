using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI fishDescription;
    public GameObject fishPanel;
    public GameObject informationPanel;
    public RectTransform fishImageTransform;
    public Image fadeOutScreen;
    Animator anim;

    public static GameUI Instance { get; private set; }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        fadeOutScreen.color = new Color(0, 0, 0, 0);
    }

    private void Start() {
        anim = GetComponent<Animator>();
        fishPanel.SetActive(false);
        informationPanel.SetActive(false);
    }

    public void LostCatch()
    {
        fishDescription.text = "Jaja se te ha escapado, vaya inútil! XD";
        fishImageTransform.localPosition = new(-9999, -9999);
        // fishPanel.SetActive(true);
        informationPanel.SetActive(true);
        StartCoroutine(HideInfoPanel());
    }

    public void ShowFish(FishData fish)
    {
        string text = fish.description.Replace("\\n", "\n");
        fishDescription.text = text;
        fishImageTransform.localPosition = fish.imageCoords;
        fishPanel.SetActive(true);
        anim.SetTrigger("Catch");
    }

    public void ShowInfo(string infoText, bool hide = true)
    {
        StopAllCoroutines();
        informationPanel.SetActive(true);
        informationPanel.GetComponentInChildren<TextMeshProUGUI>().text = infoText;
        if (hide)
            StartCoroutine(HideInfoPanel());
    }

    public void HideInfo()
    {
        informationPanel.SetActive(false);
    }

    IEnumerator HideInfoPanel()
    {
        yield return new WaitForSeconds(3);
        informationPanel.SetActive(false);
    }

    public void FadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOutCorroutine(fadeDuration));
    }

    IEnumerator FadeOutCorroutine(float fadeDuration)
    {
        fadeOutScreen.color = new Color(0, 0, 0, 0);
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            fadeOutScreen.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeOutScreen.color = new Color(0, 0, 0, 1);
    }
}
