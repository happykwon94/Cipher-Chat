#include "pch.h"
#include "client.h"

#define keyPWD "OK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";


//******************************************************************************************
// ��ȣȭ ����

void key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

char* encrypt_msg(char* plain_msg)  // ��ȣȭ
{
	static char temp[128] = { 0, };	//�ӽ������ ���ڿ�
	char cipher_msg[128] = {};
	static char return_msg[128] = { 0, };
	static char result[128] = { 0, };
	
	strcpy(temp, plain_msg);

	std::string s_temp = temp;

	while (s_temp.length() % 16 != 0)
	{
		s_temp.append(" ");
	}

	lea_cbc_enc((unsigned char*)cipher_msg, (const unsigned char*)s_temp.c_str(),
		s_temp.length(), (const unsigned char*)iv, &lea_key);

	strcpy(return_msg, cipher_msg);	
	
	char * returnchar = (char*)LocalAlloc(LPTR, strlen(return_msg) + 1);

	strcat(returnchar, return_msg);

	return returnchar;

}

//******************************************************************************************

//**********************************************************************************************************

void decrypt_msg(char* cipher_msg, char* plain_msg)
{
	static char temp[128] = { 0, };	//�ӽ������ ���ڿ�

	char plain_msg_temp[255] = { 0, };

	static char return_msg[128] = { 0, };

	char char_cipher_msg[255] = {};

	strcpy(temp, cipher_msg);

	std::string c_temp = temp;

	strcpy(char_cipher_msg, c_temp.c_str());

	lea_cbc_dec((unsigned char*)plain_msg_temp, (const unsigned char*)char_cipher_msg,
		strlen(char_cipher_msg), (const unsigned char*)iv, &lea_key);

	strcpy(plain_msg, char_cipher_msg);
}

//**********************************************************************************************************

//**********************************************************************************************************
// ��й�ȣ Ȯ��

bool recv_pwd_result_decrypt(char* input_pwd)
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

char* test_string_3(char* temp) {
	static char strTemp2[128] = { 0, };	//�ӽ������ ���ڿ�
	//sprintf_s(strTemp2, "%s strOnTest3 ���� ����", temp);	//���ڿ� ��ġ��
	strcpy(strTemp2, temp);
	return strTemp2;
}
