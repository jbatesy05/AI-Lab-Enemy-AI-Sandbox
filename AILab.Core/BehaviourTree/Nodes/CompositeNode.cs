using System;
using System.Collections.Generic;
using AILab.Core.BehaviourTree;

namespace AILab.Core.BehaviourTree.Nodes
{
    public abstract class CompositeNode : Node
    {
        protected List<Node> Children { get; }

        protected CompositeNode(string name, params Node[] children)
            : base(name)
        {
            ArgumentNullException.ThrowIfNull(children);

            Children = new List<Node>(children);
        }
    }
}
