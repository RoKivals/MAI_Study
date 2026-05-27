#include<bits/stdc++.h>
#include<unistd.h>
using namespace std;

void ReverseString(string &str) {
  int n = str.length();
  for (int i(0); i < n / 2; ++i) {
	swap(str[i], str[n - i - 1]);
  }
}

int main(int argc, char *argv[]) {
  string s;
  ofstream fout("./pipe2.txt", ios_base::out | ios_base::trunc);
  while (getline(cin, s)) {
	ReverseString(s);
	fout << s + "\r\n";
  }
  fout.close();
  return 0;
}