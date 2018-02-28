using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard_Controller : MonoBehaviour {
    [SerializeField]
    Transform _playerPosition;

    [SerializeField]
    bool _patrolWaiting;

    [SerializeField]
    float _totalWaitTime = 3f;

    [SerializeField]
    float _switchProbability = 0.2f;

    [SerializeField]
    List<WayPoints> _patrolPoints;

    Animator animator;
    NavMeshAgent _navMeshAgent;
    int _currentPatrolIndex;
    bool _travelling;
    bool _waiting;
    bool _patrolForward;
    float _waitTimer;
    float distance;
    bool trackingPlayer = false;
    float agroRange = 10f;
    private bool stop = true;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to " + gameObject);
        }
        else
        {
            if(_patrolPoints != null && _patrolPoints.Count >= 2)
            {
                _currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("Insufficient patrol points for basic patrolling behaviour.");
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        distance = Vector3.Distance(transform.position, _playerPosition.position);
        if(distance < 12)
        {
            trackingPlayer = true;
        }
        else
        {
            trackingPlayer = false;
        }
        if (!trackingPlayer)
        {
            animator.SetBool("Walking", true);
            if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
            {
                _travelling = false;
                //animator.SetBool("Walking",false);

                if (_patrolWaiting)
                {
                    _waiting = true;
                    _waitTimer = 0f;
                    //animator.SetBool("Walking", false);
                }
                else
                {
                    //animator.SetBool("Walking", true);
                    ChangePatrolPoint();
                    SetDestination();
                }
            }

            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _totalWaitTime)
                {
                    _waiting = false;

                    ChangePatrolPoint();
                    SetDestination();
                }
            }
        }
        else
        {
            TrackPlayer();
        }
	}

    private void TrackPlayer()
    {
        if (distance > agroRange)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("CombatStance", false);
            //_navMeshAgent.isStopped = true;
            //_navMeshAgent.SetDestination(AssignNewPost());
        }
        else if (distance < agroRange)
        {
            _navMeshAgent.isStopped = false;
            animator.SetBool("CombatStance", false);
            animator.SetBool("Walking", true);
            SetDestination();
        }
        if (distance <= 2.5f)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("CombatStance", true);
            _navMeshAgent.isStopped = true;
            StartCoroutine(Attack(stop));
        }
    }

    private IEnumerator Attack(bool stop)
    {
        //print("Attack");
        if (stop)
        {
            stop = !stop;
            animator.SetBool("Attack", true);
        }
        else
        {
            stop = !stop;
            animator.SetBool("Attack", false);
        }
        yield return new WaitForSeconds(.9f);

        if (distance < 2.5f)
        {
            StartCoroutine(Attack(stop));
        }
        else
        {
            animator.SetBool("Attack", false);
            StopCoroutine(Attack(stop));
        }
    }

    private void SetDestination()
    {
        if (!trackingPlayer)
        {
            if (_patrolPoints != null)
            {
                Vector3 targetVector = _patrolPoints[_currentPatrolIndex].transform.position;
                _navMeshAgent.SetDestination(targetVector);
                _travelling = true;
            }
        }
        else
        {
            _navMeshAgent.SetDestination(_playerPosition.position);
        }
    }

    private void ChangePatrolPoint()
    {
        if(Random.Range(0f,1f) <= _switchProbability)
        {
            _patrolForward = !_patrolForward;
        }

        if (_patrolForward)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
        }
        else
        {
            if(--_currentPatrolIndex < 0)
            {
                _currentPatrolIndex = _patrolPoints.Count - 1;
            }
        }
    }
}
