using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class Algoritm : BaseUnitPath
    {
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };

        public Algoritm(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(
    runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            path = FindPath().ToArray();

            if (path == null)
                path = new Vector2Int[] { StartPoint };

            List<Vector2Int> FindPath()
            {
                Node startNode = new Node(startPoint);
                Node targetNode = new Node(endPoint);

                List<Node> openList = new List<Node> { startNode };
                List<Node> closedList = new List<Node>();

                while (openList.Count > 0)
                {
                    Node currentNode = openList[0];

                    foreach (var node in openList)
                    {
                        if (node.Value < currentNode.Value)
                            currentNode = node;
                    }

                    openList.Remove(currentNode);
                    closedList.Add(currentNode);

                    if (currentNode.Position.x == targetNode.Position.x && currentNode.Position.y == targetNode.Position.y)
                    {
                        List<Vector2Int> path = new List<Vector2Int>();
                        while (currentNode != null)
                        {
                            path.Add(currentNode.Position);
                            currentNode = currentNode.Parent;
                        }

                        path.Reverse();
                        return path;
                    }

                    for (int i = 0; i < dx.Length; i++)
                    {
                        Vector2Int newPos = new Vector2Int(currentNode.Position.x + dx[i], currentNode.Position.y + dy[i]);

                        if (!runtimeModel.IsTileWalkable(newPos) && endPoint != newPos)
                            continue;

                        Node neighbor = new Node(newPos);

                        if (closedList.Contains(neighbor))
                            continue;

                        neighbor.Parent = currentNode;
                        neighbor.CalculateEstimate(targetNode.Position.x, targetNode.Position.y);
                        neighbor.CalculateValue();

                        openList.Add(neighbor);
                    }
                }

                return null;
            }
        }

        public class Node
        {
            public Vector2Int Position;
            public int Cost;
            public int Estimate;
            public int Value;
            public Node Parent;

            public Node(Vector2Int position)
            {
                Position = position;
            }

            public void CalculateEstimate(int targetX, int targetY)
            {
                Estimate = Mathf.Abs(Position.x - targetX) + Mathf.Abs(Position.y - targetY);
            }

            public void CalculateValue()
            {
                Value = Cost + Estimate;
            }

            public override bool Equals(object obj)
            {
                if (obj is not Node node)
                    return false;

                return Position.Equals(node.Position);
            }
        }
    } 
}