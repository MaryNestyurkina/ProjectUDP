// UdpSender.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <boost/asio.hpp>
#include "UDPClien.h"
#include <fstream>
#include <iostream>


int main()
{
	const char* filename = "D:\\projects\\ash\\UdpSender\\UdpSender\\Debug\\data.bin";
	std::ifstream inFile(filename, std::ios::binary | std::ios::in);

	boost::asio::io_service io_service;
	UDPClient client(io_service, "localhost", "8765");

	char buffer[UDPClient::MESSAGE_LENGTH];

	client.send("START");

	while (!inFile.eof()) {		
		inFile.read(buffer, UDPClient::MESSAGE_LENGTH);
		std::array<char, UDPClient::MESSAGE_LENGTH>* data = new std::array<char, UDPClient::MESSAGE_LENGTH>();

		for (int i = 0; i != UDPClient::MESSAGE_LENGTH; i++) {			
			data->at(i) = buffer[i];
		}

		client.send(*data);
	}

	client.send("STOP");

    return 0;
}

