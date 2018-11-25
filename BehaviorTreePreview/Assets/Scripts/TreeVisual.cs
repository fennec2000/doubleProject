﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct SLiveTreeNode
{
    public GameObject nodeObject;
    public GameObject nodeObjectText;
    public STreeNode node;
    public Vector2 size;
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
    private Vector2 shapeSize = new Vector2(100, 100);
    private Vector2 shapeSpaceSize = new Vector2(10, 10);
    private uint lastUpdate;

    // Use this for initialization
    void Start () {
        LiveTreeObjects = new List<SLiveTreeNode>();
        myCamera = GetComponent<Camera>();
        contentRec = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
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
        if (target == null && LiveTreeOverlay.activeSelf)
        {
            LiveTreeOverlay.SetActive(false);
        }
        if (target != null)
        {
            if (!LiveTreeOverlay.activeSelf)
                LiveTreeOverlay.SetActive(true);
            UpdateTree();
        }
    }

    void UpdateTree()
    {
        var treeList = target.GetComponent<Think>().BT.GetTree();

        int LTNCount = LiveTreeObjects.Count;
        for(int i = 0; i < LTNCount; ++i)
        {
            if (LiveTreeObjects[i].node.id != treeList[i].id)
                Debug.Log("Id's do not match");
            var newNode = LiveTreeObjects[i];
            newNode.node = treeList[i];

            var newText = newNode.nodeObjectText.GetComponent<Text>();
            newText.text = "ID: " + newNode.node.id + "\n" + newNode.node.name + "\n" + newNode.node.state;


            var newNodeImage = newNode.nodeObject.GetComponent<Image>();
            switch (newNode.node.state)
            {
                case ENodeStates.FAILURE:
                    newNodeImage.color = Color.red;
                    break;
                case ENodeStates.SUCCESS:
                    newNodeImage.color = Color.green;
                    break;
                case ENodeStates.RUNNING:
                    newNodeImage.color = Color.yellow;
                    break;
                default:
                    break;
            }

            LiveTreeObjects[i] = newNode;
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
                    myObj.size = shapeSize;
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

            if (tree.children.Count > 0)
            {
                List<SLiveTreeNode> ChildrenObjects = new List<SLiveTreeNode>();
                float sizecount = 0;
                float largesty = shapeSize.y;

                foreach (uint childID in tree.children)
                {
                    foreach (SLiveTreeNode NodeObject in LiveTreeObjects)
                    {
                        if (childID == NodeObject.node.id)
                        {
                            NodeObject.nodeObject.transform.SetParent(myObj.nodeObject.transform);
                            ChildrenObjects.Add(NodeObject);
                            sizecount += NodeObject.size.x;
                            if (NodeObject.size.y > largesty)
                                largesty = NodeObject.size.y;
                        }
                    }
                }

                myObj.size = new Vector2(sizecount, shapeSize.y + largesty);
                float startingPoint = sizecount * -0.5f;

                foreach (SLiveTreeNode n in ChildrenObjects)
                {
                    float localxpos = startingPoint + n.size.x / 2;
                    n.nodeObject.transform.localPosition = new Vector3(localxpos, -shapeSize.y, 0);
                    startingPoint += n.size.x;
                }
            }

            myObj.node = new STreeNode(tree);
            LiveTreeObjects.Add(myObj);
        }

        var rootnode = LiveTreeObjects[LiveTreeObjects.Count - 1];
        contentRec.sizeDelta = rootnode.size;
        rootnode.nodeObject.transform.localPosition = new Vector3(Screen.width / 2, -shapeSize.y/2, 0);
    }
}