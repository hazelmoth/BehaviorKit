using System;

namespace BehaviorKit.Nodes
{
    /// Repeatedly recreates and runs a Node for the given Task, using the given
    /// constructor arguments. Always returns a status of Running.
    public class Repeater : Node
    {
        private readonly Func<Node> _task;
        private Node _current;

        public Repeater(Func<Node> task)
        {
            _task = task;
        }

        protected override void Init() { }

        protected override void OnCancel()
        {
            if (_current != null && !_current.Stopped) _current.Cancel();
        }

        protected override Status OnUpdate()
        {
            if (_current == null) _current = _task.Invoke();
            if (_current.Update() != Status.Running) _current = null;
            return Status.Running;
        }
    }
}
