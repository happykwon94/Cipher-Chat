#pragma once

#include "pch.h"

#ifndef SERVER_EXPORTS
#define SERVER_API __declspec(dllexport)

namespace server{

	extern "C" SERVER_API void key_init();

	extern "C" SERVER_API char* decrypt_msg(char* cipher_msg);

	extern "C" SERVER_API char* recv_thread(SOCKET target);

	extern "C" SERVER_API char* compare_pwd(char* input_pwd);

	extern "C" SERVER_API char* test_string_3(char* temp);
}
#endif