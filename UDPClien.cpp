#include "stdafx.h"

#include <iostream>
#include <boost/asio.hpp>

#include "UDPClien.h"

using boost::asio::ip::udp;

UDPClient::UDPClient(boost::asio::io_service& io_service,	const std::string& host,const std::string& port) : io_service_(io_service), 
																										   socket_(io_service, udp::endpoint(udp::v4(), 0))
{
	udp::resolver resolver(io_service_);
	udp::resolver::query query(udp::v4(), host, port);
	udp::resolver::iterator iter = resolver.resolve(query);
	endpoint_ = *iter;
}

UDPClient::~UDPClient()
{
	socket_.close();
}

void UDPClient::send(std::array<char, UDPClient::MESSAGE_LENGTH>& array) {
	socket_.send_to(boost::asio::buffer(boost::asio::buffer(array)), endpoint_);
}

void UDPClient::send(const std::string& msg) {
	socket_.send_to(boost::asio::buffer(msg, msg.size()), endpoint_);
}
