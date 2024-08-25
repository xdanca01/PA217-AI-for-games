using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Sphere
{
    public Vector3 WorldCenter { get; set; }

    public float Radius { get; set; }
}

public class CombinedSteeringAgent : AbstractSteeringAgent
{
    [SerializeField]
    private SphereCollider sphereCollider;
        
    [SerializeField]
    private Path pathToFollow;

    [SerializeField]
    private LayerMask obstacleLayer;

    private Sphere[] staticObstacles;
    private CombinedSteeringAgent[] otherAgents;
    private int currentPathNode = 0;
    private int lastPointIndex = 0;
    private bool goBack = false;
    private float offsetTurn = 0.05f;
    private float range = 1.0f;
    private Vector3 avoidancePoint;
    private bool avoiding = false;
    private float speed;

    // The AI agent can be considered a sphere
    // with center at "Position" and radius equal to this property
    public float Radius => sphereCollider.bounds.extents.x; // .bounds is used instead of .radius since the radius is in local coordinates

    protected override void Awake()
    {
        base.Awake();

        SetInitialLocation();
    }

    protected override void Start()
    {
        base.Start();

        InitializeObstaclesArrays();
    }

    protected bool willCollide()
    {
        Vector3 rightVec = Quaternion.Euler(0, 25,0) * LookDirection;
        Vector3 rightVec2 = Quaternion.Euler(0, 50, 0) * LookDirection;
        Ray rightRay = new Ray(Position, rightVec);
        Ray rightRay2 = new Ray(Position, rightVec2);
        Debug.DrawRay(Position, rightVec * range * 1.2f, Color.cyan);
        Debug.DrawRay(Position, rightVec2 * range * 1.2f, Color.cyan);
        Ray ray = new Ray(Position, LookDirection);
        Debug.DrawRay(Position, LookDirection * range, Color.cyan);
        if(Physics.Raycast(ray, out RaycastHit hit, range))
        {
            string objectName = hit.collider.name;
            if(objectName.Contains("Obstacle") == true)
            {
                if(avoiding == true)
                {
                    return true;
                }
                float radius = hit.collider.transform.localScale.x;
                Vector3 normalToLook = new Vector3(-LookDirection.z, 0.0f, LookDirection.x) * (radius + Radius/2);
                avoidancePoint = hit.collider.transform.position + normalToLook;
                avoidancePoint.y = 0.0f;
                avoiding = true;
                speed = maxSpeed;
                return true;
            }
            if (objectName.Contains("Mesh") == true)
            {
                CombinedSteeringAgent agent = hit.transform.parent.GetComponent<CombinedSteeringAgent>();
                float y = Vector3.Cross(LookDirection, agent.LookDirection).y;
                //The object is comming from right
                if (y < 0)
                {
                    speed = 0.0f;
                    avoidancePoint = Position;
                    avoiding = true;
                    return true;
                }
                return false;
            }
        }
        if(Physics.Raycast(rightRay, out RaycastHit hit2, range*1.2f))
        {
            string objectName = hit2.collider.name;
            if (objectName.Contains("Mesh") == true)
            {
                CombinedSteeringAgent agent = hit2.transform.parent.GetComponent<CombinedSteeringAgent>();
                float y = Vector3.Cross(LookDirection, agent.LookDirection).y;
                //The object is comming from right
                if(y < 0)
                {
                    speed = 0.0f;
                    avoidancePoint = Position;
                    avoiding = true;
                    return true;
                }
                return false;
            }
        }
        if (Physics.Raycast(rightRay2, out RaycastHit hit3, range * 1.2f))
        {
            string objectName = hit3.collider.name;
            if (objectName.Contains("Mesh") == true)
            {
                CombinedSteeringAgent agent = hit3.transform.parent.GetComponent<CombinedSteeringAgent>();
                float y = Vector3.Cross(LookDirection, agent.LookDirection).y;
                //The object is comming from right
                if (y < 0)
                {
                    speed = 0.0f;
                    avoidancePoint = Position;
                    avoiding = true;
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    protected override void Update()
    {
        base.Update();

        // TODO Add your solution here. Feel free to add your own variables or helper functions.
        //      Use "pathToFollow.GetPathVertices()" to access the points of the path.
        //      Variables "staticObstacles" and "otherAgents" contain information about obstacles to avoid, and agents to avoid.
        //      However, your solution does not necessarily have to use these arrays – but they are here to help you in case of need.
        Vector3 target = new Vector3();
        if(willCollide() == false && avoiding == false)
        {
            Vector3[] points = pathToFollow.GetPathVertices();
            if ((points[lastPointIndex] - Position).magnitude <= offsetTurn)
            {
                if (goBack == false)
                {
                    if (lastPointIndex >= points.Length - 1)
                    {
                        goBack = true;
                        --lastPointIndex;
                    }
                    else
                    {
                        ++lastPointIndex;
                    }
                }
                else
                {
                    if (lastPointIndex == 0)
                    {
                        goBack = false;
                        ++lastPointIndex;
                    }
                    else
                    {
                        --lastPointIndex;
                    }
                }
            }
            target = points[lastPointIndex] - Position;
            SetRotationImmediate(target);
            Velocity = LookDirection * maxSpeed;
        }
        else
        {
            target = avoidancePoint;
            SetRotationTransition(target - Position);
            Velocity = LookDirection * speed;
            float distanceToTarget = new Vector2(Position.x - avoidancePoint.x, Position.z - avoidancePoint.z).magnitude;
            if(distanceToTarget <= 0.02)
            {
                avoiding = false;
            }
        }
        
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void InitializeObstaclesArrays()
    {
        // NOTE for a curious reader:
        // It is not a best practice to use LINQ in Unity because of the performance & garbage collection issues.
        // However, in this case, this is not a big deal as the performance is far from being an issue (hopefully). 
        // But in general, it is better to stay away from LINQ, mainly in Update() function and other computations repeated every frame.
        staticObstacles = FindObjectsOfType<CapsuleCollider>()
            .Where(x => obstacleLayer == (obstacleLayer | (1 << x.gameObject.layer)))
            .Select(x => new Sphere
            {
                WorldCenter = x.transform.TransformPoint(x.center),
                Radius = x.bounds.extents.x
            })
            .ToArray();

        otherAgents = FindObjectsOfType<CombinedSteeringAgent>()
            .Where(x => x != this)
            .ToArray();
    }

    private void SetInitialLocation()
    {
        var pathVertices = pathToFollow.GetPathVertices();
        Position = pathVertices[currentPathNode];
        if (pathVertices.Length > 1)
        {
            SetRotationImmediate(pathVertices[currentPathNode + 1] - Position);
        }
    }
}
