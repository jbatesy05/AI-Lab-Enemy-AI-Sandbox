using System;
using AILab.Core.BehaviourTree;

namespace AILab.Core.BehaviourTree.Nodes
{
    public class ActionNode : Node
    {
        private readonly Func<Blackboard, NodeStatus> _action;

        public ActionNode(string name, Func<Blackboard, NodeStatus> action)
            : base(name)
        {
            ArgumentNullException.ThrowIfNull(action);
            _action = action;
        }

        public override NodeStatus Tick(Blackboard blackboard)
        {
            return _action(blackboard);
        }
    }
}