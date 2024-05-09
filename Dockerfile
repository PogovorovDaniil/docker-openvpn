FROM alpine

COPY container-scripts/* /root/
COPY web-openvpn-publish/web-openvpn /root/

RUN apk update && \
    apk upgrade && \
    apk add openvpn easy-rsa iptables

RUN ln -s /lib /lib64 && \
    ln -s /lib/libc.musl-x86_64.so.1 /lib/ld-linux-x86-64.so.2

RUN mkdir -p /dev/net && \
    mknod /dev/net/tun c 10 200 && \
    chmod 600 /dev/net/tun

CMD ["/root/server-run"]
