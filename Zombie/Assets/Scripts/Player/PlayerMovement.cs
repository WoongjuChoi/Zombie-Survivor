using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float MoveSpeed = 5f; // 앞뒤 움직임의 속도
    public float RotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput _playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody _playerRigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator _playerAnimator; // 플레이어 캐릭터의 애니메이터

    private void Start() 
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        _playerInput = GetComponent<PlayerInput>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerAnimator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate() 
    {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
        Rotate();

        move();

        _playerAnimator.SetFloat(AnimationID.MOVE, _playerInput.MoveInput);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임 오????????? 회전 안하는거같아요 그러게요.. 쳬크가 적용이 안됬었나 플레이하고 누르신거같아요 ㅇㅎ
    private void move() 
    {
        var moveDistance = _playerInput.MoveInput * transform.forward * MoveSpeed * Time.deltaTime;

        _playerRigidbody.MovePosition(_playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() 
    {
        float turn = _playerInput.RotateInput * RotateSpeed * Time.deltaTime;

        //_playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0f, turn, 0f));
        _playerRigidbody.rotation *= Quaternion.Euler(0f, turn, 0f);
    }
}