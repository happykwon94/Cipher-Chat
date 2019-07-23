#include <iostream>
#include <thread>
#include <winsock2.h>
#include <process.h>
#include <vector>
#include <string>

#include "lea.h"

#pragma comment(lib, "ws2_32")
#pragma comment(lib, "libLEA")

#define PORT 9999
#define PACKET_SIZE 1024
#define SERVER_IP "10.1.3.153"
#define PWD "komsco.com"
#define ALLO "OK"
#define NOT_ALLO "NOK"


LEA_KEY lea_key;
const char* iv = "internshipprog!";

//**********************************************************************************************************
// 암호화 & 복호화 과정

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

//**********************************************************************************************************


//**********************************************************************************************************
// 쓰레드로 돌릴 함수 작성

void send_thread(SOCKET target) // 입력받고 보내기
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

void recv_thread(SOCKET target) // 전달 받고 출력하기
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

		output_buf[recvsize] = '\0';

		std::string to_string_output_buf(output_buf); //char 배열을 string으로

		output_msg = decrypt_msg(to_string_output_buf); // 복호화

		std::cout << "\n[Receive MSG] " << output_msg << std::endl;
	}
}

void run_server_thread(SOCKET client_socket)
{
	auto send_th = std::thread(send_thread, client_socket);
	auto recv_th = std::thread(recv_thread, client_socket);

	send_th.join();
	recv_th.join();
}

//**********************************************************************************************************


//**********************************************************************************************************
// 비밀번호 확인해서 로그인

bool compare_pwd(std::string input_pwd)
{
	bool result = false;
	std::string password = PWD;

	if (strncmp(input_pwd.c_str(), password.c_str(), password.length()) == 0)
	{
		result = true;
	}

	return result;
}

bool login_step(SOCKET target)
{
	int thread_counter = 0;

	while (true)
	{
		char output_buf[255] = {};
		std::string output_msg;

		int recvsize = recv(target, output_buf, sizeof(output_buf), 0); // 받아온다

		if (recvsize < 0)
		{
			std::cout << "접속종료\n" << std::endl;
			break;
		}

		//output_buf[recvsize] = '\0';

		std::string to_string_output_buf(output_buf); //char 배열을 string으로

		output_msg = decrypt_msg(to_string_output_buf); // 복호화
		to_string_output_buf.clear();

		if (compare_pwd(output_msg))
		{
			std::cout << "[ Client" << thread_counter << " Login ! ]" << std::endl;
			//char ok_sign[5] = "OK";
			//send(target, ok_sign, strlen(ok_sign), 0);
			send(target, ALLO, strlen(ALLO), 0);
			thread_counter++;
			return true;
		}
		else
		{
			std::cout << "[Client" << thread_counter << " Password FAIL]" << std::endl;
			//char not_ok_sign[5] = "NOK";
			send(target, NOT_ALLO, strlen(NOT_ALLO), 0);
		}
		output_msg.clear();
	}

	return false;
}

//**********************************************************************************************************


//**********************************************************************************************************
// 서버를 만들고 클라이언트의 접속을 기다리고 있는 소켓 생성

SOCKET make_server()
{
	WSADATA wsaData;
	int openSignal = WSAStartup(MAKEWORD(2, 2), &wsaData);

	if (openSignal != 0)
	{
		std::cout << "[Open Error - 연결 실패]" << std::endl;
		return 0;
	}

	SOCKET hListen;
	hListen = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (hListen == SOCKET_ERROR)
	{
		std::cout << "[Socket Error - 연결 실패]" << std::endl;
		return 0;
	}

	SOCKADDR_IN tListenAddr = {};
	tListenAddr.sin_family = AF_INET;
	tListenAddr.sin_port = htons(PORT);
	tListenAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	int bindSignal = bind(hListen, (SOCKADDR*)& tListenAddr, sizeof(tListenAddr));


	if (bindSignal == SOCKET_ERROR)
	{
		std::cout << "[Bind Error - 연결 실패]" << std::endl;
		return 0;
	}

	std::cout << "[Server Open] Client를 기다리고 있습니다." << std::endl;

	listen(hListen, SOMAXCONN);

	return hListen;
}

// 접속 요청 들어오면 실행
void add_accept_socket(SOCKET root_socket)
{
	sockaddr_in tcintaddr = {};
	int icintsize = sizeof(tcintaddr);
	std::vector<std::thread> thread_array;

	while (true)
	{
		SOCKET client = accept(root_socket, (SOCKADDR*)& tcintaddr, &icintsize);

		if (client == SOCKET_ERROR)
		{
			std::cout << "[Accept Error]" << std::endl;
			break;
		}

		if (login_step(client))
		{
			auto run_th = std::thread(run_server_thread, client);

			run_th.detach();
		}

	}

	std::cout << "[접속 종료]" << std::endl;
}

void clean_server(SOCKET target)
{
	closesocket(target);

	WSACleanup();
}

//**********************************************************************************************************

int main()
{
	key_init();

	SOCKET root = make_server();

	if (root != -1)
	{
		while (true)
		{
			add_accept_socket(root);
		}
	}
	clean_server(root);
}