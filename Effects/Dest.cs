using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dest : MonoBehaviour
{
    Transform tf_Target;

    // 등장 시킨것을 회전시킬거임
    bool spin = false;
    public static bool isFinished = true;

    private void Start()
    {
        tf_Target = PlayerController.instance.transform;
    }
    void Update()
    {
        if (tf_Target != null)
        {
            if (!spin)
            {
                Quaternion t_Rotation = Quaternion.LookRotation(tf_Target.position);
                Vector3 t_Eular = new Vector3(0, t_Rotation.eulerAngles.y, 0);
                transform.eulerAngles = t_Eular;
            }
            else
            {
                transform.Rotate(0, 90 * Time.deltaTime * 8, 0);
            }
        }
    }
    public IEnumerator SetAppearOrDisappear(bool p_Flag)
    {
        spin = true;

        SpriteRenderer[] t_SpriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        Color t_FrontColor = t_SpriteRenderer[0].color;
        Color t_RearColor = t_SpriteRenderer[1].color;


        // 이 부분은 원래는 1상태이니 true가 들어올땐 0으로 변경하겠단 의미  2가지 경우의 로직
        if (p_Flag)
        {
            t_FrontColor.a = 0; t_RearColor.a = 0;

            t_SpriteRenderer[0].color = t_FrontColor; t_SpriteRenderer[1].color = t_RearColor;
        }

        float t_FadeSpeed = (p_Flag == true) ? 0.01f : -0.01f;

        // 등장에 대기를 줬다
        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            // 일단 무한루프안에서 p_Flag와 알파값을 or 분기
            // ※ 이런식으로도 무한루프안에서 빠져나갈수있게 가능하다 두개조건 비교했음
            if (p_Flag && t_FrontColor.a >= 1) break;
            else if (!p_Flag && t_FrontColor.a <= 0) break;

            t_FrontColor.a += t_FadeSpeed; t_RearColor.a += t_FadeSpeed;
            t_SpriteRenderer[0].color = t_FrontColor; t_SpriteRenderer[1].color = t_RearColor;
            yield return null;
        }

        spin = false;
        isFinished = true;
        gameObject.SetActive(p_Flag);
    }
}