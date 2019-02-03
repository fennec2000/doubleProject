using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ETiles { unset, water, sand, grass }

class MapObject : MonoBehaviour
{
	const int tileSize = 10;

	Vector2 m_Position;
	GameObject[,] m_TileGO;
	ETiles[,] m_TileData;
	MapObject[] m_Neighbours;       // 8 directions clockwise, 0 north -> 7 north west

	MapObject(Vector2 initPos, ETiles[,] givenTiles, MapObject[] initNeighbours, GameObject tilePrefab, Material[] materials)
	{
		m_Position = initPos;
		m_TileData = givenTiles;
		m_TileGO = new GameObject[tileSize, tileSize];
		m_Neighbours = initNeighbours;

		for(int i = 0; i < tileSize; ++i)
		{
			for (int j = 0; j < tileSize; ++j)
			{
				m_TileGO[i, j] = Instantiate(tilePrefab, initPos + new Vector2(i, j), Quaternion.identity);

				switch (m_TileData[i,j])
				{
					case ETiles.water:
						m_TileGO[i, j].GetComponent<Renderer>().material = materials[(int)ETiles.water];
						break;
					case ETiles.sand:
						m_TileGO[i, j].GetComponent<Renderer>().material = materials[(int)ETiles.sand];
						break;
					case ETiles.grass:
						m_TileGO[i, j].GetComponent<Renderer>().material = materials[(int)ETiles.grass];
						break;
					default:
						break;
				}
				
			}
		}
	}
}

public class MapSpawner : MonoBehaviour {

	[SerializeField]
	GameObject m_TilePrefab;

	[SerializeField]
	Material[] m_TileMaterials;

	List<MapObject> m_TileList = new List<MapObject>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
