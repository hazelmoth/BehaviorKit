# BehaviorKit

A tiny, easy-to-use .NET behavior tree library.

## Nodes
Behavior trees are built around a concept of Nodes. A Node has an update/tick function, which performs some action and then returns a status of Success, Failure, or Running.

Included are several built-in control nodes, which propagate ticks to their children:
- **Conditional**
- **Sequencer**
- **Repeater**
- **TimeLimit**
- **ImpatientRepeater** (A Repeater that will restart prematurely if its child takes too long.)

Used together, these nodes can facilitate complex logic. There are also a few basic leaf nodes:
- **Execute**, which just executes a given function one time.
- **Wait**, which does nothing for some duration (based on a provided clock).
- **InstantFailer**, which just fails in a single tick (useful for complex control flow).

## Usage
You can construct simple behavior trees with just the included nodes:
```
using BehaviorKit;
using BehaviorKit.Nodes;
public class Agent
{
    private Node node;
    
    public Agent(bool isEvil) {
        node = new Sequencer(
            () => new Execute(
                () => Console.WriteLine("Hello")),
            () => new Conditional(
                () => isEvil,
                () => new Execute(
                    () => Console.WriteLine("How would you like to die today?")),
                () => new Execute(
                    () => Console.WriteLine("You look great today."))));
    }
    
    public void Tick() {
        node.Update();
    }
}
```

For atomic behaviors that last more than one tick, you can define your own nodes. For example:
```
/// A node that outputs a string, one character per tick.
public class ScrollString : Node
{
    private readonly string _str;
    private int _nextChar;

    public ScrollString(string str)
    {
        _str = str;
        _charsLeft = str.Length;
    }

    protected override void Init() { }

    protected override void OnCancel() { }

    protected override Status OnUpdate()
    {
        if (_nextChar < _str.Length) {
            Console.Write(_str[_nextChar]);
            _nextChar++;
        }

        if (_nextChar == _str.Length) {
            Console.WriteLine();
            return Status.Success;
        } else {
            return Status.Running;
        }
    }
}
```

Which can then be used as such:
```
using BehaviorKit;
using BehaviorKit.Nodes;
public class Agent
{
    private Node node;
    
    public Agent(bool isEvil) {
        node = new Sequencer(
            () => new ScrollString("Hello."),
            () => new Conditional(
                () => isEvil,
                () => new ScrollString("How would you like to die today?"),
                () => new ScrollString("You look great today.")));
    }
    
    public void Tick() {
        node.Update();
    }
}
```

You can encapsulate entire trees into their own Node classes to make complex trees more readable. You can also define your very own control nodes.

...That's basically it. Have fun! 😉
