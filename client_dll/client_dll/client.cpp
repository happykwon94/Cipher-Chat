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

	return ;
}

//char* client::encrypt_msg(char* plain_msg)  // 암호화
//{
//	static char temp[128] = { 0, };	//임시저장용 문자열
//
//	static char cipher_msg[255] = { 0, };
//
//
//	for (int i = strlen(plain_msg); (strlen(plain_msg) % 16) != 0; i++) {
//		*(plain_msg + i) = ' ';
//		*(plain_msg + i + 1) = '\0';
//	}
//
//	strcpy_s(temp, plain_msg);
//
//	lea_cbc_enc((unsigned char*)cipher_msg, (const unsigned char*)temp,
//		strlen(temp), (const unsigned char*)iv, &lea_key);
//
//	return cipher_msg;
//}

wchar_t* client::encrypt_msg(char* plain_msg)  // 암호화
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	char cipher_msg[255] = {};
	static char return_msg[128] = { 0, };
	static char result[128] = { 0, };
	
	strcpy(temp, plain_msg);

	std::string s_temp = temp;

	int size_needed = MultiByteToWideChar(CP_UTF8, 0, &s_temp[0], (int)s_temp.size(), NULL, 0);
	std::wstring wstrTo(size_needed, 0);
	MultiByteToWideChar(CP_UTF8, 0, &s_temp[0], (int)s_temp.size(), &wstrTo[0], size_needed);

	while (wstrTo.length() % 16 != 0)
	{
		wstrTo.append('\0');
	}

	lea_cbc_enc((unsigned char*)cipher_msg, (const unsigned char*)wstrTo.c_str(),
		wstrTo.length(), (const unsigned char*)iv, &lea_key);

	strcpy(return_msg, cipher_msg);

	std::string c_temp = strcpy(return_msg, cipher_msg);

	int size_needed = MultiByteToWideChar(CP_UTF8, 0, &c_temp[0], (int)c_temp.size(), NULL, 0);
	std::wstring c_wstrTo(size_needed, 0);
	MultiByteToWideChar(CP_UTF8, 0, &c_temp[0], (int)c_temp.size(), &wstrTo[0], size_needed);

	return ;
}

int client::encrypt_msg_int(char* plain_msg)  // 암호화
{
	static char temp[128] = { 0, };	//임시저장용 문자열

	strcpy(temp, plain_msg);

	std::string temp_str = temp;

	int size_needed = MultiByteToWideChar(CP_UTF8, 0, &temp_str[0], (int)temp_str.size(), NULL, 0);
	std::wstring wstrTo(size_needed, 0);
	MultiByteToWideChar(CP_UTF8, 0, &temp_str[0], (int)temp_str.size(), &wstrTo[0], size_needed);

	return wstrTo.length();
}

//******************************************************************************************

//**********************************************************************************************************
// 동작할 함수 작성
//char* client::decrypt_msg(char* cipher_msg)
//{
//	static char temp[128] = {0,};	//임시저장용 문자열
//
//	static char plain_msg[255] = { 0, };
//
//	strcpy_s(temp, cipher_msg);
//
//	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)temp,
//		strlen(temp), (const unsigned char*)iv, &lea_key);
//
//	return plain_msg;
//}

char* client::decrypt_msg(char* cipher_msg)
{
	static char temp[128] = { 0, };	//임시저장용 문자열
	char cipher_msg[255] = {};
	static char return_msg[128] = { 0, };

	strcpy(temp, cipher_msg);

	lea_cbc_dec((unsigned char*)cipher_msg, (const unsigned char*)wstrTo.c_str(),
		wstrTo.length(), (const unsigned char*)iv, &lea_key);

	strcpy(return_msg, cipher_msg);

	return return_msg;


	char plain_msg[255] = { 0, };

	static char return_msg[128] = { 0, };

	char char_cipher_msg[255] = {};
	int msg_size = 16;

	strcpy(temp, cipher_msg);

	std::string c_temp = temp;

	strcpy(char_cipher_msg, c_temp.c_str());

	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)char_cipher_msg,
		msg_size, (const unsigned char*)iv, &lea_key);

	strcpy(return_msg, char_cipher_msg);

	return return_msg;
}

int client::decrypt_msg_int(char* cipher_msg)
{
	static char temp[128] = { 0, };	//임시저장용 문자열

	char plain_msg[255] = { 0, };

	static char return_msg[128] = { 0, };

	char char_cipher_msg[255] = {};

	strcpy(temp, cipher_msg);

	std::string c_temp = temp;

	strcpy(char_cipher_msg, c_temp.c_str());

	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)char_cipher_msg,
		strlen(char_cipher_msg), (const unsigned char*)iv, &lea_key);

	strcpy(return_msg, char_cipher_msg);

	return strlen(temp);
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
