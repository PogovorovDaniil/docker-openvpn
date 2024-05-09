.PHONY: all image clean

all: image


image: web-openvpn-publish/web-openvpn
	docker build -t gigster2000/openvpn .

web-openvpn-publish/web-openvpn:
	dotnet publish ./web-openvpn -r linux-musl-x64 -c Release -o ./web-openvpn-publish

clean:
	rm -rf ./web-openvpn-publish
	rm -rf ./web-openvpn/bin
	rm -rf ./web-openvpn/obj
	docker system prune
