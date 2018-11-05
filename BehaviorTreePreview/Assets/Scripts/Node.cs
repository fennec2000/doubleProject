using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ENodeStates
{
    FAILURE,
    SUCCESS,
    RUNNING,
}

public abstract class Node
{
    // the current state of the node
    protected ENodeStates m_nodeState;

    private uint m_id = 0;

    public uint GetID() { return m_id; }
    public void SetID(uint a) { m_id = a; }

    public ENodeStates NodeState
    {
        get { return m_nodeState; }
    }

    // constructor for a node
    public Node() {}

    public abstract ENodeStates Run();
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

    public override ENodeStates Run()
    {
        bool childRunning = false;
        foreach (Node node in m_nodes)
        {
            switch (node.Run())
            {
                case ENodeStates.FAILURE:
                    m_nodeState = ENodeStates.FAILURE;
                    return m_nodeState;
                case ENodeStates.SUCCESS:
                    continue;
                case ENodeStates.RUNNING:
                    childRunning = true;
                    continue;
                default:
                    continue;
            }
        }
        if (childRunning)
            m_nodeState = ENodeStates.RUNNING;
        else
            m_nodeState = ENodeStates.SUCCESS;
        return m_nodeState;
    }
}

// selector node
// returns success if any child returns success
public class Selector : CompositeNode
{
    // constructor
    public Selector(List<Node> nodes) : base(nodes) { }

    public override ENodeStates Run()
    {
        foreach (Node node in m_nodes)
        {
            switch (node.Run())
            {
                case ENodeStates.FAILURE:
                    continue;
                case ENodeStates.SUCCESS:
                    m_nodeState = ENodeStates.SUCCESS;
                    return m_nodeState;
                case ENodeStates.RUNNING:
                    m_nodeState = ENodeStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}

// inverter node
// flips the status of the child node
public class Inverter : DecoratorNode
{
    public Inverter(Node node) : base(node) { }

    public override ENodeStates Run()
    {
        switch (m_node.Run())
        {
            case ENodeStates.FAILURE:
                m_nodeState = ENodeStates.SUCCESS;
                return m_nodeState;
            case ENodeStates.SUCCESS:
                m_nodeState = ENodeStates.FAILURE;
                return m_nodeState;
            case ENodeStates.RUNNING:
                m_nodeState = ENodeStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = ENodeStates.SUCCESS;
        return m_nodeState;
    }
}

// repeater node
// repeats the child node again
public class Repeater : DecoratorNode
{
    uint m_repeates = 2;

    public uint GetNumberOfRepeats() { return m_repeates; }

    public Repeater(Node node, uint x = 2) : base(node) { m_repeates = x; }

    public override ENodeStates Run()
    {
        if (m_repeates < 1)
            return ENodeStates.SUCCESS;

        for (int i = 1; i < m_repeates; ++i)
            m_node.Run();

        switch (m_node.Run())
        {
            case ENodeStates.FAILURE:
                m_nodeState = ENodeStates.FAILURE;
                return m_nodeState;
            case ENodeStates.SUCCESS:
                m_nodeState = ENodeStates.SUCCESS;
                return m_nodeState;
            case ENodeStates.RUNNING:
                m_nodeState = ENodeStates.RUNNING;
                return m_nodeState;
        }

        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}

// repeat till fail
// continusly runs the child node till it returns failure
public class RepeatTillFail : DecoratorNode
{
    public RepeatTillFail(Node node) : base(node) { }

    public override ENodeStates Run()
    {
        while (true)
        {
            switch (m_node.Run())
            {
                case ENodeStates.FAILURE:
                    m_nodeState = ENodeStates.FAILURE;
                    return m_nodeState;
                case ENodeStates.SUCCESS:
                    m_nodeState = ENodeStates.SUCCESS;
                    continue;
                case ENodeStates.RUNNING:
                    m_nodeState = ENodeStates.RUNNING;
                    continue;
            }
        }

        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}

// limiter node
public class Limiter : DecoratorNode
{
    private int remainingRuns = 1;
    public Limiter(Node node) : base(node) { }

    public override ENodeStates Run()
    {
        if (remainingRuns > 0)
        {
            switch (m_node.Run())
            {
                case ENodeStates.FAILURE:
                    --remainingRuns;
                    m_nodeState = ENodeStates.FAILURE;
                    return m_nodeState;
                case ENodeStates.SUCCESS:
                    --remainingRuns;
                    m_nodeState = ENodeStates.SUCCESS;
                    return m_nodeState;
                case ENodeStates.RUNNING:
                    m_nodeState = ENodeStates.RUNNING;
                    return m_nodeState;
            }
        }

        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}

// succeeder node
// runs child and always returns success
public class Succeeder : DecoratorNode
{
    public Succeeder(Node node) : base(node) { }

    public override ENodeStates Run()
    {
        if (m_node.Run() == ENodeStates.RUNNING)
            m_nodeState = ENodeStates.RUNNING;
        else
            m_nodeState = ENodeStates.SUCCESS;
        return m_nodeState;
    }
}

// failure node
// runs child and always returns failure
public class Failure : DecoratorNode
{
    public Failure(Node node) : base(node) { }

    public override ENodeStates Run()
    {
        m_node.Run();
        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}

// action node
public class ActionNode : Node
{

    public delegate ENodeStates Do();

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

    public override ENodeStates Run()
    {
        switch (m_Do())
        {
            case ENodeStates.FAILURE:
                m_nodeState = ENodeStates.FAILURE;
                return m_nodeState;
            case ENodeStates.SUCCESS:
                m_nodeState = ENodeStates.SUCCESS;
                return m_nodeState;
            case ENodeStates.RUNNING:
                m_nodeState = ENodeStates.RUNNING;
                return m_nodeState;
        }

        m_nodeState = ENodeStates.FAILURE;
        return m_nodeState;
    }
}