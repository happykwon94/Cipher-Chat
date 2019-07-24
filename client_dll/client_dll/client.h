#pragma once
#include "pch.h"

#ifndef CLINET_EXPORTS
#define CLIENT_API __declspec(dllexport)


namespace client
{
	extern "C" CLIENT_API void key_init();

	extern "C" CLIENT_API char* encrypt_msg(char* plain_msg);

	extern "C" CLIENT_API char* decrypt_msg(char* cipher_msg);

	extern "C" CLIENT_API bool recv_pwd_result_decrypt(char* input_pwd);

	extern "C" CLIENT_API char* test_string_3(char* temp);
}
#endif