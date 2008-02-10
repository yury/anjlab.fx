@echo off

set server=.\sqlexpress
set database=master
set user=sa
set password=Friday13


svn up
osql -S %server% -U %user% -P %password% -d %database% -i schema.sql
for /f "tokens=*" %%a in ('dir /b tables\*.sql') do osql -S %server% -U %user% -P %password% -d %database% -i tables\%%a  
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -U %user% -P %password% -d %database% -i functions\%%a 

