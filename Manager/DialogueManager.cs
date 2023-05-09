using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static bool isWaiting = false;

    [SerializeField] private GameObject go_DialogueBar;
    [SerializeField] private GameObject go_DialogueNameBar;

    [SerializeField] private Text txt_Dialogue;
    [SerializeField] private Text txt_Name;

    Dialogue[] dialogues;

    bool isDialogue = false; // 대화중일 경우 true
    bool isNext = false; // 특정 키 입력 대기

    [Header("텍스트 출력 딜레이")]
    [SerializeField] private float textDelay;


    int lineCount = 0; // 대화 카운트
    int contextCount = 0; // 대사 카운트

    // 다음 이벤트를 위한 세팅
    GameObject go_NextEvent;

    public void SetNextEvent(GameObject p_NextEvent)
    {
        go_NextEvent = p_NextEvent;
    }
    // 이벤트 끝나면 등장 or 퇴장시킬 오브젝트들
    GameObject[] go_Objects;
    byte appearTypeNumber;
    const byte NONE = 0, APPEAR = 1, DISAPPEAR = 2;

    public void SetAppearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = APPEAR;
    }
    public void SetDisappearObjects(GameObject[] p_Targets)
    {
        go_Objects = p_Targets;
        appearTypeNumber = DISAPPEAR;
    }

    private InteractionController theIC;
    private CameraController theCam;
    private SpriteManager theSpriteManager;
    private SplashManager theSplashManager;
    private CutSceneManager theCutSceneManager;
    private SlideManager theSlideManager;
    void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        theCam = FindObjectOfType<CameraController>();
        theSpriteManager = FindObjectOfType<SpriteManager>();
        theSplashManager = FindObjectOfType<SplashManager>();
        theCutSceneManager = FindObjectOfType<CutSceneManager>();
        theSlideManager = FindObjectOfType<SlideManager>();
    }
    private void Update()
    {
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isNext = false;
                    txt_Dialogue.text = "";
                    if(++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        contextCount = 0;
                        // lineCount 증가 == 화자가 바뀐다 
                        if (++lineCount < dialogues.Length)
                        {
                            StartCoroutine(CameraTargettingType());
                        }
                        else
                        {
                            StartCoroutine(EndDialogue());
                        }
                    }
                }
            }
        }
    }
    public void ShowDialogue(Dialogue[] p_dialogues) 
    {
        isDialogue = true;
        // 창을 띄우는데 잔여물을 초기화
        txt_Dialogue.text = "";
        txt_Name.text = "";
        theIC.SettingUI(false);
        dialogues = p_dialogues;

        StartCoroutine(StartDialogue());
    }

    private IEnumerator StartDialogue()
    {
        if (isWaiting)
            yield return new WaitForSeconds(0.5f);

        isWaiting = false;
        theCam.CamOriginSetting();
        StartCoroutine(CameraTargettingType());

    }

    // 대기시간이 필요해서 코루틴으로 변경   코루틴으로 바꾸면 해야될것들
    private IEnumerator CameraTargettingType()
    {
        // case로 간편하게 타입별로 실행할것을 분기 그것을 함수로 호출
        switch (dialogues[lineCount].cameraType)
        {
            // 코루틴이 실행되면 FadeIn이 끝날때 true가되니까 그때까지만 기다리는건데 그럼 무슨 효과
            case CameraType.FadeIn: SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeIn(false, true)); yield return new WaitUntil(() => SplashManager.isfinished); break;
            case CameraType.FadeOut: SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeOut(false, true)); yield return new WaitUntil(() => SplashManager.isfinished); break;
            case CameraType.FlashIn: SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeIn(true, true)); yield return new WaitUntil(() => SplashManager.isfinished); break;
            case CameraType.FlashOut: SettingUI(false); SplashManager.isfinished = false; StartCoroutine(theSplashManager.FadeOut(true, true)); yield return new WaitUntil(() => SplashManager.isfinished); break;
            case CameraType.ObjectFront: theCam.CameraTargetting(dialogues[lineCount].tf_Target); break;
            // 리셋 이니까 true , 대화는 끝나지 않았으니 false  끝났을때 카메라에서 따로 어떤 처리를 하려고 if문도 만듬
            case CameraType.Reset: theCam.CameraTargetting(null, 0.05f, true, false); break;
            case CameraType.ShowCutScene: SettingUI(false); CutSceneManager.isFinished = false; StartCoroutine(theCutSceneManager.CutSceneCoroutine(dialogues[lineCount].spriteName[contextCount], true)); yield return new WaitUntil(() => CutSceneManager.isFinished); break;
                //왜 타겟을 하는지 모르겠네 타겟이 풀리나 다른 특정인이 있을경우가 있어서
            case CameraType.HideCutScene: SettingUI(false); CutSceneManager.isFinished = false; StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false)); yield return new WaitUntil(() => CutSceneManager.isFinished); theCam.CameraTargetting(dialogues[lineCount].tf_Target); break;
            case CameraType.AppearSlideCG: SlideManager.isFinished = false; StartCoroutine(theSlideManager.AppearSlide(SplitSlideCGName())); yield return new WaitUntil(() => SlideManager.isFinished); theCam.CameraTargetting(dialogues[lineCount].tf_Target); break;
            case CameraType.DisappearSlideCG: SlideManager.isFinished = false; StartCoroutine(theSlideManager.DisappearSlide()); yield return new WaitUntil(() => SlideManager.isFinished); theCam.CameraTargetting(dialogues[lineCount].tf_Target); break;
            case CameraType.ChangeSlideCG: SlideManager.isChanged = false; StartCoroutine(theSlideManager.ChangeSlide(SplitSlideCGName())); yield return new WaitUntil(() => SlideManager.isChanged); theCam.CameraTargetting(dialogues[lineCount].tf_Target); break;
        }
        // ※ 여기서 실행이 되는구나 그래서 
        StartCoroutine(TypeWriter());
    }

    // Sprite 컬럼의 열에 SlideCG만 있을경우가 생겼음
    private string SplitSlideCGName()
    {
        string t_Text = dialogues[lineCount].spriteName[contextCount];
        string[] t_Array = t_Text.Split(new char[] {'/'});
        if (t_Array.Length <= 1)
            return t_Array[0];
        else
            return t_Array[1];
    }
    // 컷씬이 대화가 완전히 끝이날때 사라지게 했다
    private IEnumerator EndDialogue()
    {
        SettingUI(false);
        if (theCutSceneManager.CheckCutScene())
        {
            CutSceneManager.isFinished = false; 
            StartCoroutine(theCutSceneManager.CutSceneCoroutine(null, false)); 
            yield return new WaitUntil(() => CutSceneManager.isFinished);
        }

        AppearOrDisappearObjects();

        yield return new WaitUntil(() => Dest.isFinished);

        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        theCam.CameraTargetting(null, 0.05f, true, true);

        // 이 함수의 이 위치에서 완전히 끝날때 까지 기다린다
        yield return new WaitUntil(() => !InteractionController.isInteract);

        if (go_NextEvent != null)
        {
            go_NextEvent.SetActive(true);
            go_NextEvent = null;
        }
        else
            theIC.SettingUI(true);
    }

    private void AppearOrDisappearObjects()
    {
        if(go_Objects != null)
        {
            Dest.isFinished = false;
            for (int i = 0; i < go_Objects.Length; i++)
            {
                if(appearTypeNumber == APPEAR)
                {
                    go_Objects[i].SetActive(true);
                    StartCoroutine(go_Objects[i].GetComponent<Dest>().SetAppearOrDisappear(true));
                }
                else if (appearTypeNumber == DISAPPEAR)
                    StartCoroutine(go_Objects[i].GetComponent<Dest>().SetAppearOrDisappear(false));
            }
            go_Objects = null;
            appearTypeNumber = NONE;
        }
    }

    // 함수를 만들고 SpriteManager의 코루틴을 호출
    private void ChangeSprite()
    {
        // 있을때만
        // 빈값이 아닐때 다 넣고 있음 sprite 컬럼을 쓰고있기 때문에 컷씬이름이 들어갈수있음
        // 다시 컷씬로직 수정후 타겟이 null이 아닐때만 대화상태가 이미지가 변경되도록 수정하였음
        // 카메라타입이 대화상대를 지정하고있기때문에 타입에 따라서 알아서 들어가니까 처음엔 그걸로 구분했지만 target이 없는걸로 구분했음
        // 같은 컬럼에 CG와 Slide name을 추가했기때문에 
        if(dialogues[lineCount].tf_Target != null)
        {
            if (dialogues[lineCount].spriteName[contextCount] != "")
                // 파싱을 이렇게 써먹는다
                StartCoroutine(theSpriteManager.SpriteChangeCoroutine(
                                                                    dialogues[lineCount].tf_Target, 
                                                                    dialogues[lineCount].spriteName[contextCount].Split(new char[] {'/'})[0]));
        }
    }
    // 여기서 "" 체크를 해야되서 함수를 만듬
    private void PlaySound()
    {
        if (dialogues[lineCount].voiceName[contextCount] != "")
            SoundManager.instance.PlaySound(dialogues[lineCount].voiceName[contextCount], 2);
    }
    // ※  flag변수로 문자열의 구간을 체크한 새로운 문법 
    IEnumerator TypeWriter()
    {
        SettingUI(true);
        // 이 시점 - 텍스트가 나오면서 스프라이트도 같이 변경
        ChangeSprite();
        PlaySound();

        // 대사한줄을 string 지역변수에 담아서 하나씩 출력함
        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        t_ReplaceText = t_ReplaceText.Replace("`", ",");
        // \n을 텍스트로 인식하기위해서 \를 한번 더 붙여줬다
        t_ReplaceText = t_ReplaceText.Replace("\\n", "\n");


        bool t_white = false, t_yellow = false, t_cyan = false;
        bool t_ignore = false; 

        // for문으로 문자열의 길이를 나눠서 인덱스로 구분할수가있는듯
        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            switch(t_ReplaceText[i])
            {
                case 'ⓦ': t_white = true; t_yellow = false; t_cyan = false; t_ignore = true; break;
                case 'ⓨ': t_white = false; t_yellow = true; t_cyan = false; t_ignore = true; break;
                case 'ⓒ': t_white = false; t_yellow = false; t_cyan = true; t_ignore = true; break;
                case '①': StartCoroutine(theSplashManager.Splash()); t_ignore = true; SoundManager.instance.PlaySound("Emotion1", 1); break;
                case '②': StartCoroutine(theSplashManager.Splash()); t_ignore = true; SoundManager.instance.PlaySound("Emotion2", 1); break;

            }
            string t_letter = t_ReplaceText[i].ToString();

            if (!t_ignore)
            {
                if(t_white) { t_letter = "<color=#FFFFFF>" + t_letter + "</color>"; }
                else if(t_yellow) { t_letter = "<color=#FFFC00>" + t_letter + "</color>";  }
                else if (t_cyan) { t_letter = "<color=#00F6FF>" + t_letter + "</color>"; }

                txt_Dialogue.text += t_letter;
            }
            // ignore가 true인게 있었다면 false로 다시 되돌려야 다음문자가 씹히지않음
            t_ignore = false;


            //txt_Dialogue.text += t_ReplaceText[i];
            yield return new WaitForSeconds(textDelay);
        }
        isNext = true;
    }
    private void SettingUI(bool p_flag)
    { 
        go_DialogueBar.SetActive(p_flag);

        if (p_flag)
        {
            if(dialogues[lineCount].name == "")
            {
                go_DialogueNameBar.SetActive(false);
            }
            else
            {
                go_DialogueNameBar.SetActive(true);
                txt_Name.text = dialogues[lineCount].name;
            }
        }
        else
        {
            go_DialogueNameBar.SetActive(false);
        }
    }
}
