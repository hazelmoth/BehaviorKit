using System;

namespace BehaviorKit.Nodes
{
    /// Repeatedly recreates and runs a Node for the given task. Restarts the task when it
    /// returns success or failure, or when a maximum time limit is reached. Optionally
    /// returns Success when a node succeeds, instead of restarting.
    public class ImpatientRepeater : Node
    {
        private readonly Func<Node> _task;
        private readonly Func<float> _timeProvider;
        private readonly float _maxRestartTime;
        private readonly bool _finishOnSuccess;
        private Node _current;
        private float _lastStartTime;
    
        public ImpatientRepeater(
            Func<Node> task,
            Func<float> timeProvider,
            float maxRestartTime,
            bool finishOnSuccess = false)
        {
            _task = task;
            _timeProvider = timeProvider;
            _maxRestartTime = maxRestartTime;
            _finishOnSuccess = finishOnSuccess;
            _lastStartTime = timeProvider.Invoke();
        }

        protected override void Init()
        {
            Restart();
        }

        protected override void OnCancel()
        {
            if (_current != null && !_current.Stopped) _current.Cancel();
        }

        protected override Status OnUpdate()
        {
            Status currentStatus = _current.Update();
            if (_finishOnSuccess && currentStatus == Status.Success) return Status.Success;

            if (currentStatus != Status.Running
                || _timeProvider.Invoke() - _lastStartTime > _maxRestartTime)
            {
                Restart();
            }
            return Status.Running;
        }

        private void Restart()
        {
            _current = _task.Invoke();
            _lastStartTime = _timeProvider.Invoke();
        }
    }
}
