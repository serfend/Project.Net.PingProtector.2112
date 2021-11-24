@echo off
echo.
echo         ###########################################################
echo         #                                                         #
echo         #                   《危险端口封锁工具》                  #
echo         #                   Version：2017.11.21                   #
echo         #                                                         #
echo         #                       【工具说明】                      #
echo         #                                                         #
echo         #     《危险端口封锁工具》用于辅助作业终端安全配置工作，  #
echo         # 主要启用了Windows防火墙，通过增加入站规则和出站规则对常 #
echo         # 见的危险端口进行封锁，本脚本将封锁以下端口：            #
echo         #    【20、21端口】，FTP服务默认端口；                    #
echo         #    【22端口】SSH服务端口                                #
echo         #    【23端口】Telnet服务端口                             #
echo         #    【135端口】使用RPC协议提供DCOM服务                   #
echo         #    【137端口】使用NetBIOS协议提供host或IP地址查询服务   #
echo         #    【138端口】使用NetBIOS协议提供局域网名称或计算机名称 #
echo         #    【139端口】通过网上邻居访问局域网共享文件和打印机    #
echo         #    【445端口】基于CIFS协议访问共享文件和打印机          #
echo         #    【3389端口】远程桌面服务                             #
echo         #    【5800端口】VNC-http端口                             #
echo         #    【3306端口】MySQL数据库服务默认端口                  #
echo         #    【5236端口】达梦数据库服务默认端口                   #
echo         #    【80、8000、8080端口】Web服务常用端口                #
echo         #                                                         #
echo         ###########################################################
echo.
echo                        确认以上信息后按任意键开始……
pause > nul
echo.
echo Windows主机防火墙启动中……
netsh advfirewall set allprofile state on
echo 正在关闭20端口……
netsh advfirewall firewall add rule name = "Disable port 20 - TCP" dir = in action = block protocol = TCP localport = 20
echo 正在关闭21端口……
netsh advfirewall firewall add rule name = "Disable port 21 - TCP" dir = in action = block protocol = TCP localport = 21
echo 正在关闭22端口……
netsh advfirewall firewall add rule name = "Disable port 22 - TCP" dir = in action = block protocol = TCP localport = 22
echo 正在关闭23端口……
netsh advfirewall firewall add rule name = "Disable port 23 - TCP" dir = in action = block protocol = TCP localport = 23
echo 正在关闭135端口……
netsh advfirewall firewall add rule name = "Disable port 135 - TCP" dir = in action = block protocol = TCP localport = 135
echo 正在关闭137端口……
netsh advfirewall firewall add rule name = "Disable port 137 - TCP" dir = in action = block protocol = TCP localport = 137
echo 正在关闭138端口……
netsh advfirewall firewall add rule name = "Disable port 138 - TCP" dir = in action = block protocol = TCP localport = 138
echo 正在关闭139端口……
netsh advfirewall firewall add rule name = "Disable port 139 - TCP" dir = in action = block protocol = TCP localport = 139
echo 正在关闭445端口……
netsh advfirewall firewall add rule name = "Disable port 445 - TCP" dir = in action = block protocol = TCP localport = 445
echo 正在关闭3389端口……
netsh advfirewall firewall add rule name = "Disable port 3389 - TCP" dir = in action = block protocol = TCP localport = 3389
echo 正在关闭3306端口……
netsh advfirewall firewall add rule name = "Disable port 3306 - TCP" dir = in action = block protocol = TCP localport = 3306
echo 正在关闭5236端口……
netsh advfirewall firewall add rule name = "Disable port 5236 - TCP" dir = in action = block protocol = TCP localport = 5236
echo 正在关闭5236端口……
netsh advfirewall firewall add rule name = "Disable port 5236 - TCP" dir = in action = block protocol = TCP localport = 5800
echo 正在关闭80端口……
netsh advfirewall firewall add rule name = "Disable port 80 - TCP" dir = in action = block protocol = TCP localport = 80
echo 正在关闭8000端口……
netsh advfirewall firewall add rule name = "Disable port 8000 - TCP" dir = in action = block protocol = TCP localport = 8000
echo 正在关闭4999端口……
netsh advfirewall firewall add rule name = "Disable port 4999 - TCP" dir = in action = block protocol = TCP localport = 4999
echo 正在关闭50000端口……
netsh advfirewall firewall add rule name = "Disable port 50000 - TCP" dir = in action = block protocol = TCP localport = 50000
echo =============================================
echo 作业终端的端口封锁工作已完成！