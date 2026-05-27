#include <iostream>
#include <string>
#include <unistd.h>
#include <fcntl.h>
#include <semaphore.h>
#include "functions.cpp"
using namespace std;

int main(int argc, char* argv[]) {
    int fdAB[2], fdBC[2];
    fdAB[0] = atoi(argv[0]);
    fdAB[1] = atoi(argv[1]);
    fdBC[0] = atoi(argv[2]);
    fdBC[1] = atoi(argv[3]);
    sem_t* semA = sem_open(SEMAPHORE_A, O_CREAT, 0777, 1);
    sem_t* semB = sem_open(SEMAPHORE_B, O_CREAT, 0777, 0);
    sem_t* semC = sem_open(SEMAPHORE_C, O_CREAT, 0777, 0);
    while (true) {
        while (get_semaphore_value(semB) == 0) {
            continue;
        }
        if (get_semaphore_value(semB) == 2) {
            break;
        }
        int size;
        read(fdAB[0], &size, sizeof(int));
        cout << "In count: " << size << endl;
        set_semaphore_value(semC, 1);
        set_semaphore_value(semB, 0);
        while (get_semaphore_value(semB) == 0) {
            continue;
        }
        if (get_semaphore_value(semB) == 2) {
            break;
        }
        read(fdBC[0], &size, sizeof(int));
        cout << "Out count: " << size << endl;
        set_semaphore_value(semA, 1);
        set_semaphore_value(semB, 0);
        while (get_semaphore_value(semB) == 0) {
            continue;
        }
        if (get_semaphore_value(semB) == 2) {
            break;
        }
    }
    sem_close(semA);
    sem_unlink(SEMAPHORE_A);
    sem_close(semB);
    sem_unlink(SEMAPHORE_B);
    sem_close(semC);
    sem_unlink(SEMAPHORE_C);
    close(fdAB[0]);
    close(fdAB[1]);
    close(fdBC[0]);
    close(fdBC[1]);
    return 0;
}
