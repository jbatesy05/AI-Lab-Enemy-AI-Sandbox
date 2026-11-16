using System;
using AILab.Core.BehaviourTree;

namespace AILab.Core.BehaviourTree.Nodes
{
    public class ConditionNode : Node
    {
        private readonly Func<Blackboard, bool> _condition;

        public ConditionNode(string name, Func<Blackboard, bool> condition)
            : base(name)
        {
            ArgumentNullException.ThrowIfNull(condition);
            _condition = condition;
        }

        public override NodeStatus Tick(Blackboard blackboard)
        {
            return _condition(blackboard)
                ? NodeStatus.Success
                : NodeStatus.Failure;
        }
    }
}