using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishDB : MonoBehaviour
{
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

    public static FishDB Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        ShowFish(null);
    }

    public FishData GetRandomFish()
    {
        return fishData[Random.Range(0, fishData.Length)];
    }

    public void ShowFish(FishData fish)
    {
        foreach(var f in fishData)
            f.sceneFishInstance.SetActive(fish == f);

    }
}

[System.Serializable]
public class FishData
{
    public string name;
    public string description;
    public Vector2 imageCoords;
    public GameObject sceneFishInstance;

    public FishData(string name, string description, Vector2 imageCoords)
    {
        this.name = name;
        this.description = description;
        this.imageCoords = imageCoords;
    }
}
