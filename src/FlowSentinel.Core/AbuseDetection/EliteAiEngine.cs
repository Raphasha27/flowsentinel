using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowSentinel.Core.AbuseDetection;

/// <summary>
/// The Elite AI Engine handles complex pathfinding and trajectory analysis
/// for FlowSentinel's high-level defense mechanisms.
/// Uses A* with Manhattan heuristics and Flood-Fill safety fallbacks.
/// </summary>
public class EliteAiEngine
{
    public record Position(int X, int Y);

    private class Node : IComparable<Node>
    {
        public Position Pos { get; }
        public double G { get; set; } // Cost from start
        public double H { get; set; } // Heuristic
        public double F => G + H;
        public Node? Parent { get; set; }

        public Node(Position pos) => Pos = pos;

        public int CompareTo(Node? other) => F.CompareTo(other?.F ?? 0);
    }

    /// <summary>
    /// Calculates the optimal path between two points using A*.
    /// </summary>
    public List<Position> FindPath(Position start, Position target, HashSet<Position> obstacles, int width, int height)
    {
        var openSet = new List<Node> { new Node(start) { G = 0, H = GetHeuristic(start, target) } };
        var closedSet = new HashSet<Position>();

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => n.F).First();
            if (current.Pos == target) return RetracePath(current);

            openSet.Remove(current);
            closedSet.Add(current.Pos);

            foreach (var neighborPos in GetNeighbors(current.Pos, width, height))
            {
                if (obstacles.Contains(neighborPos) || closedSet.Contains(neighborPos)) continue;

                var newG = current.G + 1;
                var neighborNode = openSet.FirstOrDefault(n => n.Pos == neighborPos);

                if (neighborNode == null || newG < neighborNode.G)
                {
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighborPos);
                        openSet.Add(neighborNode);
                    }
                    neighborNode.G = newG;
                    neighborNode.H = GetHeuristic(neighborPos, target);
                    neighborNode.Parent = current;
                }
            }
        }

        return new List<Position>(); // No path found
    }

    /// <summary>
    /// Fallback mechanism: Flood-fill to find the largest safe area.
    /// Used when A* fails to reach the primary target.
    /// </summary>
    public int CalculateSafetyScore(Position start, HashSet<Position> obstacles, int width, int height)
    {
        var visited = new HashSet<Position>();
        var queue = new Queue<Position>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var n in GetNeighbors(current, width, height))
            {
                if (!obstacles.Contains(n) && !visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
        }

        return visited.Count;
    }

    private double GetHeuristic(Position a, Position b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private IEnumerable<Position> GetNeighbors(Position pos, int width, int height)
    {
        if (pos.X > 0) yield return pos with { X = pos.X - 1 };
        if (pos.X < width - 1) yield return pos with { X = pos.X + 1 };
        if (pos.Y > 0) yield return pos with { Y = pos.Y - 1 };
        if (pos.Y < height - 1) yield return pos with { Y = pos.Y + 1 };
    }

    private List<Position> RetracePath(Node node)
    {
        var path = new List<Position>();
        var curr = node;
        while (curr != null)
        {
            path.Add(curr.Pos);
            curr = curr.Parent;
        }
        path.Reverse();
        return path;
    }
}
