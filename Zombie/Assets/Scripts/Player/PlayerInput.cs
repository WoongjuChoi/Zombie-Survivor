using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour {
    public string MoveAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    public string RotateAxisName = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string FireButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    public string ReloadButtonName = "Reload"; // 재장전을 위한 입력 버튼 이름

    // 값 할당은 내부에서만 가능
    public float MoveInput { get; private set; } // 감지된 움직임 입력값
    public float RotateInput { get; private set; } // 감지된 회전 입력값
    public bool CanFire { get; private set; } // 감지된 발사 입력값
    public bool CanReload { get; private set; } // 감지된 재장전 입력값

    //private const float MOVE_SCALE = 1f / 3f;

    // 매프레임 사용자 입력을 감지
    private void Update() {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        //if (GameManager.instance.isGameover)
        //{
        //    MoveInput = 0;
        //    RotateInput = 0;
        //    CanFire = false;
        //    CanReload = false;
        //    return;
        //}

        //var moveInputValue = Input.GetAxis(MoveAxisName);

        //if (moveInputValue == 0)
        //{
        //    MoveInput = 0f;
        //}
        //else
        //{
        //    MoveInput += moveInputValue * MOVE_SCALE * Time.deltaTime;
        //    MoveInput = Mathf.Clamp(MoveInput, -1f, 1f); // Clamp는 min과 max 사이의 값으로 만들어줌
        //}

        MoveInput = Input.GetAxis(MoveAxisName);
        RotateInput = Input.GetAxis(RotateAxisName);
        CanFire = Input.GetButton(FireButtonName);
        CanReload = Input.GetButtonDown(ReloadButtonName);
    }
}