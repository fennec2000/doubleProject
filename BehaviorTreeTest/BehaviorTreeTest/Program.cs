using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTreeTest
{
    class Program
    {
        static int counter = 0;
        static int target = 10;

        static NodeStates Equal()
        {
            Console.WriteLine("Action: Equal: counter: " + counter + ", target: " + target);
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }
        static NodeStates NotEqual()
        {
            Console.WriteLine("Action: NotEqual: counter: " + counter + ", target: " + target);
            return counter != target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        static NodeStates CounterInc()
        {
            counter += 1;
            Console.WriteLine("Action: Inc: counter: " + counter + ", target: " + target);
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        static NodeStates CounterDec()
        {
            counter -= 1;
            Console.WriteLine("Action: Dec: counter: " + counter + ", target: " + target);
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        static NodeStates CounterAdd(int a)
        {
            counter += a;
            Console.WriteLine("Action: Add: counter: " + counter + ", target: " + target);
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        static void Main(string[] args)
        {
            BehaviourTree firstTree = new BehaviourTree();
            BehaviourTree secondTree = new BehaviourTree();

            var T3  = firstTree.CreateActionNode(CounterInc);
            var T2A = firstTree.CreateActionNode(CounterInc);
            var T2B = firstTree.CreateDecoratorNode(DecoratorNodeType.Inverter, T3);
            var T2C = firstTree.CreateActionNode(CounterInc);

            var children = new List<uint> { T2A, T2B, T2C };
            var root = firstTree.CreateCompositeNode(CompositeNodeTypes.Selector, children);
            firstTree.SetRootNode(root);

            firstTree.SaveTree();
            secondTree.LoadTree();
            BehaviourTree thirdTree = new BehaviourTree("default.tree");

            //ActionNode T3 = new ActionNode(CounterInc);
            //ActionNode T2A = new ActionNode(CounterInc);
            //Inverter T2B = new Inverter(T3);
            //ActionNode T2C = new ActionNode(CounterInc);

            //List<Node> rootChildren = new List<Node>
            //{
            //    T2A,
            //    T2B,
            //    T2C
            //};

            //Selector rootNode = new Selector(rootChildren);

            //SaveTree(rootNode);

            // pause at end
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
