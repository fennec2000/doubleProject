using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        }

        protected void SaveTreeRec(Node node, string path)
        {
            string nodeName = node.ToString();
            string output = node.GetID() + "," + nodeName;

            // leaf node
            if (nodeName == "ActionNode")
            {
                ActionNode a = (ActionNode)node;
                output += "," + a.GetFunction().Method.Name;
            }
            // decorator node
            else if (nodeName == "Inverter" || nodeName == "Repeater" || nodeName == "RepeatTillFail" || nodeName == "Limiter")
            {
                DecoratorNode d = (DecoratorNode)node;
                SaveTreeRec(d.ChildNode, path);
                output += "," + d.ChildNode.GetID();
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
            Console.WriteLine(output);
        }

        public void LoadTree(string filePath = "Default.tree")
        {

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
            // Get nodes
            List<Node> children = new List<Node>();
            foreach(uint id in nodeIDs)
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
                    return 0; // tryed to create unknown composit node
            }

            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }

        public uint CreateDecoratorNode(DecoratorNodeType decorator, uint nodeID)
        {
            Node newNode;
            if (!m_nodeDic.TryGetValue(nodeID, out Node result))
                return 0;   // failed to get nodeID

            switch (decorator)
            {
                case DecoratorNodeType.Inverter:
                    newNode = new Inverter(result);
                    break;
                case DecoratorNodeType.Repeater:
                    newNode = new Repeater(result);
                    break;
                case DecoratorNodeType.RepeatTillFail:
                    newNode = new RepeatTillFail(result);
                    break;
                case DecoratorNodeType.Limiter:
                    newNode = new Limiter(result);
                    break;
                default:
                    return 0; // tryed to create unknown decorator node
            }
            
            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }

        public uint CreateActionNode(ActionNode.Do func)
        {
            ActionNode newNode = new ActionNode(func);

            newNode.SetID(++m_nodeID);
            uint a = newNode.GetID();
            m_nodeDic.Add(a, newNode);
            return a;
        }
    }
}
