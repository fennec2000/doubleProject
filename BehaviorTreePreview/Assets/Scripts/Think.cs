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
    private BehaviourTree m_BT;
    [SerializeField]
    private NavMeshAgent agent;
    private Vector3 targetPos;
    private bool haveTarget = false;
    private EStatsTypes targetType;


    // Use this for initialization
    void Start()
    {
        m_BT = new BehaviourTree();
        var qFood = m_BT.CreateActionNode(IsFoodLow);
        var gFood = m_BT.CreateActionNode(FetchFood);
        var GetFoodList = new List<uint> { qFood, gFood };

        var qWater = m_BT.CreateActionNode(IsWaterLow);
        var gWater = m_BT.CreateActionNode(FetchWater);
        var GetWaterList = new List<uint> { qWater, gWater };

        var qHeat = m_BT.CreateActionNode(IsHeatLow);
        var gHeat = m_BT.CreateActionNode(FetchHeat);
        var GetHeatList = new List<uint> { qHeat, gHeat };

        var cFood = m_BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetFoodList);
        var cWater = m_BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetWaterList);
        var cHeat = m_BT.CreateCompositeNode(CompositeNodeTypes.Sequence, GetHeatList);

        var resourceList = new List<uint> {
            m_BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cFood),
            m_BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cWater),
            m_BT.CreateDecoratorNode(DecoratorNodeType.Succeeder, cHeat)
        };

        var root = m_BT.CreateCompositeNode(CompositeNodeTypes.Sequence, resourceList);
        m_BT.SetRootNode(root);
        UpdateGUI();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_BT.RunTree();

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
    }

    void UpdateGUI()
    {
        m_GUIText.text = "Food: " + m_Stats[0, 0] + '\n';
        m_GUIText.text += "Water: " + m_Stats[1, 0] + '\n';
        m_GUIText.text += "Heat: " + m_Stats[2, 0];
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
