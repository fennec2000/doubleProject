using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using TestingNodeFunctions;

/*
 * Error codes:
 * return 0xffffffff; // 4294967295 // id already exists
 * return 0xfffffffe; // 4294967294 // failed to create new node
 */

enum CompositeNodeTypes
{
    Selector,
    Sequence,
}
enum DecoratorNodeType
{
    Inverter,
    Repeater,
    RepeatTillFail,
    Limiter,
}

namespace BehaviorTreeTest
{
    class BehaviourTree
    {
        private uint m_nodeID = 0;
        private Node m_rootNode = null;
        private IDictionary<uint, Node> m_nodeDic = new Dictionary<uint, Node>();

        public BehaviourTree() { }
        public BehaviourTree(string filePath) { LoadTree(filePath); }

        public void SaveTree(string filePath = "Default.tree")
        {
            File.WriteAllText(filePath, "");
            SaveTreeRec(m_rootNode, filePath);
            File.AppendAllText(filePath, "0,root," + m_rootNode.GetID());
        }

        protected bool CheckForSavedNode(uint checkNodeID, string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string s = line.Split(',')[0];
                if (UInt32.TryParse(s, out uint id))
                    if (id == checkNodeID)
                        return true;
            }
            return false;
        }

        protected void SaveTreeRec(Node node, string path)
        {
            uint nodeID = node.GetID();
            if (CheckForSavedNode(nodeID, path))
                return;

            string nodeName = node.ToString();
            string output = nodeID + "," + nodeName;

            // leaf node
            if (nodeName == "ActionNode")
            {
                ActionNode a = (ActionNode)node;
                output += "," + a.GetFunction().Method.Name;
            }
            // decorator node
            else if (nodeName == "Inverter" || nodeName == "Repeater" || nodeName == "RepeatTillFail" || nodeName == "Limiter")
            {
                if (nodeName == "Repeater")
                {
                    Repeater r = (Repeater)node;
                    SaveTreeRec(r.ChildNode, path);
                    output += "," + r.ChildNode.GetID() + "," + r.GetNumberOfRepeats();
                }
                else
                {
                    DecoratorNode d = (DecoratorNode)node;
                    SaveTreeRec(d.ChildNode, path);
                    output += "," + d.ChildNode.GetID();
                }
            }
            // composite node
            else if (nodeName == "Selector" || nodeName == "Sequence")
            {
                CompositeNode c = (CompositeNode)node;
                foreach (Node childNode in c.ChildNodeList)
                {
                    SaveTreeRec(childNode, path);
                    output += "," + childNode.GetID();
                }
                
            }
            File.AppendAllText(path, output + Environment.NewLine);
        }

        public void LoadTree(string filePath = "Default.tree")
        {
            List<string> data;
            string[] lines = File.ReadAllLines(filePath);
            uint x;
            int count;
            List<uint> children = new List<uint>();
            foreach (string line in lines)
            {
                data = line.Split(',').ToList<string>();
                count = data.Count();
                if (UInt32.TryParse(data[0], out uint id))
                {
                    if (id > m_nodeID)
                        m_nodeID = id;

                    switch (data[1])
                    {
                        case "ActionNode":
                            Type calledType = typeof(TestMethods);
                            MethodInfo method = calledType.GetMethod(data[2]);
                            ActionNode.Do action = Delegate.CreateDelegate(typeof(ActionNode.Do), null, method) as ActionNode.Do;
                            CreateActionNodeWithID(action, id);
                            break;


                        case "Inverter":
                            if (UInt32.TryParse(data[2], out x))
                                CreateDecoratorNodeWithID(DecoratorNodeType.Inverter, x, id);
                            break;
                        case "Repeater":
                            if (UInt32.TryParse(data[2], out x))
                            {
                                if (data.Count >= 3)    // does it contain extra data
                                {
                                    if (UInt32.TryParse(data[3], out uint y))
                                        CreateDecoratorNodeWithID(DecoratorNodeType.Repeater, x, id, y);
                                }
                                else
                                    CreateDecoratorNodeWithID(DecoratorNodeType.Repeater, x, id);
                            }
                                
                            break;
                        case "RepeatTillFail":
                            if (UInt32.TryParse(data[2], out x))
                                CreateDecoratorNodeWithID(DecoratorNodeType.RepeatTillFail, x, id);
                            break;
                        case "Limiter":
                            if (UInt32.TryParse(data[2], out x))
                                CreateDecoratorNodeWithID(DecoratorNodeType.RepeatTillFail, x, id);
                            break;


                        case "Selector":
                            for(int i = 2; i < count; ++i)
                                if (UInt32.TryParse(data[i], out x))
                                    children.Add(x);

                            CreateCompositeNodeWithID(CompositeNodeTypes.Selector, children, id);
                            break;
                        case "Sequence":
                            for (int i = 2; i < count; ++i)
                                if (UInt32.TryParse(data[i], out x))
                                    children.Add(x);

                            CreateCompositeNodeWithID(CompositeNodeTypes.Sequence, children, id);
                            break;

                        // Set the root node at the end
                        case "root":
                            if (UInt32.TryParse(data[2], out x))
                                SetRootNode(x);
                            break;
                    }
                }
            }
        }

