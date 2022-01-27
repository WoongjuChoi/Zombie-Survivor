using System.Collections;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }


    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem MuzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem ShellEjectEffect; // 탄피 배출 효과


    private AudioSource _gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float Damage = 25; // 공격력
    private float _fireDistance = 50f; // 사정거리

    public int RemainedAmmo = 100; // 남은 전체 탄약
    public int MagCapacity = 25; // 탄창 용량
    public int MagAmmo; // 현재 탄창에 남아있는 탄약


    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간
    private float _lastFireTime; // 총을 마지막으로 발사한 시점

    public State state { get; private set; } // 현재 총의 상태

    private LineRenderer _bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        _bulletLineRenderer = GetComponent<LineRenderer>();
        _bulletLineRenderer.positionCount = 2;
        _bulletLineRenderer.enabled = false;

        _gunAudioPlayer = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        // 총 상태 초기화
        MagAmmo = MagCapacity;
        state = State.Ready;
        _lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        if (state == State.Ready && Time.time >= _lastFireTime + timeBetFire)
        {
            _lastFireTime = Time.time;
            Shot();
        }
    }

    // 실제 발사 처리
    private void Shot() {
        RaycastHit hit;

        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, _fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.OnDamage(Damage, hit.point, hit.normal);
            }

            hitPosition = hit.point;
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * _fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));

        --MagAmmo;
        if (MagAmmo <= 0)
        {
            state = State.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        MuzzleFlashEffect.Play();
        ShellEjectEffect.Play();

        _gunAudioPlayer.PlayOneShot(shotClip);

        _bulletLineRenderer.SetPosition(0, fireTransform.position);
        _bulletLineRenderer.SetPosition(1, hitPosition);
        _bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        _bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload() {
        if (state == State.Reloading || RemainedAmmo <= 0 || MagAmmo >= MagCapacity)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        
        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }
}