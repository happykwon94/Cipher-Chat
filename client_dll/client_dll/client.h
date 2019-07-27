#pragma once
#include "pch.h"

extern "C"
{
	__declspec(dllexport) void key_init();

	__declspec(dllexport) char* encrypt_msg(char* plain_msg);

	__declspec(dllexport) void decrypt_msg(char* cipher_msg, char* plain_msg);

	__declspec(dllexport) bool recv_pwd_result_decrypt(char* input_pwd);

	__declspec(dllexport) char* test_string_3(char* temp);

}