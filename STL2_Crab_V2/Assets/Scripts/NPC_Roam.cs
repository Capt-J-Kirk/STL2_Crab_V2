using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Roam : MonoBehaviour
{
    private int currentAnimation = 0;
    public Animation animation;
    public List<string> animNames = new();
    public Dictionary<string,float> animSpeeds = new();
    private NavMeshAgent navMeshAgent;

    public Vector3 nextPosition;
    LayerMask walkableLayers;

    private float walkRadius = 50f;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animation = GetComponent<Animation>();
        foreach (AnimationState state in animation) {
            animNames.Add(state.name);
            animSpeeds.Add(state.name, 0f);
        }
        animSpeeds[animNames[0]] = 0.90f;
        animSpeeds[animNames[1]] = 0.90f;
        animSpeeds[animNames[2]] = 2.00f;
        walkableLayers = 1 << LayerMask.NameToLayer("Default");
    }

    void Start()
    {
        SetAnimation();
        FindNextDestination(walkRadius);
    }

    private void FixedUpdate()
    {
        if (navMeshAgent.remainingDistance < 0.5f) FindNextDestination(walkRadius);
    }

    private void SetAnimation()
    {
        AnimationClip clip = animation.GetClip(animNames[currentAnimation]);
        if (clip != null)
        {
            animation.clip = clip;

            animation.Play();
        }
        else
        {
            Debug.LogError("Animation '" + animNames[currentAnimation] + "' not found.");
        }
    }



    private void FindNextDestination(float radius)
    {
        // Generate a random position within a radius
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, walkableLayers))
        {
            navMeshAgent.SetDestination(hit.position);
            navMeshAgent.speed = animSpeeds[animNames[currentAnimation]];
        }
    }


}
