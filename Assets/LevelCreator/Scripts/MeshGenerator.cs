﻿using UnityEngine;
using System.Collections.Generic;
using Resources;
using Enums;

public class MeshGenerator : MonoBehaviour
{

    public SquareGrid squareGrid;
    List<Vector3> vertices;
    List<int> triangles;

    public void GenerateMesh(Tile[,] map, float squareSize, GameObject room, int roomId, bool onlyWalls = false)
    {
        squareGrid = new SquareGrid(map, squareSize, roomId, onlyWalls);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        room.GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 2:
                MeshFromPoints(square.centreRight, square.bottomRight, square.centreBottom);
                break;
            case 4:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }

    }
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);

    }
    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }
    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    void OnDrawGizmos()
    {/*
		if (squareGrid != null) {
			for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
				for (int y = 0; y < squareGrid.squares.GetLength(1); y ++) {

					Gizmos.color = (squareGrid.squares[x,y].topLeft.active)?Color.cyan : Color.white;
					Gizmos.DrawCube(squareGrid.squares[x,y].topLeft.position, Vector3.one * .5f);

					Gizmos.color = (squareGrid.squares[x,y].topRight.active)?Color.cyan : Color.white;
					Gizmos.DrawCube(squareGrid.squares[x,y].topRight.position, Vector3.one * .5f);

					Gizmos.color = (squareGrid.squares[x,y].bottomRight.active)?Color.cyan : Color.white;
					Gizmos.DrawCube(squareGrid.squares[x,y].bottomRight.position, Vector3.one * .5f);

					Gizmos.color = (squareGrid.squares[x,y].bottomLeft.active)?Color.cyan:Color.white;
					Gizmos.DrawCube(squareGrid.squares[x,y].bottomLeft.position, Vector3.one * .5f);

                    Gizmos.color = Color.gray;

                    Gizmos.DrawLine(squareGrid.squares[x, y].topLeft.position, squareGrid.squares[x, y].bottomLeft.position);
                    Gizmos.DrawLine(squareGrid.squares[x, y].bottomLeft.position, squareGrid.squares[x, y].bottomRight.position);
                    Gizmos.DrawLine(squareGrid.squares[x, y].bottomRight.position, squareGrid.squares[x, y].topRight.position);
                    Gizmos.DrawLine(squareGrid.squares[x, y].topRight.position, squareGrid.squares[x, y].topLeft.position);
                }
			}
		}*/
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(Tile[,] map, float squareSize, int id, bool OnlyWalls)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2, 0);
                    if (map[x,y] == null)
                    {
                        controlNodes[x, y] = new ControlNode(pos, false, squareSize);
                    } 
                    else if (OnlyWalls)
                    {
                        if (map[x, y].Type == tileType.Wall)
                        {
                            if (map[x, y].RoomId == id)
                            {
                                controlNodes[x, y] = new ControlNode(pos, true, squareSize);
                            }
                            else
                                controlNodes[x, y] = new ControlNode(pos, false, squareSize);
                        }
                        else
                            controlNodes[x, y] = new ControlNode(pos, false, squareSize);
                    }
                    else
                    {
                        if (map[x, y].RoomId == id)
                        {
                            controlNodes[x, y] = new ControlNode(pos, map[x,y].Type == tileType.Floor, squareSize);
                        }
                        else
                            controlNodes[x, y] = new ControlNode(pos, false, squareSize);
                    }
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }
    public class Square
    {

        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }

    }
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }
    public class ControlNode : Node
    {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.up * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}