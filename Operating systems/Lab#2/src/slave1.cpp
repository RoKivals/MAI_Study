#include<bits/stdc++.h>
#include<sys/wait.h>
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

  // out - открытие на запись, trunc - очистка содержимого при открытии
  ofstream fout("./pipe1.txt", ios_base::out | ios_base::trunc);
  while (getline(cin, s)) {
	ReverseString(s);
	fout << s + "\r\n";
  }
  fout.close();
  return 0;
}