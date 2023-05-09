using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float fadeSpeed;

    // 같으면 굳이 바꾸지않으려고 체크
    private bool CheckSameSprite(SpriteRenderer p_SpriteRenderer, Sprite p_Sprite)
    {
        if (p_SpriteRenderer.sprite == p_Sprite)
            return true;
        else
            return false;
    }
    public IEnumerator SpriteChangeCoroutine(Transform p_Target, string p_SpriteName)
    {
        // 기존 한개에서 시퀀스의 요소가 두개가됨 front, back 생김
        SpriteRenderer[] t_SpriteRenderer = p_Target.GetComponentsInChildren<SpriteRenderer>();
        // ※ 이게 되네
        // typeof(Sprite) 가능한지 여부체크 에러 날수도있어서
        // as Sprite 강제 변환
        Sprite t_Sprite = Resources.Load("Characters/" + p_SpriteName, typeof(Sprite)) as Sprite;

        // 변경 알파값을 없애지않고 대입하면 겹쳐보이는 현상이 있는것같음 그건 아니였음
        if(!CheckSameSprite(t_SpriteRenderer[0], t_Sprite))
        {
            Color t_color = t_SpriteRenderer[0].color;
            Color t_ShadowColor = t_SpriteRenderer[1].color;
            t_color.a = 0;
            t_ShadowColor.a = 0;
            t_SpriteRenderer[0].color = t_color;
            t_SpriteRenderer[1].color = t_ShadowColor;

            t_SpriteRenderer[0].sprite = t_Sprite;
            t_SpriteRenderer[1].sprite = t_Sprite;

            // 변경한 스프라이트가 서서히 보인다
            while (t_color.a < 1)
            {
                t_color.a += fadeSpeed;
                t_ShadowColor.a += fadeSpeed;
                t_SpriteRenderer[0].color = t_color;
                t_SpriteRenderer[1].color = t_ShadowColor;
                yield return null;
            }
        }
    }

}
