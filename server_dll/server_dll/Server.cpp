#include "pch.h"
#include "Server.h"

#define PWD "komsco.com"
#define ALLO "OK"
#define NOT_ALLO "NOK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";

void server::key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

char* server::encrypt_msg(char* plain_msg)  // 암호화
{
	static char temp[128] = { 0, };	//임시저장용 문자열

	static char cipher_msg[255] = { 0, };


	for (int i = strlen(plain_msg); (strlen(plain_msg) % 16) != 0; i++) {
		*(plain_msg + i) = '*';
		*(plain_msg + i + 1) = '\0';
	}

	strcpy_s(temp, plain_msg);

	lea_cbc_enc((unsigned char*)cipher_msg, (const unsigned char*)temp,
		strlen(temp), (const unsigned char*)iv, &lea_key);

	return cipher_msg;

}

char* server::decrypt_msg(char* cipher_msg)
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	strcpy_s(temp, cipher_msg);

	static char plain_msg[255] = { 0, };

	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)temp,
		strlen(temp), (const unsigned char*)iv, &lea_key);

	return plain_msg;

}


char* server::compare_pwd(char* input_pwd)
{
	bool result = false;

	static char* plain_pwd = decrypt_msg(input_pwd);

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
	strcpy(strTemp2, temp);
	return strTemp2;
}
