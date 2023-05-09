using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float applySpeed;
    [SerializeField] float fieldSensitivity;
    [SerializeField] float fieldLookLimitX; // 캠 위아래 축 X 

    [SerializeField] Transform tf_Crosshair;

    [SerializeField] Transform tf_Cam;
    [SerializeField] Vector2 camBoundary; // 캠의 가두기 영역
    [SerializeField] float sightMoveSpeed; // 좌우 움직임 스피드
    [SerializeField] float sightSensivitity; // 고개의 움직임 속도
    [SerializeField] float lookLimitX;
    [SerializeField] float lookLimitY;


    float currentAngleX;
    float currentAngleY;

    [SerializeField] GameObject go_NotCamDown;
    [SerializeField] GameObject go_NotCamUp;
    [SerializeField] GameObject go_NotCamLeft;
    [SerializeField] GameObject go_NotCamRight;

    // 처음에 1을 넣었지만 cam위치가 변경되면 유동적으로 바뀔수있게 이런식으로 사용 
    float originPosY;

    public void Reset()
    {
        tf_Crosshair.localPosition = Vector3.zero;
        currentAngleX = 0;
        currentAngleY = 0;
    }

    void Start()
    {
        originPosY = tf_Cam.localPosition.y;
    }
    void Update()
    {
        if (!InteractionController.isInteract)
        {
            if (CameraController.onlyView)
            {
                CrosshairMoving();
                ViewMoving();
                KeyViewMoving();
                CameraLimit();
                NotCamUI();
            }
            else
            {
                FieldMoving();
                FieldLooking();
            }
        }
    }

    // 움직임을 방향키로
    private void FieldMoving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            // 방향
            Vector3 t_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            if (Input.GetKey(KeyCode.LeftShift))
                applySpeed = runSpeed;
            else
                applySpeed = walkSpeed;

            // 월드로하면 부모 기준이라 한방향으로만 나아간다? 그렇다는데
            // 로컬로해서 플레이어 기점으로 움직일수있게한다 Space.Self
            transform.Translate(t_Dir * applySpeed * Time.deltaTime, Space.Self);
        }
    }

    // 보는것을 마우스 값으로 
    private void FieldLooking()
    {
        // 마우스의 x 스크린 축기준으로 값을 얻어와서 플레이어의 y축을 회전시키기때문에
        if (Input.GetAxisRaw("Mouse X") != 0)
        {
            float t_AngleY = Input.GetAxisRaw("Mouse X");
            // ※ 임시변수에 값을 설정해두고 플레이어 대입하는 기본적인 패턴
            Vector3 t_Rot = new Vector3(0, t_AngleY * fieldSensitivity, 0);
            // x축은 플레이어를 회전시킴
            transform.rotation = Quaternion.Euler(transform.localEulerAngles + t_Rot);

        }
        // 마우스 기준 상하는 y값임 카메라가 회전할축은 x축기준 x축기준으로 위아래를 바라볼수있음
        if(Input.GetAxisRaw("Mouse Y") != 0)
        {
            // y축은 카메라의 앵글값을 변경시킴
            float t_AngleX = Input.GetAxisRaw("Mouse Y");
            // 상하는 반전이 있어서 빼줌 
            currentAngleX -= t_AngleX;
            currentAngleX = Mathf.Clamp(currentAngleX, -fieldLookLimitX, fieldLookLimitX);
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, 0, 0);
        }
    }
    private void NotCamUI()
    {
        go_NotCamDown.SetActive(false);
        go_NotCamUp.SetActive(false);
        go_NotCamLeft.SetActive(false);
        go_NotCamRight.SetActive(false);

        if (currentAngleY >= lookLimitX)
            go_NotCamRight.SetActive(true);
        else if (currentAngleY <= -lookLimitX)
            go_NotCamLeft.SetActive(true);

        if (currentAngleX <= -lookLimitY)
            go_NotCamUp.SetActive(true);
        else if (currentAngleX >= lookLimitY)
            go_NotCamDown.SetActive(true);

    }
    private void CameraLimit()
    {
        if (tf_Cam.localPosition.x >= camBoundary.x)
                tf_Cam.localPosition = new Vector3(camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        else if (tf_Cam.localPosition.x <= -camBoundary.x)
                tf_Cam.localPosition = new Vector3(-camBoundary.x, tf_Cam.localPosition.y, tf_Cam.localPosition.z);

        if(tf_Cam.localPosition.y >= originPosY + camBoundary.y)
                tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, originPosY + camBoundary.y, tf_Cam.localPosition.z);
        else if (tf_Cam.localPosition.y <= 1 -camBoundary.y)
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, originPosY + -camBoundary.y, tf_Cam.localPosition.z);
    }
    private void KeyViewMoving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            currentAngleY += sightSensivitity * Input.GetAxisRaw("Horizontal");
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + sightMoveSpeed * Input.GetAxisRaw("Horizontal"), tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        }
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            currentAngleX -= sightSensivitity * Input.GetAxisRaw("Vertical");
            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + sightMoveSpeed * Input.GetAxisRaw("Vertical"), tf_Cam.localPosition.z);
        }
        // 여기서 계속 앵글값이 치환되고있다 그래서 상호작용이 끝나면 그 값이 바로 적용되버린다 리셋을 해야된다고함
        tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
    }
    private void ViewMoving()
    {
        // 스크린사이즈보다 넘어가는것을 기준으로 고개를 돌릴수있도록함 
        // crosshair는 x지만 rotation은 y임
        if(tf_Crosshair.localPosition.x > (Screen.width / 2 - 100) || tf_Crosshair.localPosition.x < (-Screen.width / 2 + 100))
        {
            currentAngleY += (tf_Crosshair.localPosition.x > 0) ? sightSensivitity : -sightSensivitity;
            currentAngleY = Mathf.Clamp(currentAngleY, -lookLimitX, lookLimitX);

            float t_applySpeed = (tf_Crosshair.localPosition.x > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + t_applySpeed, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
        }
        if (tf_Crosshair.localPosition.y > (Screen.height / 2 - 100) || tf_Crosshair.localPosition.y < (-Screen.height / 2 + 100))
        {
            currentAngleX += (tf_Crosshair.localPosition.y > 0) ? -sightSensivitity : sightSensivitity;
            currentAngleX = Mathf.Clamp(currentAngleX, -lookLimitY, lookLimitY);

            float t_applySpeed = (tf_Crosshair.localPosition.y > 0) ? sightMoveSpeed : -sightMoveSpeed;
            tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + t_applySpeed, tf_Cam.localPosition.z);
        }
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);

    }
    private void CrosshairMoving()
    {
        tf_Crosshair.localPosition = new Vector2(Input.mousePosition.x - (Screen.width / 2), 
                                                Input.mousePosition.y - (Screen.height / 2));//Input.mousePosition;

        float t_cursorPosX = tf_Crosshair.localPosition.x;
        float t_cursorPosY = tf_Crosshair.localPosition.y;

        t_cursorPosX = Mathf.Clamp(t_cursorPosX, (-Screen.width / 2 + 50), (Screen.width / 2 - 50));
        t_cursorPosY = Mathf.Clamp(t_cursorPosY, (-Screen.height / 2 + 50), (Screen.height / 2 - 50));

        tf_Crosshair.localPosition = new Vector2(t_cursorPosX, t_cursorPosY);
    }
}
