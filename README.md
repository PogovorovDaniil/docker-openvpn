# docker-openvpn

This project provides all necessary tools to deploy an OpenVPN server in a Docker container. It includes scripts for building the container image, initializing the server, and managing clients.

## Prerequisites

Before using this project, you need to install Docker and Docker Compose. Follow these links for installation instructions:

- [Install Docker](https://docs.docker.com/get-docker/)
- [Install Docker Compose](https://docs.docker.com/compose/install/)

## Installation and Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/PogovorovDaniil/docker-openvpn.git
   cd docker-openvpn
   ```

2. Build the Docker image:
   ```bash
   ./image-build
   ```

3. Create and initialize the OpenVPN server:
   ```bash
   ./server-create
   ```

4. Start the OpenVPN server:
   ```bash
   docker-compose up -d
   ```

5. To stop and remove the server containers:
   ```bash
   docker-compose down
   ```

## Client Management

All client files are stored at ./volume/client.

### Client Commands:

- Create a new user:
  ```bash
  ./client create {username}
  ```

- Remove a user:
  ```bash
  ./client remove {username}
  ```

- Build a configuration file for a client:
  ```bash
  ./client build {username}
  ```

- List all clients:
  ```bash
  ./client list
  ```

## General Information

After executing `./server-create`, the OpenVPN server will be initialized and ready for use. You can manage client profiles using the `./client` script, and start or stop the VPN server using Docker Compose.

This project makes it easy to scale and manage a VPN server, providing a simple interface for handling clients.
