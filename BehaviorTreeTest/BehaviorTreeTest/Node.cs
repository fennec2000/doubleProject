using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum NodeStates
{
    FAILURE,
    SUCCESS,
    RUNNING,
}

public abstract class Node
{
    // the current state of the node
    protected NodeStates m_nodeState;

    private uint m_id = 0;

    public uint GetID() { return m_id; }
    public void SetID(uint a) { m_id = a; }

    public NodeStates NodeState
    {
        get { return m_nodeState; }
    }

    // constructor for a node
    public Node() {}

    public abstract NodeStates Run();
}

public abstract class DecoratorNode : Node
{
    protected Node m_node;

    public Node ChildNode
    {
        get { return m_node; }
    }

    public DecoratorNode(Node node)
    {
        m_node = node;
    }
}

public abstract class CompositeNode : Node
{
    // List of child nodes
    protected List<Node> m_nodes = new List<Node>();

    public List<Node> ChildNodeList
    {
        get { return m_nodes; }
    }

    public CompositeNode(List<Node> nodes)
    {
        m_nodes = nodes;
    }
}

// Sequence node
// runs all children and returns success if no child fails
public class Sequence : CompositeNode
{
    // constructor
    public Sequence(List<Node> nodes) : base(nodes) { }

    public override NodeStates Run()
    {
        bool childRunning = false;
        foreach (Node node in m_nodes)
        {
            switch (node.Run())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    childRunning = true;
                    continue;
                default:
                    continue;
            }
        }
        if (childRunning)
            m_nodeState = NodeStates.RUNNING;
        else
            m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}

// selector node
// returns success if any child returns success
public class Selector : CompositeNode
{
    // constructor
    public Selector(List<Node> nodes) : base(nodes) { }

    public override NodeStates Run()
    {
        foreach (Node node in m_nodes)
        {
            switch (node.Run())
            {
                case NodeStates.FAILURE:
                    continue;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// inverter node
// flips the status of the child node
public class Inverter : DecoratorNode
{
    public Inverter(Node node) : base(node) { }

    public override NodeStates Run()
    {
        switch (m_node.Run())
        {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}

// repeater node
// repeats the child node again
public class Repeater : DecoratorNode
{
    public Repeater(Node node) : base(node) { }

    public override NodeStates Run()
    {
        // hard coded to run twice, possible upgrade to allow the number of repeats
        int x = 1;
        for (int i = 0; i < x; ++i)
            m_node.Run();

        switch (m_node.Run())
        {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }

        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// repeat till fail
// continusly runs the child node till it returns failure
public class RepeatTillFail : DecoratorNode
{
    public RepeatTillFail(Node node) : base(node) { }

    public override NodeStates Run()
    {
        while (true)
        {
            switch (m_node.Run())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    continue;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    continue;
            }
        }

        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// limiter node
public class Limiter : DecoratorNode
{
    private int remainingRuns = 1;
    public Limiter(Node node) : base(node) { }

    public override NodeStates Run()
    {
        if (remainingRuns > 0)
        {
            switch (m_node.Run())
            {
                case NodeStates.FAILURE:
                    --remainingRuns;
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;
                case NodeStates.SUCCESS:
                    --remainingRuns;
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
            }
        }

        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// succeeder node
// runs child and always returns success
public class Succeeder : DecoratorNode
{
    public Succeeder(Node node) : base(node) { }

    public override NodeStates Run()
    {
        m_node.Run();
        m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}

// failure node
// runs child and always returns failure
public class Failure : DecoratorNode
{
    public Failure(Node node) : base(node) { }

    public override NodeStates Run()
    {
        m_node.Run();
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// action node
public class ActionNode : Node
{

    public delegate NodeStates Do();

    // stored action
    private Do m_Do;

    // contruction with action
    public ActionNode(Do function)
    {
        m_Do = function;
    }

    public Do GetFunction()
    {
        return m_Do;
    }

    public override NodeStates Run()
    {
        switch (m_Do())
        {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}