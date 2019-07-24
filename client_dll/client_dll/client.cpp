#include "pch.h"
#include "client.h"

#define keyPWD "OK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";


//******************************************************************************************
// ��ȣȭ ����

void client::key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

char* client::encrypt_msg(char* plain_msg)  // ��ȣȭ
{
	static char temp[128] = { 0, };	//�ӽ������ ���ڿ�

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
// ������ �Լ� �ۼ�
char* client::decrypt_msg(char* cipher_msg)
{
	static char temp[128] = { 0, };	//�ӽ������ ���ڿ�
	strcpy(temp, cipher_msg);

	static char plain_msg[255] = { 0, };

	lea_cbc_dec((unsigned char*)plain_msg, (const unsigned char*)temp,
		strlen(temp), (const unsigned char*)iv, &lea_key);

	return plain_msg;
}

//**********************************************************************************************************

//**********************************************************************************************************
// ��й�ȣ Ȯ��

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
	static char strTemp2[128] = { 0, };	//�ӽ������ ���ڿ�
	//sprintf_s(strTemp2, "%s strOnTest3 ���� ����", temp);	//���ڿ� ��ġ��
	strcpy(strTemp2, temp);
	return strTemp2;
}