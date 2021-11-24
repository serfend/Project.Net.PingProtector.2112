
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Ole /v EnableDCOM   /t REG_SZ /d N /f
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Rpc /v "DCOM Protocols" /t REG_MULTI_SZ /d "ncacn_spx\0ncacn_nb_nb\0ncacn_nb_ipx" /f
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Rpc\Internet

reg add HKLM\SYSTEM\CurrentControlSet\Services\NetBT\Parameters /v SMBDeviceEnabled   /t reg_dword /d 0 /f


reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\NetBT /v Start /t reg_dword /d 4 /f
reg add HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\NetBIOS /v Start /t reg_dword /d 4 /f

sc config LanmanServer start= disabled
sc config msdtc start= disabled

shutdown -r

