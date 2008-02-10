/*
This table contains list of countries (in Russian)
Author: Alex Zakharov
Date: 2/1/2008
*/

if not exists (select * from sysobjects where id = object_id(N'tools.Country') and xtype = 'U' )
begin

CREATE TABLE tools.Country (
	CountryId smallint not null identity(1,1),
	FullName nvarchar(512),
	ShortName nvarchar(128) not null
	CONSTRAINT PK_Country PRIMARY KEY CLUSTERED (CountryId ASC)
)		

DELETE FROM tools.Country
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������� � �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������')
INSERT INTO tools.Country(ShortName) VALUES(N'��������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������ � �����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������-����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������-����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������������')
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������-�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������')
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������������� ���������� �����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������������� ����������')
INSERT INTO tools.Country(ShortName) VALUES(N'������')
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������')
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������')
INSERT INTO tools.Country(ShortName) VALUES(N'�����')
INSERT INTO tools.Country(ShortName) VALUES(N'����-�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������� �������-��������������� ����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����')
INSERT INTO tools.Country(ShortName) VALUES(N'�����-����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���-������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������')
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����')
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������')
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������')
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����� ��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������')
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������������ �������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����')
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����-����� ������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������')
INSERT INTO tools.Country(ShortName) VALUES(N'���������� ���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���-������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���-���� � ��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������� ������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����-������� � ���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����-���� � �����') 
INSERT INTO tools.Country(ShortName) VALUES(N'����-�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������ ��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������') 
INSERT INTO tools.Country(ShortName) VALUES(N'��������')  
INSERT INTO tools.Country(ShortName) VALUES(N'����������� ����� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������� �������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������-�����')
INSERT INTO tools.Country(ShortName) VALUES(N'�����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����-�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������� � ������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'������������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������')
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'�����') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'��������')
INSERT INTO tools.Country(ShortName) VALUES(N'��������������������� ����������')
INSERT INTO tools.Country(ShortName) VALUES(N'���') 
INSERT INTO tools.Country(ShortName) VALUES(N'�����')
INSERT INTO tools.Country(ShortName) VALUES(N'����������') 
INSERT INTO tools.Country(ShortName) VALUES(N'����')
INSERT INTO tools.Country(ShortName) VALUES(N'���������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'���-�����')
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������������� ������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������') 
INSERT INTO tools.Country(ShortName) VALUES(N'�������')
INSERT INTO tools.Country(ShortName) VALUES(N'����� ������')
INSERT INTO tools.Country(ShortName) VALUES(N'������') 
INSERT INTO tools.Country(ShortName) VALUES(N'������')

end
go