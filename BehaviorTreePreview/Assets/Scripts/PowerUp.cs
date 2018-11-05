using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float[,] mapSize = { {-49, 49 }, { 0.5f, 0.5f }, { -49, 49 } };
    [SerializeField]
    private EStatsTypes myPowerType;

    [SerializeField]
    private int rotateSpeed = 90;


    internal EStatsTypes MyPowerType
    {
        get
        {
            return myPowerType;
        }

        set
        {
            myPowerType = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        var shader = Shader.Find("Diffuse");
        Renderer rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("_Color");

        switch (MyPowerType)
        {
            case EStatsTypes.Food:
                rend.material.color = Color.green;
                break;
            case EStatsTypes.Water:
                rend.material.color = Color.blue;
                break;
            case EStatsTypes.Heat:
                rend.material.color = Color.red;
                break;
        }
        rend.material.shader = shader;

        Vector3 newPos = new Vector3( Random.Range(mapSize[0,0], mapSize[0,1]),
            Random.Range(mapSize[1, 0], mapSize[1, 1]),
            Random.Range(mapSize[2, 0], mapSize[2, 1]));
        this.transform.position = newPos;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 newPos = new Vector3(Random.Range(mapSize[0, 0], mapSize[0, 1]),
            Random.Range(mapSize[1, 0], mapSize[1, 1]),
            Random.Range(mapSize[2, 0], mapSize[2, 1]));
        this.transform.position = newPos;

        if(collision.gameObject.GetComponent<Think>() != null)
            collision.gameObject.GetComponent<Think>().AddPower(MyPowerType, Random.Range(5, 40));
    }
}
