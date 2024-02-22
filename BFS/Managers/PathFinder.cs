using System.Collections.Generic;
using System.Threading.Tasks;
using BFSPathFinding.Models;
using Microsoft.Xna.Framework;

namespace BFSPathFinding.Managers;

public static class PathFinder
{
    class Node
    {
        public readonly int X;
        public readonly int Y;
        public Node Parent;
        public bool Visited;

        public Node(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    private static Node[,] _nodeMap;
        private static Map _map;
        private static readonly int[] row = { -1, 1, 0, 0 };  // Up, Down, Left, Right
        private static readonly int[] col = { 0, 0, -1, 1 };  // Corresponding to the rows


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
                    if (_map.Tiles[x,y].Blocked)
                    {
                        _nodeMap[x, y].Visited = true;
                    }
                }
            }
        }

        public static async Task<List<Vector2>> BFSearch(int goalX, int goalY)
        {
            CreateNodeMap();
            Queue<Node> q = new Queue<Node>();
            
            var start = _nodeMap[0, 0];
            start.Visited = true;
            q.Enqueue(start);

            while (q.Count > 0)
            {
                Node curr = q.Dequeue();

                if (curr.X == goalX && curr.Y == goalY) //We found the path!
                {
                    return RetracePath(goalX,goalY);
                }

                for (int i = 0; i < row.Length; i++)
                {
                    int newX = curr.X + row[i];
                    int newY = curr.Y + col[i];

                    if (IsValid(newX,newY) && !_nodeMap[newX,newY].Visited)
                    {
                        q.Enqueue(_nodeMap[newX,newY]);
                        _nodeMap[newX,newY].Visited = true;
                        _map.Tiles[newX, newY].Visited = true;
                        _nodeMap[newX,newY].Parent = curr;
                    }
                }

                await Task.Delay(25);
            }

            return null; //If nothing found return null
        }

        private static List<Vector2> RetracePath(int goalX, int goalY)
        {
            Stack<Vector2> stack = new Stack<Vector2>();
            List<Vector2> result = new List<Vector2>();
            Node curr = _nodeMap[goalX, goalY];

            while (curr is not null) //retrace back with parent like a chain going end to start. That's why we using stack. Because we going from end to start. 
            {
                _map.Tiles[curr.X, curr.Y].Path = true;
                stack.Push(_map.Tiles[curr.X,curr.Y].Position); //push to current mao's position to stack. Those will be path locations. 
                curr = curr.Parent;
            }

            while (stack.Count > 0) //reverse the stack and add to result
            {
                result.Add(stack.Pop());
            }

            return result;
        }
    }