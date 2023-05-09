using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    // 조사 객체가 어떤건지 객체를 구분하는 bool 변수들
    public bool isDoor;
    public bool isObject;

    // 크로스헤어를 통해서 받아올 이름
    [SerializeField] private string interationName;

    public string GetName()
    {
        return interationName;
    }
}
