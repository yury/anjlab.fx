@echo off

set server=.
set database=123
set user=sa
set password=Friday13

rem svn up

osql -S %server% -U %user% -P %password% -d %database% -i schema.sql
for /f "tokens=*" %%a in ('dir /b tables\*.sql') do osql -S %server% -U %user% -P %password% -d %database% -i tables\%%a  
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -U %user% -P %password% -d %database% -i functions\%%a 
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -U %user% -P %password% -d %database% -i procedures\%%a 

