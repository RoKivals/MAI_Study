const char* SEMAPHORE_A = "sem_a";
const char* SEMAPHORE_B = "sem_b";
const char* SEMAPHORE_C = "sem_c";
const char* INPUT_END = "EOF";

int get_semaphore_value(sem_t* semaphore) {
    int s;
    sem_getvalue(semaphore, &s);
    return s;
}

void set_semaphore_value(sem_t* semaphore, int n) {
    while (get_semaphore_value(semaphore) < n) {
        sem_post(semaphore);
    }
    while (get_semaphore_value(semaphore) > n) {
        sem_wait(semaphore);
    }
}