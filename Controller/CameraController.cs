using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 이게 트루일때 고정위치값에 위치시키고 아닐시 대화하기전 위치값을 그대로 적용
    public static bool onlyView = true;

    // 리셋이되면 원래 각도로 세팅
    private Vector3 originPos;
    private Quaternion originRot;

    private InteractionController theIC;
    private PlayerController thePlayer;

    // 여기에 코루틴을 넣고 널인지 아닌지 판별해서 무언가를 할수가있음
    Coroutine coroutine;

    private void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        thePlayer = FindObjectOfType<PlayerController>();
    }

    // 유니티상의 카메라의 처음 위치값을 보고 설정하면됨
    public void CamOriginSetting()
    {
        originPos = transform.position;
        // 백터를 쿼터니언으로
        // ※ 다행 카메라가 돌아올때 180도 회전했음 방향의 문제 zero -> back으로 변경 바라보는 방향을 고쳤음 
        //originRot = Quaternion.Euler(Vector3.back);
        // ※ 이것도 된다고함
        originRot = transform.rotation;
    }
    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.05f, bool p_isReset = false, bool p_isFinish = false)
    {
        //StopAllCoroutines(); 1. 리셋 코루틴이 연속할때 끊기된다고함 난 문제없는데
        if (!p_isReset)
        {
            if(p_Target != null)
            {
                StopAllCoroutines(); // 2. 위치옮기고 코루틴 변수를만듬
                coroutine = StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
            }
        }
        else
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            StartCoroutine(CameraResetCoroutine(p_CamSpeed, p_isFinish));
        }
    }
    // ※ 타겟의 위치만 알아도 타겟의 방향을 구할수가 있다.
    IEnumerator CameraTargettingCoroutine(Transform p_Target, float p_CamSpeed = 0.1f)
    {
        Vector3 t_TargetPos = p_Target.position;
        // 거리조절
        Vector3 t_TargetFrontPos = t_TargetPos + (p_Target.forward * 1.3f);
        Vector3 t_Direction = (t_TargetPos - t_TargetFrontPos).normalized;

        while(transform.position != t_TargetFrontPos || Quaternion.Angle(transform.rotation, Quaternion.LookRotation(t_Direction)) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, t_TargetFrontPos, p_CamSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_Direction), p_CamSpeed);
            yield return null;
            //Debug.Log("찍히세요");
            // 여긴 정상적으로 벗어난다
        }
    }
    IEnumerator CameraResetCoroutine(float p_CamSpeed = 0.1f, bool p_isFinish = false)
    {
        yield return new WaitForSeconds(0.5f);

        while (transform.position != originPos || Quaternion.Angle(transform.rotation, originRot) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, p_CamSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, originRot, p_CamSpeed);
            // Quaternion.LookRotation(Vector3.back), p_CamSpeed); 이게 코루틴을 못빠져나오게 했네 왜인지는 귀찮다
            yield return null;
        }
        // ※ 디버그를 찍어보니 코루틴을 못벗어나고있다 원인 발견
        //Debug.Log("찍히는지");
        transform.position = originPos;
        //transform.rotation = originRot;

        if (p_isFinish)
        {
            // 대화와 동시에 유아이가 나오는 현상때문
            thePlayer.Reset();
            //  다음 이벤트가 있을경우 상호작용 UI가 잠깐 나타나는 문제 해결을 위해서 SettingUI는 다음이벤트가 없을경우로 옮겨줬음
            InteractionController.isInteract = false;
            //theIC.SettingUI(true);  // 모든 대화가 끝났으면 리셋
            //이게 실행되려면 flag값과 코루틴이 완전히 끝날때임 이때 UI 상호작용 유아이를 띄우고싶어서
            // 그리고 상호작용이 false됨과동시에 Resets 의 변수값이 바로들어와서 리셋을 해줘야된다
        }
    }
}
