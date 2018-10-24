namespace TestingNodeFunctions
{
    public class TestMethods
    {
        public int counter = 0;
        public int target = 10;

        public NodeStates CounterInc()
        {
            counter += 1;
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        public NodeStates Equal()
        {
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }
        public NodeStates NotEqual()
        {
            return counter != target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        public NodeStates CounterDec()
        {
            counter -= 1;
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }

        public NodeStates CounterAdd(int a)
        {
            counter += a;
            return counter == target ? NodeStates.SUCCESS : NodeStates.FAILURE;
        }
    }
}
