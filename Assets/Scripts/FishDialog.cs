using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishDialogs : MonoBehaviour
{
    public TextMeshProUGUI FishTitle;
    public TextMeshProUGUI FishDescription;
    public Dictionary<string, string> fishDialogues = new ()
    {
        { "Atún", "¡He pescado un atún! En estas aguas no es muy común…" },
        { "Amarguillo", "¡He pescado un amarguillo! Pero no le noto el saborcillo…" },
        { "Pez payaso", "¡He pescado un pez payaso! ¡El rey del humor, abran paso!" },
        { "Salmón real", "¡Chachiiii! ¡He pescado un salmón real! ¡Fíjate qué porte más señorial!" },
        { "Dorada japonesa", "¡He pescado una dorada japonesa! ¡Konnichiwa, princesa!" },
        { "Perca", "¡He pescado una perca! ¡Da impresión verla de cerca!" },
        { "Esturión", "¡He pescado un esturión! Me merezco una ovación." },
        { "Pez cirujano", "¡He pescado un pez cirujano! ¡El esfuerzo no ha sido en vano." }
    };

    private void Start() {
        FishTitle.text = "";
        FishDescription.text = "";
    }

    public void GetRandomFish()
    {
        List<string> keys = new List<string>(fishDialogues.Keys);
        string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
        // FishTitle.text = $"¡He pescado un {randomKey}!";
        FishDescription.text = fishDialogues[randomKey];
        StartCoroutine(ClearTextsCoroutine());
    }

    public void LostCatch()
    {
        FishDescription.text = "Jaja se te ha escapado, vaya inútil! XD";
        StartCoroutine(ClearTextsCoroutine());
    }

    IEnumerator ClearTextsCoroutine()
    {
        yield return new WaitForSeconds(5);
        FishTitle.text = "";
        FishDescription.text = "";
    }
    
}
