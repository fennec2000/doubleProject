using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestingNodeFunctions;

namespace DecoratorNodeTesting
{

    [TestClass]
    public class InverterTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void SUCCESS1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Inverter test = new Inverter(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.SUCCESS, test.NodeState);
        }

        [TestMethod]
        public void FAILURE1()
        {
            myTestsFuncs.counter = 9;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Inverter test = new Inverter(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.FAILURE, test.NodeState);
        }
    }

    [TestClass]
    public class RepeaterTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void FAILURE1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Repeater test = new Repeater(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.FAILURE, test.NodeState);
        }

        [TestMethod]
        public void SUCCESS1()
        {
            myTestsFuncs.counter = 8;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Repeater test = new Repeater(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.SUCCESS, test.NodeState);
        }
    }

    [TestClass]
    public class RepeatTillFailTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void FAILURE1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            RepeatTillFail test = new RepeatTillFail(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.FAILURE, test.NodeState);
        }
    }

    [TestClass]
    public class LimiterTests
    {
        TestMethods myTestsFuncs = new TestMethods();

        [TestMethod]
        public void FAILURE1()
        {
            myTestsFuncs.counter = 0;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Limiter test = new Limiter(IncAction);
            test.Run();
            test.Run();
            test.Run();

            Assert.AreEqual(NodeStates.FAILURE, test.NodeState);
            Assert.AreEqual(1, myTestsFuncs.counter);
        }

        [TestMethod]
        public void FAILURE2()
        {
            myTestsFuncs.counter = 9;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Limiter test = new Limiter(IncAction);
            test.Run();
            test.Run();
            test.Run();

            Assert.AreEqual(NodeStates.FAILURE, test.NodeState);
            Assert.AreEqual(10, myTestsFuncs.counter);
        }

        [TestMethod]
        public void SUCCESS1()
        {
            myTestsFuncs.counter = 9;
            myTestsFuncs.target = 10;
            ActionNode IncAction = new ActionNode(myTestsFuncs.CounterInc);
            Limiter test = new Limiter(IncAction);
            test.Run();

            Assert.AreEqual(NodeStates.SUCCESS, test.NodeState);
            Assert.AreEqual(10, myTestsFuncs.counter);
        }
    }
}
