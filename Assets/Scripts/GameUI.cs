using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI fishDescription;
    public GameObject fishPanel;
    public GameObject informationPanel;
    public RectTransform fishImageTransform;
    Animator anim;

    public static GameUI Instance { get; private set; }

    private void Awake() {
        if (Instance == null)
            Instance = this;
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

    public void ShowInfo(string infoText)
    {
        StopAllCoroutines();
        informationPanel.SetActive(true);
        informationPanel.GetComponentInChildren<TextMeshProUGUI>().text = infoText;
        StartCoroutine(HideInfoPanel());
    }

    IEnumerator HideInfoPanel()
    {
        yield return new WaitForSeconds(3);
        informationPanel.SetActive(false);
    }
}
