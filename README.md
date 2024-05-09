# docker-openvpn

This project provides all necessary tools for deploying an OpenVPN server in a Docker container. It includes scripts for building the container image, initializing the server, and managing clients.

## Features

- **Full configuration via environment variables**: Set protocol, DNS, and other network parameters via environment variables.
- **Client management**: Ability to manage client profiles through a web interface.
- **Scalability**: Easy addition and management of VPN clients.

## Prerequisites

Before using this project, you need to install Docker and Docker Compose. Installation instructions can be found at the following links:
- [Install Docker](https://docs.docker.com/get-docker/)
- [Install Docker Compose](https://docs.docker.com/compose/install/)

## Installation and Configuration

1. Create a directory for the server and navigate into it:
```bash
mkdir openvpn
cd openvpn
```
2. Create a `docker-compose.yml` file with the following content:
```yaml
services:
    openvpn:
        image: gigster2000/openvpn:v1.0
        restart: unless-stopped
        environment:
            - PROTOCOL=udp
            - INTERFACE_TYPE=tun
            - PORT=1194
            - DNS1=1.1.1.1
            - DNS2=1.0.0.1
            - SUBNET=172.16.88.0 255.255.255.0
            - WEB_OPENVPN_PASSWORD=qwerty
        volumes:
            - ./volume:/etc/openvpn
        ports:
            - 1194:1194/udp
            - 8080:8080/tcp
        cap_add:
            - NET_ADMIN
        devices:
            - /dev/net/tun
```

3. Launch the project:
```bash
docker compose up -d
```
This will create and start all necessary Docker containers in the background. Please note that the initial launch may take longer due to VPN server initialization.

## Usage

- **Accessing the web interface**: After starting the server, open `http://<server-IP>:8080` in your browser to access the web interface.
- **Client management**: Use the web interface to create, delete, or manage client profiles. To authenticate, use the password specified in the docker-compose file under environment: WEB_OPENVPN_PASSWORD.
- **Server management**: You can start and stop the VPN server using Docker Compose commands.
```bash
docker compose up -d # Start
docker compose down # Stop
```

## Support

If you have any questions or issues, feel free to create an issue in the GitHub repository.

## License

This project is distributed under the MIT License. See the `LICENSE` file for details.
