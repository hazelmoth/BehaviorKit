using System;
using BehaviourKit;

namespace BehaviorKit.Nodes
{
    /// A node that just executes a given function immediately, and returns success.
    public class Execute : Node
    {
        private readonly Action _function;

        public Execute(Action function)
        {
            _function = function;
        }

        protected override void Init() { }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            _function();
            return Status.Success;
        }
    }
}
