#include <bits/stdc++.h>
using namespace std;

int main(int argc, char *argv[]) {
  ofstream fout("./pipe2.txt", ios_base::out | ios_base::trunc);

  string mapped2 = argv[0];
  size_t mapSize2 = mapped2.length();

  for (int i(mapSize2 - 2); i >= 0; --i) {
	fout << mapped2[i];
  }
  fout.close();
}