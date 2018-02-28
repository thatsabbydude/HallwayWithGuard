using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour {

    private Animator animator;
    private float distance;
    public float agroRange;
    private bool stop = true;
    private bool atCurrentPost = false;

    private Transform _currentPost;
    public Transform _post1;
    public Transform _post2;
    public Transform _post3;
    public Transform _post4;

    [SerializeField]
    Transform _playerPosition;

    NavMeshAgent _navMeshAgent;

	// Use this for initialization
	void Start () {
        agroRange = 10;
        animator = GetComponent<Animator>();
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(_navMeshAgent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to " + gameObject);
        }
        _navMeshAgent.SetDestination(_post1.position);
	}

    private void Update()
    {
        //if (transform.position.Equals(_currentPost))
        //    atCurrentPost = true;
        //else
        //    atCurrentPost = false;

        distance = Vector3.Distance(transform.position, _playerPosition.position);
        print(distance);
        //Patrol();
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
            SetDestination(_playerPosition.transform.position);
        }
        if (distance <= 2.5f)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("CombatStance", true);
            _navMeshAgent.isStopped = true;
            StartCoroutine(Attack(stop));
        }
    }

    private Vector3 AssignNewPost()
    {
        if (atCurrentPost)
        {
            int newPost = Random.Range(1, 5);
            switch (newPost)
            {
                case 1:
                    _currentPost = _post1;
                    break;
                case 2:
                    _currentPost = _post2;
                    break;
                case 3:
                    _currentPost = _post3;
                    break;
                case 4:
                    _currentPost = _post4;
                    break;
            }
        }
        return _currentPost.position;
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
        
        if(distance < 2.5f)
        {
            StartCoroutine(Attack(stop));
        }
        else
        {
            animator.SetBool("Attack", false);
            StopCoroutine(Attack(stop));
        }
    }

    private void SetDestination(Vector3 destination)
    {
            _navMeshAgent.SetDestination(destination);
    }
}
