using System;
using System.Collections.Generic;
using System.Text;

namespace AILab.Core.BehaviourTree
{
    public abstract class Node
    {
        public string Name { get; }

        protected Node(string name)
        {
            Name = name;
        }

        public abstract NodeStatus Tick(Blackboard blackboard);
    }
}
