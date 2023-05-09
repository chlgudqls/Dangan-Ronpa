using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    // ���� ��ü�� ����� ��ü�� �����ϴ� bool ������
    public bool isDoor;
    public bool isObject;

    // ũ�ν��� ���ؼ� �޾ƿ� �̸�
    [SerializeField] private string interationName;

    public string GetName()
    {
        return interationName;
    }
}
