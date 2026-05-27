#include <iostream>
#include <string>
#include <unistd.h>
#include <fcntl.h>
#include <semaphore.h>
#include "functions.cpp"
using namespace std;

int main(int argc, char* argv[]) {
    int fdAC[2], fdAB[2];
    fdAC[0] = atoi(argv[0]);
    fdAC[1] = atoi(argv[1]);
    fdAB[0] = atoi(argv[2]);
    fdAB[1] = atoi(argv[3]);
    sem_t* semA = sem_open(SEMAPHORE_A, O_CREAT, 0777, 1);
    sem_t* semB = sem_open(SEMAPHORE_B, O_CREAT, 0777, 0);
    sem_t* semC = sem_open(SEMAPHORE_C, O_CREAT, 0777, 0);

    while (true) {
        string input;
        getline(cin, input);
        if (input == INPUT_END) {
            set_semaphore_value(semA, 2);
            set_semaphore_value(semB, 2);
            set_semaphore_value(semC, 2);
            break;
        }
        int size = input.length();
        write(fdAC[1], &size, sizeof(int));
        write(fdAB[1], &size, sizeof(int));
        for (int i = 0; i < size; ++i) {
            write(fdAC[1], &input[i], sizeof(char));
        }
        set_semaphore_value(semB, 1);
        set_semaphore_value(semA, 0);
        while (get_semaphore_value(semA) == 0) {
            continue;
        }
    }
    sem_close(semA);
    sem_unlink(SEMAPHORE_A);
    sem_close(semB);
    sem_unlink(SEMAPHORE_B);
    sem_close(semC);
    sem_unlink(SEMAPHORE_C);
    close(fdAC[0]);
    close(fdAC[1]);
    close(fdAB[0]);
    close(fdAB[1]);
    return 0;
}
