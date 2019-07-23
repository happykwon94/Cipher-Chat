#include "pch.h"
#include "client.h"

#define PACKET_SIZE 1024
#define keyPWD "OK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";


//******************************************************************************************
// 암호화 과정

void client::key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

char* client::encrypt_msg(char* plain_msg)  // 암호화
{
	char char_plain_msg[255] = {};
	char c_msg[255] = {};

	for (int i = strlen(plain_msg); strlen(plain_msg) % 16 != 0; i++) {
		plain_msg[i] = (char)" ";
	}

	strcpy_s(char_plain_msg, plain_msg);

	lea_cbc_enc((unsigned char*)c_msg, (const unsigned char*)char_plain_msg,
		strlen(char_plain_msg), (const unsigned char*)iv, &lea_key);

	return c_msg;
}

//******************************************************************************************

//**********************************************************************************************************
// 동작할 함수 작성

//**********************************************************************************************************

//**********************************************************************************************************
// 비밀번호 확인

bool client::recv_pwd_result_decrypt(char* input_pwd)
{
	bool result = false;
	char output_buf[255] = {};
	std::string ok_sign = keyPWD;

	if (strlen(input_pwd) < 0)
	{
		return false;
	}

	if (strncmp(keyPWD, input_pwd, strlen(keyPWD)) == 0)
	{
		result = true;
	}

	return result;
}

//**********************************************************************************************************