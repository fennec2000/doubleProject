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

public class MapObject : ScriptableObject
{
	const int tileSize = 10;

	Vector2 m_Position;
	STile[,] m_Tiles;
	MapObject[] m_Neighbours;       // 8 directions clockwise, 0 north -> 7 north west

	public Vector2 GetPosition() { return m_Position; }

	public STile GetTile(Vector2Int pos)
	{
		// get area and pos in area
		int x = 0;
		int y = 0;
		while (pos.x > tileSize - 1) { pos.x -= tileSize; ++x; }
		while (pos.y > tileSize - 1) { pos.y -= tileSize; ++y; }
		while (pos.x < 0) { pos.x += tileSize; --x; }
		while (pos.y < 0) { pos.y += tileSize; --y; }

		return ReturnTile(new Vector2Int(x, y), pos);
	}

	public STile ReturnTile(Vector2Int area, Vector2Int pos)
	{
		if (area.x > 0 && m_Neighbours[2] != null)
		{
			--area.x;
			return m_Neighbours[2].ReturnTile(area, pos);
		}

		if (area.x < 0 && m_Neighbours[6] != null)
		{
			++area.x;
			return m_Neighbours[6].ReturnTile(area, pos);
		}

		if (area.y > 0 && m_Neighbours[0] != null)
		{
			--area.y;
			return m_Neighbours[0].ReturnTile(area, pos);
		}

		if (area.y < 0 && m_Neighbours[4] != null)
		{
			++area.y;
			return m_Neighbours[4].ReturnTile(area, pos);
		}

		return m_Tiles[pos.x, pos.y];
	}

	public MapObject[] GetNeighbours() { return m_Neighbours; }

	public void SetNeighbour(MapObject given, int direction)
	{
		if (m_Neighbours.Length > direction)
			m_Neighbours[direction] = given;
	}

	public bool ContainsTile(STile tile)
	{
		return (m_Tiles[tile.m_Position.x, tile.m_Position.y].Equals(tile)) ? true : false;
	}

	public List<STile> GetTileNeighbours(STile tile) 
	{
		// 8 directions clockwise, 0 north -> 7 north west
		var result = new List<STile>(); 
		bool[] dirChecks = new bool[4]; // NESW

		// grid is tileSize x tileSize
		if (tile.m_Position.y < tileSize - 1)
			dirChecks[0] = true;
		if (tile.m_Position.x < tileSize - 1)
			dirChecks[1] = true;
		if (tile.m_Position.y > 0)
			dirChecks[2] = true;
		if (tile.m_Position.x > 0)
			dirChecks[3] = true;

		if (dirChecks[0] && dirChecks[1] && dirChecks[2] && dirChecks[3])
		{
			return new List<STile>() {
				m_Tiles[tile.m_Position.x,		tile.m_Position.y + 1],
				m_Tiles[tile.m_Position.x + 1,	tile.m_Position.y + 1],
				m_Tiles[tile.m_Position.x + 1,	tile.m_Position.y	 ],
				m_Tiles[tile.m_Position.x + 1,	tile.m_Position.y - 1],
				m_Tiles[tile.m_Position.x,		tile.m_Position.y - 1],
				m_Tiles[tile.m_Position.x - 1,	tile.m_Position.y - 1],
				m_Tiles[tile.m_Position.x - 1,	tile.m_Position.y	 ],
				m_Tiles[tile.m_Position.x - 1,	tile.m_Position.y + 1]
			};
		}
		
		else
		{
			// 0 1
			if (dirChecks[0])
			{
				result.Add(m_Tiles[tile.m_Position.x, tile.m_Position.y + 1]);

				if (dirChecks[1])
				{
					result.Add(m_Tiles[tile.m_Position.x + 1, tile.m_Position.y + 1]);
				}
				else
				{
					result.Add(m_Neighbours[2].m_Tiles[0, tile.m_Position.y + 1]);
				}
			}
			else
			{
				result.Add(m_Neighbours[0].m_Tiles[tile.m_Position.x, 0]);

				if (dirChecks[1])
				{
					result.Add(m_Neighbours[0].m_Tiles[tile.m_Position.x + 1, 0]);
				}
				else
				{
					result.Add(m_Neighbours[1].m_Tiles[0, 0]);
				}
			}

			// 2 3
			if (dirChecks[1])
			{
				result.Add(m_Tiles[tile.m_Position.x + 1, tile.m_Position.y]);

				if (dirChecks[2])
				{
					result.Add(m_Tiles[tile.m_Position.x + 1, tile.m_Position.y - 1]);
				}
				else
				{
					result.Add(m_Neighbours[4].m_Tiles[tile.m_Position.x + 1, tileSize - 1]);
				}

			}
			else
			{
				result.Add(m_Neighbours[2].m_Tiles[0, tile.m_Position.y]);

				if (dirChecks[2])
				{
					result.Add(m_Neighbours[2].m_Tiles[0, tile.m_Position.y - 1]);
				}
				else
				{
					result.Add(m_Neighbours[3].m_Tiles[0, tileSize - 1]);
				}
			}

			// 4 5
			if (dirChecks[2])
			{
				result.Add(m_Tiles[tile.m_Position.x, tile.m_Position.y - 1]);

				if (dirChecks[3])
				{
					result.Add(m_Tiles[tile.m_Position.x - 1, tile.m_Position.y - 1]);
				}
				else
				{
					result.Add(m_Neighbours[6].m_Tiles[tileSize - 1, tile.m_Position.y - 1]);
				}
			}
			else
			{
				result.Add(m_Neighbours[4].m_Tiles[tile.m_Position.x, tileSize - 1]);

				if (dirChecks[3])
				{
					result.Add(m_Neighbours[4].m_Tiles[tile.m_Position.x - 1, tileSize - 1]);
				}
				else
				{
					result.Add(m_Neighbours[5].m_Tiles[tileSize - 1, tileSize - 1]);
				}
			}

			// 6 7
			if (dirChecks[3])
			{
				result.Add(m_Tiles[tile.m_Position.x - 1, tile.m_Position.y]);

				if (dirChecks[0])
				{
					result.Add(m_Tiles[tile.m_Position.x - 1, tile.m_Position.y + 1]);
				}
				else
				{
					result.Add(m_Neighbours[0].m_Tiles[tile.m_Position.x - 1, 0]);
				}
			}
			else
			{
				result.Add(m_Neighbours[6].m_Tiles[tileSize - 1, tile.m_Position.y]);

				if (dirChecks[0])
				{
					result.Add(m_Neighbours[6].m_Tiles[tileSize - 1, tile.m_Position.y + 1]);
				}
				else
				{
					result.Add(m_Neighbours[7].m_Tiles[tileSize - 1, 0]);
				}
			}
		}

		return result;
	}

