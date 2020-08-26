using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PacMan.CharacterMovement.Enemy
{
    public static class Pathfinding
    {
        public static List<Vector3Int> GetPath(Vector3Int currentPosition, Vector3Int targetPosition, LayerMask movementLayer, Vector3Int bannedNode)
        {
            int cutoff = 0;
            
            var frontier = new PriorityQueue();
            frontier.Enqueue(currentPosition, 0);

            var cameFrom = new Dictionary<Vector3Int, Vector3Int> {[currentPosition] = Vector3Int.zero};
            var costSoFar = new Dictionary<Vector3Int, float> {[currentPosition] = 0};

            while (frontier.Count > 0 && cutoff < 30)
            {
                cutoff++;

                var current = frontier.Dequeue();

                if (current.Key == targetPosition)
                {
                    break;
                }

                var successors = GetConnectedNodes(current.Key, movementLayer, bannedNode);
                foreach (var next in successors)
                {
                    var newCost = costSoFar[current.Key] + 1;
                    if (!costSoFar.ContainsKey(next) || (costSoFar.ContainsKey(next) && costSoFar[next] > newCost))
                    {
                        costSoFar[next] = newCost;
                        frontier.Enqueue(next, GetCost(next, targetPosition));
                        cameFrom[next] = current.Key;
                    }
                }
            }

            var pathList = new List<Vector3Int>();

            if (cameFrom.Count > 0)
            {
                var node = cameFrom.Last().Key;
                while (node != Vector3Int.zero)
                {
                    pathList.Add(node);
                    node = cameFrom[node];
                }
            }

            pathList.Reverse();
            return pathList;
        }

        private static List<Vector3Int> GetConnectedNodes(Vector3Int currentNode, LayerMask movementLayer, Vector3Int bannedNode)
        {
            var allNodes = new List<Vector3Int>();
            allNodes.Add(currentNode + Vector3Int.left);
            allNodes.Add(currentNode + Vector3Int.up);
            allNodes.Add(currentNode + Vector3Int.right);
            allNodes.Add(currentNode + Vector3Int.down);

            var validNodes = new List<Vector3Int>();
            foreach (var node in allNodes)
            {
                if (node != bannedNode && Physics2D.OverlapCircle(new Vector3(node.x, node.y, node.z), 0.2f, movementLayer))
                {
                    validNodes.Add(node);
                }
            }

            return validNodes;
        }

        private static float GetCost(Vector3Int currentNode, Vector3Int targetNode)
        {
            return Math.Abs(currentNode.x - targetNode.x) + Math.Abs(currentNode.y - targetNode.y);
        }
    }

    public class PriorityQueue
    {
        private readonly List<KeyValuePair<Vector3Int, float>> _elements = new List<KeyValuePair<Vector3Int, float>>();

        public int Count => _elements.Count;

        public void Enqueue(Vector3Int item, float priority)
        {
            _elements.Add(new KeyValuePair<Vector3Int, float>(item, priority));
        }

        // Returns the Location that has the lowest priority
        public KeyValuePair<Vector3Int, float> Dequeue()
        {
            var bestIndex = 0;

            for (var i = 0; i < _elements.Count; i++)
            {
                if (_elements[i].Value < _elements[bestIndex].Value)
                {
                    bestIndex = i;
                }
            }

            var bestItem = _elements[bestIndex];
            _elements.RemoveAt(bestIndex);
            return bestItem;
        }

        public float AtSamePosition(Vector3Int element)
        {
            foreach (var e in _elements)
            {
                if (Vector3.Distance(e.Key, element) < 0.2f)
                {
                    return e.Value;
                }
            }

            return -1f;
        }

        public List<Vector3Int> Keys => _elements.Select((e) => e.Key).ToList();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var e in _elements)
            {
                var key = e.Key;
                sb.Append($"({key.x}, {key.y}), {e.Value} ");
            }

            return sb.ToString();
        }
    }
}