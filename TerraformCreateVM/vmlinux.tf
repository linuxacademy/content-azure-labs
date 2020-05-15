locals {
  resource_group_name = ""
  public_key = ""
  location = "southcentralus"
}

# Configure Azure Provider
provider "azurerm" {
    version = "=2.0.0"
    features {}
}

# Configure Azure Provider
provider "azurerm" {
    version = "=2.0.0"
    features {}
}

# Create virtual network
resource "azurerm_virtual_network" "lab_vnet" {
    name = "labVNET"
    address_space = ["10.0.0.0/16"]
    location = "local.location"
    resource_group_name = "local.resource_group_name"
}

# Create subnet
resource "azurerm_subnet" "lab_subnet" {
    name = "labSUBNET"
    resource_group_name = "local.resource_group_name"
    virtual_network_name = "${azurerm_virtual_network.lab_vnet.name}"
    address_prefix = "10.0.1.0/24"
}

# Create public IPs
resource "azurerm_public_ip" "lab_pip" {
    name = "labPIP"
    location = "local.location"
    resource_group_name = "local.resource_group_name"
    allocation_method = "Dynamic"
}

# Create Network Security Group and rule
resource "azurerm_network_security_group" "lab_nsg" {
    name = "labNSG"
    location = "local.location"
    resource_group_name = "local.resource_group_name"
    security_rule {
        name = "SSH"
        priority = 1001
        direction = "Inbound"
        access = "Allow"
        protocol = "Tcp"
        source_port_range = "*"
        destination_port_range = "22"
        source_address_prefix = "*"
        destination_address_prefix = "*"
    }
}

# Create network interface
resource "azurerm_network_interface" "lab_nic" {
    name = "labNIC"
    location = "local.location"
    resource_group_name = "local.resource_group_name"
    
    ip_configuration {
        name = "labNicConfiguration"
        subnet_id = "${azurerm_subnet.lab_subnet.id}"
        private_ip_address_allocation = "dynamic"
        public_ip_address_id = "${azurerm_public_ip.lab_pip.id}"
    }
}

# Create virtual machine
resource "azurerm_virtual_machine" "lab_vm" {
    name = "labVM"
    location = "local.location"
    resource_group_name = "local.resource_group_name"
    network_interface_ids = ["${azurerm_network_interface.lab_nic.id}"]
    vm_size = "Standard_B1ms"
    storage_os_disk {
        name = "labVMOSDISK"
        caching = "ReadWrite"
        create_option = "FromImage"
        managed_disk_type = "Premium_LRS"
    }
    storage_image_reference {
        publisher = "Canonical"
        offer = "UbuntuServer"
        sku = "16.04.0-LTS"
        version = "latest"
    }
    os_profile {
        computer_name = "labvm"
        admin_username = "labvmadmin"
    }
    os_profile_linux_config {
        disable_password_authentication = true
        ssh_keys {
            path = "/home/labvmadmin/.ssh/authorized_keys"
            key_data = "local.public_key"
        }
    }
}