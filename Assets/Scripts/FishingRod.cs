using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public Transform bait;
    public Transform catchAlert;
    public float lerpTime = 1;
    public float catchTime = 1f;
    public AudioClip catchingAudio;
    public AudioClip fishCaughtAudio;
    public AudioClip fishLostAudio;
    public enum FishingState {None, CatchingFish, FishCatched, CatchingMines, MineCatched }
    public FishingState fishingState;
    public AudioClip explosionSound;
    public GameObject explosionParticles;
    Vector3 baitPos;
    [SerializeField] float baitRaycastLength = 0.105f;
    
    public static FishingRod Instance { get; private set; }

    public AudioClip baitOnWaterSound;
    [SerializeField]
    bool baitOnWater = false;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        catchAlert.gameObject.SetActive(false);
        fishingState = FishingState.None;
        FindFirstObjectByType<BaitEventManager>().OnBaitEaten += BaitTriggered;
    }

    private void Update() {
        if (TryGetClosestGround(out Vector3 pos))
            baitPos = pos;
        Vector3 newPosition = Vector3.Lerp(bait.position, baitPos, lerpTime * Time.deltaTime);
        newPosition.y = baitPos.y;
        bait.position = newPosition;
    }

    private void LateUpdate() {
        if (IsBaitOnWater())
        {
            if (!baitOnWater)
            {
                AudioManager.instance.PlayOneShot(baitOnWaterSound);
                baitOnWater = true;
            }
        }
        else
            baitOnWater = false;
    }

    public void StartFishingMines()
    {
        fishingState = FishingState.CatchingMines;
    }

    bool TryGetClosestGround(out Vector3 hit)
    {
        hit = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            hit = raycastHit.point;
            return true;
        }

        return false;
    }

    public bool IsBaitOnWater()
    {
        Ray ray = new(bait.position + new Vector3(0, 0.1f, 0), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, baitRaycastLength, LayerMask.GetMask("Ground")))
        {
            Debug.Log(hit.collider.gameObject.name, hit.collider.gameObject);
            return true;
        }
        return false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(bait.position + new Vector3(0, 0.1f, 0), Vector3.down * baitRaycastLength);
    }

    public void CatchFish(FishMovement fish)
    {
        fishingState = FishingState.CatchingFish;
        catchAlert.gameObject.SetActive(true);
        StartCoroutine(CatchTimer(fish));
    }

    public void EatFish()
    {
        FishDB.Instance.ShowFish(null);
        fishingState = FishingState.None;
    }

    IEnumerator CatchTimer(FishMovement fish)
    {
        AudioManager.instance.PlayClip(catchingAudio, true);
        Animator anim = bait.GetComponent<Animator>();
        anim.SetBool("Bitten", true);
        float timer = 0;
        while(timer < catchTime)
        {
            if (!IsBaitOnWater())
            {
                print("Yeah! I caught a fish");
                fish.DespawnFish();
                catchAlert.gameObject.SetActive(false);
                FindFirstObjectByType<FishDB>().GetRandomFish();
                AudioManager.instance.PlayClip(fishCaughtAudio, false);
                anim.SetBool("Bitten", false);
                var fishData = FishDB.Instance.GetRandomFish();
                FishDB.Instance.ShowFish(fishData);
                GameUI.Instance.ShowFish(fishData);
                fishingState = FishingState.FishCatched;
                yield break;
            }
            yield return null;
            timer += Time.deltaTime;
        }
        AudioManager.instance.PlayClip(fishLostAudio, false);
        anim.SetBool("Bitten", false);
        fish.DespawnFish();
        catchAlert.gameObject.SetActive(false);
        GameUI.Instance.ShowInfo("Jaja se te ha escapado, vaya inútil! XD");
        fishingState = FishingState.None;
    }

    GameObject _currentMine = null;

    public void BaitTriggered(Collider obj)
    {
        if (fishingState == FishingState.CatchingMines)
        {
            if (_currentMine == null && obj.CompareTag("Mine"))
            {
                fishingState = FishingState.MineCatched;
                _currentMine = obj.gameObject;
                _currentMine.transform.SetParent(bait);
                _currentMine.transform.localPosition = Vector3.zero;
            }
        }
        else if (fishingState == FishingState.MineCatched)
        {
            if (obj.TryGetComponent(out ShipController ship))
            {
                ship.Damage();
                // Spawn particles
                AudioManager.instance.PlayOneShot(explosionSound);
                Instantiate(explosionParticles, _currentMine.transform.position, Quaternion.identity);
                Destroy(_currentMine);
                _currentMine = null;
                fishingState = FishingState.CatchingMines;
            }
        }
    }
}
