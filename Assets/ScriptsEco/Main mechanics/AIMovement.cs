using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public bool IsRunningAway = false;
    public Vector3[] Way;
    public Vector3 RunningAwayDirection;
    public bool CentrifugalMovement = true;

    public int CurrentDistance = 0;

    Rigidbody2D rb;
    PathFinding pf;
    Animator anim;
    PrimitiveHunter ph;
    bio b;

    public Vector2 MovementVector;
    public GameObject Target = null;

    public bool ManuallyControlled = false;
    void Awake()
    {
        pf = GameObject.Find("PathFinding").GetComponent<PathFinding>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ph = GetComponent<PrimitiveHunter>();
        b = GetComponent<bio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ManuallyControlled) ManualControlling();

        MovementVector = Vector3.zero;
        if (CurrentDistance > 0 && IsRunningAway == false)
        {
            CheckMovementSecondVariation();
            if (CurrentDistance > 0) GetDirectionSecondVariant(Way[CurrentDistance]);
        }
        else
        {
            if (Target != null)
            {
                Chase();
            }
        }
        if (anim != null)
        {
            anim.SetFloat("Horizontal", MovementVector.x);
            anim.SetFloat("Vertical", MovementVector.y);
            anim.SetFloat("Speed", MovementVector.sqrMagnitude);
            if (MovementVector == Vector2.zero)
            {
                transform.rotation = Quaternion.identity;
            }
        }

    }
    private void FixedUpdate()
    {
        if (IsRunningAway)
        {
            if (b.HabitationType == 0)
            {
                if (ph.gm.tc.WalkableTile(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime)))
                {
                    rb.MovePosition(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    RandomizeDirection();
                }
            }
            if (b.HabitationType == 1)
            {
                if (ph.gm.tc.WalkableTile(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime)) == false)
                {
                    rb.MovePosition(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    RandomizeDirection();
                }
            }
            if (b.HabitationType == 2)
            {
                if (ph.gm.tc.WalkableTile(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime)))
                {
                    rb.MovePosition(transform.position + (RunningAwayDirection * b.MoveSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    RandomizeDirection();
                }
            }
        }
        else
        {
            MakeFacing(MovementVector);
            rb.MovePosition(rb.position + (MovementVector * b.MoveSpeed * Time.fixedDeltaTime));
        }
        
       // transform.Translate(MovementVector * Speed * Time.fixedDeltaTime);
    }
    private void ManualControlling()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 rawMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (rawMousePos.x - (int)rawMousePos.x > 0.5f) rawMousePos.x = (int)rawMousePos.x + 1;
            if (rawMousePos.y - (int)rawMousePos.y > 0.5f) rawMousePos.y = (int)rawMousePos.y + 1;
            Vector3Int MousePos = new Vector3Int((int)rawMousePos.x, (int)rawMousePos.y, 0);
            pf.TestTarget = MousePos;
            pf.TestOrder = true;
        }
    }
    private void GetDirection(Vector3 destination)
    {
        MovementVector = destination - transform.position;
        if (MovementVector.x > 0) MovementVector.x = 1;
        if (MovementVector.x < 0) MovementVector.x = -1;
        if (MovementVector.y > 0) MovementVector.y = 1;
        if (MovementVector.y < 0) MovementVector.y = -1;
    }
    private void GetDirectionSecondVariant(Vector3 destination)
    {
        MovementVector = destination - transform.position;
        if (MovementVector.x > 0)
        {
            MovementVector.x = 1;
        }
        else
        {
            if (MovementVector.x < 0) MovementVector.x = -1;
            else
            {
                if (MovementVector.y > 0) MovementVector.y = 1;
                else
                {
                    if (MovementVector.y < 0) MovementVector.y = -1;
                }
            }
        }
        float sum = Mathf.Abs(MovementVector.x) + Mathf.Abs(MovementVector.y);
        MovementVector = new Vector3(MovementVector.x / sum, MovementVector.y / sum, 0);


    }
    private void CheckMovement()
    {
        try
        {
            if (Vector3.Distance(transform.position, Way[CurrentDistance]) < 0.3f)
            {
                CurrentDistance++;
                if (CurrentDistance >= Way.Length)
                {
                    CurrentDistance = 0;
                    MovementVector = Vector2.zero;
                }
            }
        }
        catch
        {
            CurrentDistance = 0;
            MovementVector = Vector2.zero;
        }
    }
    private void CheckMovementSecondVariation()
    {
        try
        {
            Vector3 dif = Way[CurrentDistance] - transform.position;
            if (Mathf.Abs(dif.x) < 0.12f)
            {
                transform.position = new Vector3(Way[CurrentDistance].x, transform.position.y, 0);
            }
            if (Mathf.Abs(dif.y) < 0.12f)
            {
                transform.position = new Vector3(transform.position.x, Way[CurrentDistance].y, 0);
            }
            if (transform.position == Way[CurrentDistance])
            {
                CurrentDistance++;
                if (CurrentDistance >= Way.Length)
                {
                    CurrentDistance = 0;
                    MovementVector = Vector2.zero;
                }
            }
        }
        catch
        {
            CurrentDistance = 0;
            MovementVector = Vector2.zero;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.rotation = Quaternion.identity;
    }
    private void Chase()
    {
        if (Vector3.Distance(transform.position, Target.transform.position) > 1.25f)
        {
            GetDirectionSecondVariant(Target.transform.position);
        }
    }
    private void RandomizeDirection()
    {
        float random = Random.Range(0.0f, 1.0f);
        RunningAwayDirection = new Vector3(random, 1 - random, 0);
        if (Random.Range(1, 3) == 1) RunningAwayDirection = new Vector3(RunningAwayDirection.x * -1, RunningAwayDirection.y);
        if (Random.Range(1, 3) == 1) RunningAwayDirection = new Vector3(RunningAwayDirection.x, RunningAwayDirection.y * -1);
    }
    public void SetTarget(GameObject targ)
    {
        Target = targ;
        if (pf.CalculateWay(Target.transform.position, transform.position, b.HabitationType))
        {
            Way = pf.Way;
            CurrentDistance = 1;
        }
    }
    public void SetDestrination(Vector3 target)
    {
        Target = null;
        if (pf.CalculateWay(target, transform.position, b.HabitationType))
        {
            Way = pf.Way;
            CurrentDistance = 1;
        }
    }
    public void MakeFacing(Vector3 direction)
    {
        if (direction.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }
}
