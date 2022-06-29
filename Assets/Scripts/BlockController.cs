using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    const int BlocksCount = 6;
    const int NarrowPathCount = 10;
    const int VertexesCount = 13;
    private static readonly List<int> BlocksList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    private static readonly List<int[]> ConnectionList = new List<int[]>()
    {
        new int[] { 1, 8, 9, 12 },
        new int[] { 0, 2, 3 },
        new int[] { 1, 5 },
        new int[] { 1, 4, 8 },
        new int[] { 3, 5, 6},
        new int[] { 2, 4 },
        new int[] { 4, 7, 9 },
        new int[] { 6, 10, 11 },
        new int[] { 0, 3, 9 },
        new int[] { 0, 6, 8, 10 },
        new int[] { 7, 9, 11 },
        new int[] { 7, 10, 12 },
        new int[] { 0, 11 },
    };

    private static readonly int[,] BlockMatrix = new int[NarrowPathCount, 2]
    {
        { 11, 12 },
        { 10, 11 },
        { 7, 10 },
        { 0, 9 },
        { 6, 9 },
        { 4, 6 },
        { 3, 4 },
        { 2, 5 },
        { 3, 8 },
        { 0, 1 }
    };

    private static int[,] ConnectionMatrix = new int[VertexesCount, VertexesCount];

    public static void GenerateBlocks()
    {
        foreach (var blockId in BlocksList)
        {
            GameObject.Find(IdToBlockName(blockId)).SetActive(true);
        }
        int[,] E = new int[VertexesCount, VertexesCount];
        for (var i = 0; i < VertexesCount; i++)
        {
            E[i, i] = 1;
        }

        for (var cntr = 0; cntr < 100; cntr++)
        {
            var blockIdList = BlocksList.OrderBy(x => Guid.NewGuid()).Take(BlocksCount).ToList();
            for (var i = 0; i < VertexesCount; i++)
            {
                foreach (var j in ConnectionList[i])
                {
                    ConnectionMatrix[i, j] = 1;
                }
            }

            foreach (var id in blockIdList)
            {
                ConnectionMatrix[BlockMatrix[id, 0], BlockMatrix[id, 1]] = 0;
                ConnectionMatrix[BlockMatrix[id, 1], BlockMatrix[id, 0]] = 0;
            }

            var A2 = BoolMultiply(ConnectionMatrix, ConnectionMatrix);
            var A3 = BoolMultiply(A2, ConnectionMatrix);
            var A4 = BoolMultiply(A3, ConnectionMatrix);
            var A5 = BoolMultiply(A4, ConnectionMatrix);

            var G = BoolAdd(new List<int[,]> { E, ConnectionMatrix, A2, A3, A4, A5 });
            var isReachable = CheckReachability(G);
            if (isReachable)
            {
                foreach (var blockId in BlocksList)
                {
                    if (!blockIdList.Contains(blockId))
                        GameObject.Find(IdToBlockName(blockId)).SetActive(false);
                }

                return;
            }
        }
    }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBlocks();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static string IdToBlockName(int id) => id == 0 ? "BLock" : $"BLock_{id}";

    // Check reachability from 0 (home) point only.
    private static bool CheckReachability(int[,] A)
    {
        int cA = A.GetLength(1);
        for (int j = 0; j < cA; j++)
        {
            if (A[0, j] == 0)
                return false;
        }

        return true;
    }

    private static int[,] BoolAdd(List<int[,]> matrixList)
    {
        int rA = matrixList[0].GetLength(0);
        int cA = matrixList[0].GetLength(1);
        int[,] result = new int[rA, cA];
        foreach (var A in matrixList)
        {
            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cA; j++)
                {
                    result[i, j] += A[i, j];
                }
            }
        }

        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cA; j++)
            {
                result[i, j] = result[i, j] > 0 ? 1 : 0;
            }
        }

        return result;
    }

    private static int[,] BoolMultiply(int[,] A, int[,] B)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int cB = B.GetLength(1);
        int[,] result = new int[rA, cB];
        for (int i = 0; i < rA; i++)
        {
            for (int j = 0; j < cB; j++)
            {
                int temp = 0;
                for (int k = 0; k < cA; k++)
                {
                    temp += A[i, k] * B[k, j];
                }

                result[i, j] = temp > 0 ? 1 : 0;
            }
        }

        return result;
    }
}
