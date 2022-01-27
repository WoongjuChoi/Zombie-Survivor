using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun Gun; // 사용할 총
    public Transform GunPivot; // 총 배치의 기준점
    public Transform LeftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform RightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private PlayerInput _playerInput; // 플레이어의 입력
    private Animator _playerAnimator; // 애니메이터 컴포넌트

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        _playerInput = GetComponent<PlayerInput>();
        _playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        Gun.gameObject.SetActive(true);
    }
    
    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        Gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전
        if (_playerInput.CanFire)
        {
            Gun.Fire();
        }
        else if (_playerInput.CanReload)
        {
            if (Gun.Reload())
            {
                _playerAnimator.SetTrigger(AnimationID.RELOAD);
            }
        }
    }

    // 탄약 UI 갱신
    private void UpdateUI() {
        if (Gun != null && UIManager.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            UIManager.instance.UpdateAmmoText(Gun.MagAmmo, Gun.RemainedAmmo);
        }
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex) {
        GunPivot.position = _playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        setIKTransform(AvatarIKGoal.LeftHand, LeftHandMount);
        setIKTransform(AvatarIKGoal.RightHand, RightHandMount);
    }

    private void setIKTransform(AvatarIKGoal goal, Transform goalTransform, float weight = 1f)
    {
        _playerAnimator.SetIKPositionWeight(goal, weight);
        _playerAnimator.SetIKRotationWeight(goal, weight);

        _playerAnimator.SetIKPosition(goal, goalTransform.position);
        _playerAnimator.SetIKRotation(goal, goalTransform.rotation);
    }
}