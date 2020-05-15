param(
    [string]$content_package_name = "AnsibleCreateVM"
)


Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$url = "https://github.com/linuxacademy/content-azure-labs/blob/master/zips/${content_package_name}.zip?raw=true" 
wget -UseBasicParsing -OutFile d:\content.zip $url

Unzip -zipfile D:\content.zip d:\

Remove-Item -Path D:\content.zip
