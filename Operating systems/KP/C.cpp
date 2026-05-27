#include <iostream>
#include <string>
#include <unistd.h>
#include <fcntl.h>
#include <semaphore.h>
#include "functions.cpp"
using namespace std;
int main(int argc, char* argv[]) {
    int fdAC[2], fdBC[2];
    fdAC[0] = atoi(argv[0]);
    fdAC[1] = atoi(argv[1]);
    fdBC[0] = atoi(argv[2]);
    fdBC[1] = atoi(argv[3]);
    sem_t* semA = sem_open(SEMAPHORE_A, O_CREAT, 0777, 1);
    sem_t* semB = sem_open(SEMAPHORE_B, O_CREAT, 0777, 0);
    sem_t* semC = sem_open(SEMAPHORE_C, O_CREAT, 0777, 0);

    while (true) {
        while (get_semaphore_value(semC) == 0) {
            continue;
        }
        if (get_semaphore_value(semC) == 2) {
            break;
        }
        int size;
        string input;
        read(fdAC[0], &size, sizeof(int));
        int count = 0;
        for (int i = 0; i < size; ++i) {
            char c;
            read(fdAC[0], &c, sizeof(char));
            input.push_back(c);
            count+=1;
        }
        cout << input << endl;
        write(fdBC[1], &count, sizeof(int));
        set_semaphore_value(semB, 1);
        set_semaphore_value(semC, 0);
    }
    sem_close(semA);
    sem_unlink(SEMAPHORE_A);
    sem_close(semB);
    sem_unlink(SEMAPHORE_B);
    sem_close(semC);
    sem_unlink(SEMAPHORE_C);
    close(fdAC[0]);
    close(fdAC[1]);
    close(fdBC[0]);
    close(fdBC[1]);
    return 0;
}

