@echo off
cls
echo.
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  Windows 安全加固 
echo.
echo                           30所
echo                           2018-04-07
echo.
echo  注：        不要放在根目录执行
echo              请用Administrator账号运行（开启UAC后必须以管理员身份运行）
echo              建议手工删除智能ABC输入法
echo.
echo  内容包括：
echo              关闭默认共享
echo              关闭危险服务
echo              加固帐户策略
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
cls

echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  是否关闭默认共享、关闭危险服务、加固帐户策略  
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo ----------------------------------------------------
echo 清除admin$共享
net share admin$ /del 
echo ----------------------------------------------------
echo 清除ipc$共享
net share ipc$ /del
echo ----------------------------------------------------
echo 清除C盘共享
net share c$ /del  
echo ---------------------------------------------------- 
echo 清除D盘共享
net share d$ /del  
echo ----------------------------------------------------
echo 清除E盘共享
net share d$ /del  
echo ----------------------------------------------------
echo 清除F盘共享
net share d$ /del  
echo ----------------------------------------------------
echo 清除G盘共享
net share d$ /del  
echo ----------------------------------------------------


echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在关闭危险的服务
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo ----------------------------------------------------
echo Name: lmhosts
echo 提供 TCP/IP (NetBT) 服务上的 NetBIOS 和网络上客户端的 NetBIOS 名称解析的支持，从而使用户能够共享文件、打印和登录到网络。如果此服务被停用，这些功能可能不可用。如果此服务被禁用，任何依赖它的服务将无法启动。
sc config lmhosts start= DISABLED
sc stop lmhosts
echo ----------------------------------------------------
echo Name: tlntsvr
echo 允许远程用户登录到此计算机并运行程序，并支持多种 TCP/IP Telnet 客户端，包括基于 UNIX 和 Windows 的计算机。如果此服务停止，远程用户就不能访问程序，任何直接依赖于它的服务将会启动失败。
sc config tlntsvr start= DISABLED
sc stop tlntsvr
echo ----------------------------------------------------
echo Name: Browser
echo 维护网络上计算机的更新列表，并将列表提供给计算机指定浏览。如果服务停止，列表不会被更新或维护。如果服务被禁用，任何直接依赖于此服务的服务将无法启动。
sc config  Browser start= DISABLED
sc stop Browser
echo ----------------------------------------------------
echo Name: RemoteRegistry
echo 使远程用户能修改此计算机上的注册表设置。如果此服务被终止，只有此计算机上的用户才能修改注册表。如果此服务被禁用，任何依赖它的服务将无法启动。
sc config  RemoteRegistry start= DISABLED
sc stop RemoteRegistry
echo ----------------------------------------------------
echo Name: lanmanserver
echo 支持此计算机通过网络的文件、打印、和命名管道共享。如果服务停止，这些功能不可用。如果服务被禁用，任何直接依赖于此服务的服务将无法启动。
sc config  lanmanserver start= DISABLED
sc stop lanmanserver
echo ----------------------------------------------------
echo Name: Schedule
echo 使用户可以在此计算机上配置和计划自动任务。此服务还托管多个 Windows 系统关键任务。如果此服务被停止或禁用，这些任务将无法在计划的时间运行。如果此服务被禁用，则明确依赖它的所有服务将无法启动。
sc config Schedule start= DISABLED
sc stop Schedule
echo ----------------------------------------------------

echo Name: KMService
echo 为此计算机注册并更新 IP 地址。如果此服务停止，计算机将不能接收动态 IP 地址和 DNS 更新。如果此服务被禁用，所有明确依赖它的服务都将不能启动。
sc config  KMService start= DISABLED
sc stop KMService
echo ----------------------------------------------------

echo Name: WZCSVC
echo 用于自动配置无线网络设备和连接品质反馈。如果你没有无线网络，那么将其设置为禁用即可。
sc config  WZCSVC start= DISABLED
sc stop WZCSVC
echo ----------------------------------------------------

echo Name: bthserv
echo Bluetooth 服务支持发现和关联远程 Bluetooth 设备。停止或禁用此服务可能导致已安装的 Bluetooth 设备无法正确操作，还会阻止发现和关联新设备。
sc config bthserv start= DISABLED
sc stop bthserv
echo ----------------------------------------------------

echo Name: ShellHWDetection
echo ShellHWDetection服务为自动播放硬件事件提供通知。
sc config ShellHWDetection start= DISABLED
sc stop ShellHWDetection
echo ----------------------------------------------------

echo Name: TrkWks
echo TrkWks服务维护某个计算机内或某个网络中的计算机的 NTFS 文件之间的链接。
sc config TrkWks start= DISABLED
sc stop TrkWks
echo ----------------------------------------------------

echo Name: Dnscache
echo Dnscache服务DNS 客户端服务(dnscache)缓存域名系统(DNS)名称并注册该计算机的完整计算机名称。如果该服务被停止，将继续解析 DNS 名称。然而，将不缓存 DNS 名称的查询结果，且不注册计算机名称。如果该服务被禁用，则任何明确依赖于它的服务都将无法启动。
sc config Dnscache start= DISABLED
sc stop Dnscache
echo ----------------------------------------------------

echo Name: ALG
echo ALG服务为 Internet 连接共享提供第三方协议插件的支持。
sc config ALG start= DISABLED
sc stop ALG
echo ----------------------------------------------------

