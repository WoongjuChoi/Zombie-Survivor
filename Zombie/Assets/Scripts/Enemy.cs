using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 적 AI를 구현한다
public class Enemy : LivingEntity
{
    public LayerMask TargetMask; // 추적 대상 레이어
    public ParticleSystem HitEffect; // 피격시 재생할 파티클 효과
    public AudioClip DeathClip; // 사망시 재생할 소리
    public AudioClip HitClip; // 피격시 재생할 소리

    public float Damage = 20f; // 공격력
    public float AttackCooltime = 0.5f; // 공격 간격

    private LivingEntity _target; // 추적할 대상
    private NavMeshAgent _navMeshAgent; // 경로계산 AI 에이전트


    private Animator _animator; // 애니메이터 컴포넌트
    private AudioSource _audioPlayer; // 오디오 소스 컴포넌트
    private Renderer _renderer; // 렌더러 컴포넌트

    private float _lastAttackTime; // 마지막 공격 시점

    class AnimParamID
    {
        public static readonly int HasTarget = Animator.StringToHash("HasTarget");
        public static readonly int Die = Animator.StringToHash("Die");
    }

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (_target != null && !_target.IsDead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    //private bool HasSearchedTarget() => _target != null && _target.IsDead == false;
    private bool HasSearchedTarget()
    {
        if (_target?.IsDead == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        // 초기화
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioSource>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        StartingHealth = newHealth;
        Damage = newDamage;
        _navMeshAgent.speed = newSpeed;
        _renderer.material.color = skinColor;
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        _animator.SetBool(AnimParamID.HasTarget, hasTarget);
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath()
    {
        // 살아있는 동안 무한 루프
        while (IsDead == false)
        {
            if (HasSearchedTarget())
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_target.transform.position);
            }
            else
            {
                _navMeshAgent.isStopped = true;

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, TargetMask);

                for (int i = 0; i < colliders.Length; ++i)
                {
                    var targetCandidate = colliders[i].GetComponent<LivingEntity>();

                    if (targetCandidate?.IsDead == false)
                    {
                        _target = targetCandidate;

                        break;
                    }
                }
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead)
        {
            return;
        }

        HitEffect.transform.position = hitPoint;
        HitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
        HitEffect.Play();

        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.TakeDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            colliders[i].enabled = false;
        }

        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;

        _animator.SetTrigger(AnimParamID.Die);

        _audioPlayer.PlayOneShot(DeathClip);
    }

    private void OnTriggerStay(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행   
        if (IsDead == false && Time.time >= _lastAttackTime + AttackCooltime)
        {
            var attackTarget = other.GetComponent<LivingEntity>();

            if (attackTarget != null && attackTarget == _target)
            {
                _lastAttackTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                attackTarget.TakeDamage(Damage, hitPoint, hitNormal);
            }
        }
    }
}