#! /usr/bin/env bash

set -o errexit
set -o nounset

readonly TESTS_DIR=tests
readonly TESTS_COUNT=5

function log_info() {
  local message=$1
  log_ "info" "${message}"
}

function log_error() {
  local message=$1
  log_ "error" "${message}"
}

function log_() {
  local type=$1
  local message=$2
  local date_str=$(date +'%Y-%m-%d %H:%M:%S')
  echo "[${type}] [${date_str}] ${message}"
}

# $1 - первый аргумент
# $2 - второй аргумент
# $@ - все аргументы

function main() {
  log_info "Stage #01 Making clean..."
  if ! make clean; then
    log_error "Failed to make clean."
    return 1
  fi

  log_info "Stage #02 Building binary..."
  if ! make; then
    log_error "Failed make."
    return 1
  fi

  rm -rf ${TESTS_DIR}
  mkdir ${TESTS_DIR}
  log_info "Stage #03 Generating tests..."
  if ! python3 generator.py ${TESTS_DIR} ${TESTS_COUNT}; then
    log_error "Failed to generate tests."
    return 1
  fi

  log_info "Stage #04 Checking..."
  for test_file in "${TESTS_DIR}"/*.t; do

    local tmp_file=tmp.a
    if ! ./sort <${test_file} >${tmp_file}; then
      log_error "Failed to run sort with ${test_file}"
      return 1
    fi

    local answer_file=${test_file%.*}.a
    if ! diff -u ${tmp_file} ${answer_file}; then
      log_error "Failed to compare ${test_file} and ${answer_file}."
      return 1
    fi
    log_info "${test_file}: OK"
  done

  log_info "Stage #05 Benchmarking..."
  if ! make benchmark; then
    log_info "Failed to compile benchmark."
    return 1
  fi
  local benchmark_bin=./benchmark
  for test_file in $(ls ${TESTS_DIR}/*.t); do
    log_info "Running ${test_file}"
    if ! ${benchmark_bin} <${test_file}; then
      log_error "Failed to run ${benchmark_bin} for ${test_file}."
      return 1
    fi
  done
}

main $@
