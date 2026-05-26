#include <stdio.h>
#include "DataBase.h"

// Создаём БД
void create() {
  char fileName[40];
  FILE *file;
  printf("%s", "Enter the name of the file you want to create:\n");
  scanf("%s", fileName);
  file = fopen(fileName, "rb");
  if (file != NULL) {
	printf("Such file already exists\n");
  } else {
	file = fopen(fileName, "ab");
  }
  printf("Creation was successful!\n");
  fclose(file);
}

// Добавляем записи в БД
void add() {
  int records;
  char fileName[40];
  FILE *file;
  PC add;
  printf("Enter the name of the file you want to add the records:\n");
  scanf("%s", fileName);
  file = fopen(fileName, "rb");
  if (file == NULL) {
	printf("Such file not exists!\n");
  } else {
	file = fopen(fileName, "ab");
	printf("Enter how many records do you want to add:\n");
	scanf("%d", &records);
	for (int i = 0; i < records; i++) {
	  printf("Enter student's surname:\n");
	  scanf("%s", add.surname);
	  printf("Enter student's PC number of processors:\n");
	  scanf("%d", &add.numberOfProcessors);
	  printf("Enter student's PC type of processors:\n");
	  printf("1. x32\n2. x64\n");
	  scanf("%s", add.typeOfProcessors);
	  printf("Enter student's PC RAM capacity(GB):\n");
	  scanf("%d", &add.memoryCapacity);
	  printf("Enter student's PC type of video controller:\n");
	  printf("1. built-in\n2. external\n3. AGP\n4. PCI\n");
	  scanf("%s", add.typeOfVideoController);
	  printf("Enter student's PC video memory capacity(GB):\n");
	  scanf("%d", &add.videoMemoryCapacity);
	  printf("Enter student's PC video memory type(GB):\n");
	  printf("1. SCSI/IDE\n2. ATA/SATA\n");
	  scanf("%s", add.videoMemoryType);
	  printf("Enter student's PC number of hard drives:\n");
	  scanf("%d", &add.numberOfHardDrives);
	  printf("Enter student's PC capacity of hard drives(GB):\n");
	  scanf("%d", &add.capacityOfHardDrives);
	  printf("Enter student's PC number of peripherals:\n");
	  scanf("%d", &add.peripherals);
	  printf("Enter student's PC OC:\n");
	  printf("1) Windows\n2) Linux\n3) MacOS\n");
	  scanf("%s", add.OC);
	  fwrite(&add, sizeof(PC), 1, file);
	}
	printf("Records added successfully!\n");
	fclose(file);
  }
}

// Выводим БД
void print() {
  char fileName[40];
  printf("Enter the name of the file you want to print:\n");
  scanf("%s", fileName);
  FILE *file;
  PC read;
  file = fopen(fileName, "rb");
  if (file == NULL) {
	printf("Such file not exists\n");
  } else {
	printf(
		"|Student's surname|Num Proc|Type of proc| RAM |Type video control|Video memory|Video memory type|Num HD|Capacity HD|Num peripherals|OC|\n");
	printf(
		"-------------------------------------------------------------------------------------------------------------------------------\n");
	while (fread(&read, sizeof(PC), 1, file) != EOF && !feof(file)) {
	  printf("|%17s|%8d|%12s|%3dGB|%18s|%10dGB|%17s|%4dGB|%9dGB|%15d|%2s|\n",
			 read.surname,
			 read.numberOfProcessors,
			 read.typeOfProcessors,
			 read.memoryCapacity,
			 read.typeOfVideoController,
			 read.videoMemoryCapacity,
			 read.videoMemoryType,
			 read.numberOfHardDrives,
			 read.capacityOfHardDrives,
			 read.peripherals,
			 read.OC);
	}
	fclose(file);
  }
}

// Удаление БД (файла)
void removes() {
  char fileName[40];
  printf("Enter the name of the file you want to delete:\n");
  scanf("%s", fileName);
  FILE *file;
  file = fopen(fileName, "rb");
  if (!file) {
	printf("Such file not exists!\n");
  } else {
	remove(fileName);
	printf("The file was successfully deleted!\n");
	fclose(file);
  }
}

// Подсчёт пользователей, имеющих
void CountUsers() {
  char fileName[40];
  int p;
  printf("Enter the name of the file you want to know how many users have no more than \"p\" peripheral devices:\n");
  scanf("%s", fileName);
  printf("Enter 'p':\n");
  scanf("%d", &p);
  FILE *file;
  PC f;
  int count = 0;
  file = fopen(fileName, "rb");
  if (!file) {
	printf("Such file not exists!\n");
  } else {
	while (fread(&f, sizeof(PC), 1, file) != EOF && !feof(file)) {
	  if (f.peripherals <= p) {
		count++;
	  }
	}
	printf("%d", count);
	fclose(file);
  }
}