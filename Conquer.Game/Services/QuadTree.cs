using System.Drawing;

namespace Conquer.Game.Services;

public class QuadTree<T> where T : class
{
    private Rectangle _bounds;
    private Quadrant? _root;
    private Dictionary<T, Quadrant>? _table;

    public Rectangle Bounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            ReIndex();
        }
    }

    public void Insert(T node, Rectangle bounds)
    {
        if (_bounds.Width == 0 || _bounds.Height == 0)
        {
            throw new ArgumentException("Bounds must be non zero.", nameof(bounds));
        }

        if (bounds.Width == 0 || bounds.Height == 0)
        {
            throw new ArgumentException("Bounds must be non zero.", nameof(bounds));
        }

        _root ??= new(_bounds);
        var parent = _root.Insert(node, bounds);
        _table ??= new();
        _table[node] = parent;
    }

    public IEnumerable<T> GetNodesInside(Rectangle bounds)
    {
        return GetNodes(bounds).Select(n => n.Node);
    }

    public bool HasNodesInside(Rectangle bounds) => _root?.HasIntersectingNodes(bounds) == true;

    private IEnumerable<QuadNode> GetNodes(Rectangle bounds)
    {
        var result = new List<QuadNode>();
        _root?.GetIntersectingNodes(result, bounds);
        return result;
    }

    public bool Remove(T node)
    {
        if (_table == null || !_table.TryGetValue(node, out var parent))
        {
            return false;
        }

        parent.RemoveNode(node);
        _table.Remove(node);
        return true;
    }

    private void ReIndex()
    {
        var nodes = GetNodes(_bounds);
        _root = null;
        foreach (var n in nodes)
        {
            Insert(n.Node, n.Bounds);
        }
    }

    internal class QuadNode
    {
        public QuadNode(T node, Rectangle bounds)
        {
            Node = node;
            Bounds = bounds;
        }

        public T Node { get; set; }
        public Rectangle Bounds { get; }
        public QuadNode? Next { get; set; }
    }

    internal class Quadrant
    {
        private readonly Rectangle _bounds;
        private Quadrant? _bottomLeft;
        private Quadrant? _bottomRight;
        private QuadNode? _nodes;
        private Quadrant? _topLeft;
        private Quadrant? _topRight;

        public Quadrant(Rectangle bounds)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                throw new ArgumentException("Bounds must be non zero.", nameof(bounds));
            }

            _bounds = bounds;
        }

        internal Rectangle Bounds => _bounds;

        internal Quadrant Insert(T node, Rectangle bounds)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                throw new ArgumentException("Bounds must be non zero.", nameof(bounds));
            }

            var toInsert = this;
            while (true)
            {
                var w = toInsert._bounds.Width / 2;
                var h = toInsert._bounds.Height / 2;

                var topLeft = new Rectangle(toInsert._bounds.Left, toInsert._bounds.Top, w, h);
                var topRight = new Rectangle(toInsert._bounds.Left + w, toInsert._bounds.Top, w, h);
                var bottomLeft = new Rectangle(toInsert._bounds.Left, toInsert._bounds.Top + h, w, h);
                var bottomRight = new Rectangle(toInsert._bounds.Left + w, toInsert._bounds.Top + h, w, h);

                Quadrant? child = null;

                if (w != 0 && h != 0)
                {
                    if (topLeft.Contains(bounds))
                    {
                        toInsert._topLeft ??= new(topLeft);
                        child = toInsert._topLeft;
                    }
                    else if (topRight.Contains(bounds))
                    {
                        toInsert._topRight ??= new(topRight);
                        child = toInsert._topRight;
                    }
                    else if (bottomLeft.Contains(bounds))
                    {
                        toInsert._bottomLeft ??= new(bottomLeft);
                        child = toInsert._bottomLeft;
                    }
                    else if (bottomRight.Contains(bounds))
                    {
                        toInsert._bottomRight ??= new(bottomRight);
                        child = toInsert._bottomRight;
                    }
                }

                if (child != null)
                {
                    toInsert = child;
                }
                else
                {
                    var n = new QuadNode(node, bounds);
                    if (toInsert._nodes == null)
                    {
                        n.Next = n;
                    }
                    else
                    {
                        var x = toInsert._nodes;
                        n.Next = x.Next;
                        x.Next = n;
                    }

                    toInsert._nodes = n;
                    return toInsert;
                }
            }
        }

        internal void GetIntersectingNodes(List<QuadNode> nodes, Rectangle bounds)
        {
            if (bounds.IsEmpty)
            {
                return;
            }

            var w = _bounds.Width / 2;
            var h = _bounds.Height / 2;

            var topLeft = new Rectangle(_bounds.Left, _bounds.Top, w, h);
            var topRight = new Rectangle(_bounds.Left + w, _bounds.Top, w, h);
            var bottomLeft = new Rectangle(_bounds.Left, _bounds.Top + h, w, h);
            var bottomRight = new Rectangle(_bounds.Left + w, _bounds.Top + h, w, h);

            if (topLeft.IntersectsWith(bounds) && _topLeft != null)
            {
                _topLeft.GetIntersectingNodes(nodes, bounds);
            }

            if (topRight.IntersectsWith(bounds) && _topRight != null)
            {
                _topRight.GetIntersectingNodes(nodes, bounds);
            }

            if (bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
            {
                _bottomLeft.GetIntersectingNodes(nodes, bounds);
            }

            if (bottomRight.IntersectsWith(bounds) && _bottomRight != null)
            {
                _bottomRight.GetIntersectingNodes(nodes, bounds);
            }

            GetIntersectingNodes(_nodes, nodes, bounds);
        }

        private static void GetIntersectingNodes(QuadNode? last, List<QuadNode> nodes, Rectangle bounds)
        {
            if (last == null)
            {
                return;
            }

            var n = last;
            do
            {
                n = n?.Next;
                if (n?.Bounds.IntersectsWith(bounds) == true)
                {
                    nodes.Add(n);
                }
            } while (n != last);
        }

        internal bool HasIntersectingNodes(Rectangle bounds)
        {
            if (bounds.IsEmpty)
            {
                return false;
            }

            var w = _bounds.Width / 2;
            var h = _bounds.Height / 2;

            var topLeft = new Rectangle(_bounds.Left, _bounds.Top, w, h);
            var topRight = new Rectangle(_bounds.Left + w, _bounds.Top, w, h);
            var bottomLeft = new Rectangle(_bounds.Left, _bounds.Top + h, w, h);
            var bottomRight = new Rectangle(_bounds.Left + w, _bounds.Top + h, w, h);

            var found = false;

            if (topLeft.IntersectsWith(bounds) && _topLeft != null)
            {
                found = _topLeft.HasIntersectingNodes(bounds);
            }

            if (!found && topRight.IntersectsWith(bounds) && _topRight != null)
            {
                found = _topRight.HasIntersectingNodes(bounds);
            }

            if (!found && bottomLeft.IntersectsWith(bounds) && _bottomLeft != null)
            {
                found = _bottomLeft.HasIntersectingNodes(bounds);
            }

            if (!found && bottomRight.IntersectsWith(bounds) && _bottomRight != null)
            {
                found = _bottomRight.HasIntersectingNodes(bounds);
            }

            if (!found)
            {
                found = HasIntersectingNodes(_nodes, bounds);
            }

            return found;
        }

        private static bool HasIntersectingNodes(QuadNode? last, Rectangle bounds)
        {
            if (last == null)
            {
                return false;
            }

            var n = last;
            do
            {
                n = n?.Next;
                if (n?.Bounds.IntersectsWith(bounds) == true)
                {
                    return true;
                }
            } while (n != last);

            return false;
        }

        internal bool RemoveNode(T node)
        {
            var rc = false;

            if (_nodes == null)
            {
                return rc;
            }

            var p = _nodes;
            while (p?.Next?.Node != node && p?.Next != _nodes)
            {
                p = p?.Next;
            }

            if (p.Next.Node != node)
            {
                return rc;
            }

            rc = true;
            var n = p.Next;
            if (p == n)
            {
                _nodes = null;
            }
            else
            {
                if (_nodes == n)
                {
                    _nodes = p;
                }

                p.Next = n.Next;
            }

            return rc;
        }
    }
}