#include "pch.h"
#include "Server.h"

#define PWD "komsco.com"
#define ALLO "OK"
#define NOT_ALLO "NOK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";

extern "C" SERVER_API void server::key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}


extern "C" SERVER_API char* server::decrypt_msg(char* cipher_msg)
{
	char char_cipher_msg[255] = {};
	char p_msg[255] = {};

	strcpy_s(char_cipher_msg, cipher_msg);

	lea_cbc_dec((unsigned char*)p_msg, (const unsigned char*)char_cipher_msg,
		strlen(char_cipher_msg), (const unsigned char*)iv, &lea_key);

	return p_msg;
}


extern "C" SERVER_API char* server::compare_pwd(char* input_pwd)
{
	bool result = false;

	char* plain_pwd = decrypt_msg(input_pwd);

	if (strncmp(plain_pwd, PWD, strlen(PWD)) == 0)
	{
		return (char*)ALLO;
	}
	else
	{
		return (char*)NOT_ALLO;
	}

}


extern "C" SERVER_API char* server::test_string_3(char* temp) {
	static char strTemp2[128] = { 0, };	//임시저장용 문자열
	//sprintf_s(strTemp2, "%s strOnTest3 에서 리턴", temp);	//문자열 합치기
	//strcpy(strTemp2, temp);
	return strTemp2;
}