echo Name: BITS
echo BITS服务为 使用空闲网络带宽在后台传送文件。如果该服务被禁用，则依赖于 BITS 的任何应用程序(如 Windows Update 或 MSN Explorer)将无法自动下载程序和其他信息。
sc config BITS start= DISABLED
sc stop BITS
echo ----------------------------------------------------

echo Name: Spooler
echo Spooler服务为将文件加载到内存供稍后打印。
sc config Spooler start= DISABLED
sc stop Spooler
echo ----------------------------------------------------

echo Name: WinRM
echo WinRM服务Windows 远程管理(WinRM)服务执行 WS-Management 协议来实现远程管理。WS-Management 是用于远程软件和硬件管理的标准 Web 服务协议。WinRM 服务侦听网络上的 WS-Management 请求并对它们进行处理。通过组策略或使用 winrm.cmd 命令行工具的侦听程序，来配置 WinRM 服务，以使其可通过网络侦听。WinRM 服务提供对 WMI 数据的访问并启用事件集合。事件集合及对事件的订阅需要服务处于运行状态。传输 WinRM 消息时使用 HTTP 和 HTTPS 协议。WinRM 服务不依赖于 IIS ，但在同一计算机上预配置为与 IIS 共享端口。WinRM 服务保留 /wsman URL 前缀。若要防止与 IIS 发生冲突，管理员应确保 IIS 上承载的所有网站均不使用 /wsman URL 前缀。
sc config WinRM start= DISABLED
sc stop WinRM
echo ----------------------------------------------------




echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在开启必要的安全服务
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo ----------------------------------------------------
echo Name: wscsvc
echo WSCSVC(Windows 安全中心)服务监视并报告计算机上的安全健康设置。健康设置包括防火墙(打开/关闭)、防病毒软件(打开/关闭/过期)、反间谍软件(打开/关闭/过期)、Windows Update(自动/手动下载并安装更新)、用户帐户控制(打开/关闭)以及 Internet 设置(推荐/不推荐)。
sc config wscsvc start= ENABLE
sc start wscsvc
echo ----------------------------------------------------
echo Name:MpsSvc
echo Windows 防火墙通过阻止未授权用户通过 Internet 或网络访问您的计算机来帮助保护计算机。
sc config MpsSvc start= ENABLE
sc start MpsSvc
echo ----------------------------------------------------

echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在开启审核策略
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo [version] >secaudit.inf REM 审核策略配置模块
echo signature="$CHICAGO$" >>secaudit.inf
echo [Event Audit] >>secaudit.inf
echo ----------------------------------------------------
echo 审核系统事件
echo AuditSystemEvents=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核对象访问
echo AuditObjectAccess=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核特权使用
echo AuditPrivilegeUse=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核策略更改
echo AuditPolicyChange=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核进程追踪
echo AuditProcessTracking=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核目录服务访问
echo AuditDSAccess=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核账户管理
echo AuditAccountManage=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核账户登录事件
echo AuditAccountLogon=3 >>secaudit.inf
echo ----------------------------------------------------
echo 审核登录事件
echo AuditLogonEvents=3 >>secaudit.inf
echo ----------------------------------------------------
secedit /configure /db secaudit.sdb /cfg secaudit.inf
echo ----------------------------------------------------
;del secaudit.*


echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在进行帐户策略加固
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo [version] >account.inf REM 帐户口令授权配置模块
echo signature="$CHICAGO$" >>account.inf
echo [System Access] >>account.inf
echo ----------------------------------------------------
echo 开启帐户密码复杂性要求
echo PasswordComplexity=1 >>account.inf
echo ----------------------------------------------------
echo 修改帐户密码最小长度为10
echo MinimumPasswordLength=10 >>account.inf
echo ----------------------------------------------------
echo 修改帐户密码最长留存期为14天
echo MaximumPasswordAge=14 >>account.inf
echo ----------------------------------------------------
echo 修改强制密码历史为5次
echo PasswordHistorySize=5 >>account.inf
echo ----------------------------------------------------
echo 设定帐户锁定阀值为5次
echo LockoutBadCount=5 >>account.inf
echo ----------------------------------------------------
echo 禁用Guest帐户
echo EnableGuestAccount=0 >>account.inf
echo ----------------------------------------------------
secedit /configure /db account.sdb /cfg account.inf
echo ----------------------------------------------------
del account.*

echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在结束并删除飞秋
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
@echo off 
echo 结束飞秋并删除
set exe=飞秋.exe
For /f "tokens=2,3 delims=," %%i In ('wmic process get ProcessId^,ExecutablePath
/format:csv^|find /i "%exe%"') do (
taskkill /F /im %exe%
del "%%i" /q 
)

echo 结束飞秋并删除
set exe=feiq.exe
For /f "tokens=2,3 delims=," %%i In ('wmic process get ProcessId^,ExecutablePath
/format:csv^|find /i "%exe%"') do (
taskkill /F /im %exe%
del "%%i" /q 
)

echo 删除文件
del C:\Windows\KMService.exe
del C:\Users\Administrator\AppData\Local\Temp\EFCB.tmp\KMService.exe
del C:\Users\Administrator\AppData\Local\Temp\50F.tmp\KMService.exe

echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
echo.
echo  正在设置屏幕保护时间
echo.
echo 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
set r="HKCU\Control Panel\Desktop"
reg add %r% /v ScreenSaveActive /d 1 /f >nul
reg add %r% /v ScreenSaverIsSecure /d 1 /f >nul
reg add %r% /v ScreenSaveTimeOut /d 540 /f >nul
reg add %r% /v SCRNSAVE.EXE /d  %windir%\system32\ssText3d.scr  /f >nul




