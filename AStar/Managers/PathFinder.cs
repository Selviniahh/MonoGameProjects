using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AStar.Models;
using Microsoft.Xna.Framework;

namespace AStar.Managers;

public static class PathFinder
{
    class Node
    {
        public readonly int X; //Relative indexes
        public readonly int Y; //Relative indexes
        public Node Parent;
        public bool Visited;

        //A* implementation
        public int Hcost = 0; // Current to goal cost. We need to manually calculate HCost with using GoalX and GoalY. 
        public int GCost = 0; // Current to Start cost. Every time we move, it means we are adding +1 to GCost for the current node. 
        public int FCost => GCost + Hcost;
        public static int GetHCost(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    private static Node[,] _nodeMap;
    private static Map _map;
    private static readonly int[] row = { -1, 1, 0, 0 }; //  Right, Left, Up, Down
    private static readonly int[] col = { 0, 0, -1, 1 }; // Corresponding to the rows


    public static void Init(Map map)
    {
        _map = map;
    }

    private static bool IsValid(int x, int y)
    {
        return x >= 0 && x < _nodeMap.GetLength(0) && y >= 0 && y < _nodeMap.GetLength(1);
    }

    private static void CreateNodeMap()
    {
        _nodeMap = new Node[_map.Size.X, _map.Size.Y];

        for (int x = 0; x < _map.Size.X; x++)
        {
            for (int y = 0; y < _map.Size.Y; y++)
            {
                _map.Tiles[x, y].Path = false;
                _map.Tiles[x, y].Visited = false;
                _nodeMap[x, y] = new Node(x, y);
                if (_map.Tiles[x, y].Blocked)
                {
                    _nodeMap[x, y].Visited = true;
                }
            }
        }
    }

    public static async void AStar(int goalX, int goalY)
    {
        CreateNodeMap();
        List<Node> openList = new List<Node>(); // List of nodes
        (int startX, int startY) = _map.ScreenToMap(new Vector2(0, 0)); // Start at first tile

        // Determine the start, mark it visited, and calculate the H cost.
        Node start = _nodeMap[startX, startY];
        start.Visited = true;
        start.Hcost = Node.GetHCost(startX, startY, goalX, goalY);
        openList.Add(start); // We added something to the list so we can start while loop

        while (openList.Count > 0)
        {
            Node curr = openList.OrderBy(n => n.FCost).First(); // Find the lowest Fcost one inside node list
            openList.Remove(curr);
            if (curr.X == goalX && curr.Y == goalY) // If the path is found, retrace
            {
                RetracePath(goalX, goalY);
                return;
            }

            for (int i = 0; i < row.Length; i++)
            {
                int newX = curr.X + row[i];
                int newY = curr.Y + col[i];

                if (IsValid(newX, newY) && !_nodeMap[newX, newY].Visited) // Valid and not visited?
                {
                    Node neighbour = _nodeMap[newX, newY];
                    int turnPenalty = 5;

                    // Calculate the turn penalty if there is a parent and the direction changes
                    if (curr.Parent != null && (curr.X - curr.Parent.X != row[i] || curr.Y - curr.Parent.Y != col[i]))
                    {
                        turnPenalty = 5; // Arbitrary penalty for turning
                    }
                    
                    int costToMove = curr.GCost + 1 + turnPenalty; // Moving costs +1, and turning might add penalty

                    // If costToMove is less than the neighbour's GCost or the neighbour is not in openList
                    if (costToMove < neighbour.GCost || !openList.Contains(neighbour))
                    {
                        neighbour.GCost = costToMove;
                        neighbour.Hcost = Node.GetHCost(newX, newY, goalX, goalY);
                        neighbour.Parent = curr;
                        _map.Tiles[newX, newY].VisitColor = Color.DarkGray;
                        openList.Add(neighbour);
                        _nodeMap[newX, newY].Visited = true;
                        _map.Tiles[newX, newY].Visited = true;
                    }
                }
            }

        }

        // If nothing is found, return
        return;
    }


    private static List<Vector2> RetracePath(int goalX, int goalY)
    {
        Stack<Vector2> stack = new Stack<Vector2>();
        List<Vector2> result = new List<Vector2>();
        Node curr = _nodeMap[goalX, goalY];

        while (curr is not null) //retrace back with parent like a chain going end to start. That's why we using stack. Because we going from end to start. 
        {
            _map.Tiles[curr.X, curr.Y].Path = true;
            stack.Push(_map.Tiles[curr.X, curr.Y].Position); //push to current mao's position to stack. Those will be path locations. 
            curr = curr.Parent;
        }

        while (stack.Count > 0) //reverse the stack and add to result
        {
            result.Add(stack.Pop());
        }

        return result;
    }
}