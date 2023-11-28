using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PumpNozzle : MonoBehaviour
{
    [SerializeField, Tooltip("The layer of the enemies this nozzle can attach to")]
    private LayerMask enemyLayer;
    //if this nozzle is currently capable of sticking to enemies
    private bool canStick = false;
    //if this nozzle is currently attached to an enemy
    private bool stuckToEnemy = false;
    //the enemy this nozzle is currently attached to
    private Enemy currentAttachedEnemy;

    [SerializeField, Tooltip("the amount of time it takes for the nozzle to return after an unsuccesful launch")]
    private float nozzleReturnTime = 2f;
    [SerializeField, Tooltip("the amount of time it takes for the nozzle to return after killing an enemy")]
    private float nozzleKillReturnTime = .3f;
    private float nozzleReturnTimer = 0;
    private bool waitingForNozzleReturn = false;

    [SerializeField, Tooltip("this object's Rigidbody")]
    private Rigidbody rb;

    [SerializeField, Tooltip("this object's fixed joint")]
    private FixedJoint joint;

    [SerializeField, Tooltip("the Pump this nozzle is attached to")]
    private Pump owningPump;

    [SerializeField, Tooltip("The transform this nozzle attaches to the pump at")]
    private Transform pumpAttach;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
            if (!rb)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
        }
        if (!joint)
        {
            joint = GetComponent<FixedJoint>();
            if (!joint)
            {
                joint = gameObject.AddComponent<FixedJoint>();
            }
        }
    }

    public void SetPump(Pump pump, Transform attach)
    {
        pump = owningPump;
        pumpAttach = attach;
    }

    public void ConnectToPump()
    {
        transform.position = pumpAttach.position;
        transform.rotation = pumpAttach.rotation;
        joint.connectedBody = owningPump.gameObject.GetComponent<Rigidbody>();
        owningPump.OnNozzleReattach();
    }

    public void Shoot(Vector3 launchVelocity)
    {
        rb.velocity = launchVelocity;
        canStick = true;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (canStick)
        {
            if (col.gameObject.layer == enemyLayer)
            {
                StickToEnemy(col.gameObject.GetComponent<Enemy>());
            }
            else
            {
                waitingForNozzleReturn = true;
                nozzleReturnTimer = nozzleReturnTime;
            }
        }
        
    }
    
    /// <summary>
    /// called when the nozzle succesfully sticks to an enemy.
    /// Allows for damage and keeps object stuck
    /// </summary>
    /// <param name="enemy"></param>
    private void StickToEnemy(Enemy enemy)
    {
        currentAttachedEnemy = enemy;
        stuckToEnemy = true;
        enemy.SetStuck(true);
        joint.connectedBody = enemy.gameObject.GetComponent<Rigidbody>();
    }

    public void ForceRelease()
    {
        if (stuckToEnemy)
        {
            currentAttachedEnemy.SetStuck(false);
            currentAttachedEnemy = null;
            stuckToEnemy = false;
            ConnectToPump();
        }

    }

    void FixedUpdate()
    {
        nozzleReturnTimer -= Time.fixedDeltaTime;
        if (nozzleReturnTimer < 0)
        {
            waitingForNozzleReturn = false;
            ConnectToPump();
        }
    }

    public void Pump()
    {
        if (stuckToEnemy)
        {
            currentAttachedEnemy.Damage();
        }
    }
}
