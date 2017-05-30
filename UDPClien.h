#pragma once
using boost::asio::ip::udp;

class UDPClient
{
public:
	static const int MESSAGE_LENGTH = 224;
	UDPClient(boost::asio::io_service& io_service, const std::string& host, const std::string& port);
	~UDPClient();

	void send(std::array<char, MESSAGE_LENGTH>& array);
	void send(const std::string& msg);

private:
	boost::asio::io_service& io_service_;
	udp::socket socket_;
	udp::endpoint endpoint_;	
};
