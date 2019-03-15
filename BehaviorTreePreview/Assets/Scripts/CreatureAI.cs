using BehaviorTreeTest;
using System.Collections.Generic;
using UnityEngine;

/*
 *  TODO List
 *  creatures remember last food and water
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
	[SerializeField]
	private Core m_Core;

	private BehaviourTree m_Behaviour;
	private CCreatureStats m_Stats;

	private STile m_Target;
	private STile m_TargetLast;
	private MapSpawner m_Map;
	[SerializeField]
	private Rigidbody m_Rigidbody;
	[SerializeField]
	private Material m_DeadMaterial;
	[SerializeField]
	private TextMesh m_Text;
	[SerializeField]
	private AudioSource m_Audio;

	private float m_TargetDistance = 0.2f;
	private float m_Speed = 1.0f;
	private float m_ViewDistance = 3.0f;
	private byte m_Drain = 1;
	private byte m_Gain = 10;
	private byte[] m_StatBoundrys = new byte[3] { 77, 128, 179 };

	private float m_Timer = 0;
	private float m_DrainTimer = 0;

	public enum ECreatureState { Normal, Dead }
	private ECreatureState m_State = ECreatureState.Normal;

	internal BehaviourTree Behaviour { get => m_Behaviour; set => m_Behaviour = value; }

	public (CCreatureStats, ECreatureState) GetCreatureStats() { return (m_Stats, m_State); }

	public void Setup(CCreatureStats parent, MapSpawner map, Core core)
	{
		m_Core = core;
		m_Map = map;
		CreateSurviveTree();
		m_Stats = new CCreatureStats(parent);
		var result = m_Map.GetFirstTarget(transform.position);
		m_Target = result[0];
		m_TargetLast = result[1];
		gameObject.SetActive(true);
	}

	private void CreateSurviveTree()
	{
		m_Behaviour = new BehaviourTree();

		// general actions
		var actionFood = m_Behaviour.CreateActionNode(EatFood);
		var actionWater = m_Behaviour.CreateActionNode(DrinkWater);
		var move = m_Behaviour.CreateActionNode(CreatureMoving);
		var newTarget = m_Behaviour.CreateActionNode(NewTarget);

		// medium water
		var waterMed = m_Behaviour.CreateActionNode(WaterMed);
		var waterBelowMed = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, waterMed);

		var findWater = m_Behaviour.CreateActionNode(TargetWater);
		var invertFindWater = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, findWater);
		var targetWater = m_Behaviour.CreateActionNode(IsTargetNearWater);
		var invertTargetWater = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, targetWater);

		var medWaterFindWaterSeqList = new List<uint> { targetWater, invertFindWater };
		var medWaterFindWaterSeq = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Selector, medWaterFindWaterSeqList);

		var medWaterSeqList = new List<uint> { waterBelowMed, medWaterFindWaterSeq, move, actionWater };
		var medWaterSeq = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Sequence, medWaterSeqList);


		// medium food
		var foodMed = m_Behaviour.CreateActionNode(FoodMed);
		var foodBelowMed = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, foodMed);

		var findFood = m_Behaviour.CreateActionNode(TargetFood);
		var invertFindFood = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, findFood);
		var targetFood = m_Behaviour.CreateActionNode(IsTargetFood);
		var invertTargetFood = m_Behaviour.CreateDecoratorNode(DecoratorNodeType.Inverter, targetFood);

		var medFoodFindFoodSeqList = new List<uint> { targetFood, invertFindFood };
		var medFoodFindFoodSeq = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Selector, medFoodFindFoodSeqList);

		var medFoodSeqList = new List<uint> { foodBelowMed, medFoodFindFoodSeq, move, actionFood };
		var medFoodSeq = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Sequence, medFoodSeqList);

		// Explore
		var exploreList = new List<uint> { move, newTarget };
		var explore = m_Behaviour.CreateCompositeNode(CompositeNodeTypes.Sequence, exploreList);

		// top level
		var children = new List<uint> { medWaterSeq, medFoodSeq, explore };
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
		if (m_State == ECreatureState.Normal)
		{
			if (m_Core.GameSpeed == 0)
				return;

			// update move
			Move();

			m_Timer += Time.deltaTime * m_Core.GameSpeed;
			m_DrainTimer += Time.deltaTime * m_Core.GameSpeed;

			if (m_Timer >= 1 / m_Core.AIUpdates)
			{
				m_Behaviour.RunTree();
				m_Timer = 0;
			}

			if (m_DrainTimer >= 1)
			{
				m_DrainTimer = 0;

				// Drain
				if (m_Stats.Water > m_Drain)
					m_Stats.Water -= m_Drain;
				else
				{
					m_Stats.Water = 0;
					m_State = ECreatureState.Dead;

					var rend = GetComponentInChildren<Renderer>();
					if (rend != null)
						rend.material = m_DeadMaterial;
				}

				if (m_Stats.Food > m_Drain)
					m_Stats.Food -= m_Drain;
				else
				{
					m_Stats.Food = 0;
					m_State = ECreatureState.Dead;

					var rend = GetComponentInChildren<Renderer>();
					if (rend != null)
						rend.material = m_DeadMaterial;
				}
			}
		}
		
	}

	private ENodeStates CreatureMoving()
	{
		if (m_Target.m_GameObject != null)
		{
			if (Vector3.Distance(transform.position, m_Target.m_GameObject.transform.position) > m_TargetDistance * m_Core.GameSpeed)
				return ENodeStates.RUNNING;

			return ENodeStates.SUCCESS;
		}

		return ENodeStates.FAILURE;
	}

	void Move()
	{
		if (m_Target.m_GameObject != null)
		{
			if (Vector3.Distance(transform.position, m_Target.m_GameObject.transform.position) > m_TargetDistance * m_Core.GameSpeed)
			{
				// debug
				Debug.DrawLine(transform.position, m_Target.m_GameObject.transform.position);

				// look at
				transform.LookAt(m_Target.m_GameObject.transform);

				// move to target
				Vector3 vec = m_Target.m_GameObject.transform.position - transform.position;
				var dist = vec.normalized * m_Speed * Time.deltaTime * m_Core.GameSpeed;

				if (dist.magnitude > vec.magnitude)
					transform.position += vec;
				else
					transform.position += dist;
			}
			else
			{
				transform.position = m_Target.m_GameObject.transform.position;
			}
		}
	}

	private ENodeStates TargetFood()
	{
		Vector2 dirrectionVector = new Vector2(transform.forward.x, transform.forward.z);
		var result = m_Map.CreatureTileVision(new Vector2(transform.position.x, transform.position.z), dirrectionVector.normalized * m_ViewDistance);

		for(int i = 0; i < result.Count; ++i)
		{
			if(result[i].m_TileType == ETiles.grass)
			{
				m_Target = result[i];
				return ENodeStates.SUCCESS;
			}
		}

		return ENodeStates.FAILURE;
	}

	private ENodeStates TargetWater()
	{
		Vector2 dirrectionVector = new Vector2(transform.forward.x, transform.forward.z);
		var result = m_Map.CreatureTileVision(new Vector2(transform.position.x, transform.position.z), dirrectionVector.normalized * m_ViewDistance);

		for (int i = 0; i < result.Count; ++i)
		{
			if (result[i].m_TileType == ETiles.grass && m_Map.GetNeighbourWater(m_Target))
			{
				m_Target = result[i];
				return ENodeStates.SUCCESS;
			}
		}

		return ENodeStates.FAILURE;
	}

	private ENodeStates NewTarget()
	{
		// new target
		var result = m_Map.GetNewTarget(m_Target, m_TargetLast);
		m_TargetLast = m_Target;
		m_Target = result;

		CreatureSpeak("Exploring");

		return ENodeStates.SUCCESS;
	}

	private ENodeStates EatFood()
	{
		if (Vector3.Distance(transform.position, m_Target.m_GameObject.transform.position) <= m_TargetDistance)
		{
			if (m_Target.m_TileType == ETiles.grass)
			{
				m_Map.ChangeTile(m_Target, ETiles.sand);

				m_Stats.Food += m_Gain;
			}
		}

		return ENodeStates.FAILURE;
	}

	private ENodeStates DrinkWater()
	{
		if (Vector3.Distance(transform.position, m_Target.m_GameObject.transform.position) <= m_TargetDistance)
		{
			if (m_Map.GetNeighbourWater(m_Target))
			{
				m_Stats.Water += m_Gain;
				return ENodeStates.SUCCESS;
			}
		}

		return ENodeStates.FAILURE;
	}

	private ENodeStates FoodHigh()
	{
		return m_Stats.Food >= m_StatBoundrys[2] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates FoodMed()
	{
		if (m_Stats.Food < m_StatBoundrys[1])
			CreatureSpeak("Hungry");

		return m_Stats.Food >= m_StatBoundrys[1] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates FoodLow()
	{
		return m_Stats.Food >= m_StatBoundrys[0] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}

	private ENodeStates WaterHigh()
	{
		return m_Stats.Water >= m_StatBoundrys[2] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates WaterMed()
	{
		if (m_Stats.Water < m_StatBoundrys[1])
			CreatureSpeak("Thirsty");

		return m_Stats.Water >= m_StatBoundrys[1] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}
	private ENodeStates WaterLow()
	{
		return m_Stats.Water >= m_StatBoundrys[0] ? ENodeStates.SUCCESS : ENodeStates.FAILURE;
	}

	private ENodeStates IsTargetFood()
	{
		if (m_Target.m_TileType == ETiles.grass)
			return ENodeStates.SUCCESS;
		return ENodeStates.FAILURE;
	}

	private ENodeStates IsTargetNearWater()
	{
		if (m_Map.GetNeighbourWater(m_Target))
			return ENodeStates.SUCCESS;
		return ENodeStates.FAILURE;
	}

	void CreatureSpeak(string message)
	{
		// check if there is a update in text
		if (m_Text.text == message)
			return;

		// play audio if not speaking
		if (!m_Audio.isPlaying)
			m_Audio.Play();

		// update text
		m_Text.text = message;
	}
}
