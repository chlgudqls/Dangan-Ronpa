using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    [SerializeField] private float disappearTime;
    // 활성화된 후에 일정시간이 지나면 사라지게함
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
