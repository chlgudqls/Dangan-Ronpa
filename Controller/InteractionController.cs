using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera cam;

    RaycastHit hitInfo;

    [SerializeField] GameObject go_NomalCrosshair;
    [SerializeField] GameObject go_InterativeCrosshair;
    [SerializeField] GameObject go_Crosshair;
    [SerializeField] GameObject go_Cursor;
    [SerializeField] GameObject go_FieldCursor;

    [SerializeField] GameObject go_TargetNameBar;
    [SerializeField] Text txt_TargetName;

    [SerializeField] Image img_Interation;
    [SerializeField] Image img_InterationEffect;

    bool isContact = false;
    public static bool isInteract = false;

    [SerializeField] public ParticleSystem ps_QuestionEffect;

    private DialogueManager theDM;

    public void SettingUI(bool p_flag)
    {
        go_Crosshair.SetActive(p_flag);
        

        if (!p_flag)
        {
            StopCoroutine("Interaction");
            Color color = img_Interation.color;
            color.a = 0f;
            img_Interation.color = color;
            go_TargetNameBar.SetActive(false);
            go_Cursor.SetActive(false);
            go_FieldCursor.SetActive(false);
        }
        else
        {
            if (CameraController.onlyView)
                go_Cursor.SetActive(true);
            else
                go_FieldCursor.SetActive(true);

            go_NomalCrosshair.SetActive(true);
            go_InterativeCrosshair.SetActive(false);
        }
        isInteract = !p_flag;
    }
    private void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
    }
    void Update()
    {
        if (!isInteract)
        {
            CheckObject();
            ClickLeftBtn();
        }
    }

    // 분기기준 카메라에서 마우스포인터까지의 방향 or 카메라의 정면
    private void CheckObject()
    {
        // 마우스 커서기준은 only view 에서만
        if (CameraController.onlyView)
        {
            Vector3 t_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            //카메라가 주체가되서 스크린의점을 넘겨서 계산을 시키면 origin과 target의 위치를 구함
            // 그냥 ray를 넣어도 되네
            // 충돌한 모든 것들에 대한 이벤트
            if(Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo, 100))
                Contect();
            // 충돌하지 않은 것들에 대한 이벤트
            else
                NotContect();
        }
        else
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 2))
                Contect();
            // 충돌하지 않은 것들에 대한 이벤트
            else
                NotContect();
        }
    }
    // 충돌여부에 따라 이벤트를 실행하는 함수
    private void Contect()
    {
        // 뭐가 충돌했는지에 따라서 적용할것들
        if(hitInfo.transform.CompareTag("Interaction"))
        {
            go_TargetNameBar.SetActive(true);
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName();
            if (!isContact)
            {
                isContact = true;
                go_InterativeCrosshair.SetActive(true);
                go_NomalCrosshair.SetActive(false);
                // 한가지 경우에만 상호작용 Effect 띄움 
                if (!CameraController.onlyView)
                {
                    StopCoroutine("Interaction");
                    StopCoroutine("InteractionEffect");
                    StartCoroutine(Interaction(true));
                    StartCoroutine("InteractionEffect");
                }
            }
        }
        // tag가 Interaction가 아닌것들
        else
        {
            NotContect();
        }

    }
    // 충돌하지않았을때 호출할 함수 
    private void NotContect()
    {
        if (isContact)
        {
            go_TargetNameBar.SetActive(false);
            isContact = false;
            go_InterativeCrosshair.SetActive(false);
            go_NomalCrosshair.SetActive(true);
            if (!CameraController.onlyView)
            {
                StopCoroutine("Interaction");
                StartCoroutine(Interaction(false));
            }
        }
    }
    // 상호작용의 상태에 따라서 알파값을 변화시킴
    IEnumerator Interaction(bool p_Appear)
    {
        Color color = img_Interation.color;
        if (p_Appear)
        {
            color.a = 0;
            while (color.a < 1)
            {
                color.a += 0.1f;
                img_Interation.color = color;
                yield return null;
            }
        }
        else
        {
            while (color.a > 0)
            {
                color.a -= 0.1f;
                img_Interation.color = color;
                yield return null;
            }
        }
    }
    IEnumerator InteractionEffect()
    {
        while (isContact && !isInteract)
        {
            Color color = img_InterationEffect.color;
            color.a = 0.5f;

            img_InterationEffect.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            Vector3 t_scale = img_InterationEffect.transform.localScale;
            while (color.a > 0)
            {
                color.a -= 0.01f;
                img_InterationEffect.color = color;
                t_scale.Set(t_scale.x + Time.deltaTime, t_scale.y + Time.deltaTime, t_scale.z + Time.deltaTime);
                img_InterationEffect.transform.localScale = t_scale;
                yield return null;
            }
            yield return null;
        }
    }
    // 이름그대로 죄클릭했을때 실행하는 함수
    private void ClickLeftBtn()
    {
        if (!isInteract)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isContact)
                    // 충돌했을때 
                    Interact();
            }
        }
    } 
    private void Interact()
    {
        // 이펙트 발사를 한번만 하려고 이 변수를 만듬
        isInteract = true;

        StopCoroutine("Interaction");
        Color color = img_Interation.color;
        color.a = 0f;
        img_Interation.color = color;

        ps_QuestionEffect.gameObject.SetActive(true);
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos); 
        ps_QuestionEffect.transform.position = cam.transform.position;
        StartCoroutine(WaitCollision());
    }
    IEnumerator WaitCollision()
    {
        yield return new WaitUntil(() => QuestionEffect.isCollide);
        QuestionEffect.isCollide = false;

        InteractionEvent t_Event = hitInfo.transform.GetComponent<InteractionEvent>();

        if(hitInfo.transform.GetComponent<InteractionType>().isObject)
        {
            DialogueCall(t_Event);
        }
        else
        {
            // ※ isObject 체크가 아니지만 그중에서도 t_Event 가 붙어있다 그럼 대화를 출력
            if(t_Event != null && t_Event.GetDialogue() != null)
                DialogueCall(t_Event);
            else
                TransferCall();
        }
    }

    private void TransferCall()
    {
        string t_SceneName = hitInfo.transform.GetComponent<InteractionDoor>().GetSceneName();
        string t_LocationName = hitInfo.transform.GetComponent<InteractionDoor>().GetLocationName();
        //Debug.Log(t_SceneName);
        StartCoroutine(FindObjectOfType<TransferManager>().Transfer(t_SceneName, t_LocationName));
    }

    // 함수명짓는법
    private void DialogueCall(InteractionEvent p_Event)
    {
        // ※ 이런것도있네 int형인데 bool값도 있음 너무 헷갈림
        // 조건이 만족하면 if안의 것이 실행됨 check함수에서 true가 반환되야됨
        // 이벤트가 처음이면서 조건이 true가 될때만 if문에 정상적으로 실행된다는건데
        // false면 거른다는거고 대충 결과값이 false면서 함수안에서 true가 나와서
        // ※※※※※ condition 까지 체크해서 true여야만 if안이 정상적으로 된다 eventflag는 당연히 false여야 하며
        if (!DatabaseManager.instance.eventFlags[p_Event.GetEventNum()])
        {
            theDM.SetNextEvent(p_Event.GetNextEvent());
            // enum은 AppearType.Appear 이렇게 기본값을 가져와서 비교가능
            if (p_Event.GetAppearType() == AppearType.Appear) theDM.SetAppearObjects(p_Event.GetTargets());
            else if (p_Event.GetAppearType() == AppearType.Disappear) theDM.SetDisappearObjects(p_Event.GetTargets());
        }

        theDM.ShowDialogue(p_Event.GetDialogue());
    }
}
