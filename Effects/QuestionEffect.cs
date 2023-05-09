using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionEffect : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Vector3 targetPos = new Vector3();

    [SerializeField] ParticleSystem ps_Effect;

    public static bool isCollide = false;

    public void SetTarget(Vector3 _target)
    {
        targetPos = _target;
    }
    void Update()
    {
        // setTarget 이 호출되서 변수안에 값이 들어왔다
        if(targetPos != Vector3.zero)
        {
            // target값이 들어와서 두 위치값을 뺸값의 제곱근이 0.1 이하로 좁혀질때까지
            if ((transform.position - targetPos).sqrMagnitude >= 0.1f)
                transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed);
            else
            {
                // 파티클 실행
                ps_Effect.gameObject.SetActive(true);
                ps_Effect.transform.position = transform.position;
                isCollide = true;
                ps_Effect.Play();
                targetPos = Vector3.zero;
                gameObject.SetActive(false);
            }
        }
    }
}
