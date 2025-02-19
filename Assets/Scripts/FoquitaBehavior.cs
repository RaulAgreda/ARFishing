using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FoquitaBehavior : MonoBehaviour
{
    public Transform foodToEatPosition;
    public float distanceToFood = 0.1f;
    public float distanceToEat = 0.01f;
    public Transform bellyBone;
    public GameObject deathParticles;
    public int maxFishEaten = 6;
    public AudioClip eatSound;
    public AudioClip deathSound;
    AudioSource audioSource;
    Transform bait;
    Animator anim;
    int fishEaten = 0;

    void Awake()
    {
        bait = GameObject.FindGameObjectWithTag("Bait").transform;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (FishingRod.Instance.fishingState == FishingRod.FishingState.FishCatched)
        {
            float d = Vector3.Distance(bait.position, foodToEatPosition.position);
            anim.Play("Eat", 1, 1 - Mathf.Clamp01(d / distanceToFood));
            if (d < distanceToEat)
                EatFish();
        }
    }

    private void EatFish()
    {
        fishEaten++;
        FishingRod.Instance.EatFish();
        bellyBone.localScale *= 1.2f;
        if (fishEaten >= maxFishEaten)
        {
            var particles = Instantiate(deathParticles);
            particles.transform.SetParent(null);
            particles.transform.position = transform.position;
            AudioManager.instance.PlayOneShot(deathSound);
            DialogsController.instance.FoquitaExplodedEvent();
            Destroy(gameObject);
        }
        else
        {
            audioSource.PlayOneShot(eatSound);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(foodToEatPosition.position, distanceToFood / 2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(foodToEatPosition.position, distanceToEat / 2);
    }
}
