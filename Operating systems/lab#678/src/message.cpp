#include "msg.h"
#include <cstring>

zmq::message_t fill_message_exec(int dest_id, std::string &text, std::string &pattern) {
  header_t header{
	  DIR_TO_CLIENT,
	  MSG_EXEC,
	  dest_id,
  };

  int size_of_message = sizeof(header_t) + text.size() + pattern.size() + 2 * sizeof(int);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  // Копируем header
  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  // Копируем размер текста
  int text_size = text.size();
  std::memcpy(dest_ptr, &text_size, sizeof(text_size));
  dest_ptr += sizeof(text_size);

  // копируем сам текст
  std::copy(text.begin(), text.end(), dest_ptr);
  dest_ptr += text.size();

  // Копируем размер паттерна
  int pattern_size = pattern.size();
  std::memcpy(dest_ptr, &pattern_size, sizeof(pattern_size));
  dest_ptr += sizeof(pattern_size);

  // копируем сам паттерн
  std::copy(pattern.begin(), pattern.end(), dest_ptr);
  dest_ptr += pattern.size();

  return data;
}

zmq::message_t fill_message_exec_answer(std::vector<int> &enrties) {
  header_t header{
	  DIR_TO_SERVER,
	  MSG_EXEC_ANSWER,
	  -1,
  };

  int size_of_message = sizeof(header_t) + sizeof(int) * enrties.size() + sizeof(int);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  // Копируем header
  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  // Копируем размер массива обнаруженных вхождений
  int entries_size = enrties.size();
  std::memcpy(dest_ptr, &entries_size, sizeof(entries_size));
  dest_ptr += sizeof(entries_size);

  // копируем сам массив
  std::copy(enrties.begin(), enrties.end(), (int *) dest_ptr);
  dest_ptr += sizeof(int) * enrties.size();

  return data;
}

zmq::message_t fill_message_create(int parent_id, int child_id) {
  header_t header{
	  DIR_TO_CLIENT,
	  MSG_CREATE,
	  parent_id
  };

  create_body body{
	  child_id
  };

  int size_of_message = sizeof(header_t) + sizeof(body);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  // Копируем header
  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  // Копируем body
  std::memcpy(dest_ptr, &body, sizeof(body));
  dest_ptr += sizeof(body);

  return data;
}

zmq::message_t fill_message_create_answer(int pid, std::string error) {
  header_t header{
	  DIR_TO_SERVER,
	  MSG_CREATE_ANSWER,
	  -1
  };

  int size_of_message = sizeof(header_t) + sizeof(pid) + sizeof(int) + error.size() * sizeof(error);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  // Копируем header
  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  // Копируем pid
  std::memcpy(dest_ptr, &pid, sizeof(pid));
  dest_ptr += sizeof(pid);

  // Копируем error.size
  int error_size = error.size();
  std::memcpy(dest_ptr, &error_size, sizeof(error_size));
  dest_ptr += sizeof(error_size);

  // Копируем error
  std::copy(error.begin(), error.end(), dest_ptr);
  dest_ptr += error.size();

  return data;
}

zmq::message_t fill_message_remove(int dest_id) {
  header_t header{
	  DIR_TO_CLIENT,
	  MSG_REMOVE,
	  dest_id
  };

  int size_of_message = sizeof(header_t);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  return data;
}

zmq::message_t fill_message_ping(int dest_id) {
  header_t header{
	  DIR_TO_CLIENT,
	  MSG_PING,
	  dest_id
  };

  int size_of_message = sizeof(header_t);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  return data;
}

zmq::message_t fill_message_ping_answer(int src_id) {
  header_t header{
	  DIR_TO_SERVER,
	  MSG_PING_ANSWER,
	  -1
  };

  ping_body_answer body{
	  src_id
  };

  int size_of_message = sizeof(header_t) + sizeof(body);
  zmq::message_t data(size_of_message);
  char *dest_ptr = (char *) data.data();

  std::memcpy(dest_ptr, &header, sizeof(header));
  dest_ptr += sizeof(header);

  std::memcpy(dest_ptr, &body, sizeof(body));
  dest_ptr += sizeof(body);

  return data;
}

header_t *get_message_header(zmq::message_t &data) {
  return (header_t *) data.data();
}

create_body get_message_create(zmq::message_t &data) {
  create_body body;
  std::memcpy(&body, (char *) data.data() + sizeof(header_t), sizeof(body));
  return body;
}

create_body_answer get_message_create_answer(zmq::message_t &data) {
  char *src_ptr = (char *) data.data() + sizeof(header_t);

  // Восстанавливаем pid
  int pid;
  std::memcpy(&pid, src_ptr, sizeof(int));
  src_ptr += sizeof(pid);

  // Восстанавливаем error size
  int error_size;
  std::memcpy(&error_size, src_ptr, sizeof(int));
  src_ptr += sizeof(error_size);

  // Восстанавливаем error
  std::string error(src_ptr, error_size);
  src_ptr += error_size;

  return {pid, error};
}

exec_body get_message_exec(zmq::message_t &data) {
  char *src_ptr = (char *) data.data() + sizeof(header_t);

  int text_size;
  std::memcpy(&text_size, src_ptr, sizeof(int));
  src_ptr += sizeof(int);

  std::string text(src_ptr, text_size);
  src_ptr += text_size;

  int pattern_size;
  std::memcpy(&pattern_size, src_ptr, sizeof(int));
  src_ptr += sizeof(int);

  std::string pattern(src_ptr, pattern_size);
  src_ptr += pattern_size;

  return {text, pattern};
}

exec_body_answer get_message_exec_answer(zmq::message_t &data) {
  char *src_ptr = (char *) data.data() + sizeof(header_t);

  int entries_size;
  std::memcpy(&entries_size, src_ptr, sizeof(int));
  src_ptr += sizeof(int);

  std::vector<int> entries;
  int *src_int_ptr = (int *) src_ptr;
  std::copy(src_int_ptr, src_int_ptr + entries_size, std::back_inserter(entries));

  return {entries};
}

ping_body_answer get_message_ping_answer(zmq::message_t &data) {
  ping_body_answer body;
  std::memcpy(&body, (char *) data.data() + sizeof(header_t), sizeof(body));
  return body;
}