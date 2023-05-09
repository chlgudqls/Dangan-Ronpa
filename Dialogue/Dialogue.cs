using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 선택한 타입에 따라서 결과를 실행시킨다 
public enum CameraType
{
    ObjectFront,
    Reset,
    FadeOut,
    FadeIn,
    FlashOut,
    FlashIn,
    ShowCutScene,
    HideCutScene,
    AppearSlideCG,
    DisappearSlideCG,
    ChangeSlideCG,
}

public enum AppearType
{
    None,
    Appear,
    Disappear,
}

// 여러명이 말하기때문에 클래스를 배열로 만듬
[System.Serializable]
public class Dialogue
{
    // 파싱 필요없이 인스펙터창에서 직접수정하는것들
    [Header("카메라가 타겟팅할 대상")]
    public CameraType cameraType;
    public Transform tf_Target;

    // 파싱
    //[Tooltip("대사 치는 캐릭터 이름")]
    [HideInInspector]
    public string name;
    //[Tooltip("대사 내용")]
    [HideInInspector]
    public string[] contexts;
    [HideInInspector]
    public string[] spriteName;
    [HideInInspector]
    public string[] voiceName;
}

[System.Serializable]
public class EventTiming
{
    public int eventNum;
    public int[] eventConditions;
    public bool conditionFlag;
    public int eventEndNum;
}

[System.Serializable]
public class DialogueEvent
{
    public string name;
    public EventTiming eventTiming;

    public Vector2 line;
    public Dialogue[] dialogues;

    // ※ 상호작용하기전과 후의 대사를 다르게 할거임
    [Space]
    public Vector2 lineB;
    public Dialogue[] dialoguesB;

    [Space]
    public AppearType appearType;
    public GameObject[] go_Targets;
    [Space]
    // 연속된 이벤트를 만들기 위해서 새로운 변수를 추가
    // ※ 널의 유무에 따라서 다음 이벤트를 실행함 대화시스템을 갱신해서 유지 or 종료 판단
    public GameObject go_NextEvent;

    // ※ isSame 대화를 전,후로 나눴는데 후에서 true로 체크하고 전대화로직에 if true추가하면 전 대화만 실행됨 같은 대화내용이라는뜻
    public bool isSame;
}
