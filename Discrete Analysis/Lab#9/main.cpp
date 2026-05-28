#include <iostream>
#include <climits>
#include <queue>
#include <vector>
#include <cstdint>

using namespace std;

bool BFS(vector<vector<uint32_t> > &rGraph, int s, int t, vector<int32_t> &parent, vector<bool> visited) {
  queue<int> q;
  q.push(s);
  visited[s] = true;
  parent[s] = -1;

  while (!q.empty()) {
    int u = q.front();
    q.pop();

    for (int v(0); v < t + 1; v++) {
      if (!visited[v] && rGraph[u][v] > 0) {
        q.push(v);
        parent[v] = u;
        visited[v] = true;
      }
    }
  }
  return (visited[t]);
}

int64_t FFA(vector<vector<uint32_t>> &graph, int s, int t) {
  int u, v;
  vector<vector<uint32_t>> rGraph;

  rGraph.resize(t + 1);
  for (u = 0; u < t + 1; ++u) {
    rGraph[u].resize(t + 1);
    for (v = 0; v < t + 1; ++v) {
      rGraph[u][v] = graph[u][v];
    }
  }

  vector<int32_t> parent(t + 1);
  vector<bool> visited(t + 1);
  int64_t maxFlow = 0;

  while (BFS(rGraph, s, t, parent, visited)) {
    uint32_t pathFlow = INT_MAX;
    for (v = t; v != s; v = parent[v]) {
      u = parent[v];
      pathFlow = min(pathFlow, rGraph[u][v]);
    }

    for (v = t; v != s; v = parent[v]) {
      u = parent[v];
      rGraph[u][v] -= pathFlow;
      rGraph[v][u] += pathFlow;
    }

    maxFlow += pathFlow;
  }
  return maxFlow;
}

int main() {
  int32_t ver, edg;
  cin >> ver >> edg;
  vector<vector<uint32_t>> graph;

  graph.resize(ver + 1);
  for (int i(0); i < ver + 1; ++i) {
    graph[i].resize(ver + 1);
  }

  int32_t from, to, val;
  for (int j(0); j < edg; ++j) {
    cin >> from >> to >> val;
    graph[from][to] = val;
  }

  cout << FFA(graph, 1, ver) << '\n';

  return 0;
}