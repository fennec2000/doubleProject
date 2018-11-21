using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct SLiveTreeNode
{
    public GameObject nodeObject;
    public GameObject nodeObjectText;
    public STreeNode node;
};

public class TreeVisual : MonoBehaviour {

    private GameObject target;
    private bool haveTarget;
    private Camera myCamera;
    private RectTransform contentRec;
    [SerializeField]
    public GameObject LiveTreeOverlay;
    public Texture2D SquareTex;
    public Texture2D DiamondTex;
    public Texture2D CircleTex;
    private List<SLiveTreeNode> LiveTreeObjects;

    // Use this for initialization
    void Start () {
        LiveTreeObjects = new List<SLiveTreeNode>();
        myCamera = GetComponent<Camera>();
        contentRec = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        DisplayTree();
    }

    public void SetTargetNull()
    {
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        // raycast object and check if it has Think script, then set at target
        if (target == null && Input.GetMouseButtonUp(0)) // left click
        {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.GetComponent<Think>())
                {
                    target = objectHit.gameObject;
                    DrawTree();
                }
            }
        }

        DisplayTree();
    }

    void DisplayTree()
    {
        if (target == null)
        {
            LiveTreeOverlay.SetActive(false);
        }
        else
        {
            LiveTreeOverlay.SetActive(true);
        }
    }

    void DrawTree()
    {
        // delete children
        var children = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetComponentInChildren<Transform>();
        foreach (Transform child in children)
        {
            GameObject.Destroy(child.gameObject);
        }

        var treeList = target.GetComponent<Think>().BT.GetTree();

        contentRec.sizeDelta = new Vector2(100 * treeList.Count, 100);
        float current = 0;

        foreach(var tree in treeList)
        {
            var myObj = new SLiveTreeNode();
            myObj.nodeObject = new GameObject("Image");
            myObj.nodeObject.transform.SetParent(LiveTreeOverlay.transform.GetChild(0).GetChild(0).transform);

            myObj.nodeObjectText = new GameObject("Text");
            Text newText = myObj.nodeObjectText.AddComponent<Text>();
            myObj.nodeObjectText.transform.SetParent(myObj.nodeObject.transform);
            
            newText.text = "ID: " + tree.id + "\n" + tree.name + "\n" + tree.state;
            newText.alignment = TextAnchor.MiddleCenter;
            newText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            newText.color = Color.black;

            Image newImage = myObj.nodeObject.AddComponent<Image>();
            switch (tree.Type)
            {
                case "ActionNode":
                    newImage.sprite = Sprite.Create(CircleTex, new Rect(0.0f, 0.0f, CircleTex.width, CircleTex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                case "DecoratorNode":
                    newImage.sprite = Sprite.Create(DiamondTex, new Rect(0.0f, 0.0f, DiamondTex.width, DiamondTex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                case "CompositeNode":
                    newImage.sprite = Sprite.Create(SquareTex, new Rect(0.0f, 0.0f, SquareTex.width, SquareTex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                default:
                    break;
            }

            switch (tree.state)
            {
                case ENodeStates.FAILURE:
                    newImage.color = Color.red;
                    break;
                case ENodeStates.SUCCESS:
                    newImage.color = Color.green;
                    break;
                case ENodeStates.RUNNING:
                    newImage.color = Color.yellow;
                    break;
                default:
                    break;
            }

            myObj.nodeObject.transform.localPosition = new Vector3(50 + 100 * current, -50, 0);
            ++current;

            myObj.node = new STreeNode(tree);

            LiveTreeObjects.Add(myObj);
        }
    }
}
