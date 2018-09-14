#!/bin/sh
sudo -i
apt-get update
apt-get install apache2 -y
ufw allow 'Apache Full'
ufw enable 
