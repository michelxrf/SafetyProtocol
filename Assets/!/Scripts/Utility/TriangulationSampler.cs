using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class TriangulationSampler : MonoBehaviour
{
    // Used for generation patrol points across the level's navmesh for workers to move around
    [SerializeField] private GameObject patrolPointPrefab;
    [SerializeField] private GameObject patrolPointContainer;

    public List<PatrolPoint> GenerateRandomPatrolPoints(int density = 1)
    {
        // Instantiate the patrol points in scene

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        UnityEngine.Debug.Log("Generating random patrol points...");

        List<Vector3> points = GeneratePointsFromTriangulation(density);
        List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
        
        foreach (var point in points)
        {
            GameObject newPatrolPoint = Instantiate(patrolPointPrefab, point, Quaternion.identity, patrolPointContainer.transform);
            patrolPoints.Add(newPatrolPoint.GetComponent<PatrolPoint>());
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Generation finished in {stopwatch.ElapsedMilliseconds} ms and generated {patrolPoints.Count} patrol points.");
        return patrolPoints;
    }
   
    private List<Vector3> GeneratePointsFromTriangulation(int samplesPerTriangle = 1)
    {
        // uses triangulation to generate random points based on the navigation mesh's triangules

        List<Vector3> points = new();
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            Vector3 v0 = triangulation.vertices[triangulation.indices[i]];
            Vector3 v1 = triangulation.vertices[triangulation.indices[i + 1]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[i + 2]];

            for (int j = 0; j < samplesPerTriangle; j++)
            {
                points.Add(GetRandomPointInTriangle(v0, v1, v2));
            }
        }

        return points;
    }

    private Vector3 GetRandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        // pic a random point inside a triangle

        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;
        return (1 - r1) * a + (r1 * (1 - r2)) * b + (r1 * r2) * c;
    }
}
