using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourKit;

namespace BehaviorKit.Nodes
{
    /// Executes multiple tasks in sequence, updating each Node until it returns Success.
    /// Nodes are created only when the respective task is reached in the sequence.
    /// Immediately returns Failure if any Node fails. Returns Success if all Nodes return
    /// Success.
    public class Sequencer : Node
    {
        private readonly IList<Func<Node>> _sequence;
        private int _index;
        private Node _current;
        private Status _lastStatus;

        /// Takes a sequence of functions, each of which returns the Node to
        /// be executed at that point in the sequence.
        public Sequencer(params Func<Node>[] nodes)
        {
            if (nodes.Length == 0)
            {
                throw new ArgumentException("Sequencer must have at least one node");
            }
            _sequence = nodes.ToList();
        }

        protected override void Init()
        {
            _index = 0;
            _current = _sequence[0].Invoke();
            _lastStatus = Status.Running;
        }

        protected override void OnCancel()
        {
            if (_current != null && !_current.Stopped) _current.Cancel();
        }

        protected override Status OnUpdate()
        {
            _lastStatus = _current.Update();
            
            if (_lastStatus == Status.Success)
            {
                _index++;
                if (_index >= _sequence.Count) return Status.Success;
                _current = _sequence[_index].Invoke();
            }

            return _lastStatus == Status.Failure
                ? Status.Failure 
                : Status.Running;
        }
    }
}