        public void RunTree()
        {
            m_rootNode.Run();
        }

        public void SetRootNode(uint node)
        {
            if (m_nodeDic.TryGetValue(node, out Node result))
                m_rootNode = result;
        }

        public uint CreateCompositeNode(CompositeNodeTypes composite, List<uint> nodeIDs)
        {
            Node newNode = MakeCompositeNode(composite, nodeIDs);
            if (newNode == null)
                return 0xfffffffe;  // failed to create new node
            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }

        private uint CreateCompositeNodeWithID(CompositeNodeTypes composite, List<uint> nodeIDs, uint nodeID)
        {
            if (m_nodeDic.ContainsKey(nodeID))
                return 0xffffffff;   // id already exists
            Node newNode = MakeCompositeNode(composite, nodeIDs);
            if (newNode == null)
                return 0xfffffffe;  // failed to create new node
            newNode.SetID(nodeID);
            m_nodeDic.Add(nodeID, newNode);
            return nodeID;
        }

        private Node MakeCompositeNode(CompositeNodeTypes composite, List<uint> nodeIDs)
        {
            // Get nodes
            List<Node> children = new List<Node>();
            foreach (uint id in nodeIDs)
            {
                if (m_nodeDic.TryGetValue(id, out Node result))
                    children.Add(result);
            }

            // create composite node
            Node newNode;
            switch (composite)
            {
                case CompositeNodeTypes.Selector:
                    newNode = new Selector(children);
                    break;
                case CompositeNodeTypes.Sequence:
                    newNode = new Sequence(children);
                    break;
                default:
                    return null;
            }

            return newNode;
        }

        public uint CreateDecoratorNode(DecoratorNodeType decorator, uint childNodeID, uint extra = 0)
        {
            Node newNode = MakeDecoratorNode(decorator, childNodeID, extra);
            if (newNode == null)
                return 0xfffffffe;  // failed to create new node

            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }

        private uint CreateDecoratorNodeWithID(DecoratorNodeType decorator, uint childNodeID, uint nodeID, uint extra = 0)
        {
            if (m_nodeDic.ContainsKey(nodeID))
                return 0xffffffff;   // id already exists
            Node newNode = MakeDecoratorNode(decorator, childNodeID, extra);
            if (newNode == null)
                return 0xfffffffe;  // failed to create new node

            newNode.SetID(nodeID);
            m_nodeDic.Add(nodeID, newNode);
            return nodeID;
        }

        private Node MakeDecoratorNode(DecoratorNodeType decorator, uint childNodeID, uint extra = 0)
        {
            Node newNode;
            if (!m_nodeDic.TryGetValue(childNodeID, out Node result))
                return null;   // failed to get childNodeID

            switch (decorator)
            {
                case DecoratorNodeType.Inverter:
                    newNode = new Inverter(result);
                    break;
                case DecoratorNodeType.Repeater:
                    newNode = new Repeater(result, extra);
                    break;
                case DecoratorNodeType.RepeatTillFail:
                    newNode = new RepeatTillFail(result);
                    break;
                case DecoratorNodeType.Limiter:
                    newNode = new Limiter(result);
                    break;
                default:
                    return null; // tried to create unknown decorator node
            }

            return newNode;
        }

        public uint CreateActionNode(ActionNode.Do func)
        {
            ActionNode newNode = new ActionNode(func);

            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }

        private uint CreateActionNodeWithID(ActionNode.Do func, uint nodeID)
        {
            if (m_nodeDic.ContainsKey(nodeID))
                return 0xffffffff;   // id already exists
            ActionNode newNode = new ActionNode(func);

            newNode.SetID(nodeID);
            m_nodeDic.Add(nodeID, newNode);
            return nodeID;
        }
    }
}
