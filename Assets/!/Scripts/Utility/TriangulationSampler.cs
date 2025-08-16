using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class TriangulationSampler : MonoBehaviour
{
    // Used for generation patrol points across the level's navmesh for workers to move around
    [SerializeField] private GameObject patrolPointPrefab;
    [SerializeField] private GameObject patrolPointContainer;

    public List<PatrolPoint> GenerateRandomPatrolPoints(int totalPoints)
    {
        // Instantiate the patrol points in scene
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        UnityEngine.Debug.Log($"Generating {totalPoints} random patrol points...");
        List<Vector3> points = GeneratePointsFromTriangulation(totalPoints);
        List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
        
        foreach (var point in points)
        {
            GameObject newPatrolPoint;
            if (patrolPointContainer != null)
            {
                newPatrolPoint = Instantiate(patrolPointPrefab, point, Quaternion.identity, patrolPointContainer.transform);
            }
            else
            {
                newPatrolPoint = Instantiate(patrolPointPrefab, point, Quaternion.identity);
            }
            patrolPoints.Add(newPatrolPoint.GetComponent<PatrolPoint>());
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Generation finished in {stopwatch.ElapsedMilliseconds} ms and generated {patrolPoints.Count} patrol points.");
        return patrolPoints;
    }

    private List<Vector3> GeneratePointsFromTriangulation(int totalPoints)
    {
        // uses triangulation to generate random points based on the navigation mesh's triangles
        // now uses area-weighted distribution to avoid clustering in small triangles
        List<Vector3> points = new();
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // Calculate area for each triangle
        List<float> triangleAreas = new List<float>();
        float totalArea = 0f;

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            Vector3 v0 = triangulation.vertices[triangulation.indices[i]];
            Vector3 v1 = triangulation.vertices[triangulation.indices[i + 1]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[i + 2]];

            float area = CalculateTriangleArea(v0, v1, v2);
            triangleAreas.Add(area);
            totalArea += area;
        }

        // Generate exactly totalPoints using area-weighted selection
        for (int p = 0; p < totalPoints; p++)
        {
            int triangleIndex = SelectTriangleByArea(triangleAreas, totalArea);
            int vertexIndex = triangleIndex * 3;

            Vector3 v0 = triangulation.vertices[triangulation.indices[vertexIndex]];
            Vector3 v1 = triangulation.vertices[triangulation.indices[vertexIndex + 1]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[vertexIndex + 2]];

            points.Add(GetRandomPointInTriangle(v0, v1, v2));
        }

        return points;
    }

    private float CalculateTriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        // Calculate triangle area using cross product: Area = 0.5 * ||(b-a) x (c-a)||
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return 0.5f * Vector3.Cross(ab, ac).magnitude;
    }

    private int SelectTriangleByArea(List<float> areas, float totalArea)
    {
        // Select triangle with probability proportional to its area
        float randomValue = Random.value * totalArea;
        float cumulativeArea = 0f;

        for (int i = 0; i < areas.Count; i++)
        {
            cumulativeArea += areas[i];
            if (randomValue <= cumulativeArea)
                return i;
        }

        return areas.Count - 1; // fallback
    }

    private Vector3 GetRandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        // pick a random point inside a triangle
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;
        return (1 - r1) * a + (r1 * (1 - r2)) * b + (r1 * r2) * c;
    }
}