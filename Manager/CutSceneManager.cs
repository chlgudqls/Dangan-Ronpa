using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public static bool isFinished = false;

    SplashManager theSplashManager;
    CameraController theCam;

    [SerializeField] Image img_CutScene;
    void Start()
    {
        theSplashManager = FindObjectOfType<SplashManager>();
        theCam = FindObjectOfType<CameraController>();
    }

    public bool CheckCutScene()
    {
        return img_CutScene.gameObject.activeSelf;
    }

    // 컷씬 + Fade 효과 코루틴
    public IEnumerator CutSceneCoroutine(string p_CutSceneName, bool p_isShow)
    {
        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeOut(true, false));
        yield return new WaitUntil(() => SplashManager.isfinished);

        if (p_isShow)
        {
            // 매개변수가 null 이거나, 다른 이름일경우 파일에서 탐색한 값이 null 이 나옴
            Sprite t_Sprite = Resources.Load<Sprite>("CutScenes/" + p_CutSceneName);
            if (t_Sprite != null)
            {
                img_CutScene.gameObject.SetActive(true);
                img_CutScene.sprite = t_Sprite;
                // 컷씬이 나올때 카메라 리셋 그래서
                theCam.CameraTargetting(null, 0.1f, true, false);
            }
            else
                Debug.LogError("잘못된 컷씬 CG 파일 이름입니다");
        }
        else
            img_CutScene.gameObject.SetActive(false);

        SplashManager.isfinished = false;
        StartCoroutine(theSplashManager.FadeIn(true, false));
        yield return new WaitUntil(() => SplashManager.isfinished);

        yield return new WaitForSeconds(0.5f);

        isFinished = true;
    }
}
