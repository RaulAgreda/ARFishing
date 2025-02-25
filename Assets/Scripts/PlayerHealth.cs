using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public GameObject heartIconPrefab;
    public Sprite heartBrokenIcon;
    public Transform heartsPanel;
    public Image brokenScreenImage;
    public AudioClip brokenGlassSound;
    int _currentHealth;
    bool hitCooldown = false;

    void Start()
    {
        _currentHealth = maxHealth;
        for (int i = 0; i < maxHealth; i++)
            Instantiate(heartIconPrefab, heartsPanel);

        heartsPanel.gameObject.SetActive(false);
        brokenScreenImage.color = new Color(1, 1, 1, 0);
    }

    public void TakeDamage()
    {
        if (hitCooldown)
            return;
        heartsPanel.gameObject.SetActive(true);
        _currentHealth--;
        // Remove a heart icon
        heartsPanel.GetChild(heartsPanel.childCount - 1).GetComponent<Image>().sprite = heartBrokenIcon;
        Destroy(heartsPanel.GetChild(heartsPanel.childCount - 1).gameObject, 1);
        if (_currentHealth <= 0)
        {
            DialogsController.instance.Die();
        }
        AudioManager.instance.PlayOneShot(brokenGlassSound);
        StopAllCoroutines();
        StartCoroutine(ShowBrokenScreen());
    }
    IEnumerator ShowBrokenScreen()
    {
        hitCooldown = true;
        brokenScreenImage.color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.5f);
        float fadeTime = 2f;
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            brokenScreenImage.color = new Color(1, 1, 1, 1 - elapsedTime / fadeTime);
            yield return null;
        }
        hitCooldown = false;
    }

}
