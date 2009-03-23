@echo off
color 1E


set server=%1%
set database=%2%

echo *-----------------------------------------------------------------------------*
echo * FX schema for SQL Server 2005                                               *
echo * Copyright (c) AnjLab 2008, http://anjlab.com. All rights reserved.          *
echo * The code can be used for free as long as this copyright notice isnt removed *
echo *-----------------------------------------------------------------------------*

osql -S %server% -E -d %database% -n -Q "create schema fx"
echo * Installing tables...                                                        *
for /f "tokens=*" %%a in ('dir /b tables\*.sql') do osql -S %server% -E -n -d %database% -i tables\%%a  
echo * Installing functions...                                                     *
for /f "tokens=*" %%a in ('dir /b functions\*.sql') do osql -S %server% -E -n -d %database% -i functions\%%a 
echo * Installing procedures...                                                    *
for /f "tokens=*" %%a in ('dir /b procedures\*.sql') do osql -S %server% -E -n -d %database% -i procedures\%%a 
echo *-----------------------------------------------------------------------------*
echo * FX schema is installed successfuly                                          *
echo *-----------------------------------------------------------------------------*

