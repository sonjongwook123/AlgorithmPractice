using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public Transform startNode;
    public Transform endNode;
    public List<Transform> allNodes;
    public Dictionary<Transform, Dictionary<Transform, float>> adjacencyList =
    new Dictionary<Transform, Dictionary<Transform, float>>();

    void Start()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            adjacencyList[allNodes[i]] = new Dictionary<Transform, float>(); // 트랜스폼 노드 할당

            for (var j = 0; j < allNodes.Count; j++) // 선택 된 노드 제외 나머지 노드 반복문문
            {
                if (allNodes[j] != allNodes[i]) // 선택 된 노드가 아닌지 검사
                {
                    if (allNodes[j] != null)
                    {
                        // 선택된 노드 => 다른 노드와 거리 저장장
                        adjacencyList[allNodes[i]][allNodes[j]] =
                        Vector3.Distance(allNodes[i].position, allNodes[j].position);
                    }
                }
            }
        }

        if (startNode != null && endNode != null)
        {
            List<Transform> shortestPath = FindShortestPath(startNode, endNode);
            if (shortestPath != null)
            {
                Debug.Log("Shortest Path: " + string.Join(" -> ", shortestPath.Select(t => t.name)));
                // 거리 이동
            }
            else
            {
                Debug.LogWarning("Path not found!");
            }
        }
        else
        {

        }
    }

    List<Transform> FindShortestPath(Transform start, Transform end)
    {
        // 검사 알고리즘 노드용 템프 데이터들들
        Dictionary<Transform, float> distance =
        allNodes.ToDictionary(node => node, node => Mathf.Infinity); // 초기값 노드 목록 생성

        Dictionary<Transform, Transform> previous =
        allNodes.ToDictionary(node => node, Node => (Transform)null); // 이전 노드 목록 생성성

        HashSet<Transform> unVisited = new HashSet<Transform>(allNodes); // 모든 노드 트랜스폼 오브젝트트

        distance[start] = 0; // 시작 노드 값 0으로 초기화
                             /////////////////////////////////////////////////

        while (unVisited.Count > 0) // 방문하지 않은 노드가 없으면 break
        {
            Transform currentNode =
            unVisited.OrderBy(node => distance[node]).First(); // 현재 노드에서 가장 가까운 노드
            unVisited.Remove(currentNode); // 선택된 가장 가까운 노드는 방문하지 않는 노드에서 제거

            if (currentNode == end)
            {
                break;
            }

            if (distance[currentNode] == Mathf.Infinity)
            {
                break;
            }

            if (adjacencyList.ContainsKey(currentNode)) // 다익스트라 노드 리스트에서 현재 선택된 노드 확인
            {
                foreach (var neighborPair in adjacencyList[currentNode])
                {
                    Transform neighbor = neighborPair.Key; // 노드리스트 배열의 하위 딕셔너리 노드 참조
                    float weight = neighborPair.Value; // 하위 딕셔너리 노드(트랜스폼)의 거리 값
                    float newDistance = distance[currentNode] + weight; // 거리값에 가중치(거리) 추가

                    if (newDistance < distance[neighbor]) // 거리 값이 기존 값보다 작다면
                    {
                        distance[neighbor] = newDistance; // 거리를 새 거리로
                        previous[neighbor] = currentNode; // 이전 노드를 현재 트랜스폼(노드)으로
                    }
                }
            }
        }

        List<Transform> path = new List<Transform>(); // 패스 
        Transform current = end;

        while (current != null)
        {
            path.Add(current); // 마지막 노드를 추가
            current = previous[current]; // 마지막 노드의 이전 노드를 추가
        }

        path.Reverse(); // 도착순서로 값을 쌓았으니 반대로 뒤집음


        return path.First() == start ? path : null; // 첫번째 패스에 시작 노드(트랜스폼)을 넣음음
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying == true)
        {
            List<Transform> path = FindShortestPath(startNode, endNode);
            if (path.Count > 1)
            {
                Gizmos.color = Color.green;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i].position, path[i + 1].position);
                }
            }
        }
    }

}
