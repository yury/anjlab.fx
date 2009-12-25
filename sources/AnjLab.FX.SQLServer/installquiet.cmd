@echo off
color 1E

set server=%1%
set database=%2%
set user=%3%
set password=%4%
set language=%5

echo * Installing AnjLab FX schema for SQL Server 2005 / 2008
for /f %%i in ("%0") do set curpath=%%~dpi 
cd /d %curpath%
osql -S %server% -u %user% -p %password% -d %database% -n -Q "declare @SQL nvarchar(100) set @SQL = N'create schema fx' if not exists(select * from sys.schemas where name = 'fx') exec sp_executesql @SQL"
for /f "tokens=*" %%a in ('dir /b tables\%language%\*.sql') do osql -S %server% -U %user% -P %password% -n -d %database% -i tables\%language%\%%a  
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -u %user% -p %password% -n -d %database% -i functions\%%a 
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -u %user% -p %password% -n -d %database% -i procedures\%%a 

