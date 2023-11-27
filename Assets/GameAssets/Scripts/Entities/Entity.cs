using DG.Tweening;
using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum EntityState
{
    Idle,
    Roaming,
    HuntingFood,
    Fleeing,
    Breeding
}

public class Entity : MonoBehaviour
{
    public string DisplayName;
    public float VisionRadius;
    public float MovementSpeed;
    [SerializeField] private float m_hunger;
    public float Hunger
    {
        get { return m_hunger; }
        set
        {
            float newHunger = Mathf.Clamp(value, 0f, MaxHunger);

            if (m_hunger != newHunger)
            {
                m_hunger = newHunger;
                OnHungerChanged?.Invoke(m_hunger);
            }
        }
    }
    public float MaxHunger;
    // Amount of Hunger lost every EntityUpdate
    public float HungerDepletionRate;
    // List of FoodSources that this Entity can eat
    public List<FoodSource> Diet;
    // The maximum amount of time that this Entity will pursue a FoodSource
    public float MaxHuntDuration;
    // Minimum amount of time after a successful hunt until the Entity can hunt a FoodSource again
    public float HuntCooldown;
    // When Hunger >= BreedingHungerRequirement, the Entity will start looking for a breeding partner
    // When Hunger < BreedingHungerRequirement, the Entity will start looking for food
    public float BreedingHungerRequirement;
    // Minimum amount of time after a breed until the Entity can breed again
    public float BreedingCooldown;
    public bool IsDead;

    protected EntityState m_state = EntityState.Idle;
    public EntityState State
    {
        get { return m_state; }
        protected set
        {
            if (m_state != value)
            {
                OnExitState(m_state);
                m_state = value;
                OnStateChanged?.Invoke(m_state);
                OnEnterState(m_state);
            }
        }
    }

    public UnityAction<float> OnHungerChanged;
    public UnityAction<EntityState> OnStateChanged;
    public UnityAction OnDeath;
    public static UnityAction OnEntitySpawn;
    public static UnityAction OnEntityDeath;

    [SerializeField] protected Transform m_visionRadiusIndicator;
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected GameObject BreedEventPrefab;
    [SerializeField] protected GameObject m_deathAudioPrefab;

    protected bool m_canStartHunt
    {
        get
        {
            return m_huntCooldownCounter >= HuntCooldown;
        }
    }

    protected bool m_canBreed
    {
        get
        {
            if(State == EntityState.Fleeing || State == EntityState.HuntingFood || State == EntityState.Breeding)
            {
                return false;
            }

            if(m_breedCooldownCounter >= BreedingCooldown && Hunger >= BreedingHungerRequirement) 
            {
                return m_breedEvent == null;
            }
            else
            {
                return false;
            }
        }
    }

    protected NavMeshAgent m_navMeshAgent;
    public float UpdateRate = .25f;
    public float m_updateCounter = 0f;
    public float m_hungerDepletionCounter = 0f;
    public float m_huntCooldownCounter = Mathf.Infinity;
    public float m_breedCooldownCounter = 0f;
    public float m_minDistanceToEatFood = 1f;

    public FoodSource m_huntTarget;
    public List<Entity> m_sameSpeciesEntitiesInRange = new List<Entity>();
    public List<FoodSource> m_foodSourcesInRange = new List<FoodSource>();

    protected void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        // Set up vision radius indicator
        Vector3 newScale = m_visionRadiusIndicator.localScale;
        newScale.x = VisionRadius;
        newScale.z = VisionRadius;
        m_visionRadiusIndicator.localScale = newScale;

        m_navMeshAgent.speed = MovementSpeed;

        // Randomize starting hunger
        Hunger = Mathf.Ceil(Random.Range(BreedingHungerRequirement, MaxHunger));

        // Add some randomness to counters and cooldowns
        UpdateRate = Mathf.Abs(Random.Range(UpdateRate - .05f, UpdateRate + .05f));
        HuntCooldown = Mathf.Abs(Random.Range(HuntCooldown - 5f, HuntCooldown + 5f));
        BreedingCooldown = Mathf.Abs(Random.Range(BreedingCooldown - 5f, BreedingCooldown + 5f));

        m_updateCounter = Random.Range(0, UpdateRate);

