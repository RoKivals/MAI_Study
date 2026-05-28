#include <iostream>
#include <vector>
#include <algorithm>

struct Interval {
  int start;
  int end;
  int number;
};

bool comparator1(const Interval &gap1, const Interval &gap2) {
  return (gap1.start < gap2.start) || ((gap1.start == gap2.start) && (gap1.end < gap2.end));
}

bool comparator2(const Interval &gap1, const Interval &gap2) {
  return gap1.number < gap2.number;
}

int main() {
  int n, m;
  std::cin >> n;
  std::vector<Interval> gaps;

  for (int i(0); i < n; i++) {
    Interval a;
    std::cin >> a.start >> a.end;
    a.number = i;
    gaps.push_back(a);
  }

  std::cin >> m;
  sort(gaps.begin(), gaps.end(), comparator1);
  std::vector<Interval> res;
  int leftEnd = 0, rightEnd = 0;
  Interval currSeg;

  for (int i(0); i < gaps.size(); i++) {
    while ((gaps[i].end <= leftEnd) && (i < gaps.size())) {
      i++;
    }
    currSeg = gaps[i];
    while ((gaps[i].start <= leftEnd) && (i != gaps.size())) {
      if (gaps[i].end >= rightEnd) {
        rightEnd = gaps[i].end;
        currSeg = gaps[i];
      }
      i++;
    }
    res.push_back(currSeg);
    leftEnd = rightEnd;
    if (rightEnd >= m) {
      break;
    }
    if (gaps[i].start <= leftEnd) {
      i--;
    }
  }
  if (rightEnd >= m) {
    std::cout << res.size() << '\n';
    sort(res.begin(), res.end(), comparator2);
    for (auto &gap: res) {
      std::cout << gap.start << ' ' << gap.end << '\n';
    }
  } else {
    std::cout << '0' << '\n';
  }
  return 0;
}
