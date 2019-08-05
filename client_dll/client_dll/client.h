#pragma once
#include "pch.h"

extern "C" {
	__declspec(dllexport) void key_init();

	__declspec(dllexport) unsigned char* encrypt_msg(unsigned char* plain_msg, int& size);

	__declspec(dllexport) unsigned char* decrypt_msg(unsigned char* cipher_msg, int& size);
}
