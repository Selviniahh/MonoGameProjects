using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AStar.Models;
using Microsoft.Xna.Framework;

namespace AStar.Managers
{
    public static class PathFinder
    {
        class Node : IComparable<Node>
        {
            public int X { get; }
            public int Y { get; }
            public Node Parent { get; set; }
            public int GCost { get; set; }
            public int HCost { get; set; }
            public int FCost => GCost + HCost;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int CompareTo(Node other)
            {
                int compare = FCost.CompareTo(other.FCost);
                if (compare == 0)
                    compare = HCost.CompareTo(other.HCost);
                return compare;
            }
        }

        private static Map _map;
        private static readonly int[] dx = { -1, 1, 0, 0 }; // Left, Right, Up, Down
        private static readonly int[] dy = { 0, 0, -1, 1 };

        public static void Init(Map map)
        {
            _map = map;
        }

        public static async void AStar(int goalX, int goalY)
        {
            // Clear previous path and visited tiles
            ResetMapTiles();

            var openSet = new PriorityQueue<Node, int>();
            var cameFrom = new Dictionary<(int, int), Node>();
            var gScore = new Dictionary<(int, int), int>();
            var openSetHash = new HashSet<(int, int)>();
            var closedSet = new HashSet<(int, int)>();

            (int startX, int startY) = _map.ScreenToMap(new Vector2(0, 0));
            var startNode = new Node(startX, startY);
            gScore[(startX, startY)] = 0;
            startNode.HCost = Heuristic(startX, startY, goalX, goalY);
            openSet.Enqueue(startNode, startNode.FCost);
            openSetHash.Add((startX, startY));

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                openSetHash.Remove((current.X, current.Y));
                closedSet.Add((current.X, current.Y));

                // Mark current tile as visited for visualization
                _map.Tiles[current.X, current.Y].Visited = true;
                _map.Tiles[current.X, current.Y].VisitColor = Color.LightGray;

                if (current.X == goalX && current.Y == goalY)
                {
                    RetracePath(cameFrom, current);
                    return;
                }

                for (int i = 0; i < dx.Length; i++)
                {
                    int neighborX = current.X + dx[i];
                    int neighborY = current.Y + dy[i];

                    if (!IsValid(neighborX, neighborY))
                        continue;

                    if (_map.Tiles[neighborX, neighborY].Blocked)
                        continue;

                    if (closedSet.Contains((neighborX, neighborY)))
                        continue;

                    int tentativeGScore = gScore[(current.X, current.Y)] + 1;

                    bool isInOpenSet = openSetHash.Contains((neighborX, neighborY));

                    if (!isInOpenSet || tentativeGScore < gScore.GetValueOrDefault((neighborX, neighborY), int.MaxValue))
                    {
                        cameFrom[(neighborX, neighborY)] = current;
                        gScore[(neighborX, neighborY)] = tentativeGScore;
                        int hCost = Heuristic(neighborX, neighborY, goalX, goalY);
                        var neighborNode = new Node(neighborX, neighborY)
                        {
                            Parent = current,
                            GCost = tentativeGScore,
                            HCost = hCost
                        };

                        if (!isInOpenSet)
                        {
                            openSet.Enqueue(neighborNode, neighborNode.FCost);
                            openSetHash.Add((neighborX, neighborY));

                            // For visualization: Mark the neighbor tile as being in the open set
                            _map.Tiles[neighborX, neighborY].VisitColor = Color.DarkGray;
                        }
                        else
                        {
                            // Update the priority in the queue
                            // Since PriorityQueue doesn't support decrease-key operation, we can re-enqueue the node
                            openSet.Enqueue(neighborNode, neighborNode.FCost);
                        }
                    }
                }

                // Include delay for visualization
                await Task.Delay(15);
            }

            // No path found
        }

        private static void ResetMapTiles()
        {
            for (int y = 0; y < _map.Size.Y; y++)
            {
                for (int x = 0; x < _map.Size.X; x++)
                {
                    _map.Tiles[x, y].Visited = false;
                    _map.Tiles[x, y].Path = false;
                    _map.Tiles[x, y].VisitColor = Color.White;
                }
            }
        }

        private static int Heuristic(int x1, int y1, int x2, int y2)
        {
            // Manhattan distance
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < _map.Size.X && y >= 0 && y < _map.Size.Y;
        }

        private static void RetracePath(Dictionary<(int, int), Node> cameFrom, Node current)
        {
            var path = new List<Node>();
            while (cameFrom.ContainsKey((current.X, current.Y)))
            {
                path.Add(current);
                current = cameFrom[(current.X, current.Y)];
            }
            path.Add(current);
            path.Reverse();

            foreach (var node in path)
            {
                _map.Tiles[node.X, node.Y].Path = true;
            }
        }
    }
}
