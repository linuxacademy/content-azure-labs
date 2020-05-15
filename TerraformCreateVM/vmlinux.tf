locals {
  resource_group_name = ""
  location = "westus"
  public_key = ""
}

# configure azure provider
provider "azurerm" {
    version = "=2.0.0"
    features {}
}

resource "random_string" "rand" {
 length = 24
 special = false
 upper = false
}

# create network security group
resource "azurerm_network_security_group" "lab_nsg" {
  name                = random_string.rand.result
  location            = local.location
  resource_group_name = local.resource_group_name
}

# create network security group rule
resource "azurerm_network_security_rule" "example" {
  name                        = "SSH"
  priority                    = 100
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "Tcp"
  source_port_range           = "*"
  destination_port_range      = "22"
  source_address_prefix       = "*"
  destination_address_prefix  = "*"
  resource_group_name         = local.resource_group_name
  network_security_group_name = azurerm_network_security_group.lab_nsg.name
}

# create virtual network
resource "azurerm_virtual_network" "lab_vnet" {
  name                = random_string.rand.result
  location            = local.location
  resource_group_name = local.resource_group_name
  address_space       = ["10.0.0.0/16"]
  dns_servers         = ["10.0.0.4", "10.0.0.5"]
}

# create subnet
resource "azurerm_subnet" "lab_subnet" {
  name                 = "labSUBNET"
  resource_group_name  = local.resource_group_name
  virtual_network_name = azurerm_virtual_network.lab_vnet.name
  address_prefix       = "10.0.2.0/24"
}

# create public ip
resource "azurerm_public_ip" "lab_pip" {
  name                = "PublicIp1"
  location            = local.location
  resource_group_name = local.resource_group_name
  allocation_method   = "Dynamic"
}

# create network interface
resource "azurerm_network_interface" "lab_nic" {
  name                = "labNIC"
  location            = local.location
  resource_group_name = local.resource_group_name

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.lab_subnet.id
    private_ip_address_allocation = "Dynamic"
  }
}

resource "azurerm_virtual_machine" "lab_vm" {
  name                  = "labVM"
  location              = local.location
  resource_group_name   = local.resource_group_name
  network_interface_ids = [azurerm_network_interface.lab_nic.id]
  vm_size               = "Standard_B1ms"

  storage_image_reference {
    publisher = "Canonical"
    offer     = "UbuntuServer"
    sku       = "16.04-LTS"
    version   = "latest"
  }
  storage_os_disk {
    name              = "myosdisk1"
    caching           = "ReadWrite"
    create_option     = "FromImage"
    managed_disk_type = "Standard_LRS"
  }
  os_profile {
    computer_name  = "hostname"
    admin_username = "testadmin"
  }
  os_profile_linux_config {
    disable_password_authentication = true
    ssh_keys {
      path = "/home/testadmin/.ssh/authorized_keys"
      key_data = local.public_key
    }
  }
}