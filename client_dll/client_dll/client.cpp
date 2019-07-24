#include "pch.h"
#include "client.h"

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

//******************************************************************************************

//**********************************************************************************************************
// 동작할 함수 작성
char* client::decrypt_msg(char* cipher_msg)
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	strcpy(temp, cipher_msg);

	static char plain_msg[255] = { 0, };

	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)temp,
		strlen(temp), (const unsigned char*)iv, &lea_key);

	return plain_msg;
}

//**********************************************************************************************************

//**********************************************************************************************************
// 비밀번호 확인

bool client::recv_pwd_result_decrypt(char* input_pwd)
{
	bool result = false;

	if (strlen(input_pwd) < 0)
	{
		return result;
	}

	if (strncmp(keyPWD, input_pwd, strlen(keyPWD)) == 0)
	{
		result = true;
	}

	return result;
}

//**********************************************************************************************************

char* client::test_string_3(char* temp) {
	static char strTemp2[128] = { 0, };	//임시저장용 문자열
	//sprintf_s(strTemp2, "%s strOnTest3 에서 리턴", temp);	//문자열 합치기
	strcpy(strTemp2, temp);
	return strTemp2;
}