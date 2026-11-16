using AILab.Core.BehaviourTree;

namespace AILab.Core.BehaviourTree.Nodes
{
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(string name, params Node[] children)
            : base(name, children)
        {
        }

        public override NodeStatus Tick(Blackboard blackboard)
        {
            foreach (Node child in Children)
            {
                NodeStatus status = child.Tick(blackboard);

                if (status == NodeStatus.Failure)
                {
                    return NodeStatus.Failure;
                }

                if (status == NodeStatus.Running)
                {
                    return NodeStatus.Running;
                }
            }

            return NodeStatus.Success;
        }
    }
}
