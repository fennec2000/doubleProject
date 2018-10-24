using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestingNodeFunctions;

namespace CompositeNodeTesting
{
    [TestClass]
    public class SelectorNodeTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void SUCCESS1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;

            Selector rootNode;

            ActionNode T2A;
            Inverter T2B;
            ActionNode T2C;

            ActionNode T3;

            T3 = new ActionNode(myTestsFuncs.CounterInc);

            T2A = new ActionNode(myTestsFuncs.CounterInc);
            T2B = new Inverter(T3);
            T2C = new ActionNode(myTestsFuncs.CounterInc);

            List<Node> rootChildren = new List<Node>();
            rootChildren.Add(T2A);
            rootChildren.Add(T2B);
            rootChildren.Add(T2C);

            rootNode = new Selector(rootChildren);

            rootNode.Run();

            Assert.AreEqual(NodeStates.SUCCESS, rootNode.NodeState);
        }

        [TestMethod]
        public void SUCCESS2()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;

            ActionNode T3 = new ActionNode(myTestsFuncs.CounterInc);

            ActionNode T2A = new ActionNode(myTestsFuncs.CounterInc);
            Inverter T2B = new Inverter(T3);
            ActionNode T2C = new ActionNode(myTestsFuncs.CounterInc);

            List<Node> rootChildren = new List<Node> { T2A, T2B, T2C };

            Selector rootNode = new Selector(rootChildren);
            rootNode.Run();

            Assert.AreEqual(NodeStates.SUCCESS, rootNode.NodeState);
        }
    }

    [TestClass]
    public class SequenceNodeTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void SUCCESS1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 1;

            ActionNode T3 = new ActionNode(myTestsFuncs.CounterInc);

            ActionNode T2A = new ActionNode(myTestsFuncs.CounterInc);
            Inverter T2B = new Inverter(T3);

            List<Node> rootChildren = new List<Node> { T2A, T2B };

            Sequence rootNode = new Sequence(rootChildren);
            rootNode.Run();

            Assert.AreEqual(NodeStates.SUCCESS, rootNode.NodeState);
        }

        [TestMethod]
        public void FAILURE1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 1;

            ActionNode T3 = new ActionNode(myTestsFuncs.CounterInc);

            ActionNode T2A = new ActionNode(myTestsFuncs.CounterInc);
            Inverter T2B = new Inverter(T3);
            ActionNode T2C = new ActionNode(myTestsFuncs.CounterInc);

            List<Node> rootChildren = new List<Node> { T2A, T2B, T2C };

            Sequence rootNode = new Sequence(rootChildren);
            rootNode.Run();

            Assert.AreEqual(NodeStates.FAILURE, rootNode.NodeState);
        }
    }
}
