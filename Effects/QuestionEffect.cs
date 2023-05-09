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
        // setTarget �� ȣ��Ǽ� �����ȿ� ���� ���Դ�
        if(targetPos != Vector3.zero)
        {
            // target���� ���ͼ� �� ��ġ���� �A���� �������� 0.1 ���Ϸ� ������������
            if ((transform.position - targetPos).sqrMagnitude >= 0.1f)
                transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed);
            else
            {
                // ��ƼŬ ����
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
