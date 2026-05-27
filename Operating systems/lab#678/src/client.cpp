// Управляющий узел
#include <bits/stdc++.h>
#include "zmq.hpp"
#include <unistd.h>
#include <csignal>
#include "msg.h"
#include "ZMQTools.h"

static zmq::context_t context;

static zmq::socket_t pub_sock(context, ZMQ_PUB);
static zmq::socket_t sub_sock(context, ZMQ_SUB);

static std::string socket_name;
static std::vector<std::string> sockets_of_children;

void server_init() {
  try {
	socket_name = create_name_of_socket(0);
	pub_sock.bind(socket_name);

	// setsockopt - изменить свойства сокета. Обязательным для SUB явлется сво-во ZMQ_SUBSCRIBE
	// Который определяет префикс, с которого должно начинаться сбщ,
	// чтобы данный подписчик посчитал нужным его принять

	sub_sock.setsockopt(ZMQ_SUBSCRIBE, 0, 0);
	int timeout = 1000;
	// Максимальное время ожидания приёма - 1000 мс
	sub_sock.setsockopt(ZMQ_RCVTIMEO, timeout);
  }
  catch (zmq::error_t) {
	std::cout << "Error:ZMQ: " << zmq_strerror(errno) << std::endl;
	exit(-1);
  }

}

bool receive_msg(zmq::message_t &msg) {
  zmq::recv_result_t res;
  res = sub_sock.recv(msg);

  return res.has_value();
}

bool is_exists(int id) {
  zmq::message_t data = fill_message_ping(id);
  pub_sock.send(data);

  zmq::message_t receive_data;
  header_t *header;

  if (!receive_msg(receive_data))
	return false;

  ping_body_answer ans = get_message_ping_answer(receive_data);
  if (ans.src_id != id)
	return false;

  return true;
}

void create() {
  int new_node_pid = -1;
  int parent_id, child_id;
  std::cin >> child_id >> parent_id;

  if (parent_id != -1 && !is_exists(parent_id)) {
	std::cout << "Error: Parent not found" << std::endl;
	return;
  }
  if (is_exists(child_id)) {
	std::cout << "Error: Already exists" << std::endl;
	return;
  }

  if (parent_id == -1) { // Управляющий узел создаёт дочерний процесс
	int fork_pid = fork();
	if (fork_pid == -1) {
	  std::cout << "Error:fork: " << strerror(errno) << std::endl;
	  return;
	}
	if (fork_pid == 0) {
	  std::stringstream sstream;
	  sstream << child_id;
	  execl("server", "server", sstream.str().c_str(), socket_name.c_str(), NULL);
	}
	// Подписываемся на дочерний узел
	std::string parent_pub_socket_name = create_name_of_socket_to_parent(child_id);
	sockets_of_children.push_back(parent_pub_socket_name);
	sub_sock.connect(parent_pub_socket_name);

	new_node_pid = fork_pid;
  } else {
	// иначе отсылкаем запрос всем детям(вниз по дереву)
	zmq::message_t data = fill_message_create(parent_id, child_id);
	pub_sock.send(data);

	zmq::message_t receive_data;
	receive_msg(receive_data);
	create_body_answer ans = get_message_create_answer(receive_data);

	if (ans.pid == -1) {
	  std::cout << "Error:remote_create: " << ans.error << std::endl;
	  return;
	}

	new_node_pid = ans.pid;
  }

  std::cout << "OK: " << new_node_pid << std::endl;
}

void exec() {
  /*  > exec id
	  > text_string
	  > pattern_string
	  [result] – номера позиций, где найден образец, разделенный точкой с запятой
   */
  std::string text, pattern;
  int dest_id;

  std::cin >> dest_id;
  std::cin.get();
  std::getline(std::cin, text);
  std::getline(std::cin, pattern);

  if (!is_exists(dest_id)) {
	std::cout << "Error:" << dest_id << ": Not found" << std::endl;
	return;
  }

  zmq::message_t data = fill_message_exec(dest_id, text, pattern);
  pub_sock.send(data);

  zmq::message_t receive_data;
  header_t *header;
  receive_msg(receive_data);
  exec_body_answer ans = get_message_exec_answer(receive_data);

  std::cout << "OK:" << dest_id << ": [";
  if (ans.entries.size() > 0)
	std::cout << ans.entries[0];
  for (int i = 1; i < ans.entries.size(); ++i) {
	std::cout << "; " << ans.entries[i];
  }
  std::cout << "]" << std::endl;
}

void ping() {
  int dest_id;
  std::cin >> dest_id;
  if (is_exists(dest_id))
	std::cout << "OK: 1" << std::endl;
  else
	std::cout << "Error: Not found" << std::endl;
}

void server_run() {
  std::string input_cmd;
  while (std::cin >> input_cmd) {
	try {
	  if (input_cmd == "create")
		create();
	  else if (input_cmd == "exec")
		exec();
	  else if (input_cmd == "ping")
		ping();
	  else {
		std::cout << "Error: данной команды нет!" << std::endl;
	  }
	}
	catch (zmq::error_t) {
	  std::cout << "Error:ZMQ: " << zmq_strerror(errno) << std::endl;
	}
  }
}

int main() {
  server_init();
  server_run();
}