	public MapObject(Vector2 initPos, ETiles[,] givenTiles, MapObject[] initNeighbours, GameObject tilePrefab, Material[] materials, GameObject givenParent)
	{
		GameObject newGO = new GameObject("Area");
		newGO.transform.SetParent(givenParent.transform);
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
				m_Tiles[i, j].m_GameObject = Instantiate(tilePrefab, new Vector3(initPos.x + i, 0, initPos.y + j), Quaternion.identity, newGO.transform);

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
	Core m_Core;

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
		spawned.Setup(null, this, m_Core);
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
		var x = pos.x * 10;
		var y = pos.y * 10;
		return new MapObject[] {
			GetMapObjectPos(new Vector2(x, y+10)),
			GetMapObjectPos(new Vector2(x+10, y+10)),
			GetMapObjectPos(new Vector2(x+10, y)),
			GetMapObjectPos(new Vector2(x+10, y-10)),
			GetMapObjectPos(new Vector2(x, y-10)),
			GetMapObjectPos(new Vector2(x-10, y-10)),
			GetMapObjectPos(new Vector2(x-10, y)),
			GetMapObjectPos(new Vector2(x-10, y+10))
		};
	}

	// Update is called once per frame
	void Update () {
		
	}

	public STile[] GetFirstTarget(Vector3 pos)
	{
		// get section
		var x = Mathf.FloorToInt(pos.x);
		var areaX = (x / 10) * 10;
		var gridX = x - areaX;

		var y = Mathf.FloorToInt(pos.z);
		var areaY = (y / 10) * 10;
		var gridY = y - areaX;

		Vector2Int areaPos = new Vector2Int(areaX, areaY);

		// gerate results
		var result = new STile[2];
		MapObject area;

		for (int i = 0; i < m_TileList.Count; ++i)
		{
			area = m_TileList[i];
			if (area.GetPosition() == areaPos)
			{
				result[1] = area.GetTile(new Vector2Int(gridX, gridY));
				var rand = Random.Range(0,7);
				var nextTarget = new Vector2Int(gridX + rand / 3 - 1, gridY + rand % 3 - 1);
				result[0] = area.GetTile(nextTarget);
				break;
			}
		}

		return result;
	}

