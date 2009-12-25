@echo off
cls
color 1E

echo ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿
echo ³ AnjLab FX schema for SQL Server 2005 \ 2008                                 ³
echo ³ Copyright (c) AnjLab 2008-2010, http://anjlab.com. All rights reserved.     ³
echo ³ The code can be used for free as long as this copyright notice isnt removed ³
echo ÃÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ´


if [%1]==[] (goto error)
if [%2]==[] (goto error)
if [%3]==[] (goto error)
if [%4]==[] (goto error)
if [%5]==[] (echo ³ Language is not set. Using English by default.                              ³) 
if [%5]==[] (set language=Eng) else (set language=%5)

set server=%1%                                
set database=%2%
set user=%3%
set password=%4%

echo ³ Getting current directory                                                   ³
for /f %%i in ("%0") do set curpath=%%~dpi
cd /d %curpath%

osql -S %server% -U %user% -P %password% -d %database% -n -Q "declare @SQL nvarchar(100) set @SQL = N'create schema fx' if not exists(select * from sys.schemas where name = 'fx') exec sp_executesql @SQL"
echo ³ Installing tables...                                                        ³
for /f "tokens=*" %%a in ('dir /b tables\%language%\*.sql') do osql -S %server% -U %user% -P %password% -n -d %database% -i tables\%language%\%%a  
echo ³ Installing functions...                                                     ³
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -U %user% -P %password% -n -d %database% -i functions\%%a 
echo ³ Installing procedures...                                                    ³
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -U %user% -P %password% -n -d %database% -i procedures\%%a 

echo ÃÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ´
echo ³ FX schema is installed successfuly                                          ³
echo ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ
color 2E
goto exit

:error
echo ³ Installation could not neen performed because one or more parameters are    ³
echo ³ missed. Script usage:                                                       ³
echo ³   - Server name                                                             ³
echo ³   - Database name                                                           ³
echo ³   - User name                                                               ³
echo ³   - User password                                                           ³
echo ³   - Language (Rus or Eng)                                                   ³
echo ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ
color CE

:exit
