@echo off
echo.
echo         ###########################################################
echo         #                                                         #
echo         #                   ��Σ�ն˿ڷ������ߡ�                  #
echo         #                   Version��2017.11.21                   #
echo         #                                                         #
echo         #                       ������˵����                      #
echo         #                                                         #
echo         #     ��Σ�ն˿ڷ������ߡ����ڸ�����ҵ�ն˰�ȫ���ù�����  #
echo         # ��Ҫ������Windows����ǽ��ͨ��������վ����ͳ�վ����Գ� #
echo         # ����Σ�ն˿ڽ��з��������ű����������¶˿ڣ�            #
echo         #    ��20��21�˿ڡ���FTP����Ĭ�϶˿ڣ�                    #
echo         #    ��22�˿ڡ�SSH����˿�                                #
echo         #    ��23�˿ڡ�Telnet����˿�                             #
echo         #    ��135�˿ڡ�ʹ��RPCЭ���ṩDCOM����                   #
echo         #    ��137�˿ڡ�ʹ��NetBIOSЭ���ṩhost��IP��ַ��ѯ����   #
echo         #    ��138�˿ڡ�ʹ��NetBIOSЭ���ṩ���������ƻ��������� #
echo         #    ��139�˿ڡ�ͨ�������ھӷ��ʾ����������ļ��ʹ�ӡ��    #
echo         #    ��445�˿ڡ�����CIFSЭ����ʹ����ļ��ʹ�ӡ��          #
echo         #    ��3389�˿ڡ�Զ���������                             #
echo         #    ��5800�˿ڡ�VNC-http�˿�                             #
echo         #    ��3306�˿ڡ�MySQL���ݿ����Ĭ�϶˿�                  #
echo         #    ��5236�˿ڡ��������ݿ����Ĭ�϶˿�                   #
echo         #    ��80��8000��8080�˿ڡ�Web�����ö˿�                #
echo         #                                                         #
echo         ###########################################################
echo.
echo                        ȷ��������Ϣ���������ʼ����
pause > nul
echo.
echo Windows��������ǽ�����С���
netsh advfirewall set allprofile state on
echo ���ڹر�20�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 20 - TCP" dir = in action = block protocol = TCP localport = 20
echo ���ڹر�21�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 21 - TCP" dir = in action = block protocol = TCP localport = 21
echo ���ڹر�22�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 22 - TCP" dir = in action = block protocol = TCP localport = 22
echo ���ڹر�23�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 23 - TCP" dir = in action = block protocol = TCP localport = 23
echo ���ڹر�135�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 135 - TCP" dir = in action = block protocol = TCP localport = 135
echo ���ڹر�137�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 137 - TCP" dir = in action = block protocol = TCP localport = 137
echo ���ڹر�138�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 138 - TCP" dir = in action = block protocol = TCP localport = 138
echo ���ڹر�139�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 139 - TCP" dir = in action = block protocol = TCP localport = 139
echo ���ڹر�445�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 445 - TCP" dir = in action = block protocol = TCP localport = 445
echo ���ڹر�3389�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 3389 - TCP" dir = in action = block protocol = TCP localport = 3389
echo ���ڹر�3306�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 3306 - TCP" dir = in action = block protocol = TCP localport = 3306
echo ���ڹر�5236�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 5236 - TCP" dir = in action = block protocol = TCP localport = 5236
echo ���ڹر�5236�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 5236 - TCP" dir = in action = block protocol = TCP localport = 5800
echo ���ڹر�80�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 80 - TCP" dir = in action = block protocol = TCP localport = 80
echo ���ڹر�8000�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 8000 - TCP" dir = in action = block protocol = TCP localport = 8000
echo ���ڹر�4999�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 4999 - TCP" dir = in action = block protocol = TCP localport = 4999
echo ���ڹر�50000�˿ڡ���
netsh advfirewall firewall add rule name = "Disable port 50000 - TCP" dir = in action = block protocol = TCP localport = 50000
echo =============================================
echo ��ҵ�ն˵Ķ˿ڷ�����������ɣ