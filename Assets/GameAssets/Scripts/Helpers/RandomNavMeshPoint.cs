using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// SOURCE: https://www.reddit.com/r/Unity3D/comments/377c2u/share_getting_a_random_point_on_the_navmesh/
/// CREDIT: u/ccKep on Reddit
/// </summary>

public class RandomNavMeshPoint : Singleton<RandomNavMeshPoint>
{
    private static NavMeshTriangulation nav;
    private static Mesh mesh;
    private static float totalArea;

    private void Awake()
    {
        nav = NavMesh.CalculateTriangulation();
        mesh = new Mesh();
        mesh.vertices = nav.vertices;
        mesh.triangles = nav.indices;

        totalArea = 0.0f;
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            totalArea += GetTriangleArea(i);
        }
    }

    /** Get a random triangle on the NavMesh
      * Steps:
      * 1. Get a random triangle on the mesh (weighted by it's area)
      * 2. Get a random point inside that triangle
      */
    public static Vector3 GetRandomPointOnNavMesh()
    {
        int triangle = GetRandomTriangleOnNavMesh();
        return GetRandomPointOnTriangle(triangle);
    }

    /** Get a random triangle on the NavMesh that has connectivity to a starting point
      * Steps:
      * 1. Get a random triangle on the mesh (weighted by it's area), connected to the triangle of startingPoint
      * 2. Get a random point inside that triangle
      */
    public static Vector3 GetConnectedPointOnNavMesh(Vector3 startingPoint)
    {
        int triangle = GetRandomConnectedTriangleOnNavMesh(startingPoint);
        return GetRandomPointOnTriangle(triangle);
    }

    /** Grabs a random triangle in the mesh, weighted by size so random point distribution is even
      *
      */
    private static int GetRandomTriangleOnNavMesh()
    {
        float rnd = Random.Range(0, totalArea);
        int nTriangles = mesh.triangles.Length / 3;
        for (int i = 0; i < nTriangles; i++)
        {
            rnd -= GetTriangleArea(i);
            if (rnd <= 0)
                return i;
        }
        return 0;
    }

    /** Grabs a random triangle in the mesh (connected to p), weighted by size so random point distribution is even
      *
      */
    private static int GetRandomConnectedTriangleOnNavMesh(Vector3 p)
    {
        // Check for triangle connectivity and calculate total area of all *connected* triangles
        int nTriangles = mesh.triangles.Length / 3;
        float tArea = 0.0f;
        List<int> connectedTriangles = new List<int>();
        NavMeshPath path = new NavMeshPath();
        for (int i = 0; i < nTriangles; i++)
        {
            path.ClearCorners();
            if (NavMesh.CalculatePath(p, mesh.vertices[mesh.triangles[3 * i + 0]], NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    tArea += GetTriangleArea(i);
                    connectedTriangles.Add(i);
                }
            }
        }

        float rnd = Random.Range(0, tArea);

        foreach (int i in connectedTriangles)
        {
            rnd -= GetTriangleArea(i);
            if (rnd <= 0)
                return i;
        }
        return 0;
    }


    /** Gets a random point on a triangle.
      * 
      * @var int idx THe triangle index in the NavMesh
      */
    private static Vector3 GetRandomPointOnTriangle(int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        // Generate a random point in the trapezoid
        Vector3 result = v[0] + Random.Range(0f, 1f) * a + Random.Range(0f, 1f) * b;

        // Barycentric coordinates on triangles
        float alpha = ((v[1].z - v[2].z) * (result.x - v[2].x) + (v[2].x - v[1].x) * (result.z - v[2].z)) /
                ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float beta = ((v[2].z - v[0].z) * (result.x - v[2].x) + (v[0].x - v[2].x) * (result.z - v[2].z)) /
               ((v[1].z - v[2].z) * (v[0].x - v[2].x) + (v[2].x - v[1].x) * (v[0].z - v[2].z));
        float gamma = 1.0f - alpha - beta;

        // The selected point is outside of the triangle (wrong side of the trapezoid), project it inside through the center.
        if (alpha < 0 || beta < 0 || gamma < 0)
        {
            Vector3 center = v[0] + c / 2;
            center = center - result;
            result += 2 * center;
        }

        return result;
    }

    /** Helper function to calculate the area of a triangle.
      * Used as weights when selecting a random triangle so bigger triangles have a higher chance (hence yielding an even distribution of points on the entire mesh)
      *
      * @var int idx The index of the triangle to calculate the area of
      */
    private static float GetTriangleArea(int idx)
    {
        Vector3[] v = new Vector3[3];


        v[0] = mesh.vertices[mesh.triangles[3 * idx + 0]];
        v[1] = mesh.vertices[mesh.triangles[3 * idx + 1]];
        v[2] = mesh.vertices[mesh.triangles[3 * idx + 2]];

        Vector3 a = v[1] - v[0];
        Vector3 b = v[2] - v[1];
        Vector3 c = v[2] - v[0];

        float ma = a.magnitude;
        float mb = b.magnitude;
        float mc = c.magnitude;

        float area = 0f;

        float S = (ma + mb + mc) / 2;
        area = Mathf.Sqrt(S * (S - ma) * (S - mb) * (S - mc));

        return area;
    }
}