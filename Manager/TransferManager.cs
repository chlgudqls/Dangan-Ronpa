using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : MonoBehaviour
{
    string locationName;

    SplashManager theSplashManager;
    InteractionController theIC;

    public static bool isFinished = true;

    private void Start()
    {
        theSplashManager = FindObjectOfType<SplashManager>();
        theIC = FindObjectOfType<InteractionController>();
    }

    // 이게 호출된다 = 상호작용된 door의 정보가 들어온다
    public IEnumerator Transfer(string p_SceneName, string p_LocationName)
    {
        isFinished = false;

        theIC.SettingUI(false);
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeOut(false, true));
        yield return new WaitUntil(() => SplashManager.isfinished);

        locationName = p_LocationName;
        // 여기로 왔다는건 씬전환이 이루어짐 즉 스폰타이밍
        TransferSpawnManager.spawnTiming = true;

        SceneManager.LoadScene(p_SceneName);

    }

    // 씬전환 + 스폰이 완전히 호출이된 후에 실행되야됨
    public IEnumerator Done()
    {
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeIn(false, true));
        yield return new WaitUntil(() => SplashManager.isfinished);
        isFinished = true;

        yield return new WaitForSeconds(0.3f);

        if(!DialogueManager.isWaiting)
            theIC.SettingUI(true);
    }

    public string GetLocationName()
    {
        return locationName;
    }
}
