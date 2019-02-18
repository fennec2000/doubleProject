using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETiles { unset, water, sand, grass, any }

public struct STile
{
	public Vector2Int m_Position;
	public GameObject m_GameObject;
	public ETiles m_TileType;
}

public class MapObject : MonoBehaviour
{
	const int tileSize = 10;

	Vector2 m_Position;
	STile[,] m_Tiles;
	MapObject[] m_Neighbours;       // 8 directions clockwise, 0 north -> 7 north west

	public Vector2 GetPosition() { return m_Position; }

	public STile GetTile(Vector2Int pos) { return m_Tiles[pos.x, pos.y]; }

	public MapObject[] GetNeighbours() { return m_Neighbours; }

	public void SetNeighbour(MapObject given, int direction)
	{
		if (m_Neighbours.Length > direction)
			m_Neighbours[direction] = given;
	}

	public MapObject(Vector2 initPos, ETiles[,] givenTiles, MapObject[] initNeighbours, GameObject tilePrefab, Material[] materials, GameObject givenParent)
	{
		m_Position = initPos;

		// tiles
		m_Tiles = new STile[tileSize, tileSize];
		for(int i = 0; i < tileSize; ++i)
		{
			for (int j = 0; j < tileSize; ++j)
			{
				// position
				m_Tiles[i, j].m_Position = new Vector2Int(i, j);
				// type
				m_Tiles[i, j].m_TileType = givenTiles[i, j];
				// gameobject
				m_Tiles[i, j].m_GameObject = Instantiate(tilePrefab, new Vector3(initPos.x + i, 0, initPos.y + j), Quaternion.identity, givenParent.transform);

				// update colour
				switch (m_Tiles[i, j].m_TileType)
				{
					case ETiles.water:
						m_Tiles[i, j].m_GameObject.GetComponent<Renderer>().material = materials[(int)ETiles.water];
						break;
					case ETiles.sand:
						m_Tiles[i, j].m_GameObject.GetComponent<Renderer>().material = materials[(int)ETiles.sand];
						break;
					case ETiles.grass:
						m_Tiles[i, j].m_GameObject.GetComponent<Renderer>().material = materials[(int)ETiles.grass];
						break;
					default:
						break;
				}
			}
		}

		// neighbours
		m_Neighbours = initNeighbours;

		for (int i = 0; i < 8; ++i)
			if (m_Neighbours[i] != null)
				m_Neighbours[i].SetNeighbour(this, (i < 4) ? i + 4 : i - 4);
	}
}

public class MapSpawner : MonoBehaviour {

	[SerializeField]
	GameObject m_TilePrefab;

	[SerializeField]
	Material[] m_TileMaterials;

	[SerializeField]
	GameObject m_CreaturePrefab;

	[SerializeField]
	GameObject m_CreatureMapObj;

	List<MapObject> m_TileList = new List<MapObject>();
	List<CreatureAI> CreatureAIs = new List<CreatureAI>();

	// Use this for initialization
	void Start () {
		var mapGO = GameObject.Find("Map");

		// 4 x 4 map objects
		ETiles[,] waterTiles = new ETiles[,]	// 10x10 water tieles
		{
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water },
			{ ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water, ETiles.water }
		};
		ETiles[,] grassTiles = new ETiles[,]	// 10x10 water tieles
		{
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass },
			{ ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass, ETiles.grass }
		};

		const int mapSize = 4;
		MapObject tile;

		for(int i = 0; i < mapSize; ++i)
		{
			for(int j = 0; j < mapSize; ++j)
			{
				if (i == 0 || i == mapSize - 1 || j == 0 || j == mapSize - 1)
					tile = new MapObject(new Vector2(i*10, j*10), waterTiles, GetNeighbours(new Vector2(i, j)), m_TilePrefab, m_TileMaterials, mapGO);
				else
					tile = new MapObject(new Vector2(i*10, j*10), grassTiles, GetNeighbours(new Vector2(i, j)), m_TilePrefab, m_TileMaterials, mapGO);
				m_TileList.Add(tile);
			}
		}

		// spawns
		var spawned = Instantiate(m_CreaturePrefab, new Vector3(20, 0, 20), Quaternion.identity, m_CreatureMapObj.transform).GetComponent<CreatureAI>();
		spawned.Setup(null, this);
		CreatureAIs.Add(spawned);
	}

	MapObject GetMapObjectPos(Vector2 pos)
	{
		foreach (var mo in m_TileList)
			if (mo.GetPosition() == pos)
				return mo;
		return null;
	}

	MapObject[] GetNeighbours(Vector2 pos)
	{
		var x = pos.x;
		var y = pos.y;
		return new MapObject[] {
			GetMapObjectPos(new Vector2(x-1, y+1)), GetMapObjectPos(new Vector2(x, y+1)), GetMapObjectPos(new Vector2(x+1, y+1)),
			GetMapObjectPos(new Vector2(x-1, y)),										  GetMapObjectPos(new Vector2(x+1, y)),
			GetMapObjectPos(new Vector2(x-1, y-1)), GetMapObjectPos(new Vector2(x, y-1)), GetMapObjectPos(new Vector2(x+1, y-1))
		};
	}

	// Update is called once per frame
	void Update () {
		
	}

	public STile[] GetFirstTarget(Vector3 pos)
	{
		var x = Mathf.FloorToInt(pos.x / 10);
		var areaX = x / 10;
		var gridX = x - areaX;

		var y = Mathf.FloorToInt(pos.z / 10);
		var areaY = y / 10;
		var gridY = y - areaX;

		var result = new STile[2];

		Vector2Int areaPos = new Vector2Int(areaX, areaY);

		// old
		MapObject area;
		for (int i = 0; i < m_TileList.Count; ++i)
		{
			area = m_TileList[i];
			if (area.GetPosition() == areaPos)
			{
				result[1] = area.GetTile(areaPos);
				var rand = Random.Range(0,7);
				result[0] = area.GetTile(new Vector2Int(rand / 3 - 1, rand % 3 - 1));
				break;
			}
		}



		return result;
	}

	public STile GetNewTarget(STile current, STile old) // TODO update to use STile
	{
		// rand direction
		int rand = Random.Range(1, 100);

		if (rand == 1)  // back
			return old;

		// find dirrections
		var n = old.GetNeighbours();
		int i = 0; 
		while (i <= n.Length)
		{
			if (n[i] == old)
				break;

			if (i == n.Length)
				i = -1;		// not found old tile is not next to current tile

			++i;
		}


		if (rand > 1 && rand <= 5)	// back left
			return n[(i + 1) % 8];

		else if (rand > 9 && rand <= 24) // left
			return n[(i + 2) % 8];

		else if (rand > 39 && rand <= 57) // forward left
			return n[(i + 3) % 8];

		else if (rand > 57 && rand <= 75) // forward right
			return n[(i + 5) % 8];

		else if (rand > 24 && rand <= 39) // right
			return n[(i + 6) % 8];

		else if (rand > 5 && rand <= 9) // back right
			return n[(i + 7) % 8];

		else // 75 > 100 // forward
			return n[(i + 4) % 8];
	}
}
