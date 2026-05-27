#include <bits/stdc++.h>
using namespace std;

int main(int argc, char *argv[]) {
  ofstream fout("./pipe1.txt", ios_base::out | ios_base::trunc);

  string mapped1 = argv[0];
  size_t mapSize1 = mapped1.length();

  for (int i(mapSize1 - 2); i >= 0; --i) {
	fout << mapped1[i];
  }
  fout.close();
}
