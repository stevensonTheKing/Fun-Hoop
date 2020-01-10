using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdsManager : MonoBehaviour
{

    public string gameId = "3394775";
    public string placementId = "BannerAd";
    public bool testMode = true;

    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Show(placementId);

    }
}