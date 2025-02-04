using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishDialogs : MonoBehaviour
{
    public TextMeshProUGUI fishDescription;
    public GameObject fishPanel;
    public RectTransform fishImageTransform;
    public FishData[] fishData = 
    {
        new("Atún", "¡He pescado un atún!\nEn estas aguas no es muy común…", Vector2.zero),
        new("Amarguillo", "¡He pescado un amarguillo!\nPero no le noto el saborcillo…" , Vector2.zero),
        new("Pez payaso", "¡He pescado un pez payaso!\n¡El rey del humor, abran paso!" , Vector2.zero),
        new("Salmón real", "¡Chachiiii! ¡He pescado un salmón real!\n¡Fíjate qué porte más serial!", Vector2.zero),
        new("Dorada japonesa", "¡He pescado una dorada japonesa!\n¡Konnichiwa, princesa!" , Vector2.zero),
        new("Perca", "¡He pescado una perca!\n¡Da impresión verla de cerca!" , Vector2.zero),
        new("Esturión", "¡He pescado un esturión!\nMe merezco una ovación." , Vector2.zero),
        new("Pez cirujano", "¡He pescado un pez cirujano!\n¡El esfuerzo no ha sido en vano.", Vector2.zero)
    };

    private void Start() {
        fishPanel.SetActive(false);
    }

    public void GetRandomFish()
    {
        var randomFish = fishData[Random.Range(0, fishData.Length)];
        string text = randomFish.description.Replace("\\n", "\n");
        fishDescription.text = text;
        fishImageTransform.localPosition = randomFish.imageCoords;
        fishPanel.SetActive(true);
        StartCoroutine(ClearTextsCoroutine());
    }

    public void LostCatch()
    {
        fishDescription.text = "Jaja se te ha escapado, vaya inútil! XD";
        fishImageTransform.localPosition = new(-9999, -9999);
        fishPanel.SetActive(true);
        StartCoroutine(ClearTextsCoroutine());
    }

    IEnumerator ClearTextsCoroutine()
    {
        yield return new WaitForSeconds(5);
        fishPanel.SetActive(false);
    }


}

[System.Serializable]
public class FishData
{
    public string name;
    public string description;
    public Vector2 imageCoords;

    public FishData(string name, string description, Vector2 imageCoords)
    {
        this.name = name;
        this.description = description;
        this.imageCoords = imageCoords;
    }
}
