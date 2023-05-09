using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDoor : MonoBehaviour
{
    // 문에 따라서 온리뷰인지 아닌지 여기서 정한다
    [SerializeField] bool isOnlyView;
    [SerializeField] string sceneName;
    [SerializeField] string locationName;

    // 문에 붙어있는 스크립트 여기서 정해짐 부딪히면 얻어올수있음
    public string GetSceneName()
    {
        CameraController.onlyView = isOnlyView;
        return sceneName;
    }
    public string GetLocationName()
    {
        return locationName;
    }
}
