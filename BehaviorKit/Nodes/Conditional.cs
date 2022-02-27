using System;

namespace BehaviorKit.Nodes
{
    /// A 2-branch conditional Node. Starts a new child Node every time it switches
    /// branches. If a child node returns Success or Failure, this Node will return that
    /// same state.
    public class Conditional : Node
    {
        private readonly Func<Node> _left, _right;
        private readonly Func<bool> _condition;
        private Node _current;
        private bool _conditionMetLastFrame;

        public Conditional(Func<bool> condition, Func<Node> left, Func<Node> right)
        {
            _left = left;
            _right = right;
            _condition = condition;
        }

        protected override void Init()
        {
            _conditionMetLastFrame = _condition();
            _current = _conditionMetLastFrame ? _left.Invoke() : _right.Invoke();
        }

        protected override Status OnUpdate()
        {
            bool conditionMet = _condition();
        
            if (conditionMet != _conditionMetLastFrame)
            {
                _current.Cancel();
                // We're switching branches. Make a new node.
                _current = conditionMet ? _left.Invoke() : _right.Invoke();
            }

            _conditionMetLastFrame = conditionMet;
            return _current.Update();
        }

        protected override void OnCancel()
        {
            _current.Cancel();
        }
    }
}
