#!/bin/bash

# update 
sudo apt-get update

# install gnome desktop
sudo apt-get install -y ubuntu-gnome-desktop

# sleep
sleep 9m

# install xrdp 
sudo apt-get install -y xrdp

# change console access from root only to all users 
sudo sed -i 's/allowed_users=console/allowed_users=anybody/' /etc/X11/Xwrapper.config

# bypass the authentication screen
cat > /etc/polkit-1/localauthority.conf.d/02-allow-colord.conf << EOF
polkit.addRule(function(action, subject) {
if ((action.id == "org.freedesktop.color-manager.create-device" ||
action.id == "org.freedesktop.color-manager.create-profile" ||
action.id == "org.freedesktop.color-manager.delete-device" ||
action.id == "org.freedesktop.color-manager.delete-profile" ||
action.id == "org.freedesktop.color-manager.modify-device" ||
action.id == "org.freedesktop.color-manager.modify-profile") &&
subject.isInGroup("{group}")) {
return polkit.Result.YES;
}
}); 
EOF

# install the gnome tweak tool to display the desktop dock
sudo apt-get install -y gnome-tweak-tool

# install yum utils
sudo apt-get install -y yum-utils

# Install code-server
wget https://github.com/cdr/code-server/releases/download/2.1688-vsc1.39.2/code-server2.1688-vsc1.39.2-linux-x86_64.tar.gz
tar -xvzf code-server2.1688-vsc1.39.2-linux-x86_64.tar.gz
cp code-server2.1688-vsc1.39.2-linux-x86_64/code-server /usr/local/bin/

# Install specific extensions
# su -c "/usr/local/bin/code-server --install-extension ms-python.python" cloud_user

# Start code-server systemd service
cat > /etc/systemd/system/code-server.service <<EOF
[Unit]
Description=VSCode on the Web
After=network.target
[Service]
User=chad
Group=chad
WorkingDirectory=/home/chad
ExecStart=/usr/local/bin/code-server -p 8080 --auth none
[Install]
WantedBy=multi-user.target
EOF

systemctl start code-server.service
systemctl enable code-server.service

sleep 20

# Download the Microsoft repository GPG keys
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb

# Register the Microsoft repository GPG keys
sudo dpkg -i packages-microsoft-prod.deb

# Update the list of products
sudo apt-get update

# Enable the "universe" repositories
sudo add-apt-repository universe

# Install PowerShell
sudo apt-get install -y powershell

# Start PowerShell
pwsh