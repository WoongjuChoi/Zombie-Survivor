using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity
{
    public AudioClip DeathClip; // 사망 소리
    public AudioClip HitClip; // 피격 소리
    public AudioClip ItemPickupClip; // 아이템 습득 소리

    private AudioSource _audioPlayer; // 플레이어 소리 재생기
    private Animator _animator; // 플레이어의 애니메이터

    private PlayerMovement _playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter _playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake()
    {
        // 사용할 컴포넌트를 가져오기
        _audioPlayer = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        _playerMovement.enabled = true;
        _playerShooter.enabled = true;
    }

    // 데미지 처리
    public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // LivingEntity의 TakeDamage() 실행(데미지 적용)
        if (IsDead == false)
        {
            _audioPlayer.PlayOneShot(HitClip);

            base.TakeDamage(damage, hitPoint, hitDirection);
        }
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();

        _audioPlayer.PlayOneShot(DeathClip);

        _animator.SetTrigger("DIE");

        _playerMovement.enabled = false;
        _playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        if (IsDead)
        {
            return;
        }

        IItem item = other.GetComponent<IItem>();

        if (item != null)
        {
            item.Use(gameObject);

            _audioPlayer.PlayOneShot(ItemPickupClip);
        }
    }
}