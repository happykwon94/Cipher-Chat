#pragma once

#include "pch.h"

#ifndef SERVER_EXPORTS
#define SERVER_API __declspec(dllexport)

extern "C"{

	SERVER_API void key_init();

	SERVER_API unsigned char* encrypt_msg(unsigned char* plain_msg);

	SERVER_API unsigned char* decrypt_msg(unsigned char* cipher_msg);

	SERVER_API bool compare_pwd(char* input_pwd);

	SERVER_API char* test_string_3(char* temp);
}
#endif