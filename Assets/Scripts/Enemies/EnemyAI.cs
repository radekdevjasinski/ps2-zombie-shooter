using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private const string RagdollLayerName = "ragdoll";
    private const float RepathInterval = 0.25f;
    private const float ClosestBodyForceDivisor = 2.5f;

    [SerializeField] private GameObject playerBlocker;
    [SerializeField] private float hitForce = 250f;
    [SerializeField] private float corpseLifetime = 10f;

    public event Action<EnemyAI> Died;

    public bool IsDead { get; private set; }

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private Rigidbody[] ragdollBodies;
    private int ragdollLayer;
    private float nextRepathTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollLayer = LayerMask.NameToLayer(RagdollLayerName);
        SetRagdollEnabled(false);
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (Time.time < nextRepathTime || !agent.isOnNavMesh)
        {
            return;
        }

        agent.SetDestination(player.position);
        nextRepathTime = Time.time + RepathInterval;
    }

    public void Kill(Vector3 hitPoint, Vector3 forceDirection)
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        enabled = false;
        agent.enabled = false;
        animator.enabled = false;
        Destroy(playerBlocker);

        SetRagdollEnabled(true);
        MoveToRagdollLayer();
        ApplyHitForce(hitPoint, forceDirection.normalized);

        Died?.Invoke(this);
        Destroy(gameObject, corpseLifetime);
    }

    private void ApplyHitForce(Vector3 hitPoint, Vector3 direction)
    {
        Rigidbody closestBody = FindClosestRagdollBody(hitPoint);
        foreach (Rigidbody body in ragdollBodies)
        {
            float force = body == closestBody
                ? hitForce / ClosestBodyForceDivisor
                : hitForce / ragdollBodies.Length;
            body.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    private Rigidbody FindClosestRagdollBody(Vector3 point)
    {
        Rigidbody closestBody = null;
        float closestSqrDistance = float.MaxValue;
        foreach (Rigidbody body in ragdollBodies)
        {
            float sqrDistance = (point - body.worldCenterOfMass).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestBody = body;
            }
        }
        return closestBody;
    }

    private void SetRagdollEnabled(bool isRagdollEnabled)
    {
        foreach (Rigidbody body in ragdollBodies)
        {
            body.isKinematic = !isRagdollEnabled;
            body.interpolation = isRagdollEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
    }

    private void MoveToRagdollLayer()
    {
        if (ragdollLayer < 0)
        {
            Debug.LogError($"Layer '{RagdollLayerName}' is missing in Tags and Layers; corpse keeps its enemy layer.", this);
            return;
        }

        foreach (Transform bone in GetComponentsInChildren<Transform>())
        {
            bone.gameObject.layer = ragdollLayer;
        }
    }
}
