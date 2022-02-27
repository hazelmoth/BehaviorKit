using System;

namespace BehaviorKit.Nodes
{
    /// Runs a task to completion and returns its status if it completes within
    /// a given time limit; otherwise, stops the task and returns failure.
    public class TimeLimit : Node
    {
        private readonly Func<Node> _task;
        private readonly Func<float> _timeProvider;
        private readonly float _timeLimit;
        private Node _subNode;
        private float _startTime;
    
        public TimeLimit(Func<Node> task, Func<float> timeProvider, float timeLimit)
        {
            _task = task;
            _timeProvider = timeProvider;
            _timeLimit = timeLimit;
        }

        protected override void Init()
        {
            _subNode = _task.Invoke();
            _startTime = _timeProvider.Invoke();
        }

        protected override void OnCancel()
        {
            if (_subNode != null && !_subNode.Stopped) _subNode.Cancel();
        }

        protected override Status OnUpdate()
        {
            if (_timeProvider.Invoke() - _startTime > _timeLimit)
            {
                if (_subNode != null && !_subNode.Stopped) _subNode.Cancel();
                return Status.Failure;
            }

            return _subNode.Update();
        }
    }
}
