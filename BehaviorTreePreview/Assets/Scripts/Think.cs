using BehaviorTreeTest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum EStatsTypes { Food, Water, Heat }


public class Think : MonoBehaviour
{
    const float theta = 1.0f;
    [SerializeField]
    private Text m_GUIText;
    [SerializeField]
    private int[,] m_Stats = { {100, 90}, {100, 95}, {100, 75} };
    [SerializeField]
    const float degradeCooldown = 0.3f;
    private float currentDegradeCooldown = 0;
    private BehaviourTree bT;
    [SerializeField]
    private NavMeshAgent agent;
    private Vector3 targetPos;
    private bool haveTarget = false;
    private EStatsTypes targetType;
    private const float btUpdate = 1.0f;
    private float currentBTUpdate = btUpdate;
    private LineRenderer myLineRenderer;

    internal BehaviourTree BT
    {
        get
        {
            return bT;
        }

        set
        {
            bT = value;
        }
    }



    // Use this for initialization
    void Start()
    {
        BT = new BehaviourTree();
        var qFood = BT.CreateActionNode(IsFoodLow);
        var gFood = BT.CreateActionNode(FetchFood);
        var GetFoodList = new List<uint> { qFood, gFood };

        var qWater = BT.CreateActionNode(IsWaterLow);
        var gWater = BT.CreateActionNode(FetchWater);
        var GetWaterList = new List<uint> { qWater, gWater };

        var qHeat = BT.CreateActionNode(IsHeatLow);
        var gHeat = BT.CreateActionNode(FetchHeat);
        var GetHeatList = new List<uint> { qHeat, gHeat };

        var cFood = BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetFoodList);
        var cWater = BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetWaterList);
        var cHeat = BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetHeatList);

        var resourceList = new List<uint> {
            BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cFood),
            BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cWater),
            BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cHeat)
        };

        var root = BT.CreateCompositeNode(CompositeNodeTypes.Sequence, resourceList);
        BT.SetRootNode(root);

        myLineRenderer = GetComponent<LineRenderer>();

        UpdateGUI();
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentBTUpdate -= Time.deltaTime;

        if (currentBTUpdate <= 0)
        {
            BT.RunTree();
            currentBTUpdate = btUpdate;
        }

        if (currentDegradeCooldown <= 0)
        {
            int degrade = Random.Range(0, 10);
            switch (degrade)
            {
                case 0:
                    --m_Stats[0,0];
                    UpdateGUI();
                    currentDegradeCooldown = degradeCooldown;
                    break;
                case 1:
                    --m_Stats[1, 0];
                    UpdateGUI();
                    currentDegradeCooldown = degradeCooldown;
                    break;
                case 2:
                    --m_Stats[2, 0];
                    UpdateGUI();
                    currentDegradeCooldown = degradeCooldown;
                    break;
                default:
                    break;
            }
        }
        else
            currentDegradeCooldown -= Time.deltaTime;

        if(targetPos != null)
        {
            myLineRenderer.SetPosition(0, this.transform.position);
            myLineRenderer.SetPosition(1, targetPos);
        }
    }

    void UpdateGUI()
    {
        if (m_Stats[0, 0] < m_Stats[0, 1])
            m_GUIText.text = "<color=red>";
        else
            m_GUIText.text = "<color=black>";
        m_GUIText.text += "Food: (" + m_Stats[0, 1] + "): " + m_Stats[0, 0] + "</color>\n";

        if (m_Stats[1, 0] < m_Stats[1, 1])
            m_GUIText.text += "<color=red>";
        else
            m_GUIText.text += "<color=black>";
        m_GUIText.text += "Water: (" + m_Stats[1, 1] + "): " + m_Stats[1, 0] + "</color>\n";

        if (m_Stats[2, 0] < m_Stats[2, 1])
            m_GUIText.text += "<color=red>";
        else
            m_GUIText.text += "<color=black>";
        m_GUIText.text += "Heat: (" + m_Stats[2, 1] + "): " + m_Stats[2, 0] + "</color>\n";
    }

    public void AddPower(EStatsTypes powerType, int value)
    {
        m_Stats[(int)powerType, 0] += value;
    }

    ENodeStates IsFoodLow()
    {
        return m_Stats[0, 0] <= m_Stats[0, 1] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
    }
    ENodeStates IsWaterLow()
    {
        return m_Stats[1, 0] <= m_Stats[1, 1] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
    }
    ENodeStates IsHeatLow()
    {
        return m_Stats[2, 0] <= m_Stats[2, 1] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
    }

    ENodeStates FetchFood()
    {
        if (!haveTarget || targetType != EStatsTypes.Food)
        {
            var foodPowerUp = GameObject.Find("FoodPowerUps");
            var transformChildCount = foodPowerUp.transform.childCount;
            if (transformChildCount > 0)
            {
                // find closets
                Transform target = foodPowerUp.transform.GetChild(0);
                var ChildDistance = Vector3.Distance(transform.position, target.position);
                for (int i = 1; i < transformChildCount; ++i)
                {
                    var child = foodPowerUp.transform.GetChild(i);
                    var newDist = Vector3.Distance(transform.position, child.position);
                    if (ChildDistance > newDist)
                    {
                        target = child;
                        ChildDistance = newDist;
                    }
                }
                // move to point
                haveTarget = true;
                targetPos = target.position;
                targetType = EStatsTypes.Food;
                agent.SetDestination(targetPos);
                agent.isStopped = false;
                return ENodeStates.RUNNING;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPos) <= theta)
            {
                haveTarget = false;
                agent.isStopped = true;
                return ENodeStates.SUCCESS;
            }
            else
                return ENodeStates.RUNNING;
        }

        return ENodeStates.FAILURE;
    }

    ENodeStates FetchWater()
    {
        if (!haveTarget || targetType != EStatsTypes.Water)
        {
            var waterPowerUp = GameObject.Find("WaterPowerUps");
            var transformChildCount = waterPowerUp.transform.childCount;
            if (transformChildCount > 0)
            {
                // find closets
                Transform target = waterPowerUp.transform.GetChild(0);
                var ChildDistance = Vector3.Distance(transform.position, target.position);
                for (int i = 1; i < transformChildCount; ++i)
                {
                    var child = waterPowerUp.transform.GetChild(i);
                    var newDist = Vector3.Distance(transform.position, child.position);
                    if (ChildDistance > newDist)
                    {
                        target = child;
                        ChildDistance = newDist;
                    }
                }
                // move to point
                haveTarget = true;
                targetPos = target.position;
                targetType = EStatsTypes.Water;
                agent.SetDestination(targetPos);
                agent.isStopped = false;
                return ENodeStates.RUNNING;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPos) <= theta)
            {
                haveTarget = false;
                agent.isStopped = true;
                return ENodeStates.SUCCESS;
            }
            else
                return ENodeStates.RUNNING;
        }

        return ENodeStates.FAILURE;
    }

    ENodeStates FetchHeat()
    {
        if (!haveTarget || targetType != EStatsTypes.Heat)
        {
            var heatPowerUp = GameObject.Find("HeatPowerUps");
            var transformChildCount = heatPowerUp.transform.childCount;
            if (transformChildCount > 0)
            {
                // find closets
                Transform target = heatPowerUp.transform.GetChild(0);
                var ChildDistance = Vector3.Distance(transform.position, target.position);
                for (int i = 1; i < transformChildCount; ++i)
                {
                    var child = heatPowerUp.transform.GetChild(i);
                    var newDist = Vector3.Distance(transform.position, child.position);
                    if (ChildDistance > newDist)
                    {
                        target = child;
                        ChildDistance = newDist;
                    }
                }
                // move to point
                haveTarget = true;
                targetPos = target.position;
                targetType = EStatsTypes.Heat;
                agent.SetDestination(targetPos);
                agent.isStopped = false;
                return ENodeStates.RUNNING;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, targetPos) <= theta)
            {
                haveTarget = false;
                agent.isStopped = true;
                return ENodeStates.SUCCESS;
            }
            else
                return ENodeStates.RUNNING;
        }

        return ENodeStates.FAILURE;
    }
}
