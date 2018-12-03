using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    private enum EnemyState
    {
        None = 0, Walking, Attacking,
    }

    [SerializeField]
    private Vector3 dir;
    [SerializeField]
    private float radius;
    [SerializeField]
    private float speed;

    private EnemyAnim enemyAnim;
    private EnemyState enemyState;

    private Transform player = null;
    private Vector3 center;
    private Rigidbody2D rb2D;
    private float timer;

    private void Awake()
    {
        enemyAnim = GetComponent<EnemyAnim>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.inMenu)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            else
            {
                switch (enemyState)
                {
                    case EnemyState.None:
                        if (enemyAnim.cycleCounter != 2)
                        {
                            enemyAnim.cycleCounter = 2;
                            enemyAnim.flip = true;
                        }
                        else
                        {
                            if (Vector3.Distance(this.transform.position, player.position) > 0.5f)
                            {
                                enemyState = EnemyState.Walking;
                                center = new Vector3(this.transform.position.x, this.transform.position.y, 0.15f);
                            }
                        }
                        break;

                    case EnemyState.Walking:
                        if (enemyAnim.cycleCounter != 0)
                        {
                            enemyAnim.cycleCounter = 0;
                            enemyAnim.flip = true;
                        }
                        else
                        {
                            if (Vector3.Distance(this.transform.position, player.position) <= 0.5f)
                            {
                                enemyState = EnemyState.Attacking;
                            }
                        }
                        break;

                    case EnemyState.Attacking:
                        if (enemyAnim.cycleCounter != 1)
                        {
                            enemyAnim.cycleCounter = 1;
                            enemyAnim.flip = true;
                        }
                        else
                        {
                            if (Vector3.Distance(this.transform.position, player.position) > 0.5f)
                            {
                                enemyState = EnemyState.Walking;
                            }
                        }
                        break;
                }

                if (enemyState == EnemyState.Walking)
                {
                    timer += Time.deltaTime;
                    transform.position = center + dir * Mathf.Sin(timer * speed) * radius;
                }
                else if (enemyState == EnemyState.Attacking)
                {
                    float dist = player.position.x - this.transform.position.x;

                    if (dist > 0.5f)
                    {
                        timer += Time.deltaTime;
                        transform.position = center + dir * Mathf.Sin(timer * speed) * radius;
                    }
                }
            }
        }
        else if (GameManager.inMenu)
        {
            enemyState = EnemyState.None;

            if (enemyAnim.cycleCounter != 2)
            {
                enemyAnim.cycleCounter = 2;
                enemyAnim.flip = true;
            }
        }
    }

    private void LateUpdate()
    {
        if(enemyState == EnemyState.None)
        {
            center = new Vector3(this.transform.position.x, this.transform.position.y, 0.15f);
            StartCoroutine(FreezeYPos());
        }
    }

    IEnumerator FreezeYPos()
    {
        yield return new WaitForSeconds(1);

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }
}
