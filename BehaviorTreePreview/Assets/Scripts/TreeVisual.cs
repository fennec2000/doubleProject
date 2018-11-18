using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeVisual : MonoBehaviour {

    public GameObject target;
    public bool haveTarget;
    private Camera myCamera;
    private RectTransform contentRec;
    [SerializeField]
    public GameObject LiveTreeOverlay;
    public Texture2D SquareTex;
    public Texture2D DiamondTex;
    public Texture2D CircleTex;

    // Use this for initialization
    void Start () {
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
            var go = new GameObject("Image");
            var goText = new GameObject("Text");
            goText.transform.SetParent(go.transform);
            go.transform.SetParent(LiveTreeOverlay.transform.GetChild(0).GetChild(0).transform);

            Text newText = goText.AddComponent<Text>();
            newText.text = "ID: " + tree.id + "\n" + tree.name + "\n" + tree.state;
            newText.alignment = TextAnchor.MiddleCenter;
            newText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            newText.color = Color.black;

            Image newImage = go.AddComponent<Image>();
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

            go.transform.localPosition = new Vector3(50 + 100 * current, -50, 0);
            ++current;
        }
    }
}
