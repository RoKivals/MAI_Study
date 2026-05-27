#include <iostream>
#include <string>
#include <unistd.h>
#include <fcntl.h>
#include <semaphore.h>
#include "functions.cpp"
using namespace std;

int main() {
    int fdAC[2], fdAB[2], fdBC[2];
    pipe(fdAC);
    pipe(fdAB);
    pipe(fdBC);
    sem_unlink(SEMAPHORE_A);
    sem_unlink(SEMAPHORE_B);
    sem_unlink(SEMAPHORE_C);
    sem_t* semA = sem_open(SEMAPHORE_A, O_CREAT, 0777, 1);
    sem_t* semB = sem_open(SEMAPHORE_B, O_CREAT, 0777, 0);
    sem_t* semC = sem_open(SEMAPHORE_C, O_CREAT, 0777, 0);
    if ((semA == SEM_FAILED)||(semB == SEM_FAILED)||(semC == SEM_FAILED)){
        perror("sem_open");
        return -1;
    }
    cout << "End of input " << INPUT_END << endl;
    pid_t C = fork();
    if (C == -1) {
        perror("fork");
        return -1;
    }
    if (C == 0) {
        close(fdAB[0]);
        close(fdAB[1]);
        execl("C", to_string(fdAC[0]).c_str(), to_string(fdAC[1]).c_str(), to_string(fdBC[0]).c_str(), to_string(fdBC[1]).c_str(), NULL);
    } else {
        pid_t B = fork();
        if (B == -1){
            perror("fork");
            return -1;
        }
        if (B == 0){
            close(fdAC[0]);
            close(fdAC[1]);
            execl("B", to_string(fdAB[0]).c_str(), to_string(fdAB[1]).c_str(), to_string(fdBC[0]).c_str(), to_string(fdBC[1]).c_str(), NULL);
        } else {
            close(fdBC[0]);
            close(fdBC[1]);
            execl("A", to_string(fdAC[0]).c_str(), to_string(fdAC[1]).c_str(), to_string(fdAB[0]).c_str(), to_string(fdAB[1]).c_str(), NULL);
        }
    }
    return 0;
}
