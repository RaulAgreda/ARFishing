using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogsController : MonoBehaviour
{
    public Image memeFace;
    public static DialogsController instance;
    public GameObject UIPanel;
    public Transform dialogsPanel;

    float eventIdx = 0;

    string[] dialogs = new string[] {
        "Espera un momento...",
        "Esa cosa acaba de explotar ¿verdad?",
        "Será mejor que desaparezcamos antes de que haya problemas",
        "¡Mierda Greenpeace!",
        "¡Pesca las minas y reviéntalos!"
    };
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        memeFace.color = new Color(1, 1, 1, 0);
        dialogsPanel.GetComponent<Button>().onClick.AddListener(NextDialogEvent);
        dialogsPanel.gameObject.SetActive(false);
        UIPanel.SetActive(false);
    }

    public void FoquitaExplodedEvent()
    {
        StartCoroutine(StartDialogs());
    }

    public void NextDialogEvent()
    {
        if (eventIdx == 1)
            StartCoroutine(TextAnimationDialog(dialogs[1]));
        else if (eventIdx == 2)
            StartCoroutine(TextAnimationDialog(dialogs[2]));
        else if (eventIdx == 3)
        {
            UIPanel.SetActive(false);
            StartCoroutine(ExecuteAfterTime(3, () => 
            { 
                GameUI.Instance.ShowInfo(dialogs[3]);
                NextDialogEvent();
            }
            ));
        }
        else if (eventIdx == 4)
        {
            StartCoroutine(ExecuteAfterTime(3, () => 
            {
                FindFirstObjectByType<MineSpawner>().SpawnMines();
            }
            ));

        }
        eventIdx++;
    }

    IEnumerator TextAnimationDialog(string text)
    {
        TextMeshProUGUI textDialog = dialogsPanel.GetComponentInChildren<TextMeshProUGUI>();
        textDialog.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            textDialog.text += text[i];
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator StartDialogs()
    {
        yield return new WaitForSeconds(3);
        UIPanel.SetActive(true);
        while (memeFace.color.a < 1)
        {
            memeFace.color += new Color(0, 0, 0, 0.01f);
            yield return null;
        }
        yield return new WaitForSeconds(1);
        dialogsPanel.gameObject.SetActive(true);
        StartCoroutine(TextAnimationDialog(dialogs[0]));
        eventIdx++;
    }

    IEnumerator ExecuteAfterTime(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
