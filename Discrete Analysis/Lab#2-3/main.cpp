#include <iostream>
#include <string>
#include <fstream>
#include "Btree.hpp"


using namespace std;

//void Parser(Btree* tree) {
//  std::string cmd
//  ;
//  while (std::cin >> cmd) {
//    if (cmd == "+") {
//      std::string key;
//      uint64_t value;
//      std::cin >> key >> value;
//
//      for (int i = 0; i < key.size(); i++) {
//        key[i] = tolower(key[i]);
//      }
//      uint64_t res;
//      if (tree->PairInTree(tree->root, key, res)) {
//        std::cout << "Exist\n";
//      }
//      else {
//        tree->TreeInsert(tree, key, value);
//        std::cout << "OK\n";
//      }
//    }
//    else if (cmd == "-") {
//      std::string key;
//      std::cin >> key;
//      for (int i = 0; i < key.size(); i++) {
//        key[i] = tolower(key[i]);
//      }
//      uint64_t res;
//      if ( !TreeSearch(tree->root, key, res)) {
//        std::cout << "NoSuchWord\n";
//      }
//      else {
//        TreeDelete(tree, key);
//        std::cout << "OK\n";
//      }
//    }
//    else if (cmd == "!") {
//      std::string cmd, path;
//      std::cin >> cmd;
//      if (cmd == "Save") {
//        std::cin >> path;
//        std::ofstream ostream(path, std::ios::binary);
//        Save(tree->root, ostream);
//        ostream.close();
//        std::cout << "OK\n";
//      }
//      else if (cmd == "Load") {
//        std::cin >> path;
//        std::ifstream istream(path, std::ios::binary);
//        Destroy(tree->root);
//        TreeInit(tree);
//        Load(tree, istream);
//        istream.close();
//        std::cout << "OK\n";
//      }
//      else {
//        std::cout << "ERROR\n";
//      }
//    }
//    else {
//      for (int i = 0; i < cmd.size(); i++) {
//        cmd[i] = tolower(cmd[i]);
//      }
//      uint64_t val;
//      if ( !TreeSearch(tree->root, cmd, val)) {
//        std::cout << "NoSuchWord\n";
//      }
//      else {
//        std::cout << "OK: " << val << "\n";
//      }
//    }
//  }
//}


int main() {


//  std::ios_base::sync_with_stdio(false);
//  std::cin.tie(0);
//  Btree *tree;
//  tree = new Btree;
//  TreeInit(tree);
//  Parser(tree);
//  Destroy(tree->root);
//  delete tree;
//  return 0;
}