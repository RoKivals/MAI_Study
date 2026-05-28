#pragma once

#include <iostream>

template<class T>
class Vector {
  // _capacity - allocated memory, _size - cnt elements
  uint16_t _capacity;
  uint16_t _size;
  T *_data;
  const float GROWTH_COEF = 1.6;

  void Reallocate();

 public:
  // Con/De-structor
  Vector();

  Vector(const uint16_t &, const T &);

  explicit Vector(const uint16_t &);

  Vector(Vector<T> &);

  Vector(Vector<T> &&);

  ~Vector();

  // Methods
  uint16_t Size();

  bool Empty();

  void PushBack(const T &);

  T PopBack();

  void Set(size_t, const T &);

  Vector<T> &operator=(const Vector &);

  Vector<T> &operator=(Vector &&) noexcept;

  T &operator[](uint16_t);

  T *Begin();

  T *End();
};

template<class T>
Vector<T> &Vector<T>::operator=(const Vector &another) {
  for (int i(0); i < another.Size(); ++i) {
    this->PushBack(another[i]);
  }
  return *this;
}

template<class T>
Vector<T> &Vector<T>::operator=(Vector &&another) noexcept {
  delete[] _data;
  _data = another._data;
  another._data = nullptr;
  _capacity = another._capacity;
  another._capacity = 0;
  _size = another._size;
  another._size = 0;
  return *this;
}

template<class T>
Vector<T>::Vector() {
  this->_size = 0;
  this->_capacity = 2;
  this->_data = new T[_capacity];
}

template<class T>
Vector<T>::Vector(const uint16_t &size) {
  this->_size = size;
  this->_capacity = size;
  this->_data = new T[_capacity];
}

template<class T>
Vector<T>::Vector(const uint16_t &size, const T &data) {
  _size = size;
  _capacity = _size * 2;
  _data = new T[_capacity];
  for (uint16_t i(0); i < _size; ++i) {
    _data[i] = data;
  }
}

template<class T>
Vector<T>::Vector(Vector<T> &another) {
  _size = another._size;
  _capacity = another._capacity;
  _data = new T[_capacity];
  for (uint16_t i(0); i < _size; ++i) {
    _data[i] = another._data[i];
  }
}

template<class T>
Vector<T>::Vector(Vector<T> &&other) {
  _capacity = other._capacity;
  _size = other.Size();
  delete[] _data;
  _data = other._data;

  other._data = nullptr;
  other._capacity = 0;
  other._size = 0;
}

template<class T>
Vector<T>::~Vector() {
  delete[] _data;
  _data = nullptr;
}

template<class T>
uint16_t Vector<T>::Size() {
  return _size;
}

template<class T>
bool Vector<T>::Empty() {
  return _size == 0;
}

template<class T>
void Vector<T>::PushBack(const T &value) {
  if (this->_size >= this->_capacity) {
    Reallocate();
  }
  _data[_size++] = value;
}

template<class T>
T Vector<T>::PopBack() {
  --_size;
  return _data[_size + 1];
}

template<class T>
void Vector<T>::Set(size_t pos, const T &value) {
  _data[pos] = value;
}

template<class T>
T &Vector<T>::operator[](uint16_t pos) {
  if (pos >= _size) {
    throw std::out_of_range("Invalid index");
  }
  return _data[pos];
}

template<class T>
T *Vector<T>::Begin() {
  return &_data[0];
}

template<class T>
T *Vector<T>::End() {
  return &_data[_size - 1];
}

template<class T>
void Vector<T>::Reallocate() {
  _capacity *= GROWTH_COEF;
  T *new_data = new T[_capacity];

  for (unsigned int i = 0; i < _size; ++i) {
    new_data[i] = _data[i];
  }

  delete[] _data;
  _data = new_data;
}