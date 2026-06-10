#include <stdio.h>
int power(int a, int b) {
	int res = 1;
	for (int i = 0; i<b; i++) {
		res *= a;
	}
	return res;
}
long f(long a) {
    int k = 0;
    long a1 = a;
	if (a == 0) {
		a = 0;
	} else {
		while (a1 != 0) {
			a1 /= 10;
			k++;
		}
	}
	if (k % 2 != 0) {
		k /= 2;
		for (int i = k; i > 0; i--) {
			a -= a % power(10,i+1) / power(10,i) * power(10,i);
			a += a % power(10, i) / power(10, i-1) * power(10, i);
		}
		
		a /= 10;
	}
	return a;
}
int main() {
	long a;
	while (scanf("%ld", &a) == 1) {
	    if (f(a) != 0) {
	        printf("%ld\n",f(a));
	    }
	}
 	return 0;
}
