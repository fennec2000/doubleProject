using BehaviorTreeTest;
using System.Collections;
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

	[SerializeField]
	private Core m_Core;

	[SerializeField]
	private GameObject m_StatsObj;
	private Text m_StatsText;

	private CreatureAI targetAI;
    private GameObject target;
    private Camera myCamera;
    private RectTransform contentRec;
    public GameObject LiveTreeOverlay;
    public Texture2D SquareTex;
    public Texture2D DiamondTex;
    public Texture2D CircleTex;
    public Toggle toggle;
    private List<SLiveTreeNode> LiveTreeObjects;
    private Vector2 shapeSize = new Vector2(100, 100);
    private BehaviourTree targetBT;

    // Use this for initialization
    void Start () {
        LiveTreeObjects = new List<SLiveTreeNode>();
        myCamera = GetComponent<Camera>();
        contentRec = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
		m_StatsText = m_StatsObj.GetComponentInChildren<Text>();
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
				var creatureAI = objectHit.GetComponentInParent<CreatureAI>();
                if (creatureAI != null)
                {
					target = creatureAI.gameObject;
					targetAI = target.GetComponent<CreatureAI>();
					targetBT = creatureAI.Behaviour;
					m_StatsObj.SetActive(true);

					DrawTree();
                }
            }
        }

        // turn off the overlay if active and no target
        if (target == null && LiveTreeOverlay.activeSelf)
        {
            LiveTreeOverlay.SetActive(false);
			m_StatsObj.SetActive(false);
			m_Core.GameState = Core.EGameState.Simulating;
		}

        // set overlay active and update
        if (target != null)
        {
            if (!LiveTreeOverlay.activeSelf)
			{
				LiveTreeOverlay.SetActive(true);
				m_StatsObj.SetActive(true);
			}
            UpdateTree();
			UpdateStats();

		}
    }

	void UpdateStats()
	{
		var stats = targetAI.GetCreatureStats();
		string generated = "Stats:\n";
		generated += "Food: " + stats.Food + '\n';
		generated += "Water: " + stats.Water + '\n';

		m_StatsText.text = generated;
	}

	void UpdateTree()
	{
		var transform = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetChild(0).GetComponentInChildren<Transform>();
		var rootNode = targetBT.GetRootTreeNode();

		UpdateNode(transform, rootNode);
	}

    void UpdateNode(Transform transform, STreeNode node)
    {
		// update text
		var text = transform.GetChild(0).GetComponent<Text>();
		text.text = "ID: " + node.id + "\n" + node.name + "\n" + node.state;

		// update image
		var image = transform.GetComponent<Image>();

		switch (node.state)
		{
			case ENodeStates.FAILURE:
				image.color = Color.red;
				break;
			case ENodeStates.SUCCESS:
				image.color = Color.green;
				break;
			case ENodeStates.RUNNING:
				image.color = Color.yellow;
				break;
			default:
				image.color = Color.gray;
				break;
		}

		if (node.children != null)
		{
			for (int i = 0; i < node.children.Count; ++i)
			{
				UpdateNode(transform.GetChild(i + 1), node.children[i]);
			}
		}
	}

    void DrawTree()
    {
		m_Core.GameState = Core.EGameState.Observing;

        // delete children
        var children = LiveTreeOverlay.transform.GetChild(0).GetChild(0).GetComponentInChildren<Transform>();
        foreach (Transform child in children)
        {
            GameObject.Destroy(child.gameObject);
        }
        LiveTreeObjects.Clear();

		// get tree node list
		var rootNode = targetBT.GetRootTreeNode();

		var firstNode = DrawNode(rootNode);

		contentRec.sizeDelta = firstNode.size;
		firstNode.nodeObject.transform.localPosition = new Vector3(firstNode.size.x / 2, -shapeSize.y, 0);
		
	}

	SLiveTreeNode DrawNode(STreeNode tree)
	{
		// new object for each node
		var myObj = new SLiveTreeNode();
		myObj.node = tree;

		// new image
		myObj.nodeObject = new GameObject("Image");
		myObj.nodeObject.transform.SetParent(LiveTreeOverlay.transform.GetChild(0).GetChild(0).transform);

		// new text for image
		myObj.nodeObjectText = new GameObject("Text");
		Text newText = myObj.nodeObjectText.AddComponent<Text>();
		myObj.nodeObjectText.transform.SetParent(myObj.nodeObject.transform);


		newText.text = "ID: " + tree.id + "\n" + tree.name + "\n" + tree.state;
		newText.alignment = TextAnchor.MiddleCenter;
		newText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		newText.color = Color.black;

		Image newImage = myObj.nodeObject.AddComponent<Image>();

		// Set image type
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

		// set colour
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
				newImage.color = Color.gray;
				break;
		}

		if (myObj.node.children != null)
		{
			List<SLiveTreeNode> childList = new List<SLiveTreeNode>();

			foreach (var child in myObj.node.children)
			{
				var childNode = DrawNode(child);
				var childTrans = childNode.nodeObject.transform;
				childTrans.SetParent(myObj.nodeObject.transform);
				childList.Add(childNode);
			}

			// positions get
			Vector2 childSumSize = new Vector2();
			foreach (var child in childList)
				childSumSize += child.size;

			// set
			float x = -childSumSize.x / 2;
			foreach (var child in childList)
			{
				x += child.size.x / 2;
				child.nodeObject.transform.localPosition = new Vector3(x, -shapeSize.y * 1.5f, 0);
				x += child.size.x / 2;
			}

			myObj.size = childSumSize + new Vector2(0, shapeSize.y * 1.5f);
		}

		return myObj;
	}

    public void ToggleSetBTIdleUpdate()
    {
        targetBT.UpdateIdleNodes(toggle.isOn);
    }
}
