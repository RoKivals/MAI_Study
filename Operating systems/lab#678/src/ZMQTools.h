#ifndef OPERATINGSYSTEMS_LAB_678_SRC_ZMQTOOLS_H_
#define OPERATINGSYSTEMS_LAB_678_SRC_ZMQTOOLS_H_
#include "zmq.hpp"
#include <string>

extern const int BROADCAST_ID;

std::string create_name_of_socket(int node_id);
std::string create_name_of_socket_to_children(int node_id);
std::string create_name_of_socket_to_parent(int node_id);

#endif //OPERATINGSYSTEMS_LAB_678_SRC_ZMQTOOLS_H_
