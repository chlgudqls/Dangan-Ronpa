using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    [SerializeField] private float disappearTime;
    // Ȱ��ȭ�� �Ŀ� �����ð��� ������ ���������
    private void OnEnable()
    {
        StartCoroutine(DisappearCoroutine());
    }
    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(disappearTime);

        gameObject.SetActive(false);
    }
}
