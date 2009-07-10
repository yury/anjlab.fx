@echo off
color 1E

set server=%1%
set database=%2%

echo * Installing FX schema for SQL Server 2005

osql -S %server% -E -d %database% -n -Q "create schema fx"
for /f "tokens=*" %%a in ('dir /b tables\*.sql') do osql -S %server% -E -n -d %database% -i tables\%%a  
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -E -n -d %database% -i functions\%%a 
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -E -n -d %database% -i procedures\%%a 

