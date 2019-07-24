#include <iostream>
#include <WinSock2.h>
#include <process.h>
#include <thread>
#include <string>

#include "lea.h"

#pragma comment(lib, "ws2_32")

#define PORT 9999
#define PACKET_SIZE 1024
#define SERVER_IP "10.1.3.153"
#define keyPWD "OK"

LEA_KEY lea_key;
const char* iv = "internshipprog!";

//******************************************************************************************
// 암호화 과정

void key_init()
{
	lea_set_key(&lea_key, (const unsigned char*)iv, 16);
}

std::string encrypt_msg(std::string plain_msg)  // 암호화
{
	char char_plain_msg[255] = {};
	char c_msg[255] = {};

	while (plain_msg.length() % 16 != 0)
	{
		plain_msg.append(" ");
	}

	strcpy_s(char_plain_msg, plain_msg.c_str());

	lea_cbc_enc((unsigned char*)c_msg, (const unsigned char*)char_plain_msg,
		strlen(char_plain_msg), (const unsigned char*)iv, &lea_key);

	std::string cipher_msg(c_msg);

	return cipher_msg;
}

std::string decrypt_msg(std::string cipher_msg) // 복호화
{
	char char_cipher_msg[255] = {};
	char p_msg[255] = {};

	strcpy_s(char_cipher_msg, cipher_msg.c_str());

	lea_cbc_dec((unsigned char*)p_msg, (const unsigned char*)char_cipher_msg,
		strlen(char_cipher_msg), (const unsigned char*)iv, &lea_key);

	std::string plain_msg(p_msg);

	return plain_msg;
}

//******************************************************************************************

//**********************************************************************************************************
// 동작할 함수 작성

void send_msg(SOCKET target) // 입력받고 보내기
{
	while (true)
	{
		std::string input_msg;
		std::string c_msg;

		std::cout << ">> ";
		std::getline(std::cin, input_msg);

		if (input_msg.length() > 250)
		{
			std::cout << "글자수가 너무 많습니다!" << std::endl;
			break;
		}

		c_msg = encrypt_msg(input_msg); // 암호화

		const char* ptr_c_msg = c_msg.c_str(); // string 형 변환

		send(target, ptr_c_msg, strlen(ptr_c_msg), 0); // 타겟에 전송

		std::cin.clear();
		//std::cin.ignore(32767,'\n');
	}
}

void recv_msg(SOCKET target) // 전달 받고 출력하기
{
	while (true)
	{
		char output_buf[255] = {};
		std::string output_msg;

		int recvsize = recv(target, output_buf, sizeof(output_buf), 0); // 받아온다

		if (recvsize < 0)
		{
			printf("접속종료\n");
			break;
		}
		
		char output_temp[255] = {};
		strcpy(output_temp, output_buf);
		
		output_buf[recvsize] = '\0';

		std::string to_string_output_buf(output_buf); //char 배열을 string으로

		output_msg = decrypt_msg(to_string_output_buf); // 복호화

		std::cout << "\n[Receive MSG] " << output_msg << std::endl;
	}
}

void run_client(SOCKET target)
{
	auto send_th = std::thread(send_msg, target);
	auto recv_th = std::thread(recv_msg, target);

	send_th.join();
	recv_th.join();
}

//**********************************************************************************************************

SOCKET connect_server()
{
	WSAData wsaData;
	int open_flag = WSAStartup(MAKEWORD(2, 2), &wsaData);

	if (open_flag != 0)
	{
		std::cout << "[Open Error]" << std::endl;
	}

	SOCKET hSocket;
	hSocket = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);

	SOCKADDR_IN tAddr = { 0 };
	tAddr.sin_family = AF_INET;
	tAddr.sin_port = htons(PORT);
	tAddr.sin_addr.s_addr = inet_addr(SERVER_IP);

	int connect_flag = connect(hSocket, (SOCKADDR*)& tAddr, sizeof(tAddr));
	// connect(소켓, 소켓 구성요소 구조체 주소, 그 구조체 크기);

	if (connect_flag == SOCKET_ERROR)
	{
		std::cout << "[Connect Error] 서버가 닫혀있습니다." << std::endl;
		return 0;
	}
	else
	{
		return hSocket;
	}
}

//**********************************************************************************************************

//**********************************************************************************************************
// 비밀번호 확인

bool recv_pwd_result_decrypt(SOCKET target)
{
	bool result = false;
	char output_buf[255] = {};
	std::string ok_sign = keyPWD;

	int recv_result_size = recv(target, output_buf, sizeof(output_buf), 0);

	if (recv_result_size < 0)
	{
		printf("접속종료\n");
		return 0;
	}

	//output_buf[recv_result_size] = '\0';

	std::string to_string_output_buf(output_buf); //char 배열을 string으로

	if (ok_sign.compare(to_string_output_buf) == 0)
	{
		result = true;
	}

	return result;
}

bool send_pwd(SOCKET target)
{
	while (true)
	{
		std::string input_pwd;
		std::string cipher_input_pwd;
		std::string pwd_result;

		std::cout << "[ Input Your Password ] " << std::endl;
		std::cout << ">> ";
		std::getline(std::cin, input_pwd);

		if (input_pwd.length() > 250)
		{
			std::cout << "글자수가 너무 많습니다!" << std::endl;
			break;
		}

		cipher_input_pwd = encrypt_msg(input_pwd); // 암호화

		const char* ptr_c_input_pwd = cipher_input_pwd.c_str(); // string 형 변환

		send(target, ptr_c_input_pwd, strlen(ptr_c_input_pwd), 0);

		if (recv_pwd_result_decrypt(target))
		{
			system("cls");
			std::cout << "[ Login Success ]" << std::endl;
			return true;
		}
		else
		{
			std::cout << "[ Login Fail ]" << std::endl;
		}
		std::cin.clear();
	}
}

//**********************************************************************************************************

int main()
{
	key_init();

	SOCKET root = connect_server();

	if (root != 0)
	{
		std::cout << "[ Welcome Chatting Server ]" << std::endl;

		if (send_pwd(root))
		{
			run_client(root);
		}
	}
}