#include "pch.h"
#include "Server.h"

#define PWD "komsco.com"

LEA_KEY lea_key;
const char* iv = "internshipprog!";

void key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

unsigned char* encrypt_msg(unsigned char* plain_msg, int& size)  // 암호화
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	unsigned char cipher_msg_op[1024] = {0, };
	int result_cipher[128];

	strcpy(temp, (char*)plain_msg);

	std::string s_temp = temp;

	while (s_temp.length() % 16 != 0)
	{
		s_temp += (char)" ";
	}

	lea_cbc_enc(cipher_msg_op, (const unsigned char*)s_temp.c_str(),
		s_temp.length(), (const unsigned char*)iv, &lea_key);

	size = strlen((const char*)cipher_msg_op);

	return cipher_msg_op;
}

unsigned char* decrypt_msg(unsigned char* cipher_msg, int& size)
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	unsigned char plain_msg_op[1024] = { 0,  };

	lea_cbc_dec(plain_msg_op, (const unsigned char*)cipher_msg, strlen((const char*)cipher_msg), (const unsigned char*)iv, &lea_key);

	size = strlen((const char*)plain_msg_op);

	return plain_msg_op;
}


