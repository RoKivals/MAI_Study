#ifndef OPERATINGSYSTEMS_LAB_678_SRC_MESSAGE_H_
#define OPERATINGSYSTEMS_LAB_678_SRC_MESSAGE_H_

#include "zmq.hpp"
#include <string>
#include <vector>

// Тип сообщения
enum message_type_t {
  MSG_PING,
  MSG_CREATE,
  MSG_REMOVE,
  MSG_EXEC,
  MSG_EXEC_ANSWER,
  MSG_PING_ANSWER,
  MSG_CREATE_ANSWER,
};

// Направление сообщения (между хостом и сервером)
enum message_direction_t {
  DIR_TO_CLIENT,
  DIR_TO_SERVER,
};

// Лист с информацией о сообщении
struct header_t {
  message_direction_t dir;
  message_type_t type;
  int to_id_node;
};

// ------------------------------------------------------------//
// различные структуры ответов на сообщения //
struct create_body {
  int child_id;
};

struct exec_body {
  std::string text;
  std::string pattern;
};

struct ping_body_answer {
  int src_id;
};

struct exec_body_answer {
  std::vector<int> entries;
};

struct create_body_answer {
  int pid;
  std::string error;
};

// -------------------------------------------------------//

zmq::message_t fill_message_exec(int dest_id, std::string &text, std::string &pattern);
zmq::message_t fill_message_exec_answer(std::vector<int> &enrties);
zmq::message_t fill_message_create(int parent_id, int child_id);
zmq::message_t fill_message_create_answer(int pid, std::string error);
zmq::message_t fill_message_remove(int dest_id);
zmq::message_t fill_message_ping(int dest_id);
zmq::message_t fill_message_ping_answer(int src_id);

header_t *get_message_header(zmq::message_t &data);
create_body get_message_create(zmq::message_t &data);
create_body_answer get_message_create_answer(zmq::message_t &data);
exec_body get_message_exec(zmq::message_t &data);
exec_body_answer get_message_exec_answer(zmq::message_t &data);
ping_body_answer get_message_ping_answer(zmq::message_t &data);

#endif //OPERATINGSYSTEMS_LAB_678_SRC_MESSAGE_H_