	public MapObject GetArea(Vector2 pos)
	{
		pos.x = Mathf.FloorToInt(pos.x / 10 * 10);
		pos.y = Mathf.FloorToInt(pos.y / 10 * 10);
		MapObject area;

		for (int i = 0; i < m_TileList.Count; ++i)
		{
			area = m_TileList[i];
			if (area.GetPosition() == pos)
			{
				return area;
			}
		}

		Debug.Log("GetArea Could not find area.");
		return m_TileList[0];
	}

	public STile GetNewTarget(STile current, STile old) // TODO update to use STile
	{
		STile result = old;

		// rand direction
		int rand = Random.Range(1, 100);

		MapObject currentMapObject = null;
		var numOfTiles = m_TileList.Count;
		for (int i = 0; i < numOfTiles; ++i)
		{
			if (m_TileList[i].ContainsTile(current))
			{
				currentMapObject = m_TileList[i];
				break;
			}
		}
		

		var n = currentMapObject.GetTileNeighbours(current);
		int p = 0;	// the position / forward direction

		for (int i = 0; i < 8; ++i) // only ever 8 neighbours
			if (n[i].Equals(old))
				p = 4 + i;
		if (rand > 75 && rand <= 100) // forward
			result = currentMapObject.GetTile(current.m_Position + current.m_Position - old.m_Position);

		else if (rand > 57 && rand <= 75) // forward right
			result = n[(p + 1) % 8];

		else if (rand > 24 && rand <= 39) // right
			result = n[(p + 2) % 8];

		else if (rand > 5 && rand <= 9) // back right
			result = n[(p + 3) % 8];

		// back is old and is done asap

		else if (rand > 1 && rand <= 5) // back left
			result = n[(p + 5) % 8];

		else if (rand > 9 && rand <= 24) // left
			result = n[(p + 6) % 8];

		else if (rand > 39 && rand <= 57) // forward left
			result = n[(p + 7) % 8];

		else
		{
			// something went wrong
			Debug.Log("Get new target didnt get return dirrection");
			return current;
		}

		// if its not water its fine
		if (result.m_TileType != ETiles.water)
			return result;

		// if its water find a non water tile
		List<STile> nonWaterTiles = new List<STile>();
		for(int i = 0; i < n.Count; ++i)
		{
			if (n[i].m_TileType != ETiles.water)
				nonWaterTiles.Add(n[i]);
		}

		if (nonWaterTiles.Count > 0)
		{
			rand = Random.Range(0, nonWaterTiles.Count);
			return nonWaterTiles[rand];
		}
		else
			return old;
	}

	public List<STile> CreatureTileVision(Vector2 pos, Vector2 visionVector)
	{
		var correctedPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
		var visionEnd = new Vector2Int(Mathf.RoundToInt(pos.x + visionVector.x), Mathf.RoundToInt(pos.y + visionVector.y));
		var result = new List<STile>();

		var area = GetArea(new Vector2(0, 0));

		var iMax = Mathf.Max(correctedPos.x, visionEnd.x);
		var jMax = Mathf.Max(correctedPos.y, visionEnd.y);

		for (int i = Mathf.Min(correctedPos.x, visionEnd.x); i < iMax; ++i)
		{
			for (int j = Mathf.Min(correctedPos.y, visionEnd.y); j < jMax; ++j)
			{
				result.Add(area.GetTile(new Vector2Int(i, j)));
			}
		}

		return result;
	}

	public void ChangeTile(STile tile, ETiles newTileState)
	{
		tile.m_TileType = newTileState;

		// update colour
		switch (newTileState)
		{
			case ETiles.water:
				tile.m_GameObject.GetComponent<Renderer>().material = m_TileMaterials[(int)ETiles.water];
				break;
			case ETiles.sand:
				tile.m_GameObject.GetComponent<Renderer>().material = m_TileMaterials[(int)ETiles.sand];
				break;
			case ETiles.grass:
				tile.m_GameObject.GetComponent<Renderer>().material = m_TileMaterials[(int)ETiles.grass];
				break;
			default:
				break;
		}
	}

	public bool GetNeighbourWater(STile givenTile)
	{
		// Get map
		MapObject currentMapObject = null;
		var numOfTiles = m_TileList.Count;
		for (int i = 0; i < numOfTiles; ++i)
		{
			if (m_TileList[i].ContainsTile(givenTile))
			{
				currentMapObject = m_TileList[i];
				break;
			}
		}

		// get neighbours
		var n = currentMapObject.GetTileNeighbours(givenTile);

		// find water
		foreach(var tile in n)
		{
			if (tile.m_TileType == ETiles.water)
				return true;
		}

		return false;
	}
}
