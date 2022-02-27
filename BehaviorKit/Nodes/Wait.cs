using System;
using BehaviourKit;

namespace BehaviorKit.Nodes
{
    /// A Node that does nothing for a given duration. Always returns Success afterwards.
    /// If given time is zero, returns Success immediately.
    public class Wait : Node
    {
        private readonly Func<float> _timeProvider;
        private readonly float _duration;
        private float _startTime;

        public Wait(Func<float> timeProvider, float duration)
        {
            _timeProvider = timeProvider;
            _duration = duration;
        }

        protected override void Init()
        {
            _startTime = _timeProvider.Invoke();
        }

        protected override void OnCancel() { }

        protected override Status OnUpdate()
        {
            return (_timeProvider.Invoke() - _startTime > _duration) ? Status.Success : Status.Running;
        }
    }
}
