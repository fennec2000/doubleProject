using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MapObject
{
    public GameObject myObject;
    public int life;
    public MapObject()
    {
        myObject = null;
        life = 0;
    }
}

public class MapSpawner : MonoBehaviour {

    private MapObject[,] MapArray;
    [SerializeField]
    private Transform grass;

	// Use this for initialization
	void Start () {
        MapArray = new MapObject[7, 7];
        for(int i = 0; i < 7; ++i)
        {
            int x, z;
            do
            {
                x = Random.Range(0, 7);
                z = Random.Range(0, 7);
            } while (MapArray[x, z] != null);

            var obj = new MapObject();
            obj.myObject = Instantiate(grass, new Vector3(x*100 - 350, 0.5f, z*100 - 350), new Quaternion(), gameObject.transform).gameObject;
            obj.life = 100;
            MapArray[x,z] = obj;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