        OnEntitySpawn?.Invoke();
    }

    protected void Update()
    {
        UpdateHunger();
        UpdateCounters();
        UpdateAnimator();
    }

    protected void UpdateHunger()
    {
        if (IsDead) { return; }

        m_hungerDepletionCounter += Time.deltaTime;
        if (m_hungerDepletionCounter >= 1f)
        {
            Hunger -= HungerDepletionRate;
            m_hungerDepletionCounter = 0f;

            if (Hunger <= 0 && !IsDead)
            {
                Die();
                return;
            }
        }
    }

    protected void UpdateCounters()
    {
        if (IsDead) return;

        m_updateCounter += Time.deltaTime;
        if (m_updateCounter > UpdateRate)
        {
            CheckVision();
            EntityUpdate();
            m_updateCounter = 0f;
        }

        if (m_canStartHunt == false)
        {
            m_huntCooldownCounter += Time.deltaTime;
        }

        if(m_canBreed == false)
        {
            m_breedCooldownCounter += Time.deltaTime;
        }

        if(State == EntityState.Idle)
        {
            m_idleCounter += Time.deltaTime;
        }
        else if(State == EntityState.Roaming)
        {
            m_roamingCounter += Time.deltaTime;
        }
        else if(State == EntityState.Fleeing)
        {
            m_fleeCounter += Time.deltaTime;
        }
    }

    protected void UpdateAnimator()
    {
        if (m_animator == null || IsDead) { return; }

        float moveBlend = m_navMeshAgent.velocity.magnitude;
        moveBlend = Mathf.Clamp01(moveBlend);

        m_animator.SetFloat("MovementBlend", moveBlend);
    }

    protected void EntityUpdate()
    {
        switch (State)
        {
            case EntityState.Idle:
                IdleUpdate();
                break;
            case EntityState.Roaming:
                RoamingUpdate();
                break;
            case EntityState.HuntingFood:
                HuntingFoodUpdate();
                break;
            case EntityState.Fleeing:
                FleeingUpdate();
                break;
            case EntityState.Breeding:
                BreedingUpdate();
                break;
            default:
                break;
        }
    }

    protected void OnEnterState(EntityState state)
    {
        switch (State)
        {
            case EntityState.Idle:
                OnEnterIdle();
                break;
            case EntityState.Roaming:
                OnEnterRoaming();
                break;
            case EntityState.HuntingFood:
                OnEnterHuntingFood();
                break;
            case EntityState.Fleeing:
                OnEnterFleeing();
                break;
            case EntityState.Breeding:
                OnEnterBreeding();
                break;
            default:
                break;
        }
    }

    protected void OnExitState(EntityState state)
    {
        switch (State)
        {
            case EntityState.Idle:
                OnExitIdle();
                break;
            case EntityState.Roaming:
                OnExitRoaming();
                break;
            case EntityState.HuntingFood:
                OnExitHuntingFood();
                break;
            case EntityState.Fleeing:
                OnExitFleeing();
                break;
            case EntityState.Breeding:
                OnExitBreeding();
                break;
            default:
                break;
        }
    }

    #region IDLE

    private float m_idleDurationMin = 2f;
    private float m_idleDurationMax = 10f;
    public float m_currentIdleDuration;
    public float m_idleCounter = 0f;

    protected void OnEnterIdle()
    {
        m_currentIdleDuration = Random.Range(m_idleDurationMin, m_idleDurationMax);
        m_idleCounter = 0f;

        m_navMeshAgent.destination = transform.position;
    }

    protected void IdleUpdate()
    {
        if(m_idleCounter >= m_currentIdleDuration)
        {
            m_navMeshAgent.SetDestination(transform.position + RandomV3(m_maxRoamingDistance));
            State = EntityState.Roaming;
            return;
        }

        m_navMeshAgent.destination = transform.position;

        UpdateInteractions();
    }

    protected void OnExitIdle()
    {
        //Debug.Log("OnExitIdle");
    }

    #endregion

    #region ROAMING

    public float m_maxRoamingDuration = 10f;
    public float m_roamingCounter = 0f;
    public float m_maxRoamingDistance = 5f;
    public bool m_forceToCompleteRoam = false;

    protected void OnEnterRoaming()
    {
        m_roamingCounter = 0f;
        //Debug.Log("OnEnterRoaming");
    }

    protected void RoamingUpdate()
    {
        if(m_roamingCounter >= m_maxRoamingDuration)
        {
            State = EntityState.Idle;
            return;
        }

        if(!m_forceToCompleteRoam)
        {
            UpdateInteractions();
        }

        if (m_navMeshAgent.hasPath)
        {
            return;
        }
        else
        {
            State = EntityState.Idle;
        }

        //SetRandomDestination();
    }

    protected void OnExitRoaming()
    {
        m_forceToCompleteRoam = false;

        //Debug.Log("OnExitRoaming");
    }

    #endregion

    #region HUNTINGFOOD

    protected void OnEnterHuntingFood()
    {
        //Debug.Log("OnEnterHuntingFood");
        if(m_huntTarget != null)
        {
            Entity huntEntity = m_huntTarget.GetComponent<Entity>();
            if(huntEntity != null)
            {
                huntEntity.Spook(transform);
            }
        }
    }

    protected void HuntingFoodUpdate()
    {
        if (m_huntTarget == null)
        {
            State = EntityState.Idle;
            return;
        }

        m_navMeshAgent.SetDestination(m_huntTarget.transform.position);

        if (Vector3.Distance(transform.position, m_huntTarget.transform.position) <= m_minDistanceToEatFood)
        {
            FaceTarget(m_huntTarget.transform.position);

            m_animator.SetTrigger("Attack");

            Hunger += m_huntTarget.GetFoodValue();

            m_huntTarget.OnEaten();
            m_huntTarget = null;
            m_huntCooldownCounter = 0;
            State = EntityState.Idle;
        }
    }

    protected void OnExitHuntingFood()
    {
        //Debug.Log("OnExitHuntingFood");
    }

    #endregion

    #region FLEEING

    public Transform m_fleeTarget;
    public float m_fleeMaxDuration = 10f;
    public float m_fleeCounter;

    protected void OnEnterFleeing()
    {
        //Debug.Log("OnEnterFleeing");
        m_animator.SetTrigger("Fear");

        m_fleeCounter = 0f;
    }

    protected void FleeingUpdate()
    {
        if(m_fleeCounter >= m_fleeMaxDuration)
        {
            State = EntityState.Idle;
            return;
        }

        if (m_fleeTarget != null)
        {
            AgentFlee(m_fleeTarget);
        }
        else
        {
            State = EntityState.Idle;
        }
    }

    protected void OnExitFleeing()
    {
        //Debug.Log("OnExitFleeing");
    }

    #endregion

    #region BREEDING

    private Vector3 m_breedingPosition;
    public bool IsInPositionToBreed { get; private set; } = false;
    private EntityBreedEvent m_breedEvent;

    protected void OnEnterBreeding()
    {
        //Debug.Log("OnEnterBreeding");
        IsInPositionToBreed = false;
    }

    protected void BreedingUpdate()
    {
        if(m_breedEvent == null || m_breedingPosition == null)
        {
            State = EntityState.Idle;
            return;
        }

        if(Vector3.Distance(transform.position, m_navMeshAgent.destination) > .25f)
        {
            return;
        }
        else if(IsInPositionToBreed == false)
        {
            IsInPositionToBreed = true;
            FaceTarget(m_breedingPosition);
            //Debug.Log(gameObject.name + " In Breeding Position");
        }
    }

    protected void OnExitBreeding()
    {
        //Debug.Log("OnExitBreeding");
    }

    #endregion

    protected void UpdateInteractions()
    {
        if(Hunger < BreedingHungerRequirement)
        {
            if (m_foodSourcesInRange.Count > 0 && m_canStartHunt)
            {
                m_huntTarget = m_foodSourcesInRange[0];

                State = EntityState.HuntingFood;

                return;
            }
        }
        else
        {
            if (m_sameSpeciesEntitiesInRange.Count > 0 && m_canBreed)
            {
                foreach (Entity entity in m_sameSpeciesEntitiesInRange)
                {
                    if (entity.m_canBreed)
                    {
                        // Create and setup BreedEvent
                        EntityBreedEvent breedEvent = Instantiate(BreedEventPrefab).GetComponent<EntityBreedEvent>();
                        breedEvent.Setup(this, entity);

                        break;
                    }
                }
            }
        }
    }

    protected void SetRandomDestination()
    {
        if (!m_navMeshAgent.hasPath || m_navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            Vector3 destination = transform.position + RandomV3(10);

            NavMeshHit hit;
            bool pointFound = NavMesh.SamplePosition(destination, out hit, Mathf.Infinity, NavMesh.AllAreas);

            Vector3 navDestination = hit.position;

            m_navMeshAgent.SetDestination(navDestination);

            Debug.Log("SetDestination: " + navDestination);
        }
    }

    protected void AgentFlee(Transform fleeTransform)
    {
        float fleeDistance = 5f;

        Vector3 directionToFleeTarget = transform.position - fleeTransform.position;
        Vector3 fleeDirection = directionToFleeTarget.normalized;
        Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

        m_navMeshAgent.SetDestination(fleePosition);
    }

    public void Spook(Transform t)
    {
        if(IsDead) return;

        m_fleeTarget = t;
        State = EntityState.Fleeing;
    }

    public void CallToPosition(Vector3 position)
    {
        m_forceToCompleteRoam = true;
        m_navMeshAgent.SetDestination(position + RandomV3(2));
        State = EntityState.Roaming;
    }

    public void SetBreeding(EntityBreedEvent breedEvent, Vector3 position)
    {
        m_breedEvent = breedEvent;
        m_breedingPosition = position;
        m_navMeshAgent.SetDestination(m_breedingPosition);

        State = EntityState.Breeding;
    }

    public void FinishBreeding()
    {
        m_breedCooldownCounter = 0;
        m_breedEvent = null;
        State = EntityState.Idle;
    }

    public void CancelBreeding()
    {
        m_breedEvent = null;
        State = EntityState.Idle;
    }

    public void Die()
    {
        OnDeath?.Invoke();

        IsDead = true;
        m_animator.SetTrigger("Death");

        Instantiate(m_deathAudioPrefab, transform.position, Quaternion.identity);

        Tweener scaleDownTween = transform.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear);
        scaleDownTween.OnComplete(() => { Destroy(gameObject); OnEntityDeath?.Invoke(); });

        Destroy(m_navMeshAgent);
    }

    [Button]
    public void ChangeState(EntityState newState)
    {
        State = newState;
    }

    [Button]
    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        if(lookPos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }
    }

    protected void CheckVision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, VisionRadius);

        List<Entity> entities = new List<Entity>();
        List<FoodSource> foodSources = new List<FoodSource>();

        foreach (Collider collider in colliders)
        {
            Entity entity = collider.GetComponentInParent<Entity>();
            FoodSource food = collider.GetComponentInParent<FoodSource>();

            if (entity != null && entity != this && entity.tag == this.tag)
            {
                if(!entities.Contains(entity))
                {
                    entities.Add(entity);
                }
            }
            if (food != null)
            {
                foodSources.Add(food);
            }
        }

        // Get the distinct types from Diet
        var dietTypes = Diet.Select(o => o.GetType()).Distinct();
        // Remove objects from foodSources that are not of a type present in Diet
        foodSources = foodSources.Where(o => dietTypes.Contains(o.GetType())).ToList();

        // Sort foodSources by FoodValue and then by distance
        m_foodSourcesInRange = foodSources
            .OrderByDescending(foodSource => foodSource.GetFoodValue())
            .ThenBy(foodSource => (foodSource.transform.position - transform.position).sqrMagnitude)
            .ToList();

        m_sameSpeciesEntitiesInRange = entities;
    }

    [Button]
    public void ToggleVisionRadiusIndicator(bool toggle)
    {
        m_visionRadiusIndicator.gameObject.SetActive(toggle);
    }

    public string GetDietAsString()
    {
        List<string> items = new List<string>();
        foreach (FoodSource foodSource in Diet)
        {
            items.Add(foodSource.GetDisplayName());
        }

        string commaSeparatedString = string.Join(", ", items);
        return commaSeparatedString;
    }

    protected Vector3 RandomV3(float range)
    {
        Vector3 v3 = Vector3.zero;

        v3.x += Random.Range(0, range);
        v3.z += Random.Range(0, range);

        if (Random.Range(0, 2) == 0)
        {
            v3.x = -v3.x;
        }
        if (Random.Range(0, 2) == 0)
        {
            v3.z = -v3.z;
        }

        return v3;
    }
}