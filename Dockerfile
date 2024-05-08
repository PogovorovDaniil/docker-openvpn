FROM alpine
ENV PROTOCOL=udp \
    PORT=1194 \
    DNS1=1.1.1.1 \
    DNS2=1.0.0.1 \
    SUBNET="172.16.88.0 255.255.255.0"

COPY container-scripts/* /root/
COPY web-openvpn-publish/web-openvpn /root/

RUN apk update && \
    apk upgrade && \
    apk add openvpn easy-rsa iptables

RUN mkdir -p /dev/net && \
    mknod /dev/net/tun c 10 200 && \
    chmod 600 /dev/net/tun

CMD ["/root/server-run"]
