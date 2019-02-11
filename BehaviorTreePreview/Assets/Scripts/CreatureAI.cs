using BehaviorTreeTest;
using System.Collections.Generic;
using UnityEngine;

/*
 *  TODO List
 *  have creatures walk
 *  have creatures obay water / land
 *  creatures can consume resources
 *  creatures remember last food and water
 *  creature seek out resouce when needed
 *  creature reporduce
 *  creatures get trates for parents
 *  move takes trates into account
 *  add hot and cold inc tiles
 *  add mutations
 */

public class CCreatureStats
{
	private readonly CCreatureStats m_ParentsTraits;    // Trates given to child
	private ushort m_Age = 0;           // Age of creature

	// high 179 med 128 low 77
	private byte m_Food = 200;          // starts loosing mass at 0, starts increasing mass at 255
	private ushort m_Mass = 1000;       // current mass +- 50% of massTrait death heart attack/starvation
	private byte m_Water = 200;         // dies at no water

	private byte m_LandWaterRatio;      // ratio of time spend on land to water, can only move 15% from parent

	public byte Food {
		get { return m_Food; }
		set { m_Food = value; }
	}
	public byte Water{
		get { return m_Water; }
		set { m_Water = value; }
	}

	// requires 10% to swim or walk on land more that 1 from shore
	private CCreatureStats() { m_LandWaterRatio = 127; }

	public CCreatureStats(CCreatureStats parent)
	{
		m_ParentsTraits = new CCreatureStats();
		if (parent != null)	// deep copy
		{
			m_ParentsTraits.m_Age = parent.m_Age;
			m_ParentsTraits.Food = parent.Food;
			m_ParentsTraits.m_Mass = parent.m_Mass;
			m_ParentsTraits.m_Water = parent.m_Water;
			m_ParentsTraits.m_LandWaterRatio = parent.m_LandWaterRatio;
		}

		// start with parent trates
		m_Mass = m_ParentsTraits.m_Mass;
		m_LandWaterRatio = m_ParentsTraits.m_LandWaterRatio;
	}

	
}

public class CreatureAI : MonoBehaviour
{
	// creature thinkin
	// < 70 could get water, < 50 should get water < 30 must get water
	// < 70 could get food, < 50 should get food < 30 must get food
	// creature is asexual
	// creature gets traits from parent
	// traits carnivor/herbivor, land/water (time limmited if < 33%), mass (parents avg. mass)
	// traits can only go 10% from given by parent

	private BehaviourTree m_Behaviour;
	private CCreatureStats m_Stats;

	private MapObject m_Target;
	private MapObject m_TargetLast;
	private MapSpawner m_Map;

	private float m_TargetDistance = 1.0f;

	public void Setup(CCreatureStats parent, MapSpawner map)
	{
		m_Map = map;
		m_Behaviour = new BehaviourTree("Survive.bt");
		m_Stats = new CCreatureStats(parent);
		gameObject.SetActive(true);
	}

	private void CreateSurviveTree()
	{
		m_Behaviour = new BehaviourTree();

		var move = m_Behaviour.CreateActionNode(Move);
		var action = m_Behaviour.CreateActionNode(Action);

		var children = new List<uint> { move, action };
		var root = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Selector, children);
		m_Behaviour.SetRootNode(root);

		m_Behaviour.SaveTree("Survive.bt");
	}

	// Start is called before the first frame update
	private void Start()
	{
	}

	// Update is called once per frame
	private void Update()
	{
		m_Behaviour.RunTree();
	}

	private ENodeStates Move()
	{
		if (m_Target == null)
		{
			// first target
			m_Map.GetFirstTarget();
		}
		else if (Vector3.Distance(transform.position, m_Target.transform.position) < m_TargetDistance)
		{
			// move to target
		}
		else
		{
			// new target
			var result = m_Map.GetNewTarget(m_Target, m_TargetLast);
			m_TargetLast = m_Target;
			m_Target = result;
		}

		return ENodeStates.FAILURE;
	}

	private ENodeStates Action()
	{
		return ENodeStates.FAILURE;
	}

	private ENodeStates FoodOver(byte value)
	{
		return m_Stats.Food >= value ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}

	private ENodeStates FoodHigh()
	{
		return m_Stats.Food >= 179 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates FoodMed()
	{
		return m_Stats.Food >= 128 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates FoodLow()
	{
		return m_Stats.Food >= 77 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}

	private ENodeStates WaterHigh()
	{
		return m_Stats.Water >= 179 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates WaterMed()
	{
		return m_Stats.Water >= 128 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates WaterLow()
	{
		return m_Stats.Water >= 77 ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
}
