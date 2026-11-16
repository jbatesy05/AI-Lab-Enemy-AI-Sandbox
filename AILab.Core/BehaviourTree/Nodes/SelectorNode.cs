using AILab.Core.BehaviourTree;

namespace AILab.Core.BehaviourTree.Nodes
{
    public class SelectorNode : CompositeNode
    {
        public SelectorNode(string name, params Node[] children)
            : base(name, children)
        {
        }

        public override NodeStatus Tick(Blackboard blackboard)
        {
            foreach (Node child in Children)
            {
                NodeStatus status = child.Tick(blackboard);

                if (status == NodeStatus.Success)
                {
                    return NodeStatus.Success;
                }

                if (status == NodeStatus.Running)
                {
                    return NodeStatus.Running;
                }
            }

            return NodeStatus.Failure;
        }
    }
}
