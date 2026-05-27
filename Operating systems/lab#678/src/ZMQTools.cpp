#include "ZMQTools.h"

const int BROADCAST_ID = -2;

std::string create_name_of_socket(int node_id) {
  std::stringstream sstream;
  sstream << "ipc://sock_" << node_id;
  return sstream.str();
}

std::string create_name_of_socket_to_children(int node_id) {
  std::stringstream sstream;
  sstream << create_name_of_socket(node_id) << "_to_children";
  return sstream.str();
}

std::string create_name_of_socket_to_parent(int node_id) {
  std::stringstream sstream;
  sstream << create_name_of_socket(node_id) << "_to_parent";
  return sstream.str();
}