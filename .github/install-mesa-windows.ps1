function grantRights($file) {
	takeown /f $file
	icacls $file /grant "${env:ComputerName}\${env:UserName}:F"
}

function InstallMesaOpenGL ($basedir) {
	$filepath = $basedir + "opengl32.dll"
	if (Test-Path $filepath) {
		grantRights $filepath
		Rename-Item -Path $filepath -NewName opengl32.dll_old
	}
	$source_path = "./download/*"
	Copy-Item -Path $source_path -Destination $basedir
}

Invoke-WebRequest https://github.com/jvbsl/MesaBinary/releases/download/21_shared/mesa_x64-windows_dll.zip -OutFile download.zip
Expand-Archive -Force ./download.zip

InstallMesaOpenGL "$env:WINDIR\system32\"
Write-Host "Done"