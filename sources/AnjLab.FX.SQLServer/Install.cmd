@echo off

set server=%1%
set database=%2%

rem svn up

osql -S %server% -E -d %database% -Q "create schema fx"
for /f "tokens=*" %%a in ('dir /b tables\*.sql') do osql -S %server% -E -d %database% -i tables\%%a  
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -E -d %database% -i functions\%%a 
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -E -d %database% -i procedures\%%a 

